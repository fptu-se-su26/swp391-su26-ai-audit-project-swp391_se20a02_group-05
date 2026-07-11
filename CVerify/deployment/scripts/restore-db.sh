#!/usr/bin/env bash
# ==============================================================================
# CVerify — PostgreSQL restore
# Usage: ./restore-db.sh /opt/cverify/backup/postgres/cverify_20260701_020000.sql.gz
#
# WARNING: this is destructive — it drops and recreates all data in DB_NAME.
# Requires interactive confirmation. Does not run inside CI.
# ==============================================================================
set -euo pipefail

ENV_FILE="/opt/cverify/compose/.env"
BACKUP_FILE="${1:-}"

if [ -z "$BACKUP_FILE" ] || [ ! -f "$BACKUP_FILE" ]; then
  echo "Usage: $0 <path-to-backup.sql.gz>" >&2
  exit 1
fi

# shellcheck disable=SC1090
set -a; source "$ENV_FILE"; set +a

echo "!!! This will DROP and RECREATE database '${DB_NAME}' on container cverify-postgres !!!"
read -r -p "Type the database name (${DB_NAME}) to confirm: " CONFIRM
if [ "$CONFIRM" != "${DB_NAME}" ]; then
  echo "Confirmation did not match. Aborting."
  exit 1
fi

echo "[restore-db] Stopping cverify-core to prevent writes during restore..."
docker compose -f /opt/cverify/compose/docker-compose.yml stop cverify-core

echo "[restore-db] Dropping and recreating '${DB_NAME}'..."
docker exec cverify-postgres psql -U "${DB_USER}" -d postgres \
  -c "DROP DATABASE IF EXISTS \"${DB_NAME}\";" \
  -c "CREATE DATABASE \"${DB_NAME}\";"

echo "[restore-db] Restoring from ${BACKUP_FILE}..."
gunzip -c "$BACKUP_FILE" | docker exec -i cverify-postgres psql -U "${DB_USER}" -d "${DB_NAME}"

echo "[restore-db] Re-enabling pgvector extension (in case the dump didn't include it)..."
docker exec cverify-postgres psql -U "${DB_USER}" -d "${DB_NAME}" \
  -c "CREATE EXTENSION IF NOT EXISTS vector;"

echo "[restore-db] Restarting cverify-core..."
docker compose -f /opt/cverify/compose/docker-compose.yml start cverify-core

echo "[restore-db] Done. Verify with: docker compose logs -f cverify-core"
