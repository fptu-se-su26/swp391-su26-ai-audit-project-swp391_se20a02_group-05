# CVerify — Verified Ground Truth

Reverse-engineered from source. **Every diagram in this directory must agree with this file.**
Where source contradicts existing project documentation, source wins and the divergence is recorded here.

Verification date: 2026-07-18 · Branch: `CVerify-Deployment` · Commit at time of analysis: `7eb5ff2`

---

## 1. System shape

| Container | Technology | Source |
|---|---|---|
| Web client | Next.js 16.2 App Router, React 19.2, standalone output, Zustand, axios, SignalR | `CVerify/client` |
| Core API | ASP.NET Core (.NET 10), modular monolith | `CVerify/CVerify.Core` |
| AI service | FastAPI, Python 3.11 | `CVerify/CVerify.AI` |
| Database | PostgreSQL 16 (`pgvector/pgvector:0.7.0-pg16` image) | `docker-compose.yml` |
| Cache/broker | Redis 7.2.4-alpine | `docker-compose.yml` |
| Object storage | MinIO (prod). Cloudflare R2 also supported by the same S3 client. | `deployment/docker-compose.prod.yml` |
| Edge | nginx, **host-installed, not containerised** | `deployment/nginx/cverify.conf` |

Module boundaries inside the API are enforced by a test:
`tests/CVerify.API.UnitTests/Architecture/ModularBoundaryTests.cs`.

---

## 2. Actors and roles — as enforced in code

**System roles:** `SUPER_ADMIN`, `ADMIN`, `USER` (`PermissionSeeder.cs`, `AuthService.cs:978`).

**Organization roles:** `OWNER`, `REPRESENTATIVE`, `HR`, `MEMBER` (`OrganizationRole` enum).
⚠️ No entity property is typed as this enum — `OrganizationMembership.Role`, `OrganizationAuthority.Role`
and `WorkspaceMember.Role` are plain `string`. It is a code-level vocabulary, not a column type.

⚠️ **"Recruiter" is not a role in this system.** Recruiting is performed by organization members holding
`HR` / `REPRESENTATIVE` / `OWNER`. Diagrams must not invent a Recruiter role.

**Authorization is three coexisting mechanisms:**
1. Role-based — `[Authorize(Roles="SUPER_ADMIN,ADMIN")]` (Recovery controllers only).
2. Permission-based — `[HasPermission("...")]`; the permission string *is* the policy name, resolved
   dynamically by `PermissionPolicyProvider` → `PermissionHandler`, with colon-segment wildcard matching.
3. Organization-scoped — imperative `_authService.AuthorizeAsync(userId, orgId, permission)` calls inside
   controller bodies. Scope strings are `permission:name:SCOPETYPE:scopeId`. An `ORGANIZATION`-scoped grant
   implicitly covers child `WORKSPACE` scopes (`PermissionEvaluator.cs:95,111`).

⚠️ `AppPermissions.cs` **is an empty class** — it defines no constants. Every `[HasPermission]` passes a raw
string literal. Only 9 distinct permission strings are used in attributes: `admin:ai:audit`,
`admin:roles:manage`, `admin:roles:view`, `admin:users:manage`, `admin:users:view`,
`forum:category:manage`, `forum:moderation:queue`, `forum:tag:manage`, `forum:topic:moderate`.
None of these appear in `PermissionSeeder.cs`; `admin:*` resolves via `IAdminAuthorizationService`.

---

## 3. Authentication — verified mechanism

JWT (HS256) delivered in **HttpOnly cookies**, not the `Authorization` header.

`Program.cs:506-533` — `JwtBearerEvents.OnMessageReceived` **unconditionally** sets
`context.Token = Request.Cookies["access_token"]`. A Bearer header is therefore ignored.

Cookies (`TokenService.SetTokenInsideCookie:139`): HttpOnly, Secure (non-dev), SameSite=Lax, Path=/,
`Domain = Auth.CookieDomain` — a shared parent domain, required because the client and API sit on
different subdomains.

