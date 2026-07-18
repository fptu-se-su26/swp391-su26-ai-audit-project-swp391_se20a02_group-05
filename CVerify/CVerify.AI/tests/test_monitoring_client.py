"""
test_monitoring_client.py
=========================
Unit tests for the CVerify.AI -> Core monitoring emitter.

Covers:
  1. HMAC signing parity with the Core formula
     HMAC_SHA256(METHOD + PATH + BODY + TIMESTAMP + NONCE, SHARED_SECRET)
  2. Best-effort delivery: transport failures are swallowed, not raised.

Fully offline — no network, no Core, no Anthropic API.
"""

import asyncio
import os
import sys
import unittest

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

os.environ.setdefault("ANTHROPIC_API_KEY", "dummy-key")

from app.core.config import settings
from app.core.clients import monitoring_client


class TestSigningParity(unittest.TestCase):
    def test_sign_matches_reference_vector(self):
        # Reference value computed with the exact Core formula and secret "test-secret".
        original_secret = settings.shared_secret
        settings.shared_secret = "test-secret"
        try:
            sig = monitoring_client._sign(
                "POST",
                "/api/admin/monitoring/events",
                '{"eventType":"x"}',
                "1700000000",
                "5abc",
            )
        finally:
            settings.shared_secret = original_secret

        self.assertEqual(
            sig,
            "e21b5007eec35a58e9dc4c59791c4ff43ebfbdb2433a2c377b295ab1cf82153c",
        )

    def test_sign_is_lowercase_hex_sha256(self):
        sig = monitoring_client._sign("POST", "/p", "{}", "1", "n")
        self.assertEqual(len(sig), 64)
        self.assertEqual(sig, sig.lower())


class TestEmitIsBestEffort(unittest.TestCase):
    def test_emit_returns_false_and_does_not_raise_on_transport_error(self):
        class _BoomClient:
            def __init__(self, *args, **kwargs):
                pass

            async def __aenter__(self):
                return self

            async def __aexit__(self, *args):
                return False

            async def post(self, *args, **kwargs):
                raise RuntimeError("network down")

        original = monitoring_client.httpx.AsyncClient
        monitoring_client.httpx.AsyncClient = _BoomClient
        try:
            result = asyncio.run(
                monitoring_client.emit_monitoring_event(
                    event_type="pipeline_error",
                    message="something failed",
                    severity="error",
                )
            )
        finally:
            monitoring_client.httpx.AsyncClient = original

        self.assertFalse(result)


if __name__ == "__main__":
    unittest.main()
