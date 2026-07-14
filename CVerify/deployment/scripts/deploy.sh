#!/usr/bin/env bash
# ==============================================================================
# CVerify — Manual deploy script (VPS-side)
# Mirrors what .github/workflows/deploy-vps.yml does over SSH, for manual use.
# Run from the repository's CVerify/ subdirectory (see DEPLOYMENT_GUIDE.md for
# the actual directory layout on the VPS).
# ==============================================================================
set -euo pipefail

COMPOSE_DIR="${COMPOSE_DIR:-$HOME/swp391-su26-ai-audit-project-swp391_se20a02_group-05/CVerify}"
# Which branch to deploy. Mirrors the DEPLOY_BRANCH repo variable used by
# .github/workflows/deploy-vps.yml; defaults to CVerify-uat (current production
# branch). Override for a manual deploy of a different branch:
#   DEPLOY_BRANCH=main bash deployment/scripts/deploy.sh
DEPLOY_BRANCH="${DEPLOY_BRANCH:-CVerify-uat}"
cd "$COMPOSE_DIR"

echo "[deploy] Pulling latest code (branch: ${DEPLOY_BRANCH})..."
git pull origin "$DEPLOY_BRANCH"

echo "[deploy] Building and starting containers..."
docker compose -f docker-compose.yml -f deployment/docker-compose.prod.yml \
  --env-file .env up -d --build --remove-orphans

echo "[deploy] Pruning dangling images/layers..."
docker system prune -f

echo "[deploy] Current container status:"
docker compose ps

echo "[deploy] Running health check..."
bash "$COMPOSE_DIR/deployment/scripts/health-check.sh"

echo "[deploy] Done."