- `access_token` 15 min · `refresh_token` 7 days (RememberMe) or 24 h
- Refresh rotation is lock-guarded (`lock:token:rotate:{token}`) with **reuse detection**
  (`TOKEN_REUSE_DETECTED` → revoke whole session chain) and a **grace period** for multi-tab races.
- `SessionValidationMiddleware` runs **between** `UseAuthentication()` and `UseAuthorization()`.
- OTP is **email-delivered step-up**, not TOTP. There is no authenticator-app 2FA.

Middleware order (`Program.cs:610-623`):
`RequestLogging → ExceptionHandler → CORS → SecurityHeaders → RateLimiter → Authentication →
SessionValidation → Authorization → endpoints`.

---

## 4. Data model

**152 tables · 201 declared foreign keys.** PG extensions: `citext`, `pgcrypto`.
One native PG enum: `user_status`. All other enums are `integer` or `text`.

Table counts by declaring namespace:
AiChat 2 · Auth 8 · Forum 17 · Profiles 30 · Recovery 7 · SourceCode 9 · Shared/System 72 ·
Shared/Email 1 · Pipelines 4.

⚠️ `Modules/Admin/` and `Modules/Intelligence/` have **no `Entities/` directory** — they own no tables.
Admin tables live in `Modules/Shared/Domain/Entities/`.

### 4.1 ERD hazards — must be honoured by every ERD

1. `ux_repository_assessments_repo_sha` is named like a unique constraint but is **NOT unique**.
2. The `repository_assessments` → `repository_capabilities` / `repository_domains` /
   `repository_skill_attributions` / `repository_intelligence_signals` cluster has **zero FK constraints**
   — indexes only. Draw these edges as *logical-only*.
3. `organization_followers.UserId` is part of the PK but has **no FK**, while `OrganizationId` does.
   Asymmetric; do not draw as a clean symmetric M2M.
4. `cv_repository_mappings.UserId` is indexed but has **no FK**.
5. Polymorphic columns with no FK: `profile_attachments.EntityId`, `audit_logs.ScopeId`,
   `activity_events.ResourceId/CorrelationId/CausationId`, `in_app_notifications.ResourceId`,
   `trust_profiles.TargetEntityId`, `role_assignments.ScopeId`, `roles.TenantId`,
   `pipeline_jobs.ReferenceId`, `forum_moderation_logs.TargetId`.
6. `otp_verifications.ChallengeId` and `candidate_assessments.CvId` reference targets that **do not exist**
   in the schema. Genuinely ambiguous — mark `<<Not verified>>`.
7. Primitive collections (`text[]`, `uuid[]`, `float[]`) and `jsonb` columns are **columns, not tables**:
   `users.LinkedEmails` (jsonb), `organization_recovery_claims.Documents` (jsonb),
   `capability_nodes.VectorEmbedding` (float[]), `candidate_search_profiles.SearchEmbedding` (float[]),
   `requirement_vector_snapshots.Vector` (float[]), and others.

⚠️ **The `float[]` embedding columns are schema only.** The AI service writes no embeddings — see §6.
The pgvector image is used but no vector index or similarity query was found.

### 4.2 True M2M junction tables (composite PK of exactly two FKs)

`user_roles`, `role_permissions`, `forum_topic_tags`, `forum_user_badges`, `forum_bookmarks`,
`forum_follows`, `forum_category_moderators`, `organization_followers`*, `user_followers` (self-ref),
`candidate_capability_evidences`, `capability_hierarchies` (self-ref), `capability_edges` (self-ref).
*`organization_followers` — see hazard 3.

---

## 5. API surface

34 controllers. Highlights and hazards:

**5 SSE endpoints** (`text/event-stream`):
`POST api/ai/chat/stream` · `GET api/v1/candidate-assessments/progress/{userId}` ·
`GET api/v1/hiring-requirements/{id}/progress-stream` ·
`GET api/v1/streaming/sessions/{id}/progress-stream` ·
`GET api/repository-analyses/jobs/{jobId}/progress-stream`

**2 SignalR hubs:** `/hubs/notifications` (NotificationHub — no groups, no client-invokable methods)
and `/hubs/admin` (AdminHub — group `"admins"`, gated on SUPER_ADMIN/ADMIN).
Fan-out is Redis pub/sub on channel `cverify:notifications`.

