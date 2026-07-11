#!/usr/bin/env bash
# ==============================================================================
# CVerify — PostgreSQL backup
# Container name confirmed from docker-compose.yml: cverify-postgres
# DB_NAME/DB_USER read from the same .env used by docker compose.
# ==============================================================================
set -euo pipefail

ENV_FILE="/opt/cverify/compose/.env"
BACKUP_DIR="/opt/cverify/backup/postgres"
RETENTION_DAYS=14
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

if [ ! -f "$ENV_FILE" ]; then
  echo "[backup-db] ERROR: $ENV_FILE not found" >&2
  exit 1
fi

# shellcheck disable=SC1090
set -a; source "$ENV_FILE"; set +a

mkdir -p "$BACKUP_DIR"
OUT_FILE="$BACKUP_DIR/cverify_${TIMESTAMP}.sql.gz"

echo "[backup-db] Dumping database '${DB_NAME}' from container cverify-postgres..."
docker exec cverify-postgres pg_dump -U "${DB_USER}" -d "${DB_NAME}" \
  | gzip > "$OUT_FILE"

echo "[backup-db] Wrote $OUT_FILE ($(du -h "$OUT_FILE" | cut -f1))"

echo "[backup-db] Pruning backups older than ${RETENTION_DAYS} days..."
find "$BACKUP_DIR" -name "cverify_*.sql.gz" -mtime "+${RETENTION_DAYS}" -delete -print

echo "[backup-db] Done."
