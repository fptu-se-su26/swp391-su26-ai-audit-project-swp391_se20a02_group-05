#!/usr/bin/env bash
# ==============================================================================
# CVerify — Daily backup orchestrator (PostgreSQL + MinIO)
# Single entry point invoked by the scheduler (cron or systemd timer — see
# DEPLOYMENT_GUIDE.md §9b). Runs both backups, does NOT abort after the first
# failure (a failed DB dump should not skip the MinIO backup), and exits
# non-zero if either failed so the scheduler records the run as failed.
#
# Reuses the existing single-purpose scripts — no backup logic is duplicated
# here. Run the two directly for ad-hoc backups; run this for the scheduled job.
# ==============================================================================
set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FAILED=0

echo "[backup-all] === CVerify backup run: $(date -u '+%Y-%m-%d %H:%M:%S UTC') ==="

echo "[backup-all] --- PostgreSQL ---"
if bash "$SCRIPT_DIR/backup-db.sh"; then
  echo "[backup-all] PostgreSQL backup OK."
else
  echo "[backup-all] PostgreSQL backup FAILED." >&2
  FAILED=1
fi

echo "[backup-all] --- MinIO ---"
if bash "$SCRIPT_DIR/backup-minio.sh"; then
  echo "[backup-all] MinIO backup OK."
else
  echo "[backup-all] MinIO backup FAILED." >&2
  FAILED=1
fi

if [ "$FAILED" -ne 0 ]; then
  echo "[backup-all] One or more backups FAILED." >&2
  exit 1
fi

echo "[backup-all] All backups completed successfully."
