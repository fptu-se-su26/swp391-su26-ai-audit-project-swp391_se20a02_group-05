# CVerify — Production Deployment Guide

Referenced by [`deployment/scripts/deploy.sh`](scripts/deploy.sh) and
[`deployment/github/deploy.yml`](github/deploy.yml). This is the single
source of truth for deploying CVerify to a VPS — if a step here contradicts
tribal knowledge or an old runbook, this file wins.

## 0. Which branch is production?

**`CVerify-uat`**, not `main`, as of this writing. The entire `deployment/`
directory (including this file) and the `ForwardedHeaders` reverse-proxy fix
in `CVerify.Core/Program.cs` exist only on `CVerify-uat` — verify with:

```bash
git log --oneline main..CVerify-uat -- deployment/
```

`deployment/github/deploy.yml` is a *template* that triggers on push to
`main`; it has not been copied into `.github/workflows/` yet, so there is
**no automated CI/CD** today. Every deploy is manual until that's wired up.
If/when `deployment/` is merged into `main`, update this section and enable
the workflow — don't let the two drift further apart.

## 1. Server layout

```
/opt/cverify/compose/          <- this repository, checked out to CVerify-uat
/opt/cverify/scripts/          <- copy of deployment/scripts/*.sh (see step 4)
/etc/letsencrypt/live/<DOMAIN>/  <- certbot certificates
/etc/nginx/sites-available/cverify.conf  <- deployment/nginx/cverify.conf
```

## 2. Prerequisites on a fresh VPS (Amazon Linux 2023 example)

```bash
sudo dnf install -y docker git nginx
sudo systemctl enable --now docker nginx
sudo usermod -aG docker ec2-user   # re-login after this

# certbot (via pip, since it's not in the default AL2023 repos)
sudo dnf install -y python3-pip
sudo pip3 install certbot certbot-nginx
```

Docker Compose v2 ships as a Docker plugin (`docker compose`, no hyphen) —
confirm with `docker compose version`; install the compose-plugin package if
missing.

## 3. Clone and configure secrets

```bash
sudo mkdir -p /opt/cverify
sudo chown "$USER":"$USER" /opt/cverify
git clone <REPO_URL> /opt/cverify/compose
cd /opt/cverify/compose
git checkout CVerify-uat

cp .env.example .env
nano .env   # fill in every value — see .env.example for what each one does
```

Minimum production-specific values to set correctly in `.env`:
- `DOMAIN` — the public frontend domain (e.g. `cverify.io.vn`).
- `API_DOMAIN` — the public API subdomain (e.g. `api.cverify.io.vn`). Baked
  into the frontend bundle as `NEXT_PUBLIC_API_URL` at **build time** — see
  step 6.
- `FRONTEND_URL` / `BACKEND_URL` — full origins (`https://` + host) matching
  `DOMAIN` / `API_DOMAIN`. `FRONTEND_URL` also drives CORS on the backend.
- `COOKIE_DOMAIN` — shared parent domain (e.g. `.cverify.io.vn`) so
  `access_token`/`refresh_token`/`CSRF-TOKEN` cookies set by the API on
  `API_DOMAIN` are still visible to the frontend on `DOMAIN`. Without this,
  SSR auth checks silently fail and every POST/PUT/DELETE/PATCH gets a 403
  CSRF error once frontend and API are on different subdomains.
- `Auth__TrustedDomains` — must include `DOMAIN` (e.g.
  `cverify.io.vn;www.cverify.io.vn`) or email-verification/password-reset
  links are rejected as untrusted redirects.
- `ASPNETCORE_ENVIRONMENT=Production`
- `RESET_DATABASE=false`, `ALLOW_DEVELOPMENT_SEEDING=false`
- All secrets (`JWT_KEY`/`JWT_SECRET`, `AI_SERVICE_SHARED_SECRET`,
  `TOKEN_ENCRYPTION_KEY`, DB/Redis passwords, OAuth client secrets, SMTP
  credentials, `ANTHROPIC_API_KEY`, R2 credentials) — generate fresh values,
  do not reuse dev secrets.

## 4. Install operational scripts

```bash
sudo mkdir -p /opt/cverify/scripts
cp deployment/scripts/*.sh /opt/cverify/scripts/
chmod +x /opt/cverify/scripts/*.sh
```

## 5. Nginx + SSL (host-level reverse proxy)

CVerify's Docker Compose stack does **not** include an nginx container — the
reverse proxy runs directly on the host, in front of Docker. Install it once:

