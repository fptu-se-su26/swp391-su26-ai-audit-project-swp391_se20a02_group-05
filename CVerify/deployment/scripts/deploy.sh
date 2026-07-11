#!/usr/bin/env bash
# ==============================================================================
# CVerify — Manual deploy script (VPS-side)
# Mirrors what .github/workflows/deploy.yml does over SSH, for manual use.
# Run from /opt/cverify/compose (see DEPLOYMENT_GUIDE.md for directory layout).
# ==============================================================================
set -euo pipefail

COMPOSE_DIR="/opt/cverify/compose"
cd "$COMPOSE_DIR"

echo "[deploy] Pulling latest code..."
git pull origin CVerify-uat

echo "[deploy] Building and starting containers..."
docker compose -f docker-compose.yml -f deployment/docker-compose.prod.yml \
  --env-file .env up -d --build --remove-orphans

echo "[deploy] Pruning dangling images/layers..."
docker system prune -f

echo "[deploy] Current container status:"
docker compose ps

echo "[deploy] Running health check..."
bash /opt/cverify/scripts/health-check.sh

echo "[deploy] Done."