### 5.1 ⚠️ Unauthenticated endpoints — verified, security-relevant

| Endpoint | Note |
|---|---|
| `EmailTestController` — all 6 endpoints | **No auth attribute at all**, no global fallback policy. Includes `POST api/emailtest/send-verification`, `GET api/emailtest/logs`, `DELETE api/emailtest/logs` |
| `SystemController` — all 6 endpoints | No auth attributes |
| `GET api/v1/ai-jobs/{jobId}/artifacts/{key}` | Class-level `[AllowAnonymous]`; comment says "allow internal microservice to call without bearer auth". Any in-body guard unverified |
| `GET api/v1/candidate-assessments/dev-trigger` | `[AllowAnonymous]` |
| `POST api/v1/admin/candidate-assessments/reprocess` (both routes) | `[AllowAnonymous]` on an admin route |
| `GET api/repositories/{repoId}/dev-reset-and-analyze` | `[AllowAnonymous]` |
| `POST api/auth/recovery/level2/vote` | `[AllowAnonymous]` on an admin quorum action; presumably token-gated in body — unverified |
| `POST /api/v1/analysis/jobs/{job_id}/cancel` (AI service) | **No HMAC**, unlike the other 11 AI endpoints |

Also: CORS `"AllowFrontend"` hardcodes `http://localhost:3000` and `http://127.0.0.1:3000` in **all**
environments including Production (`Program.cs`).

### 5.2 Duplicate surfaces

`api/organizations/*` and `api/workspace/*` are near-duplicate controllers. One auth divergence:
`GetWorkspaceMembers` is `[AllowAnonymous]` while `GetOrganizationMembers` is not.
`BusinessRoleController : OrganizationRoleController` re-exposes all 9 role actions under
`api/business/{orgSlug}/roles/*`.

### 5.3 Core API → AI service

Named `HttpClient` `"AiServiceClient"`, 5-minute timeout. Every call HMAC-signed by
`HmacSignatureService`: `HMACSHA256(METHOD + URL + BODY + TIMESTAMP + NONCE, SharedSecret)`,
headers `X-Client-Id`, `X-Timestamp`, `X-Nonce`, `X-Correlation-Id`, `X-Signature`.

8 endpoints called: `/chat/stream`, `/repository/assess`, `/candidate/assess/stream`,
`/candidate/assess/score`, `/hiring-requirements/generate/stream`,
`/hiring-requirements/generate-artifact/stream`, `/analysis/task/execute`, `/analysis/task/aggregate`.

Reverse direction: the AI service calls back to `GET api/v1/ai-jobs/{jobId}/artifacts/{key}`.

### 5.4 Background workers — 15 registered hosted services

`AppLoggingBackgroundWorker`, `EmailOutboxBackgroundProcessor` (5 s), `TokenCleanupBackgroundJob` (1 h),
`RecoveryClaimBackgroundWorker` (10 s), `OtpCleanupBackgroundWorker` (1 h),
`BackgroundRepositorySyncProcessor`, `AnalysisQueueRecoverySweeper`,
`BackgroundRepositoryAnalysisProcessor`, `BackgroundCandidateAssessmentProcessor`,
`BackgroundCandidateAssessmentBackfillProcessor`, `TalentOutboxBackgroundProcessor` (3 s),
`RedisNotificationSubscriberWorker`, `ActivityEventProjectionWorker` (5 s),
`CandidateRankingProjectionWorker` (15 min), `BackgroundEmailQueueProcessor`.

⚠️ `PendingLinkCleanupService.cs` is a complete `BackgroundService` that is **never registered** — dead
code. Pending OAuth links are never swept.

⚠️ All queues (`ICandidateAssessmentQueue`, `IRepositoryAnalysisQueue`, `IRepositorySyncQueue`,
`IEmailQueue`) are **singleton in-process, not durable**. `AnalysisQueueRecoverySweeper` exists
specifically to recover jobs lost across restarts.

**Outbox pattern:** single table `outbox_messages`, written in the same transaction as the business
change. **Two independent consumers** read it: `EmailOutboxBackgroundProcessor` (email) and
`TalentOutboxBackgroundProcessor` (repository-intelligence pipeline triggers).