```bash
# HTTP-only first, so certbot's HTTP-01 challenge can complete
sudo cp deployment/nginx/cverify.conf /etc/nginx/sites-available/cverify.conf
# Amazon Linux/RHEL nginx has no sites-enabled by default — either add an
# `include /etc/nginx/sites-enabled/*.conf;` line to nginx.conf's http{} block
# and symlink, or copy the file straight into /etc/nginx/conf.d/ instead.
sudo ln -s /etc/nginx/sites-available/cverify.conf /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx

sudo certbot --nginx -d <DOMAIN> -d www.<DOMAIN> -d api.<DOMAIN>
```

One certificate covers all three names as SANs — `deployment/nginx/cverify.conf`
points both the `<DOMAIN>` and `api.<DOMAIN>` server blocks at
`/etc/letsencrypt/live/<DOMAIN>/`. If you issue `api.<DOMAIN>` as a separate
certificate instead, update the `ssl_certificate*` paths in that server block.

`deployment/nginx/cverify.conf` assumes `cverify-client` on `127.0.0.1:3000`
and `cverify-core` on `127.0.0.1:5247` — this matches the host port mappings
in `docker-compose.yml`/`deployment/docker-compose.prod.yml` as long as you
haven't overridden `CORE_PORT`/`CLIENT_PORT` in `.env`. `<DOMAIN>` proxies to
the frontend; `api.<DOMAIN>` proxies straight to the API (see `API_DOMAIN` in
`.env.example`).

Renewal: `deployment/scripts/renew-ssl.sh` — put it on a cron/systemd timer
(not currently automated by anything in this repo).

## 5b. If DNS is proxied through Cloudflare

Run `nslookup <DOMAIN>` — if it resolves to a `104.x`/`172.67.x`/`2606:4700::`
address instead of the VPS's own IP, DNS is proxied through Cloudflare, and
Cloudflare's SSL/TLS mode changes what step 5 needs:
- **Full / Full (strict)**: keep step 5 as-is — Cloudflare needs a valid cert
  on the origin, which is what certbot provides.
- **Flexible**: Cloudflare terminates SSL on the client side and reaches the
  origin over **plain HTTP**. If your Nginx config force-redirects HTTP→HTTPS
  (as `deployment/nginx/cverify.conf` does), this causes a redirect loop.
  Switch Cloudflare to Full (strict) rather than removing the redirect, so
  the origin is also verifiably encrypted.

This repository does not track Cloudflare configuration — check the
Cloudflare dashboard for the account that owns the domain to confirm the
current mode before assuming either case.

## 6. Deploy

```bash
cd /opt/cverify/compose
bash deployment/scripts/deploy.sh
```

This runs the equivalent of:

```bash
docker compose -f docker-compose.yml -f deployment/docker-compose.prod.yml \
  --env-file .env up -d --build --remove-orphans
```

**Always include both `-f` flags.** The base `docker-compose.yml` alone
builds the frontend with a `localhost` API URL meant for local development —
`deployment/docker-compose.prod.yml` is what points the build at `DOMAIN`,
right-sizes container resource limits for a small VPS, and wires the
Core↔AI HMAC secret names correctly. `docker compose build && docker compose
up -d` **without** `-f deployment/docker-compose.prod.yml` is the single most
common way to break this deployment — it silently reintroduces the
`localhost` API URL bug, because `NEXT_PUBLIC_API_URL` is inlined into the
frontend bundle at **build time**, not read at container start.

If you only changed `.env` (not source code), you still need `--build` on the
`cverify-client` service specifically, or the old `localhost`/old-domain
value stays baked into the already-built image.

## 7. Verify

```bash
bash deployment/scripts/health-check.sh
docker compose ps
```

Then from a separate machine (not the VPS itself, to also exercise DNS/proxy):
```bash
curl -I https://<DOMAIN>/
curl -I https://api.<DOMAIN>/health
```
Log into the app and confirm at least one API call succeeds (open DevTools →
Network — no `ERR_CONNECTION_*` on XHR requests). This is the actual
regression check for the `localhost`-baked-into-frontend class of bug; a
green `docker compose ps` does not catch it, since all containers report
healthy independent of what URL the frontend was built with.

## 8. Security Group / firewall

Only these ports should be reachable from the public internet:
- `22` (SSH — restrict to known IPs, not `0.0.0.0/0`)
- `80`, `443` (Nginx)

`postgres`, `redis`, `cverify-ai`, `cverify-core` bind to `127.0.0.1` in
`docker-compose.yml` by design — they are meant to be reachable only from
other containers/the host, never directly from the internet.
`cverify-client` is bound to `127.0.0.1` too once
`deployment/docker-compose.prod.yml` is applied (see step 6) — if you can
still reach port `3000` from outside the VPS after a prod deploy, something
is wrong; do not "fix" that by opening port 3000 in the Security Group, fix
the compose/nginx setup instead.

## 9. Routine operations

| Task | Command |
|---|---|
| Backup DB | `bash /opt/cverify/scripts/backup-db.sh` |
| Restore DB | `bash /opt/cverify/scripts/restore-db.sh <backup-file>` |
| Clean up stale analysis workspaces | `bash /opt/cverify/scripts/cleanup-workspaces.sh` |
| Renew SSL cert | `bash /opt/cverify/scripts/renew-ssl.sh` |
| Redeploy after a `git pull` | `bash deployment/scripts/deploy.sh` (does the pull itself) |

## 10. Known gaps (as of this writing — remove each line once fixed)

- No automated CI/CD — `.github/workflows/deploy.yml` not installed (see §0).
- SSL cert renewal is not on a scheduled job.
- Cloudflare configuration (if in use) is undocumented — see §5b.
