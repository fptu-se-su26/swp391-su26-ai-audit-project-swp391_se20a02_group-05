"""
monitoring_client.py
=====================
Emits monitoring events from CVerify.AI to the CVerify.Core backend, where they are
recorded as audit logs and broadcast to admins in realtime.

Requests are authenticated with the same HMAC scheme Core uses for its own outbound
calls:  HMAC_SHA256(METHOD + PATH + BODY + TIMESTAMP + NONCE, SHARED_SECRET).

The emitter is best-effort: any failure is logged and swallowed so that monitoring
never breaks the pipeline it observes.
"""

import hmac
import json
import logging
import secrets
import time
from typing import Any, Optional

import httpx

from app.core.config import settings

logger = logging.getLogger("monitoring_client")

_INGEST_PATH = "/api/admin/monitoring/events"
_CLIENT_ID = "cverify-ai"
_TIMEOUT_SECONDS = 5.0


def _sign(method: str, path: str, body: str, timestamp: str, nonce: str) -> str:
    raw_message = f"{method.upper()}{path}{body}{timestamp}{nonce}"
    key_bytes = settings.shared_secret.strip('"').encode("utf-8")
    mac = hmac.new(key_bytes, raw_message.encode("utf-8"), digestmod="sha256")
    return mac.hexdigest().lower()


async def emit_monitoring_event(
    event_type: str,
    message: str,
    severity: str = "info",
    source: str = "CVerify.AI",
    details: Optional[dict[str, Any]] = None,
    correlation_id: Optional[str] = None,
) -> bool:
    """
    Sends a single monitoring event to Core. Returns True on a 2xx response, False otherwise.
    Never raises — monitoring must not break the caller.
    """
    payload = {
        "eventType": event_type,
        "message": message,
        "severity": severity,
        "source": source,
        "correlationId": correlation_id,
        "details": details,
        "occurredAt": None,
    }

    # Serialize once so the signed bytes match the transmitted bytes exactly.
    body = json.dumps(payload, separators=(",", ":"))
    timestamp = str(int(time.time()))
    nonce = secrets.token_hex(16)
    signature = _sign("POST", _INGEST_PATH, body, timestamp, nonce)

    headers = {
        "Content-Type": "application/json",
        "X-Client-Id": _CLIENT_ID,
        "X-Timestamp": timestamp,
        "X-Nonce": nonce,
        "X-Correlation-Id": correlation_id or nonce,
        "X-Signature": signature,
    }

    url = settings.backend_api_url.rstrip("/") + _INGEST_PATH

    try:
        async with httpx.AsyncClient(timeout=_TIMEOUT_SECONDS) as client:
            response = await client.post(url, content=body, headers=headers)
        if response.status_code // 100 == 2:
            return True
        logger.warning(
            "Monitoring event '%s' rejected by Core (status=%s).",
            event_type,
            response.status_code,
        )
        return False
    except Exception as exc:  # noqa: BLE001 - monitoring must never break the pipeline
        logger.warning("Failed to emit monitoring event '%s': %s", event_type, exc)
        return False
