#!/usr/bin/env bash
# ==============================================================================
# CVerify — MinIO object-storage restore
# Usage: ./restore-minio.sh $HOME/backups/minio/cverify_minio_20260701_020000.tar.gz
#
# WARNING: destructive — it REPLACES the entire contents of the minio_data
# volume with the archive. Requires interactive confirmation. Does not run in CI.
# ==============================================================================
set -euo pipefail

MINIO_CONTAINER="${MINIO_CONTAINER:-cverify-minio}"
BACKUP_FILE="${1:-}"

if [ -z "$BACKUP_FILE" ] || [ ! -f "$BACKUP_FILE" ]; then
  echo "Usage: $0 <path-to-cverify_minio_*.tar.gz>" >&2
  exit 1
fi

if ! docker inspect "$MINIO_CONTAINER" > /dev/null 2>&1; then
  echo "[restore-minio] ERROR: container '$MINIO_CONTAINER' not found." >&2
  exit 1
fi

echo "[restore-minio] Verifying archive integrity before touching any data..."
gzip -t "$BACKUP_FILE"

echo "!!! This will REPLACE all object data in the MinIO volume behind ${MINIO_CONTAINER} !!!"
read -r -p "Type MINIO to confirm: " CONFIRM
if [ "$CONFIRM" != "MINIO" ]; then
  echo "Confirmation did not match. Aborting."
  exit 1
fi

echo "[restore-minio] Stopping MinIO to prevent writes during restore..."
docker stop "$MINIO_CONTAINER" > /dev/null

echo "[restore-minio] Wiping and extracting ${BACKUP_FILE} into /data..."
# --volumes-from works against a stopped container: the /data mount is still
# attached to it. Wipe existing contents, then extract the archive in its place.
docker run --rm \
  --volumes-from "$MINIO_CONTAINER" \
  -v "$(cd "$(dirname "$BACKUP_FILE")" && pwd)":/restore \
  alpine:3.19 \
  sh -c "rm -rf /data/* /data/..?* /data/.[!.]* 2>/dev/null; tar xzf \"/restore/$(basename "$BACKUP_FILE")\" -C /data"

echo "[restore-minio] Restarting MinIO..."
docker start "$MINIO_CONTAINER" > /dev/null

echo "[restore-minio] Done. Verify with: docker logs -f ${MINIO_CONTAINER}"