---

## 6. AI service — what actually runs

### 6.1 ⚠️ Claimed vs implemented

The pipeline *"OCR → Text Extraction → Embedding → Skill Extraction → SFIA → O\*NET → Scoring"*
**is not what this codebase does.**

| Claimed stage | Reality |
|---|---|
| OCR / text extraction | **Dead code.** 5 extractors in `shared/extractors/`; no module under `app/` imports them (tests only). PDF is `markitdown` text-layer only. Vision OCR is gated on `ENABLE_VISION_CERTIFICATE_OCR`, default **False** |
| Embedding | **Absent.** `repository/embedding/__init__.py` = `# Pruned embedding module`. No vector library in `requirements.txt` (13 packages) |
| SFIA mapping | **Name only.** A field called `sfiaCategory` holding free text ("Software Development"). Real SFIA uses 3-letter codes + Levels 1–7. No SFIA file, no levels, no version |
| O\*NET mapping | **Constants only.** Valid-format O\*NET-SOC codes hand-copied into a ~190-entry Python dict. No O\*NET file, no API, no lookup. `onetCode` is written to output and **never read** by any scoring logic |
| Skill extraction, scoring, report | **Genuinely implemented**, and richer than the claim |

### 6.2 Four pipelines

- **Line 1 — Repository** (`github_analysis_orchestrator.py`, 3977 lines): 22 dispatchable tasks
  (L1-001…L1-018 plus SecurityAnalysis, RepositoryClassification, RepositorySummary, CvSynthesis).
  **Flat `if/elif` dispatcher — no internal DAG.** Ordering is owned by the .NET caller. State passes
  **through the filesystem**: `temp_clones/{job_id}/{task_type}_result.json`. L1-001 is a hard
  prerequisite (writes `meta.json`).
- **Line 2 — Candidate**: **16-task validated DAG**, cycle-checked at construction.
  Roots `L2-001, L2-007, L2-008, L2-011`; sink `L2-015`. See `mermaid/17-ai-pipeline-candidate-dag.mmd`.
  L2-004/005/006/009/010 are **hybrid**: deterministic result overrides the LLM's answer.
- **Line 3 — JD matching**: 13 standalone tasks, no DAG, no shared context.
  ⚠️ `L3-014` does not exist; `ApplicationQualityGate` is named but has no dispatch branch.
- **Line 4 — Requirement artifacts**: one Claude call → 5 artifacts, with Pydantic self-correction retry.
  ⚠️ `generate_artifact_stream` **ignores its `artifact_type` argument** and regenerates everything.

### 6.3 Scoring (real, deterministic)

5 dimensions, weights `skillDepth 0.35 · ownership 0.25 · architecture 0.20 · problemSolving 0.12 ·
impact 0.08`. Verified/self-declared blend `0.85/0.15` with a **0.40 ceiling** when no verified repos
exist. Cohort-normalised through an 11-point interpolation table
(`cohort_snapshot_v1.json`, synthetic, `totalCandidates: 1000`).

⚠️ `scoring_policy.json` **does not exist at the path the code loads it from**. Three call sites each
carry an inlined copy of the defaults, so the "configurable policy" is inert.

⚠️ `risk_policy.json` `type_multipliers` is present in the JSON but **never read**.

### 6.4 Claude integration

Default model `claude-3-5-sonnet-20241022`. Only other model: hardcoded `claude-3-haiku-20240307`
for vision OCR. All calls `stream=True`, `max_tokens=8192`, prompt caching via `cache_control: ephemeral`.
Retry on 429/5xx with exponential backoff + jitter (stream *creation* only — mid-stream failure is not
retried). Cancellation polls Redis `ai:cancel:{correlation_id}`.

### 6.5 ⚠️ Security stubs

`prompt_sanitizer.py` returns `is_suspicious=False` always. `malicious_repo_detector.py` returns `False`
always. `input_boundary.validate_token_count` is `pass`. **None of the three is imported anywhere.**
Untrusted repository code, commit messages and CV text reach Claude prompts with no injection screening.

