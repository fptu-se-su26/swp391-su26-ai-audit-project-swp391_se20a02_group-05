# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Môn học | Software Development Project |
| Mã môn học | SWP391 |
| Lớp | SE20A02 |
| Học kỳ | SU26 |
| Tên bài tập / Project | CVerify - Auth System |
| Tên sinh viên / Nhóm | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05 |
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 |  |  | Not Started |
| Phase 02 |  |  | Not Started |
| Phase 03 |  |  | Not Started |
| Phase 04 |  |  | Not Started |
| Phase 05 |  |  | Not Started |
| Phase 06 | 2026-05-23 ~ 2026-05-23 | Secure Authentication Refactoring & Super Admin Enhancements | Completed |

---

# [Phase 06] 

## Thông tin giai đoạn

- **Thời gian thực hiện:** 2026-05-23 ~ 2026-05-23
- **Mô tả giai đoạn:** Secure Authentication Refactoring & Super Admin Enhancements
- **Trạng thái hiện tại:** Completed

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Dynamic Super Admin Seeding: Swapped the hardcoded seeding credentials in DbInitializer.cs with parameterized @adminEmail and @adminPassword SQL parameters loaded directly from environment variables. | Đoàn Thế Lực |   | Github commit |
| 2 | Super Admin Verification & OTP Bypass: In AuthService.cs, bypassed the UserStatus.EMAIL_VERIFY_PENDING block when a Super Admin attempts login, allowing them to authenticate without forced email activation. Bypassed OTP validation in VerifyOtpAsync for the Super Admin, automatically logging an OTP_BYPASSED audit trace and storing a 10-minute setup token under "setup:token:{normalizedEmail}:{challengeId}". Assigned the privileged SUPER_ADMIN role (instead of default USER) on dynamic account creation, Google SSO sign-ins, and onboarding setup if the email matches the configured Super Admin. | Đoàn Thế Lực |   | Github commit |
| 3 | Identity State Resolution for Admin: Configured IdentityStateResolver.cs so that if the user is a Super Admin, they bypass restrictions (UserStatus.SUSPENDED, UserStatus.BANNED, or deleted states are ignored) and skip the standard REQUIRES_VERIFICATION phase. | Đoàn Thế Lực |   | Github commit |
| 4 | Client-Side Hydration Error Fix: In AuthAvatar (auth-avatar.tsx), removed a nested <Button> inside the HeroUI/NextUI <Dropdown.Trigger>. Nesting buttons violates standard HTML semantics (rendering <button> within a <button>), which caused React hydration errors in Next.js. | Đoàn Thế Lực |   | Github commit |
| 5 | Console Noise Reduction during Bootstrap: Caught guest user states gracefully inside use-auth.ts, logging [Auth System] Session bootstrap: No active session (unauthenticated guest). for 401 Unauthorized responses instead of firing a warning track trace. Cleaned up the Axios interceptor response queue error logger to log a quiet statement rather than a full stack trace error when token refresh attempts yield 401 for guests. | Đoàn Thế Lực |   | Github commit |
| 6 | Existing User Password Hash Assignment Fix: Inside CreatePasswordAsync, when the user already existed in the system (e.g. they had an onboarding invite but no set credentials), the transactional execution path previously failed to write the generated password hash back to the user record. Added the user.PasswordHash = passwordHash; assignment statement in the else block of the user status branch. | Đoàn Thế Lực |   | Github commit |
| 7 | Nullable Password Hash Support (Google SSO / passwordless auth): Modified Initialize SQL.sql to make the password_hash column on the users database table nullable (password_hash TEXT), syncing with EF Core models so passwordless accounts (Google OAuth) do not fail integrity checks. | Đoàn Thế Lực |   | Github commit |
| 8 | Npgsql Type Reloading Fix: Added Npgsql.NpgsqlConnection.ClearAllPools() immediately after EF schema generation inside DbInitializer.cs. PostgreSQL type loading throws exceptions (System.NotSupportedException) if custom domain objects (citext, user_status enums) are created while old, cached connections remain in the connection pool. Clearing the pools forces type-mapping synchronization. | Đoàn Thế Lực |   | Github commit |
| 9 | Integration Tests addition: Introduced new integration test cases inside RegistrationFlowTests.cs (e.g. CreatePassword_WhenUserAlreadyExists_ShouldSetPasswordHash and CreatePassword_WithoutFullName_ShouldResolveNameFromEmail) to safeguard registration flows and assert correct hashing/field fallback behavior. | Đoàn Thế Lực |   | Github commit |

## AI có hỗ trợ không?

- [ ] Có
- [x] Không

## Minh chứng liên quan

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | fix(auth): fix null password hash for existing users and resolve Npgsql type loading error | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/304fbb743a82709f717ff361de59c754fe3663c8 |
| Commit/PR | feat(auth): support privileged super admin env configuration & otp bypass | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/fa09d37d6047f6afb01ccfb527618db5a9fb9be2 |

## Ghi chú

