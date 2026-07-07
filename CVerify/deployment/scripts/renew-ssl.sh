#!/usr/bin/env bash
# ==============================================================================
# CVerify — SSL renewal (Let's Encrypt / Certbot, host-installed Nginx)
# Certbot's own systemd timer normally handles this automatically; this
# script exists for a manual/forced renewal and to reload Nginx afterward,
# and as the command to schedule if the systemd timer is not present on
# this VPS image (verify with `systemctl list-timers | grep certbot`).
# ==============================================================================
set -euo pipefail

echo "[renew-ssl] Attempting certificate renewal for cverify.io.vn..."
sudo certbot renew --quiet --deploy-hook "systemctl reload nginx"

echo "[renew-ssl] Current certificate status:"
sudo certbot certificates | grep -A 5 "cverify.io.vn" || echo "  (no matching certificate found — has certbot --nginx -d cverify.io.vn been run yet?)"

echo "[renew-ssl] Done."