Other verified issues: `hmac_auth.py:78` logs the correct computed signature on mismatch (signing oracle);
clone URLs embed the PAT (`https://{token}@github.com/...`) and git stderr propagates into SSE error
payloads; `AiCostTracker._executions` grows unboundedly.

### 6.6 Dead / phantom code inventory

`extractors/*` · `repository/embedding/` · `skills/data/skill_ontology.json` (4 skills, zero readers) ·
`weighted_scoring_engine.py` (zero importers) · `repo_selector.py` (`return repos`) ·
`architecture_pattern_detector.py` (`return []`) · `prompt_sanitizer.py` · `malicious_repo_detector.py` ·
`pipeline_metrics.py` (all `pass`, no `/metrics` endpoint) · `agents/base.py` `IAgent` (no implementations) ·
`PendingLinkCleanupService.cs` (never registered).

---

## 7. Deployment — GCP, not AWS

⚠️ The brief assumed AWS + Cloudflare + R2. All three are wrong for the current deployment.

| Assumption | Reality | Evidence |
|---|---|---|
| AWS EC2 | **Google Cloud Compute Engine**, `e2-medium` (2 vCPU / 4 GB), Ubuntu 22.04, treated as a plain VPS | `DEPLOYMENT_GUIDE.md` §0b: the AWS EC2 instance "is gone". `gcloud compute instances create cverify-vps` |
| Cloudflare proxy | **None. No CDN at all.** DNS resolves straight to the VM | `cverify.conf` header; `DEPLOYMENT_GUIDE.md` §5b |
| Cloudflare R2 | **Self-hosted MinIO.** Env keys keep `R2_*` names because the S3 client is generic | `docker-compose.prod.yml` comment |

DNS/registrar: **P.A Vietnam**, DNSSEC enabled. Domain `cverify.com.vn`, with `api.cverify.com.vn`.
TLS: Let's Encrypt, one cert with 3 SANs. Monitoring: **GCP Ops Agent** + Cloud Monitoring
(project `cverify-production`), 2 uptime checks, 5 alert policies. DR: daily GCE disk snapshots.

**nginx** (host-installed): upstreams `127.0.0.1:3000` (client) and `127.0.0.1:5247` (core).
Rate-limit zones `cverify_general` 10r/s and `cverify_auth` 5r/m. HSTS, nosniff, DENY, Referrer-Policy,
Permissions-Policy — **no CSP**. Dedicated SSE location with `gzip off`, `proxy_buffering off`, 300 s.

**Compose:** 5 services in dev; prod overlay adds **MinIO** (no host ports) and uses `ports: !override`
to stop the client being publicly exposed.
⚠️ `cverify-core` depends on `cverify-ai: service_healthy`, but `cverify-ai` declares **no healthcheck**.

**CI** (`ci.yml`, "CVerify Core Delivery Pipeline"): 5 jobs — `frontend-ci`, `backend-ci`,
`backend-integration-tests`, `ai-service-ci`, `docker-validation`. Coverage is uploaded but
**no threshold is enforced**.

**CD** (`deploy-vps.yml`): triggers on push to `CVerify-Deployment`, SSH via `appleboy/ssh-action`,
30 min timeout, then `health-check.sh`.
⚠️ **Deploy is NOT gated on CI** — CI runs in parallel on the same push, so a red build still ships.
The workflow header documents this as a conscious trade-off.

---

## 8. Frontend

Next.js 16 App Router. Route groups: `(auth)`, `(candidate)`, `(admin)`, plus `organization/(admin)`,
`organization/[organizationSlug]/(public)` and `/(private)`, and a `recruitment/*` sub-area.

