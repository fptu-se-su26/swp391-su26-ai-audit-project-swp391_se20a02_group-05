# CVerify — Production Deployment Guide

Referenced by [`deployment/scripts/deploy.sh`](scripts/deploy.sh) and the
repository-root `.github/workflows/deploy.yml`. This is the single source of
truth for deploying CVerify to a VPS — if a step here contradicts tribal
knowledge or an old runbook, this file wins.

**GitHub Actions workflow location gotcha:** this repository has this
`CVerify/` folder nested inside a parent monorepo. GitHub Actions only ever
reads workflows from `.github/workflows/` at the **true repository root**
(one level above `CVerify/`) — a `.github/workflows/` folder that also exists
nested inside `CVerify/` is dead weight GitHub never executes. Always edit
workflow YAML at the true root, not inside `CVerify/`.

## 0. Which branch is production?

**`CVerify-uat`.** `main` had `CVerify-uat` merged into it via PR #102, but
production deploys are driven off `CVerify-uat` directly, not `main` — keep
pushing deploy-bound work to `CVerify-uat`.

`.github/workflows/deploy.yml` triggers via `workflow_run` after
`.github/workflows/ci.yml` ("CVerify Core Delivery Pipeline") completes
successfully on `CVerify-uat`, plus a manual `workflow_dispatch`.

## 0b. Target VPS

The existing VPS (an AWS EC2 instance, user `ec2-user`) is reused for
`cverify.com.vn` — it's the same host `cverify.io.vn` was already deployed
to. If the instance has no Elastic IP attached, its public IP changes on
every stop/start; `ssh ec2-user@<IP>` failing after a reboot is the first
thing to check (AWS Console → EC2 → Instances → confirm `running` and note
the current public IP), not a DNS or Nginx problem.

## 1. Server layout

```
/home/ec2-user/swp391-su26-ai-audit-project-swp391_se20a02_group-05/   <- git root, checked out to CVerify-uat
/home/ec2-user/swp391-su26-ai-audit-project-swp391_se20a02_group-05/CVerify/  <- app root (docker-compose.yml, deployment/) — all commands below run from here
/opt/cverify/scripts/          <- copy of deployment/scripts/*.sh (see step 4)
/etc/letsencrypt/live/<DOMAIN>/  <- certbot certificates
/etc/nginx/conf.d/cverify.conf  <- deployment/nginx/cverify.conf (Amazon Linux/RHEL nginx has no sites-enabled by default; conf.d is what's actually in use)
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
confirm with `docker compose version`. `deployment/docker-compose.prod.yml`
uses the `!override` merge tag, which requires **Compose 2.24+**.

## 3. Clone and configure secrets

```bash
git clone <REPO_URL> /home/ec2-user/swp391-su26-ai-audit-project-swp391_se20a02_group-05
cd /home/ec2-user/swp391-su26-ai-audit-project-swp391_se20a02_group-05
git checkout CVerify-uat
cd CVerify

cp .env.example .env
nano .env   # fill in every value — see .env.example for what each one does
```

Minimum production-specific values to set correctly in `.env`:
- `DOMAIN` — the public frontend domain: `cverify.com.vn`.
- `API_DOMAIN` — the public API subdomain: `api.cverify.com.vn`. Baked
  into the frontend bundle as `NEXT_PUBLIC_API_URL` at **build time** — see
  step 6.
- `FRONTEND_URL` / `BACKEND_URL` — full origins (`https://` + host) matching
  `DOMAIN` / `API_DOMAIN`. `FRONTEND_URL` also drives CORS on the backend.
- `COOKIE_DOMAIN` — shared parent domain `.cverify.com.vn` so
  `access_token`/`refresh_token`/`CSRF-TOKEN` cookies set by the API on
  `API_DOMAIN` are still visible to the frontend on `DOMAIN`. Without this,
  SSR auth checks silently fail and every POST/PUT/DELETE/PATCH gets a 403
  CSRF error once frontend and API are on different subdomains.
- `Auth__TrustedDomains` — must include `DOMAIN` (e.g.
  `cverify.com.vn;www.cverify.com.vn`) or email-verification/password-reset
  links are rejected as untrusted redirects.
- `ASPNETCORE_ENVIRONMENT=Production`
- `RESET_DATABASE=false`, `ALLOW_DEVELOPMENT_SEEDING=false`
- All secrets (`JWT_KEY`, `AI_SERVICE_SHARED_SECRET`, `TOKEN_ENCRYPTION_KEY`,
  DB/Redis passwords, OAuth client secrets, SMTP/SendGrid credentials,
  `ANTHROPIC_API_KEY`, R2 credentials) — generate fresh values, do not reuse
  dev secrets.

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
sudo cp deployment/nginx/cverify.conf /etc/nginx/conf.d/cverify.conf
sudo nginx -t && sudo systemctl reload nginx

sudo certbot --nginx -d <DOMAIN> -d www.<DOMAIN> -d api.<DOMAIN>
```

Note: `certbot --nginx` rewrites `/etc/nginx/conf.d/cverify.conf` in place to
wire in the certificate paths and add its own `# managed by Certbot`
HTTP→HTTPS redirect blocks. After the first run, the live file will no
longer match `deployment/nginx/cverify.conf` byte-for-byte — that's expected
and safe. Do **not** blindly overwrite the live file with a fresh copy from
git on a later deploy; diff first, since the repo's copy lacks Certbot's
markers.

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

Renewal: `deployment/scripts/renew-ssl.sh` — put it on a cron/systemd timer.

