# AI Audit Log

## 1. Thông tin chung

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
| Ngày bắt đầu | 2026-05-11T00:00:00.000Z |
| Ngày hoàn thành | 2026-07-19T00:00:00.000Z |

---

## 2. Công cụ AI đã sử dụng

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [x] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác: ....................................

---

## 3. Mục tiêu sử dụng AI

### Mô tả mục tiêu sử dụng AI

```text
Thiết kế, phân tích, sinh code, fix bug
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-23 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Fix hybrid authentication workflow issue where users who already created a password after Google signup were still being redirected into the OTP onboarding flow instead of the password login flow. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Fix the hybrid authentication flow where users who initially signed up using Google OAuth and later completed password onboarding are incorrectly redirected back into the OTP + Create Password flow when clicking “Continue with Email” after logout. The system should detect existing password credentials and route users directly to password authentication instead of re-triggering onboarding.
```

#### 4.2. Kết quả AI gợi ý

```text
AI proposed introducing identity state detection logic before OTP issuance. The solution evolved into a state-driven authentication architecture with:
- Identity state resolver service
- Enum-based EmailAuthState
- Dedicated auth state resolution endpoint
- Inline password login UX
- Provider-aware onboarding flow
- Future-ready provider abstraction
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Identity state resolution logic
- Inline password login state
- Frontend state-driven authentication flow
- Provider-aware routing
- Verification scenario mapping
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Refined auth state naming
- Reduced provider leakage in API contracts
- Improved onboarding vs authentication separation
- Added verification and restricted account states
- Extended flow for scalability and future providers
- Evaluation Checklist
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | refactor(database): migrate backend identity system to UUID v7 with production-safe PostgreSQL reset flow | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/c96c6781188be4ea04d8384154d0878eaa87c65d |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-23 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Design the long-term authentication and trust infrastructure architecture for CVerify, including candidate identity, organization verification, provider linking, and scalable onboarding. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Complete Authentication & Identity Workflow Architecture for CVerify
```

#### 4.2. Kết quả AI gợi ý

```text
AI helped refine the overall architecture into:
- Unified identity model
- Hybrid provider linking
- Verification link vs OTP verification separation
- Organization verification workflows
- Trust-tier architecture
- Workspace-ready organization structure
- Future authentication scalability model
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Unified identity principles
- Hybrid provider linking flow
- Verification model separation
- Trust verification levels
- Organization workspace architecture
- Security requirement checklist
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Simplified some auth states
- Refined onboarding UX terminology
- Added scalable provider abstraction
- Improved separation between identity and organization trust verification
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | fix(auth): fix null password hash for existing users and resolve Npgsql type loading error | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/304fbb743a82709f717ff361de59c754fe3663c8 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-23 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Implement a trusted system-level Super Admin authentication flow that bypasses OTP verification and moves admin credentials into environment configuration. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Fix the Super Admin authentication flow so that Super Admin accounts can bypass the email OTP verification step during login and onboarding. This account is a trusted system-level identity and should not follow standard user verification flows. Additionally, move the Super Admin account configuration to environment variables instead of hardcoding it in the codebase.
```

#### 4.2. Kết quả AI gợi ý

```text
- Restricting OTP bypass exclusively to Super Admin
- Moving Super Admin configuration into environment variables
- Integrating trusted identity logic into the existing authentication architecture
- Maintaining security boundaries for standard users
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Trusted system account logic
- Environment-based admin configuration
- Scoped OTP bypass strategy
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Clarified that bypass applies only to system-level identities
- Ensured compatibility with existing provider-linking logic
- Refined wording to avoid weakening overall security assumptions
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
|---|---|---|
| Commit/PR | feat(auth): support privileged super admin env configuration & otp bypass | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/fa09d37d6047f6afb01ccfb527618db5a9fb9be2 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
 
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Authentication Architecture Design |   |   | x |   | Used AI to refine identity workflows, provider linking, onboarding states, and scalable auth architecture. |
| Backend Auth Refactor |   |   |   | x | AI generated service-layer restructuring ideas, DTO redesign, and authentication state handling flows. |
| Frontend Auth UX Flow |   |   | x |   | AI helped design progressive onboarding UX and state-driven authentication screens. |
| Security Review & Threat Analysis |   |   | x |   | Used AI to identify account enumeration risks, cache invalidation concerns, and auth boundary issues. |
| Environment Configuration |   |   |   | x | AI generated ideas for moving Super Admin configuration into environment variables securely. |
| System Design Evaluation | x |   |   |   | Final architectural decisions, trade-off evaluations, and simplification choices were reviewed manually. |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI over-engineered the authentication fix with unnecessary Redis caching and distributed state management | Architecture review showed the proposed solution introduced excessive complexity for an MVP bug fix | Simplified the design and separated MVP fixes from long-term architecture ideas |
| 2 | Some auth state naming still leaked credential assumptions indirectly | Manual security review during workflow evaluation | Refined terminology toward more neutral identity state naming |
| 3 | AI occasionally optimized for scalability before validating product scope | Comparison between implementation cost and actual business requirement | Added clearer scope constraints in prompts |
| 4 | Some generated flows duplicated normalization logic across services | Code structure review during backend planning | Planned extraction into shared normalization utilities |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Verified outputs through manual architecture review, workflow simulation, edge-case analysis, frontend/backend consistency checking, and security reasoning. Authentication flows were validated against expected onboarding behavior, provider-linking logic, and long-term scalability requirements. Code-level feasibility was also reviewed manually before implementation decisions.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
Defined the overall identity and trust architecture direction for CVerify, analyzed authentication edge cases, designed hybrid provider-linking workflows, reviewed security trade-offs, simplified over-engineered AI suggestions, and made final architectural decisions regarding onboarding UX, verification models, and scalable authentication structure. AI was used as an architectural assistant rather than a replacement for system design decisions.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Đoàn Thế Lực | DE200523 | Refactor of auth system | Có | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commits/refactor/auth-system/ |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Review code | Không |   |

---

## 9. Reflection cuối bài

### Xem chi tiết tại REFLECTION.md

---

## 10. Cam kết học thuật

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 24/5/2026 |
