# Prompt Log

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
| Ngày cập nhật gần nhất | 2026-05-23 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

---

## 3. Công cụ AI đã sử dụng

- [x] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ ] Công cụ khác: ....................................

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 2026-05-23 | ChatGPT | Fix hybrid authentication workflow issue where users who already created a password after Google signup were still being redirected into the OTP onboarding flow instead of the password login flow. | Fix the hybrid authentication ... | AI proposed introducing identi... | Có |   |
| 2 | 2026-05-23 | ChatGPT | Design the long-term authentication and trust infrastructure architecture for CVerify, including candidate identity, organization verification, provider linking, and scalable onboarding. | Complete Authentication & Iden... | AI helped refine the overall a... | Có |   |
| 3 | 2026-05-23 | ChatGPT | Implement a trusted system-level Super Admin authentication flow that bypasses OTP verification and moves admin credentials into environment configuration. | Fix the Super Admin authentica... | - Restricting OTP bypass exclu... | Có |   |

---

## 5. Prompt chi tiết

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-23 |
| Công cụ AI | ChatGPT |
| Mục đích | Fix hybrid authentication workflow issue where users who already created a password after Google signup were still being redirected into the OTP onboarding flow instead of the password login flow. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Fix the hybrid authentication flow where users who initially signed up using Google OAuth and later completed password onboarding are incorrectly redirected back into the OTP + Create Password flow when clicking “Continue with Email” after logout. The system should detect existing password credentials and route users directly to password authentication instead of re-triggering onboarding.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify uses hybrid authentication with Google OAuth + Email/Password.
- Candidate identity is unified by verified email.
- Google-first users can later add password credentials.
- Existing issue: users with password credentials are still entering OTP onboarding.
- Existing backend flow uses SendOtpAsync.
- Frontend login page currently routes all email flows into OTP onboarding.
```

#### 5.3. Kết quả AI trả về

```text
AI proposed introducing identity state detection logic before OTP issuance. The solution evolved into a state-driven authentication architecture with:
- Identity state resolver service
- Enum-based EmailAuthState
- Dedicated auth state resolution endpoint
- Inline password login UX
- Provider-aware onboarding flow
- Future-ready provider abstraction
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Identity state resolution logic
- Inline password login state
- Frontend state-driven authentication flow
- Provider-aware routing
- Verification scenario mapping
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Refined auth state naming
- Reduced provider leakage in API contracts
- Improved onboarding vs authentication separation
- Added verification and restricted account states
- Extended flow for scalability and future providers
- Evaluation Checklist
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-23 |
| Công cụ AI | ChatGPT |
| Mục đích | Design the long-term authentication and trust infrastructure architecture for CVerify, including candidate identity, organization verification, provider linking, and scalable onboarding. |
| Phần việc liên quan | Design |
| Mức độ sử dụng | Hỏi ý tưởng |

#### 5.1. Prompt nguyên văn

```text
Complete Authentication & Identity Workflow Architecture for CVerify
```

#### 5.2. Bối cảnh khi viết prompt

```text
- CVerify is evolving into an identity + trust infrastructure.
- Candidate accounts support Google OAuth and Email/Password.
- Organizations require verification workflows.
- Future scalability includes SSO, passkeys, MFA, and recruiter workspaces.
- Unified email identity model.
- Trust verification levels for organizations.
```

#### 5.3. Kết quả AI trả về

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

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Unified identity principles
- Hybrid provider linking flow
- Verification model separation
- Trust verification levels
- Organization workspace architecture
- Security requirement checklist
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Simplified some auth states
- Refined onboarding UX terminology
- Added scalable provider abstraction
- Improved separation between identity and organization trust verification
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

### Prompt số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 2026-05-23 |
| Công cụ AI | ChatGPT |
| Mục đích | Implement a trusted system-level Super Admin authentication flow that bypasses OTP verification and moves admin credentials into environment configuration. |
| Phần việc liên quan | Coding |
| Mức độ sử dụng | Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Fix the Super Admin authentication flow so that Super Admin accounts can bypass the email OTP verification step during login and onboarding. This account is a trusted system-level identity and should not follow standard user verification flows. Additionally, move the Super Admin account configuration to environment variables instead of hardcoding it in the codebase.
```

#### 5.2. Bối cảnh khi viết prompt

```text
- Existing authentication system uses OTP verification.
- Super Admin is considered a trusted system identity.
- Current admin account configuration is hardcoded.
- Need separation between system-level accounts and normal users.
```

#### 5.3. Kết quả AI trả về

```text
- Restricting OTP bypass exclusively to Super Admin
- Moving Super Admin configuration into environment variables
- Integrating trusted identity logic into the existing authentication architecture
- Maintaining security boundaries for standard users
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
- Trusted system account logic
- Environment-based admin configuration
- Scoped OTP bypass strategy
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
- Clarified that bypass applies only to system-level identities
- Ensured compatibility with existing provider-linking logic
- Refined wording to avoid weakening overall security assumptions
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| File/Link |   |