```text
 
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

```text
- Dynamic Identity State Resolver: A central engine (IdentityStateResolver.cs) that resolves user emails into distinct authentication flows (REQUIRES_ONBOARDING, REQUIRES_AUTHENTICATION, REQUIRES_VERIFICATION, ACCOUNT_RESTRICTED) before checking credentials.

- Challenge-Based Email OTP Verification: Safe verification flow using HMAC-SHA256 hashed OTP codes, dynamic resend cooldown timers, verification attempts counter, and expiry windows.

- Multi-Tenant Workspace & Company Onboarding:
  + Company metadata registration with strict tax-code verification (exact 10 digits required) and email ownership verification links.
  + Tenant workspace creation using a safe organization slug/username ([a-z0-9_]{3,30}) and mapping of "Owner" role memberships.
  + Isolated workspace entry points allowing business owners to login to specific workspaces.

- Flexible OAuth / Google SSO Support: Adjusted database schemas to support a dedicated auth_providers map and made the password_hash column on the users table nullable to allow passwordless identity providers.

- Seeded Role-Based Access Control (RBAC): Dynamic permission seeding from a permissions-registry.json registry file mapping custom roles (such as SUPER_ADMIN and USER) directly on user creation.

- Background Database-Driven Email Outbox: Transactional outbox pattern backed by a background processor (EmailOutboxBackgroundProcessor.cs) to ensure email notifications (OTP, onboarding, resets, security alerts) are saved and sent reliably.

- Active Session Management & Selective Revocation: API endpoints and hooks to query currently active sessions (with basic device, IP, and timestamp tracking) and selectively revoke refresh tokens (SessionInfo, RevokeSessionAsync).

- Zod-Backed Validation Schemas: Rich schema validation rules on the frontend covering login, registration, workspace settings, and password criteria matching.

- Double-Language Localization: Full translation maps in English (en/common.json) and Vietnamese (vi/vi/common.json) translating onboarding status messages, loaders, and layout components.

- Super Admin Privilege Configuration & Bypasses: Support for initializing the system administrator dynamically via environment configuration (SUPER_ADMIN_EMAIL and SUPER_ADMIN_PASSWORD), bypassing status restrictions and OTP validations during local testing.

```

---

## 4.2. Các chức năng chưa hoàn thành

```text
- Client-Side OAuth Handshakes: While database schemas and provider mappings are established to support Google SSO, the client-side Google sign-in buttons on the frontend remain scaffolding elements and lack direct client SDK integration (e.g. Google Identity Client / MSAL).

- Simplified Device Telemetry: Active session device detection relies on basic string-matching patterns (Windows Desktop vs Mobile Client) based on the request's user-agent header inside AuthService.cs rather than a standard parsed structure.

- Distributed Scheduler Lock: Background jobs like the outbox processor and token cleanup run on generic system timer threads without distributed locking (such as PostgreSQL Advisory Locks or Redis Redlock), which could lead to redundant processing in multi-instance horizontal scaling environments.

- Global API Rate Limiting: Security limiters exist on individual methods (such as OTP cooldowns), but global request rate-limiting (e.g. sliding window controls on critical authentication endpoints) is not configured at the middleware level.
```

---

## 4.3. Cải thiện chính

```text
- Transactional Consistency: Wrapped user, organization, member mapping, and link-consumption routines in tight EF Core transactions to prevent orphan database entries during setup errors.

- Semantic HTML Hydration: Eliminated Next.js/React hydration mismatches in the shell dashboard navigation by styling Dropdown.Trigger directly and removing nested interactive <Button> structures.

- HTML Email Verification: Added snapshot assertion tests (VerifySnapshotTests.cs) for generated HTML email template files, ensuring consistent rendering across localized variants.

- Npgsql Connection Pool Flushing: Resolved custom PostgreSQL type reload exceptions (System.NotSupportedException) on schema initialization by executing Npgsql connection pool resets dynamically.
```

---

## 4.4. Tổng kết project

```text
The commits represent a transition from a basic email-password sign-in implementation to an enterprise-grade, localized, multi-tenant authentication engine. They isolate identity states from active sessions, guarantee consistent background notifications via outboxes, support flexible social identity providers, secure database configurations with dynamic superuser properties, and protect client layouts using strict route guards and validation schemas.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
1. Distributed Locks: Integrate distributed locking (e.g. Postgres Advisory Locks) inside EmailOutboxBackgroundProcessor to prevent race conditions when running the web API inside multi-node Docker or Kubernetes containers.

2. Robust User-Agent Parsing: Incorporate a parsed telemetry package (like UAParser.cs) to report exact client specifications (e.g., "Safari on macOS Sequoia" or "Chrome on Android") inside the active sessions dashboard.

3. Multi-Factor Authentication (MFA): Support Time-Based One-Time Passwords (TOTP via Google Authenticator) or FIDO2/Passkey authentication as optional validation layers.

4. Custom Subdomains & White-Labeling: Enable dynamic DNS resolution allowing tenant organizations to bind custom domains and custom CSS themes to their workspace login portals.

5. Audit Logs Explorer: Implement an administrative UI to filter and query the security events tracked under LogAuditEventAsync.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 24/5/2026 |
