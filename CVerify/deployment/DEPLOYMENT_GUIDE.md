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

`.github/workflows/deploy.yml` triggers on **push to `CVerify-uat`** (plus a
manual `workflow_dispatch`). The branch it deploys is the single
`on.push.branches` list in that file; the deploy step pulls
`${{ github.ref_name }}`, so it follows the trigger automatically. See §6 for
how to cut over to `main` later — a one-line change. **Leave it as
`CVerify-uat` for now.**

> **Note (why the trigger is `push`, not `workflow_run`):** an earlier version
> used `on: workflow_run` to deploy only after CI passed. That never fired —
> `workflow_run` and `workflow_dispatch` only activate when the workflow file
> is on the repo's **default branch** (`main`), but `deploy.yml` lives on
> `CVerify-uat`. `push` has no such restriction, so it actually runs. CI still
> runs in parallel on the same push; deploying strictly *after* CI would mean
> folding this job into `ci.yml` as a gated final stage (not done — keeps the
> deploy path simple).

## 0b. Target VPS

The AWS EC2 instance previously used for `cverify.com.vn` is gone —
production now runs on a **Google Cloud Compute Engine VM**. Standing one up:

```bash
# Reserve a static external IP first — without this, the VM's public IP
# changes on every stop/start, exactly like the AWS "no Elastic IP" trap.
gcloud compute addresses create cverify-ip --region=<REGION>
gcloud compute addresses describe cverify-ip --region=<REGION> --format='get(address)'

# Create the VM (e2-medium = 2 vCPU / 4 GB RAM is enough for this stack;
# go e2-small only if budget-constrained — Postgres + Redis + 3 app
# containers will swap under 2 GB).
gcloud compute instances create cverify-vps \
  --zone=<ZONE> \
  --machine-type=e2-medium \
  --image-family=ubuntu-2204-lts \
  --image-project=ubuntu-os-cloud \
  --boot-disk-size=50GB \
  --address=cverify-ip \
  --tags=http-server,https-server

# Firewall: allow 80/443 from anywhere, restrict 22 to your own IP(s) —
# see section 8 for the full rule set (GCP has no inbound rules open by
# default, unlike a default AWS Security Group in some templates).
```

SSH in with `gcloud compute ssh cverify-vps --zone=<ZONE>` the first time —
this auto-generates an SSH keypair, pushes it to the VM's metadata, and
creates a Linux user matching your Google account name. Whatever that
username turns out to be, use it consistently below (this guide's examples
assume it resolves to `$HOME` on the VM, i.e. `/home/<your-user>`) — there is
no fixed `ec2-user`-style default account on GCP.

If a VM stop/start ever changes the SSH behavior, check the static address is
still attached (`gcloud compute addresses list`) before suspecting DNS or
Nginx.

## 1. Server layout

```
$HOME/swp391-su26-ai-audit-project-swp391_se20a02_group-05/   <- git root, checked out to CVerify-uat
$HOME/swp391-su26-ai-audit-project-swp391_se20a02_group-05/CVerify/  <- app root (docker-compose.yml, deployment/) — all commands below run from here
/opt/cverify/scripts/          <- copy of deployment/scripts/*.sh (see step 4)
/etc/letsencrypt/live/<DOMAIN>/  <- certbot certificates
/etc/nginx/sites-available/cverify.conf + symlink in sites-enabled/  <- deployment/nginx/cverify.conf (Ubuntu/Debian nginx uses sites-available + sites-enabled, unlike Amazon Linux's conf.d-only layout)
```

## 2. Prerequisites on a fresh VPS (Ubuntu 22.04 LTS example)

```bash
sudo apt update
sudo apt install -y git nginx ca-certificates curl

# Docker Engine + the Compose plugin from Docker's own apt repo, not Ubuntu's
# bundled `docker.io` package — Ubuntu 22.04's repo version of the Compose
# plugin lags behind the 2.24+ this stack requires (see note below).
curl -fsSL https://get.docker.com | sudo sh
sudo systemctl enable --now docker nginx
sudo usermod -aG docker "$USER"   # re-login (or `newgrp docker`) after this

# certbot via snap — Ubuntu's officially recommended install path
sudo apt install -y snapd
sudo snap install core; sudo snap refresh core
sudo snap install --classic certbot
sudo ln -s /snap/bin/certbot /usr/bin/certbot

# Ops Agent — ships RAM/CPU metrics and syslog/nginx/docker logs to Cloud
# Logging & Monitoring. Matters here specifically because GCP's own
# hypervisor-level metrics don't include in-guest RAM usage, and this stack
# (Next.js + ASP.NET Core + FastAPI + Postgres + Redis on 4GB) is close
# enough to the ceiling that an OOM kill is a real risk worth being alerted on.
curl -sSO https://dl.google.com/cloudagents/add-google-cloud-ops-agent-repo.sh
sudo bash add-google-cloud-ops-agent-repo.sh --also-run
```

