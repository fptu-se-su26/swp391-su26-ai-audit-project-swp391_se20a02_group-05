/*
 * CVerify — C4 model (Structurizr DSL)
 *
 * Reverse-engineered from source. Render with:
 *   docker run -it --rm -p 8080:8080 -v "$PWD":/usr/local/structurizr structurizr/lite
 *
 * Provenance notes:
 *   - Container boundaries follow the deployed compose services, not folder layout.
 *   - Component boundaries inside the API follow CVerify.Core/Modules/<Module>/,
 *     which is a genuine enforced boundary (tests/CVerify.API.UnitTests/Architecture/
 *     ModularBoundaryTests.cs asserts it).
 *   - Nothing below is aspirational; every element maps to code that runs.
 */
workspace "CVerify" "Candidate capability verification and evidence-based talent matching platform" {

    model {
        guest       = person "Guest"                   "Unauthenticated visitor. Public jobs, profiles, forum, rankings."
        candidate   = person "Candidate"               "Role USER. Builds CV, links source-code providers, triggers assessments."
        orgMember   = person "Organization Member"     "Org roles OWNER / REPRESENTATIVE / HR / MEMBER. Performs the recruiter function."
        admin       = person "Platform Administrator"  "Roles SUPER_ADMIN / ADMIN."

        claude   = softwareSystem "Anthropic Claude API" "LLM inference. Default claude-3-5-sonnet-20241022." "External"
        github   = softwareSystem "GitHub REST API"      "Identity, repositories, commits, issues, pull requests." "External"
        gitlab   = softwareSystem "GitLab REST API"      "OAuth token exchange, user, projects." "External"
        google   = softwareSystem "Google Identity"      "ID-token verification for Google sign-in." "External"
        vietqr   = softwareSystem "VietQR API"           "Vietnamese business-registry lookup for org verification." "External"
        mail     = softwareSystem "Email Transport"      "SMTP via MailKit, or SendGrid REST, with failover." "External"

        cverify = softwareSystem "CVerify Platform" "Verifies candidate capability from source-code evidence and matches candidates to hiring requirements." {

            nginx = container "nginx" "TLS termination, routing, rate limiting, SSE/WebSocket pass-through. Host-installed, not containerised." "nginx"

            client = container "Web Client" "Next.js 16 App Router, React 19, standalone output. Zustand state, SignalR + EventSource + fetch-stream consumers." "Next.js / TypeScript"

            api = container "Core API" "Modular monolith. Auth, profiles, source-code analysis orchestration, intelligence, forum, recovery, admin." "ASP.NET Core / C#" {
                authMod        = component "Auth Module"           "JWT-in-cookie auth, OAuth linking, OTP, sessions, organizations, workspaces, invitations."
                profilesMod    = component "Profiles Module"       "CV entities, candidate assessments, evidence, skill trees, ranking."
                sourceCodeMod  = component "SourceCode Module"     "Provider sync, repository analysis job orchestration, AI job artifacts."
                intelligenceMod= component "Intelligence Module"   "Talent discovery, unified matching engine, trust engine, ranking projections."
                forumMod       = component "Forum Module"          "Categories, topics, replies, votes, reactions, moderation queue."
                aiChatMod      = component "AiChat Module"         "Conversation persistence and SSE proxy to the AI service."
                recoveryMod    = component "Recovery Module"       "Password recovery, organization reclaim, Level-2 quorum recovery."
                adminMod       = component "Admin Module"          "User/role/permission administration, audit logs, admin hub."
                sharedSystem   = component "Shared/System"         "Hiring requirements, job vacancies, public jobs, notifications, streaming sessions, capability catalog."

                authz          = component "Authorization"         "PermissionPolicyProvider + PermissionHandler, colon-segment wildcard matching, org-scoped PermissionEvaluator."
                sessionMw      = component "SessionValidationMiddleware" "Runs between authentication and authorization. Validates session_version, org status, session revocation."
                outbox         = component "Transactional Outbox"  "OutboxMessages written in the same transaction as the business change."
                workers        = component "Background Workers"    "15 registered hosted services: email outbox, talent outbox, analysis, assessment, ranking projection, cleanup, Redis notification subscriber."
                hmacClient     = component "HMAC Signing Client"   "HmacSignatureService. Signs every outbound call to the AI service."
                hubs           = component "SignalR Hubs"          "NotificationHub (/hubs/notifications) and AdminHub (/hubs/admin, group 'admins')."
                storageSvc     = component "Storage Service"       "R2StorageService over AWS SDK S3, SigV4, ForcePathStyle. Plus EncryptedFileStorageService."
                emailInfra     = component "Email Infrastructure"  "MailKit / SendGrid senders, Polly retry + circuit breaker, Scriban templates."
                scmClients     = component "Source-Code Clients"   "GitHubSourceCodeClient, GitLabSourceCodeClient behind ISourceCodeClient."
            }

            ai = container "AI Service" "Pipeline execution and LLM orchestration. HMAC-authenticated." "FastAPI / Python 3.11" {
                hmacMw      = component "HMAC Middleware"        "Validates X-Signature, ±300s skew, Redis nonce SETNX replay guard. Fails closed."
                line1       = component "Repository Pipeline (Line 1)" "22 dispatchable tasks. Flat if/elif dispatcher; ordering owned by the caller. State passes via filesystem caches."
                line2       = component "Candidate Pipeline (Line 2)"  "16-task validated DAG. Hybrid LLM + deterministic gates that override model output."
                line3       = component "JD Matching Pipeline (Line 3)" "13 standalone tasks, no shared context."
                line4       = component "Requirement Artifacts Pipeline" "One Claude call fans out to 5 artifacts with Pydantic self-correction retry."
                scoring     = component "Scoring Engine"          "Deterministic 5-dimension model, cohort normalisation, uncertainty band, seniority classification."
                claudeSvc   = component "Claude Service"          "AsyncAnthropic, streaming, prompt caching, retry with backoff, Redis-based cancellation."
                costTracker = component "Token Accounting"        "Per-family pricing, cache-aware token split, process-local cost tracker."
                ghAnalysis  = component "GitHub Analysis"         "Repo classification, identity resolution, code sampling, technology detection, clone detection."
            }

            postgres = container "PostgreSQL" "Relational store. EF Core, snake_case, pgvector image (extension present, no embeddings written)." "PostgreSQL 16" "Database"
            redis    = container "Redis"      "Auth caches, distributed locks, pub/sub channel cverify:notifications, HMAC nonce store." "Redis 7.2" "Database"
            blob     = container "Object Storage" "Avatars, banners, media, evidence documents, pipeline artifacts. MinIO in production." "S3-compatible" "Database"
        }

        # --- People to system
        guest     -> cverify "Browses public content"
        candidate -> cverify "Builds CV, links repositories, triggers assessments, applies to jobs"
        orgMember -> cverify "Defines hiring requirements, publishes vacancies, discovers talent"
        admin     -> cverify "Administers users, roles, recovery claims, moderation"

        # --- Edge and client
        guest     -> nginx  "HTTPS"
        candidate -> nginx  "HTTPS"
        orgMember -> nginx  "HTTPS"
        admin     -> nginx  "HTTPS"
        nginx  -> client "Proxies / and /_next/static (60m cache)" "HTTP"
        nginx  -> api    "Proxies /api, /hubs (WebSocket upgrade), SSE routes (buffering + gzip off)" "HTTP"
        client -> api    "JSON over HTTPS. Cookie auth, CSRF header, single-flight refresh." "axios"
        client -> api    "SignalR WebSocket, 5 EventSource streams, 1 fetch ReadableStream" "WS / SSE"

        # --- API internals
        api -> postgres "EF Core, split queries, slow-query interceptor" "Npgsql"
        api -> redis    "Cache, locks, pub/sub" "StackExchange.Redis"
        api -> blob     "Upload, delete, presigned URLs" "AWS SDK S3"
        api -> ai       "8 POST endpoints, all HMAC-signed. 5-minute HttpClient timeout." "HTTPS"
        api -> mail     "Transactional email drained from the outbox" "SMTP / REST"
        api -> github   "Identity and repository sync" "REST"
        api -> gitlab   "OAuth and project sync" "REST"
        api -> google   "ID-token validation" "REST"
        api -> vietqr   "Business-registry lookup" "REST"

        # --- AI service
        ai -> claude   "Streaming completions, prompt caching, max_tokens 8192" "HTTPS"
        ai -> github   "Repo metadata and git clone --depth 100" "REST / git"
        ai -> redis    "HMAC nonce replay guard, cancellation flags" "redis-py"
        ai -> api      "Fetches pipeline artifacts: GET /api/v1/ai-jobs/{jobId}/artifacts/{key}" "HTTP"

        # --- Component wiring (Core API)
        client -> authMod "Auth, session, organization and workspace endpoints"
        sessionMw -> redis "session_version, org status, session-active lookups"
        authz -> redis "auth:user:{id}:permissions set"
        authMod -> outbox "Enqueues EmailVerification, PasswordReset, WelcomeNotice"
        outbox -> workers "Polled by EmailOutboxBackgroundProcessor and TalentOutboxBackgroundProcessor"
        workers -> emailInfra "Sends drained email envelopes"
        workers -> hmacClient "Queued analysis and assessment work calls the AI service"
        hmacClient -> ai "Signed POST"
        sourceCodeMod -> scmClients "Provider sync"
        profilesMod -> workers "Enqueues candidate assessments"
        sourceCodeMod -> workers "Enqueues repository analyses"
        intelligenceMod -> postgres "Ranking projections rebuilt every 15 minutes"
        workers -> hubs "Notification fan-out via Redis subscriber"
        hubs -> client "Server-to-client push"
        profilesMod -> storageSvc "Evidence and attachment storage"
        adminMod -> authz "admin:* permissions resolved via IAdminAuthorizationService"
        forumMod -> authz "forum:* permissions"

        # --- Component wiring (AI service)
        hmacClient -> hmacMw "X-Client-Id, X-Timestamp, X-Nonce, X-Signature"
        hmacMw -> line1 "Authenticated task execution"
        hmacMw -> line2 "Authenticated candidate assessment"
        hmacMw -> line3 "Authenticated JD matching"
        hmacMw -> line4 "Authenticated requirement artifact generation"
        line1 -> ghAnalysis "Clone, classify, sample"
        line1 -> claudeSvc "LLM tasks"
        line2 -> claudeSvc "14 of 16 tasks call the LLM"
        line2 -> scoring "Deterministic gates override LLM output"
        line3 -> claudeSvc "LLM matching tasks"
        line4 -> claudeSvc "Single call, validated fan-out"
        claudeSvc -> claude "Streaming"
        claudeSvc -> costTracker "Usage and cost attribution"
        ghAnalysis -> github "Repo metadata, clone"
    }

    views {
        systemContext cverify "SystemContext" {
            include *
            autolayout lr
            description "CVerify in its environment. Actors and external systems verified in source."
        }

        container cverify "Containers" {
            include *
            autolayout lr
            description "Deployed containers plus the host-installed nginx edge."
        }

        component api "CoreApiComponents" {
            include *
            autolayout lr
            description "Modules of the ASP.NET Core modular monolith, plus cross-cutting infrastructure."
        }

        component ai "AiServiceComponents" {
            include *
            autolayout lr
            description "Pipeline families and shared services inside the FastAPI AI service."
        }

        styles {
            element "Person"   { shape person background #0b3d5c color #ffffff }
            element "Software System" { background #123a2a color #ffffff }
            element "External" { background #6b7280 color #ffffff }
            element "Container" { background #1f6f4a color #ffffff }
            element "Component" { background #2f5d7c color #ffffff }
            element "Database" { shape cylinder background #7a5510 color #ffffff }
        }
    }
}