API client: `src/infrastructure/http/axios-client.ts` (canonical);
`src/services/axios-client.ts` is a `@deprecated` bridge still imported by 15 files — a live migration.
Cookie auth + CSRF header; single-flight refresh queue; browser-only in-flight GET deduplication
(added because duplicate `/auth/providers` calls tripped nginx's burst limit).

⚠️ **SSE auth diverges from the rest of the app.** `use-streaming-store.ts:405,624` appends a `token`
query parameter read from `localStorage`/`sessionStorage`, while everything else uses HttpOnly cookies.
Tokens in URLs land in nginx access logs, which are shipped to Cloud Logging.

State: Zustand only. No React Query/SWR/Redux.

---

## 9. Items marked `<<Not verified>>`

- Any in-body authorization guard on `GET api/v1/ai-jobs/{jobId}/artifacts/{key}`.
- Any in-body token gate on `POST api/auth/recovery/level2/vote`.
- Target of `otp_verifications.ChallengeId`.
- Target of `candidate_assessments.CvId` (no `cvs` table exists).
- Emitted `ON DELETE` SQL for FKs configured without an explicit `OnDelete` (marked `(default)`);
  generated migration SQL was not inspected.
- Whether the `token` key read by the streaming store is ever actually populated.
- Value sets for `user_profiles.ProfileVisibility`, `user_profiles.AiTalentDiscovery`,
  `candidate_evaluation_snapshots.VerificationState` — no enumeration found in code.
- Whether the nginx SSE regex covers all 5 SSE routes (the config comment self-flags this as unchecked).

---

## 10. Corrections found during diagram construction

These supersede earlier statements in this file and in the original analysis reports.
Recorded because the correction itself is evidence of how misleading the code comments are.

### 10.1 `hiring_requirements.Status` — earlier claim was wrong

An earlier reading claimed the service writes `Active` and `Cancelled` to this column. **It does not.**
Those assignments target *different* entities:
- `HiringRequirementService.cs:1212,:1266` writes `Active`/`Archived` to a **`CapabilityCatalogItem`**
- `HiringRequirementService.cs:920,:935,:973` writes `Cancelled`/`Failed` to a **`RequirementArtifact`**

Every actual `req.Status` assignment is one of:
**`Draft` · `Generating` · `Ready` · `Published` · `Archived`**

The real discrepancy is that **`Generating` and `Ready` are undocumented** in the entity comment, and they
are load-bearing: `JobVacancyController.cs:140` gates vacancy creation on them.

### 10.2 `analysis_jobs.Status` — observed set is 12, not 8

Full observed set: `Queued`, `Running`, `RunningAgents`, `Completed`, `Failed`, `Cancelled`, `TimedOut`,
`Preparing`, `CloningRepository`, `DetectingTechnologyStack`, `SamplingCode`, `AggregatingResults`,
`SavingReport`.

⚠️ **Six of these are currently unreachable.** Three are only ever passed to
`UpdateJobStateAsync` (declared `RepositoryAnalysisService.cs:1521`), which has **zero call sites**;
the other three appear only inside `activeStates` membership arrays, never as an assignment.

⚠️ `Pending` is **not** an `analysis_jobs` status — it belongs to
`source_code_repositories.LatestAnalysisStatus`. Earlier conflation was incorrect.

### 10.3 Other corrections

- `organization_recovery_claims.Status` has a **fifth** value, `PendingReview`
  (`RecoveryClaimBackgroundWorker.cs:76`), absent from the entity comment.
- `recovery_execution_locks.Status = "Locked"` is **never assigned**. Every row is constructed with
  `"InProgress"` (`RecoveryService.cs:531`). The documented initial state does not occur.
- `AuthService.DeleteMeAsync:1557` assigns `Status = DELETED` **directly**, bypassing `TransitionTo` and
  therefore bypassing the `IsLegalHold` guard. Its own comment cites "legacy test compatibility".
  This is a real correctness concern, not a documentation issue.
- `RecoveryService.cs` and `OrganizationReclaimService.cs` are **line-for-line duplicates**.

### 10.4 Additional `<<Not verified>>` items

- No writer was found for `UserStatus.SUSPENDED` or `BANNED`. Both are read in `IdentityStateResolver`,
  `AccountService` and `TrustEngineService`, but no code path assigns them. State-diagram edges into
  these states are labelled *"allowed by TransitionTo"* rather than *"observed"*.
- No worker transitions a rotation request to `expired`.
- PlantUML files were checked structurally (balanced `@startuml`/`@enduml`, braces, `note`/`end note`),
  not compiled — no PlantUML renderer is installed in this environment.