Docker Compose v2 ships as a Docker plugin (`docker compose`, no hyphen) —
confirm with `docker compose version`. `deployment/docker-compose.prod.yml`
uses the `!override` merge tag, which requires **Compose 2.24+**; the
`get.docker.com` script tracks Docker's current stable release and satisfies
this, but double-check the installed version before relying on it.

## 2a. Unattended security upgrades (without auto-restarting the app)

Ubuntu's automatic upgrades are worth enabling for OS security patches, but
left at their default they can also silently upgrade and restart Docker or
Nginx mid-day. Restrict them to the security pocket and blacklist the
packages this stack depends on:

```bash
sudo apt install -y unattended-upgrades
sudo dpkg-reconfigure --priority=low unattended-upgrades   # answer "Yes"
```

Then edit `/etc/apt/apt.conf.d/50unattended-upgrades` and add to the
existing `Unattended-Upgrade::Package-Blacklist` block (create it if the
line doesn't exist):

```
Unattended-Upgrade::Package-Blacklist {
    "docker-ce";
    "docker-ce-cli";
    "containerd.io";
    "nginx";
};
```

Docker/Nginx security updates still need to happen — just do them manually
(`sudo apt upgrade docker-ce nginx` etc.) during a planned maintenance
window instead of letting cron pick the moment.

## 3. Clone and configure secrets

```bash
git clone <REPO_URL> "$HOME/swp391-su26-ai-audit-project-swp391_se20a02_group-05"
cd "$HOME/swp391-su26-ai-audit-project-swp391_se20a02_group-05"
git checkout CVerify-uat
cd CVerify

cp .env.example .env
nano .env   # fill in every value — see .env.example for what each one does
chmod 600 .env   # only the deploy user can read it — it holds every production secret
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
sudo cp deployment/nginx/cverify.conf /etc/nginx/sites-available/cverify.conf
sudo ln -s /etc/nginx/sites-available/cverify.conf /etc/nginx/sites-enabled/cverify.conf
sudo rm -f /etc/nginx/sites-enabled/default   # avoid it clashing on port 80
sudo nginx -t && sudo systemctl reload nginx

sudo certbot --nginx -d <DOMAIN> -d www.<DOMAIN> -d api.<DOMAIN>
```

Note: `certbot --nginx` rewrites `/etc/nginx/sites-available/cverify.conf` in
place to wire in the certificate paths and add its own `# managed by Certbot`
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

**Moving from the old AWS VPS to the new GCP VM requires repointing these
three `A` records to the GCP static IP from step 0b** — everything else
(MX, mail, TXT) stays untouched:

DNS records already in place (update the target IP, do not need to be recreated):
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

Automated: push to `CVerify-uat` (or merge a PR into it) —
`.github/workflows/deploy.yml` fires on that push, SSHes into the VPS, and runs
the equivalent of step 6's manual command. Requires the `VPS_HOST`, `VPS_USER`,
`VPS_SSH_KEY` (and optionally `VPS_SSH_PORT`) repo secrets — see step 6a.

**Which branch deploys — the single switch.** The `on.push.branches` list in
`deploy.yml` is the one place that controls it:

```yaml
on:
  push:
    branches: [CVerify-uat]   # <-- change to [main] to cut over
```

The deploy step pulls `${{ github.ref_name }}` (the pushed branch), so it
follows this list automatically — there is no second value to keep in sync.
When everything is merged into `main` and the team is ready, change
`[CVerify-uat]` to `[main]` and commit. The manual `deploy.sh` takes an
independent `DEPLOY_BRANCH` env override (defaults to `CVerify-uat`):
`DEPLOY_BRANCH=main bash deployment/scripts/deploy.sh`. **Do not change this
now** — production still deploys from `CVerify-uat`.

Manual:
```bash
cd "$HOME/swp391-su26-ai-audit-project-swp391_se20a02_group-05/CVerify"
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
- `VPS_HOST` — the GCP static external IP (or the domain, once DNS points at
  it) from step 0b.
- `VPS_USER` — the Linux username created on the VM (see step 0b; there is
  no fixed default like AWS's `ec2-user`).
- `VPS_SSH_KEY` — a **private key** whose matching public key is authorized
  for `VPS_USER` on the VM. `gcloud compute ssh` manages its own keypair via
  OS Login/metadata by default, which `appleboy/ssh-action` can't use
  directly — generate a dedicated deploy keypair instead
  (`ssh-keygen -t ed25519 -f deploy_key -N ""`), add `deploy_key.pub` to the
  VM via `gcloud compute instances add-metadata cverify-vps --zone=<ZONE>
  --metadata-from-file ssh-keys=<(echo "$VPS_USER:$(cat deploy_key.pub)")`,
  and put `deploy_key`'s contents in the `VPS_SSH_KEY` secret.
- `VPS_SSH_PORT` (optional, defaults 22).

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

## 8. GCP firewall rules

GCP VMs have no inbound ports open by default — create explicit rules,
scoped by the network tags set in step 0b (`http-server`/`https-server`):

```bash
gcloud compute firewall-rules create cverify-allow-http-https \
  --allow=tcp:80,tcp:443 \
  --target-tags=http-server,https-server \
  --source-ranges=0.0.0.0/0

gcloud compute firewall-rules create cverify-allow-ssh \
  --allow=tcp:22 \
  --target-tags=http-server,https-server \
  --source-ranges=<YOUR_IP>/32   # restrict to known IPs, not 0.0.0.0/0
```

Only these ports should be reachable from the public internet:
- `22` (SSH — restricted above)
- `80`, `443` (Nginx)

`postgres`, `redis`, `cverify-ai`, `cverify-core` bind to `127.0.0.1` in
`docker-compose.yml` by design — they are meant to be reachable only from
other containers/the host, never directly from the internet.
`cverify-client` is bound to `127.0.0.1` too once
`deployment/docker-compose.prod.yml` is applied (see step 6) — if you can
still reach port `3000` from outside the VPS after a prod deploy, something
is wrong; do not "fix" that by opening port 3000 in a firewall rule, fix
the compose/nginx setup instead.

## 9. Routine operations

| Task | Command | Automated? |
|---|---|---|
| Backup DB + MinIO (both) | `bash /opt/cverify/scripts/backup-all.sh` | **Yes — daily (see §9b)** |
| Backup DB only | `bash /opt/cverify/scripts/backup-db.sh` | Yes, as part of `backup-all.sh` (§9b) |
| Backup MinIO only | `bash /opt/cverify/scripts/backup-minio.sh` | Yes, as part of `backup-all.sh` (§9b) |
| Restore DB | `bash /opt/cverify/scripts/restore-db.sh <backup-file>` | Manual only (destructive) |
| Restore MinIO | `bash /opt/cverify/scripts/restore-minio.sh <backup-file>` | Manual only (destructive) |
| Clean up stale analysis workspaces | `bash deployment/scripts/cleanup-workspaces.sh` | Yes — the deploy user's crontab, hourly (`:05`), logs to `~/cron-logs/cleanup-workspaces.log` |
| Renew SSL cert | `bash deployment/scripts/renew-ssl.sh` | Yes — `certbot-renew.timer` (systemd, twice daily at 03:00/15:00 UTC ±30min) running `certbot-renew.service`, which calls this script directly from the git checkout |
| Redeploy after a `git pull` | `bash deployment/scripts/deploy.sh` (does the pull itself) | Manual, or via `.github/workflows/deploy.yml` on push to the deploy branch (see §6) |
| Whole-disk snapshot | — | Yes, via a GCE snapshot schedule (see below) |

## 9b. Automated daily backup (PostgreSQL + MinIO)

`deployment/scripts/backup-all.sh` is the single scheduled entry point. It runs
`backup-db.sh` (Postgres `pg_dump` → `~/backups/postgres/cverify_*.sql.gz`) and
`backup-minio.sh` (a `tar czf` of the `minio_data` volume →
`~/backups/minio/cverify_minio_*.tar.gz`), each with **14-day retention**
(override with `RETENTION_DAYS`). It attempts both even if one fails, and exits
non-zero if either failed so the scheduler records a failed run.

MinIO is backed up by tarring its `/data` volume from inside a throwaway
`alpine` container (`--volumes-from cverify-minio`) — no dependency on the
minimal `minio/minio` image having `tar`/`sh`, and no hard-coded volume name.
This requires `deployment/docker-compose.prod.yml` to be applied (it defines the
`minio` service); a base-only stack has no MinIO and `backup-minio.sh` will exit
with a clear error.

Install the scripts once (same copy step as §4 — re-run it after pulling this
change so the new scripts land in `/opt/cverify/scripts`):

```bash
cp deployment/scripts/*.sh /opt/cverify/scripts/
chmod +x /opt/cverify/scripts/*.sh
```

**Option A — cron (simplest; matches the cleanup-workspaces pattern).** As the
deploy user (the one in the `docker` group, whose `$HOME` holds the backups):

```bash
mkdir -p "$HOME/cron-logs"
( crontab -l 2>/dev/null; \
  echo '15 2 * * * /usr/bin/env bash /opt/cverify/scripts/backup-all.sh >> $HOME/cron-logs/backup.log 2>&1' \
) | crontab -
crontab -l | grep backup-all   # confirm it's installed
```

**Option B — systemd timer (parallels `certbot-renew.timer`).** Edit
`deployment/systemd/cverify-backup.service` first and replace `YOUR_DEPLOY_USER`
with the deploy user, then:

```bash
sudo cp deployment/systemd/cverify-backup.service deployment/systemd/cverify-backup.timer /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable --now cverify-backup.timer
systemctl list-timers | grep cverify-backup   # confirm next run time
```

Use **one** of the two, not both. Both fire ~02:15 UTC daily — pick an off-peak
hour for `cverify.com.vn`'s traffic. This complements, and does not replace, the
whole-disk GCE snapshot in §9a: the snapshot recovers OS/config damage, these
dumps recover application data (and restore into a fresh DB/MinIO cleanly).

Restore is manual and destructive — see the §9 table:
`restore-db.sh <file.sql.gz>` and `restore-minio.sh <file.tar.gz>`, each with an
interactive confirmation prompt.

## 9a. Disk snapshot schedule (system-level backup, complements DB backup)

`bash backup-db.sh` only covers Postgres data. For the OS/Docker/Nginx
config layer, attach a snapshot schedule to the boot disk instead of
scripting it by hand:

```bash
gcloud compute resource-policies create snapshot-schedule cverify-daily-snapshot \
  --region="$REGION" \
  --max-retention-days=10 \
  --daily-schedule \
  --start-time=18:00   # UTC — pick an off-peak hour for cverify.com.vn's traffic

gcloud compute disks add-resource-policies cverify-vps \
  --zone="$ZONE" \
  --resource-policies=cverify-daily-snapshot
```

This is a crash-consistent snapshot of the whole disk, not a database-aware
backup — it's for recovering from OS/config damage (bad `apt upgrade`,
accidental `rm`, disk corruption), not as a substitute for `backup-db.sh`'s
Postgres dumps. Keep both.

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
- Application-data backup (Postgres **and** MinIO) now runs daily via
  `backup-all.sh` (§9b) — no longer manual-only. It stores backups **locally on
  the VPS** (`~/backups/`), so they share fate with the boot disk. The §9a
  whole-disk GCE snapshot is the current off-box safety net. If you want the
  `.sql.gz`/`.tar.gz` dumps themselves off-box too, add a `gcloud storage cp`
  of `~/backups/*` to a GCS bucket (`Nearline`/`Coldline`) after the backup —
  deliberately left out for now to avoid needing a service account + bucket
  (out of the "keep it simple, local backups" scope).
- **Deliberately deferred** (evaluated, not adopted — reconsider only if the
  team/infra grows past a single VM):
  - **OS Login** for SSH access control — would require standing up a
    dedicated service account for `appleboy/ssh-action` (SSH username
    becomes `sa_<id>`, not a human-readable one) to keep automated deploys
    working. Not worth the added CI complexity for one VM with
    IP-restricted SSH already in place (§8).
  - **Secret Manager** for `.env` values — real upside (no plaintext secrets
    on disk) but needs a service account plus a fetch-secrets-at-deploy
    step. `chmod 600 .env` (already done in §3) covers the same-VM-user
    threat model cheaply for now.