## 5b. DNS provider — P.A Vietnam (no Cloudflare proxy)

`cverify.com.vn` is registered and DNS-hosted directly at P.A Vietnam
(nameservers `dnssec1.pavietnam.vn` / `dnssec2.pavietnam.vn` /
`dnssecbak.pavietnam.net` — DNSSEC is enabled on the domain, a separate P.A
add-on). Unlike the previous `cverify.io.vn` setup, there is **no Cloudflare
proxy** in front — DNS resolves straight to the VPS, so certbot's HTTP-01
challenge on port 80 (step 5) works without any Cloudflare SSL/TLS mode
caveats. Confirm with `nslookup cverify.com.vn` — it should resolve directly
to the VPS's own IP, not a Cloudflare edge IP.

DNS records already in place (verified, do not need to be created):
- `A cverify.com.vn`, `A www.cverify.com.vn`, `A api.cverify.com.vn` → VPS
- `MX cverify.com.vn` (priority 10) → `mail.cverify.com.vn`
- `A mail.cverify.com.vn`, `A mx.cverify.com.vn` → P.A's mail server
  (separate IP from the VPS — this is P.A's **Email Server (Email Pro #1)**
  add-on, used for `@cverify.com.vn` mailboxes; unrelated to this app)
- `TXT cverify.com.vn` → `v=spf1 a mx ~all`

**Do not change the MX, `mail.*`, or `mx.*` records** when editing DNS at
`https://access.pavietnam.vn` — that breaks `@cverify.com.vn` email
delivery. Only the `A` records for the apex/`www`/`api` host names are
relevant to this app, and they already point at the VPS.

P.A's separately-purchased **Web Hosting "Khởi Đầu"** (shared cPanel/
DirectAdmin at a different IP) is not used by this deployment — it cannot
run a multi-container Docker Compose stack. It sits unused unless
repurposed for something else later; nothing in this repo depends on it.

## 6. Deploy

Automated: push to `CVerify-uat` (or merge a PR into it) — once CI
(`.github/workflows/ci.yml`) passes, `.github/workflows/deploy.yml` SSHes into
the VPS and runs the equivalent of step 6's manual command. Requires the
`VPS_HOST`, `VPS_USER`, `VPS_SSH_KEY` (and optionally `VPS_SSH_PORT`) repo
secrets to be configured — see step 6a.

Manual:
```bash
cd /home/ec2-user/swp391-su26-ai-audit-project-swp391_se20a02_group-05/CVerify
bash deployment/scripts/deploy.sh
```

This runs the equivalent of:

```bash
docker compose -f docker-compose.yml -f deployment/docker-compose.prod.yml \
  --env-file .env up -d --build --remove-orphans
```

**Always include both `-f` flags.** The base `docker-compose.yml` alone
builds the frontend with a `localhost` API URL meant for local development —
`deployment/docker-compose.prod.yml` is what points the build at `API_DOMAIN`
and right-sizes container resource limits for a small VPS. `docker compose
build && docker compose up -d` **without** `-f deployment/docker-compose.prod.yml`
is the single most common way to break this deployment — it silently
reintroduces the `localhost` API URL bug, because `NEXT_PUBLIC_API_URL` is
inlined into the frontend bundle at **build time**, not read at container
start.

If you only changed `.env` (not source code), you still need `--build` on the
`cverify-client` service specifically, or the old `localhost`/old-domain
value stays baked into the already-built image.

## 6a. GitHub Actions secrets for automated deploy

Set these in the repository's Settings → Secrets and variables → Actions:
- `VPS_HOST`, `VPS_USER`, `VPS_SSH_KEY` (private key with access to
  `/home/ec2-user/swp391-su26-ai-audit-project-swp391_se20a02_group-05` on
  the VPS), `VPS_SSH_PORT` (optional, defaults 22).

No application secret is ever passed through GitHub — the real `.env` lives
only on the VPS; the deploy step just `git pull`s and rebuilds there.

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

| Task | Command | Automated? |
|---|---|---|
| Backup DB | `bash /opt/cverify/scripts/backup-db.sh` | Manual only |
| Restore DB | `bash /opt/cverify/scripts/restore-db.sh <backup-file>` | Manual only (destructive) |
| Clean up stale analysis workspaces | `bash deployment/scripts/cleanup-workspaces.sh` | Yes — `ec2-user` crontab, hourly (`:05`), logs to `~/cron-logs/cleanup-workspaces.log` |
| Renew SSL cert | `bash deployment/scripts/renew-ssl.sh` | Yes — `certbot-renew.timer` (systemd, twice daily at 03:00/15:00 UTC ±30min) running `certbot-renew.service`, which calls this script directly from the git checkout |
| Redeploy after a `git pull` | `bash deployment/scripts/deploy.sh` (does the pull itself) | Manual, or via `.github/workflows/deploy.yml` on push to `CVerify-uat` |

Do **not** also add a cron entry for `renew-ssl.sh` — `certbot-renew.timer`
already owns that job, and running both concurrently causes a "Another
instance of Certbot is already running" lock conflict (hit this directly
while setting up cron — confirm with `systemctl list-timers | grep certbot`
before adding anything new here).

## 10. Known gaps (as of this writing — remove each line once fixed)

- DNSSEC is enabled on `cverify.com.vn` at the registrar — if DNS records
  ever need to change, re-check that DNSSEC signing isn't left in a broken
  state afterward (P.A's panel handles re-signing automatically in normal
  use, but this hasn't been stress-tested against manual record edits).
- Backup and restore are manual-only — no scheduled backup job exists yet.
