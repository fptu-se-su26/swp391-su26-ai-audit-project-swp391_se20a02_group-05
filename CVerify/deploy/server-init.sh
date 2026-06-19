#!/usr/bin/env bash
# ==============================================================================
# CVERIFY VPS FIRST-TIME SETUP SCRIPT
# ==============================================================================
# Run this ONCE as root on a fresh Ubuntu 22.04/24.04 VPS:
#
#   curl -sL https://raw.githubusercontent.com/YOUR_ORG/CVerify/main/deploy/server-init.sh | bash -s -- cverify.yourdomain.com
#
# Or upload and run locally:
#   chmod +x server-init.sh && sudo ./server-init.sh cverify.yourdomain.com
# ==============================================================================
set -euo pipefail

DOMAIN="${1:?Usage: $0 <domain>}"
DEPLOY_USER="deploy"
APP_DIR="/home/${DEPLOY_USER}/cverify"

echo "==> Setting up CVerify server for domain: ${DOMAIN}"

# -----------------------------------------------------------------------
# 1. System updates
# -----------------------------------------------------------------------
apt-get update -qq && apt-get upgrade -y -qq

# -----------------------------------------------------------------------
# 2. Install Docker (official method)
# -----------------------------------------------------------------------
if ! command -v docker &>/dev/null; then
    echo "==> Installing Docker..."
    curl -fsSL https://get.docker.com | sh
    systemctl enable docker
    systemctl start docker
fi

# -----------------------------------------------------------------------
# 3. Create deploy user (no root, but in docker group)
# -----------------------------------------------------------------------
if ! id -u "${DEPLOY_USER}" &>/dev/null; then
    echo "==> Creating deploy user: ${DEPLOY_USER}"
    useradd -m -s /bin/bash "${DEPLOY_USER}"
fi
usermod -aG docker "${DEPLOY_USER}"

# -----------------------------------------------------------------------
# 4. Set up SSH for deploy user (GitHub Actions will use this key)
# -----------------------------------------------------------------------
DEPLOY_HOME="/home/${DEPLOY_USER}"
mkdir -p "${DEPLOY_HOME}/.ssh"
chmod 700 "${DEPLOY_HOME}/.ssh"
touch "${DEPLOY_HOME}/.ssh/authorized_keys"
chmod 600 "${DEPLOY_HOME}/.ssh/authorized_keys"
chown -R "${DEPLOY_USER}:${DEPLOY_USER}" "${DEPLOY_HOME}/.ssh"

echo ""
echo ">>> IMPORTANT: Add your GitHub Actions deploy public key to:"
echo "    ${DEPLOY_HOME}/.ssh/authorized_keys"
echo "    Generate with: ssh-keygen -t ed25519 -C 'github-actions-deploy' -f deploy_key"
echo "    Then add deploy_key.pub content to authorized_keys"
echo "    And add deploy_key (private) as VPS_SSH_KEY secret in GitHub"
echo ""

# -----------------------------------------------------------------------
# 5. Create app directory
# -----------------------------------------------------------------------
mkdir -p "${APP_DIR}/nginx"
chown -R "${DEPLOY_USER}:${DEPLOY_USER}" "${APP_DIR}"

# -----------------------------------------------------------------------
# 6. Firewall (UFW)
# -----------------------------------------------------------------------
if command -v ufw &>/dev/null; then
    echo "==> Configuring firewall..."
    ufw --force reset
    ufw default deny incoming
    ufw default allow outgoing
    ufw allow ssh
    ufw allow 80/tcp
    ufw allow 443/tcp
    ufw --force enable
fi

# -----------------------------------------------------------------------
# 7. Install Certbot + obtain SSL certificate
# -----------------------------------------------------------------------
if ! command -v certbot &>/dev/null; then
    echo "==> Installing Certbot..."
    apt-get install -y -qq certbot
fi

echo "==> Obtaining SSL certificate for ${DOMAIN}..."
certbot certonly \
    --standalone \
    --non-interactive \
    --agree-tos \
    --email "admin@${DOMAIN}" \
    -d "${DOMAIN}" || {
    echo ">>> WARNING: certbot failed — ensure DNS A record for ${DOMAIN} points to this server's IP."
    echo ">>> Re-run after DNS propagates: certbot certonly --standalone -d ${DOMAIN}"
}

# -----------------------------------------------------------------------
# 8. Auto-renew SSL via cron
# -----------------------------------------------------------------------
(crontab -l 2>/dev/null; echo "0 3 * * * certbot renew --quiet --post-hook 'docker exec cverify-nginx nginx -s reload'") | crontab -

# -----------------------------------------------------------------------
# 9. Replace ${DOMAIN} in nginx.conf
#    (nginx.conf uses $DOMAIN shell variable — substitute it here)
# -----------------------------------------------------------------------
if [ -f "${APP_DIR}/nginx/nginx.conf" ]; then
    sed -i "s/\${DOMAIN}/${DOMAIN}/g" "${APP_DIR}/nginx/nginx.conf"
fi

# -----------------------------------------------------------------------
# Done
# -----------------------------------------------------------------------
echo ""
echo "==> Server setup complete!"
echo ""
echo "Next steps:"
echo "  1. Add the deploy public key to ${DEPLOY_HOME}/.ssh/authorized_keys"
echo "  2. Add all secrets to GitHub Actions (repo Settings → Secrets → Actions)"
echo "  3. Push to main branch — GitHub Actions will build images and deploy"
echo ""
echo "Manual first deploy (if needed):"
echo "  su - ${DEPLOY_USER}"
echo "  cd ~/cverify"
echo "  cp .env.production.example .env && nano .env   # fill in all values"
echo "  docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d"
