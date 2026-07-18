#!/usr/bin/env bash
# ==============================================================================
# CVerify — MinIO object-storage backup
# Backs up the `minio_data` volume (CV files, evidence attachments, cert images)
# that the `minio` service in deployment/docker-compose.prod.yml mounts at /data.
#
# Approach: a throwaway alpine container with `--volumes-from cverify-minio`
# tars /data. This is image-agnostic (the minio/minio image is too minimal to
# rely on having tar/sh inside it) and volume-name-agnostic (works no matter
# what COMPOSE_PROJECT_NAME prefixes the volume with), because it borrows the
# mount straight from the running MinIO container.
#
# MinIO stores each object as a plain file under /data, so a filesystem-level
# tar is a valid, restorable snapshot for a single-node MinIO — matching the
# "don't over-engineer" scope. For a strictly consistent copy you could stop
# the `minio` service first, but for this workload a live tar is acceptable.
# ==============================================================================
set -euo pipefail

MINIO_CONTAINER="${MINIO_CONTAINER:-cverify-minio}"
BACKUP_DIR="${MINIO_BACKUP_DIR:-$HOME/backups/minio}"
RETENTION_DAYS="${RETENTION_DAYS:-14}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

if ! docker inspect "$MINIO_CONTAINER" > /dev/null 2>&1; then
  echo "[backup-minio] ERROR: container '$MINIO_CONTAINER' not found." >&2
  echo "[backup-minio] Is deployment/docker-compose.prod.yml applied? (it defines the minio service)" >&2
  exit 1
fi

mkdir -p "$BACKUP_DIR"
OUT_FILE="$BACKUP_DIR/cverify_minio_${TIMESTAMP}.tar.gz"

echo "[backup-minio] Archiving MinIO /data from container ${MINIO_CONTAINER}..."
# --volumes-from imports MinIO's /data mount into the throwaway container.
# We write the archive to a bind-mounted host dir so it lands on the host.
docker run --rm \
  --volumes-from "$MINIO_CONTAINER" \
  -v "$BACKUP_DIR":/backup \
  alpine:3.19 \
  tar czf "/backup/$(basename "$OUT_FILE")" -C /data .

echo "[backup-minio] Wrote $OUT_FILE ($(du -h "$OUT_FILE" | cut -f1))"

echo "[backup-minio] Verifying archive integrity..."
gzip -t "$OUT_FILE"
echo "[backup-minio] Archive OK."

echo "[backup-minio] Pruning backups older than ${RETENTION_DAYS} days..."
find "$BACKUP_DIR" -name "cverify_minio_*.tar.gz" -mtime "+${RETENTION_DAYS}" -delete -print

echo "[backup-minio] Done."
