#!/usr/bin/env bash
# ==============================================================================
# CVerify — Clean stale git-clone workspaces used by the AI repository
# analysis pipeline. Volume confirmed from docker-compose.yml:
#   cverify_workspaces -> mounted at /app/temp_clones inside cverify-ai
#
# Intended to run hourly via cron (see DEPLOYMENT_CHECKLIST.md / crontab
# entry). Any clone directory older than 60 minutes is assumed to be an
# orphan from a crashed/interrupted analysis job (REPOSITORY_AUDIT.md §14 /
# earlier codebase audit flagged this as a known risk).
# ==============================================================================
set -euo pipefail

MAX_AGE_MINUTES=60

echo "[cleanup-workspaces] Removing clone directories older than ${MAX_AGE_MINUTES} minutes..."
docker exec cverify-ai find /app/temp_clones -mindepth 1 -maxdepth 1 -type d \
  -mmin "+${MAX_AGE_MINUTES}" -exec rm -rf {} + -print

echo "[cleanup-workspaces] Current usage:"
docker exec cverify-ai du -sh /app/temp_clones 2>/dev/null || true

echo "[cleanup-workspaces] Done."
