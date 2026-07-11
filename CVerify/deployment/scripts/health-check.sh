#!/usr/bin/env bash
# ==============================================================================
# CVerify — Post-deploy health check
# Endpoints confirmed from source:
#   - Core:   app.MapHealthChecks("/health")               (Program.cs:612)
#   - AI:     GET /health and /health/ready                 (CVerify.AI/app/main.py)
#   - Client: Dockerfile healthcheck hits GET / on :3000
# Host port bindings confirmed from docker-compose.yml (127.0.0.1 only for
# postgres/redis/ai/core; client is public — see docker-compose.prod.yml for
# the fix that also binds client to 127.0.0.1, in which case update the
# CLIENT_URL below to hit Nginx on :443 instead).
# ==============================================================================
set -uo pipefail

CORE_PORT="${CORE_PORT:-5247}"
AI_PORT="${AI_PORT:-8000}"
CLIENT_PORT="${CLIENT_PORT:-3000}"

FAILED=0

check() {
  local name="$1" url="$2"
  if curl -fsS --max-time 5 "$url" > /dev/null 2>&1; then
    echo "[health-check] OK   - $name ($url)"
  else
    echo "[health-check] FAIL - $name ($url)"
    FAILED=1
  fi
}

echo "[health-check] Checking containers are up..."
docker compose -f /opt/cverify/compose/docker-compose.yml ps

check "Backend API"   "http://127.0.0.1:${CORE_PORT}/health"
check "AI Service"    "http://127.0.0.1:${AI_PORT}/health"
check "Frontend"      "http://127.0.0.1:${CLIENT_PORT}/"

if [ "$FAILED" -ne 0 ]; then
  echo "[health-check] One or more checks FAILED."
  exit 1
fi

echo "[health-check] All checks passed."