#### 5.8. Ghi chú thêm

```text
 
```

---

## 6. Prompt quan trọng nhất

### 6.1. Prompt được chọn

```text
Fix the hybrid authentication flow where users who initially signed up using Google OAuth and later completed password onboarding are incorrectly redirected back into the OTP + Create Password flow when clicking “Continue with Email” after logout. The system should detect existing password credentials and route users directly to password authentication instead of re-triggering onboarding.
```

### 6.2. Vì sao prompt này quan trọng?

```text
This prompt significantly improved the authentication architecture of CVerify by transitioning from a traditional login system into a scalable identity orchestration system. It established the foundation for hybrid provider linking, future SSO integrations, and trust-based onboarding.
```

### 6.3. Kết quả prompt này mang lại

```text
AI proposed introducing identity state detection logic before OTP issuance. The solution evolved into a state-driven authentication architecture with:
- Identity state resolver service
- Enum-based EmailAuthState
- Dedicated auth state resolution endpoint
- Inline password login UX
- Provider-aware onboarding flow
- Future-ready provider abstraction
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
- Identity state resolution logic
- Inline password login state
- Frontend state-driven authentication flow
- Provider-aware routing
- Verification scenario mapping
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
- Refined auth state naming
- Reduced provider leakage in API contracts
- Improved onboarding vs authentication separation
- Added verification and restricted account states
- Extended flow for scalability and future providers
- Evaluation Checklist
```

---

## 7. Prompt chưa hiệu quả

### 7.1. Prompt chưa hiệu quả

```text
Fix the hybrid authentication flow where users who initially signed up using Google OAuth and later completed password onboarding are incorrectly redirected back into the OTP + Create Password flow when clicking “Continue with Email” after logout. The system should detect existing password credentials and route users directly to password authentication instead of re-triggering onboarding.
```

### 7.2. Vì sao prompt này chưa hiệu quả?

```text
The solution increased architectural complexity for a relatively small workflow issue and introduced unnecessary infrastructure dependencies during the MVP phase.
```

### 7.3. Cách cải thiện prompt

```text
 
```

### 7.4. Prompt sau khi cải tiến

```text
Design a scalable but minimal hybrid authentication fix where the system detects whether an email identity already has password credentials before triggering OTP onboarding. The solution should prioritize clean identity flow separation and future extensibility without introducing unnecessary infrastructure complexity.
```

### 7.5. Kết quả sau khi cải tiến prompt

```text
 
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Provide clearer system context, architecture constraints, current workflow behavior, expected UX flow, and scalability goals. Including existing backend/frontend structure, authentication rules, and future expansion plans helped generate more accurate and production-oriented solutions. Clearly defining whether the goal is an MVP fix or long-term architecture redesign also improves response quality significantly.
```

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Detailed prompts with business context, workflow diagrams, edge cases, and architectural intent produce much higher quality results than short feature requests. Separating “problem”, “current behavior”, “expected behavior”, and “constraints” helps AI reason more effectively. I also learned that AI may over-engineer solutions if scalability goals are emphasized too heavily without defining scope boundaries.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
I will define implementation scope more explicitly (MVP fix vs scalable architecture), specify acceptable complexity levels, and separate immediate bug fixes from long-term redesign discussions. I will also provide clearer constraints around performance, infrastructure, and maintainability to avoid unnecessary architectural expansion.
```

---

## 9. Phân loại prompt đã sử dụng

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt Coding | 2 |  |
| Prompt Design | 1 |  |

---

## 10. Checklist chất lượng prompt

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | x | |
| Prompt có đủ bối cảnh | x | |
| Tự kiểm tra và chỉnh sửa | x | |

---

## 11. Cam kết sử dụng prompt minh bạch

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyễn Hoàng Ngọc Ánh | 24/5/2026 |
