# AI Audit Log

## 1. Thông tin chung

| Thông tin             | Nội dung                                                                               |
| --------------------- | -------------------------------------------------------------------------------------- |
| Môn học               | Software Development Project                                                           |
| Mã môn học            | SWP391                                                                                 |
| Lớp                   | SE20A02                                                                                |
| Học kỳ                | SU26                                                                                   |
| Tên bài tập / Project | CVerify - Admin Components System Page                                                 |
| Tên sinh viên / Nhóm  | Nguyễn Hoàng Ngọc Ánh, Đoàn Thế Lực, Trương Văn Hiếu, Nguyễn La Hòa An, Trần Nhất Long |
| MSSV / Danh sách MSSV | DE200147, DE200523, DE190105, DE201043, DE200160                                       |
| Giảng viên hướng dẫn  | QuangLTN3                                                                              |
| Ngày bắt đầu          | 2026-05-29T00:00:00.000Z                                                               |
| Ngày hoàn thành       | 2026-05-29T23:59:59.000Z                                                               |

---

## 2. Công cụ AI đã sử dụng

- [ ] ChatGPT
- [x] Gemini
- [ ] Claude
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
Xây dựng trang tính năng quản trị hệ thống và trực quan hóa kiến trúc frontend mang tên Components System (/admin/components). Tích hợp kiến trúc Workspace Abstraction nâng cao, phân tách danh mục registry, lập hộp cát cô lập lỗi (Preview Sandbox Isolation) khi render preview linh hoạt và hiển thị sơ đồ quan hệ thành phần động qua React Flow (@xyflow/react).
```

## 4. Nhật ký sử dụng AI chi tiết

---

### Lần sử dụng AI số 1

| Nội dung            | Thông tin                                                                                                                                                                                |
| ------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ngày sử dụng        | 2026-05-29                                                                                                                                                                               |
| Công cụ AI          | Gemini, Antigravity                                                                                                                                                                      |
| Mục đích sử dụng    | Thiết kế cấu trúc và triển khai trang quản trị Components System tại /admin/components để trực quan hóa kiến trúc thành phần và kiểm soát chất lượng thiết kế hệ thống (design system). |
| Phần việc liên quan | Frontend / Architecture / Testing / Security                                                                                                                                             |
| Mức độ sử dụng      | Sinh chính nội dung                                                                                                                                                                      |

#### 4.1. Prompt đã sử dụng

```text
# Implement Admin Components System Page

Implement a new protected admin feature page called `Components System` that acts as an internal frontend component intelligence platform and visual component architecture explorer.

The goal is NOT to create a simple component gallery.
This system must visualize frontend component composition, hierarchy, reuse, relationships, scalability, and design-system structure.

---

# Core Feature Overview

Create a new protected route: `/admin/components`
Only authorized users with the correct admin permission can access this page.
Unauthorized users must be blocked using the existing authorization system and route guards.

---

# Access Control
Implement proper permission-based access control.
Requirements:
* Add a dedicated permission for component system access.
* Restrict page visibility in navigation for unauthorized users.
* Prevent direct URL access without permission.
* Integrate with existing auth/role/permission middleware and frontend guards.
* Ensure backend authorization validation exists if component metadata APIs are added.
Example permission naming: `components.system.read` or equivalent naming convention already used in the project.

---

# Navigation Behavior
When entering `/admin/components`:
* Replace the ENTIRE existing admin sidebar content with a dedicated Components System sidebar.
* This page behaves like a separate workspace/application inside admin.
* Preserve existing top-level app layout consistency where appropriate.

Add a prominent: `← Back to Admin` button at the top-left area.
Behavior:
* Return to the previous admin page if navigation history exists.
* Fallback to `/admin/dashboard` if no valid previous route exists.
Use smooth animated transitions between workspaces.

---

# Components System Sidebar
Replace the normal admin sidebar with a specialized component-navigation sidebar.
Sidebar sections:
Components
├── Overview
├── Atoms
├── Molecules
├── Organisms
├── Templates
├── Features
├── Experimental
├── Deprecated
├── Dependency Graph
├── Analytics
└── Settings

Requirements:
* Animated expand/collapse.
* Active item highlighting.
* Responsive behavior.
* Sticky sidebar.
* Search/filter support.
* Dark mode support.
* Minimal modern UI similar to Linear/Vercel/Raycast aesthetics.

---

# Main Page Goal
Visualize frontend architecture and component composition.
The page must display:
* Components hierarchy
* Component relationships
* Component composition
* Component scalability
* Reusability
* Design-system organization
This is effectively an internal frontend operating system.

---

# Component Organization
Use Atomic Design concepts: Atom → Molecule → Organism → Template → Feature
Examples: Input -> InputGroup -> LoginForm -> AuthCard -> AuthModal

---

# Main Layout
The main page should contain:
## 1. Visual Component Grid
Render component cards in responsive row/column layouts grouped by hierarchy level.
Components that are related should visually appear connected and close together.
Example flow: Input → PasswordInput → LoginForm → AuthCard
Requirements:
* Responsive flex/grid layout.
* Smooth animations.
* Relationship grouping.
* Visual hierarchy clarity.

---

# Component Cards
Each component card should contain:
* Component name
* Category
* Status
* Usage count
* Related components
* Small live preview
* Quick actions (Preview, Open, Code, Dependencies)

Card requirements:
* Hover animations
* Expandable details
* Modern clean design
* Lazy rendering for performance

---

# Component Relationship Graph
Implement a visual dependency/composition graph.
Use: React Flow
The graph should visualize:
* parent-child relationships
* component composition
* dependency chains
* reusable shared components
Requirements:
* Zoom/pan
* Expand/collapse
* Interactive nodes
* Highlight related components
* Performance optimized rendering

---

# Component Metadata System
Implement a component metadata structure.
Example:
export const componentMeta = {
  name: "Input",
  category: "atom",
  tags: ["form", "input"],
  related: ["PasswordInput"],
  composedOf: [],
  usedIn: ["LoginForm"],
  status: "stable",
  responsive: true,
  themeable: true,
};

Metadata should support:
* hierarchy classification
* relationships
* status tracking
* analytics
* future AI analysis support

---

# Registry Architecture
Implement an extensible registry system.
Preferred structure: components/registry/
Support:
* manual registry definitions
* future automatic AST scanning support
Architecture must be scalable for future automation.

---

# Live Component Preview
Render actual live previews of components inside cards.
Requirements:
* isolated rendering
* responsive preview modes
* safe fallback rendering
* theme-aware previews
Preview modes: Desktop, Tablet, Mobile
Theme modes: Light, Dark, High Contrast

---

# Search and Filtering
Add advanced filtering support.
Filters: category, tags, status, responsive support, accessibility support, recently updated, reusable only
Search should support: fuzzy search, tag search, component relationship search

---

# Analytics
Display component statistics: usage count, pages using component, most reused components, deprecated components, duplicate detection candidates
Example: Used in 14 forms, 8 pages, 3 modals

---

# Future-Proof Architecture
Structure implementation for future additions:
* AI-generated component insights
* duplicate detection
* AST auto-analysis
* auto-generated previews
* auto props documentation
* component health scoring
* drag-and-drop component composition sandbox
Do NOT tightly couple the system to static data.

---

# Technical Stack
Frontend: React, TypeScript, Tailwind, Framer Motion, React Flow, Zustand, Existing UI system/shadcn architecture

---

# Performance Requirements
Optimize for large component libraries.
Requirements: lazy loading, virtualization where appropriate, memoization, graph optimization, route-level code splitting
Avoid expensive full-page rerenders.

---

# UX Requirements
The page should feel like: Vercel, Linear, Raycast, modern internal developer tooling
Focus on: clean spacing, smooth motion, developer productivity, architecture visibility, scalability
```

And follow-up prompt:
```text
The implementation plan is already very strong overall, but there are several architectural improvements that should be incorporated now to avoid scalability and maintainability issues later as the Components System grows into a true frontend intelligence platform rather than just a visual component explorer.

1. Replace Route-Based Sidebar Swapping with Workspace Architecture (WorkspaceProvider, WorkspaceSidebar)
2. Normalize Component Graph Data Early (ComponentNode, ComponentEdge, types)
3. Add Registry Segmentation & Lazy Loading (registry/atoms, molecules, organisms)
4. Add Preview Sandbox Isolation (PreviewErrorBoundary, Mock Contexts)
5. Add Virtualization & Rendering Optimization Strategy (React.memo, IntersectionObserver)
6. Extend Metadata Model (maturity, owner, maintainers, dependencyRisk, reuseScore)
7. Add Keyboard-First Developer UX (CMD/CTRL + K search, arrow navigation)
8. Clarify Architectural Goal (architecture visibility, design-system governance rather than basic visual sandbox)
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất giải pháp toàn diện bao gồm:
1. Thiết lập Workspace Context (`WorkspaceProvider`) và bộ phân giải Sidebar động nhằm tách biệt hoàn toàn logic định tuyến.
2. Xây dựng mô hình dữ liệu đồ thị chuẩn hóa (Normalized Graph Model) với định nghĩa rõ ràng về đỉnh (ComponentNode) và cạnh (ComponentEdge).
3. Chia nhỏ Registry thành các tệp tin chuyên biệt theo phân loại Atomic Design để hỗ trợ dynamic/lazy loading.
4. Lập hộp cát an toàn (`PreviewSandbox`) bọc trong `PreviewErrorBoundary` cùng các Context ảo (router, i18next) để cô lập lỗi khi render trực tiếp.
5. Sử dụng React Flow (@xyflow/react) để dựng sơ đồ liên kết cây thành phần trực quan từ trái qua phải (Atoms -> Molecules -> Organisms).
6. Tích hợp thanh tìm kiếm lệnh nhanh Spotlight (CMD+K / Ctrl+K) và hỗ trợ điều hướng danh sách bằng bàn phím.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
- Boilerplate và khung gầm chính của WorkspaceProvider, useWorkspace, và useComponentSystemStore.
- Cách thiết lập sơ đồ cây node/edge và toạ độ hiển thị (X, Y) tương ứng trên khung canvas React Flow.
- Boilerplate của PreviewSandbox hỗ trợ chọn cấu hình Theme (Light, Dark, Contrast) và Device (Desktop, Tablet, Mobile).
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
- Sửa lỗi biên dịch TypeScript nghiêm trọng trong class component PreviewErrorBoundary: Lỗi truy cập biến "this.children" không tồn tại trên React 19/TypeScript. Sinh viên đã sửa đổi thành "this.props.children" đúng chuẩn.
- Khắc phục lỗi thiếu import icon "Settings" của Lucide trong components-system-view.tsx gây crash quá trình build môi trường production của Next.js.
- Bảo mật thông tin: Thiết lập đoạn mã Git tự động ghi đè remote URL trở lại dạng HTTPS an toàn ngay sau khi push commit, ngăn chặn việc rò rỉ Personal Access Token (PAT) trong tệp tin plain-text local .git/config.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nhãn | Nội dung |
| --------------- | ---- | -------- |
| Commit/PR       | feat(admin): implement Components System visual architecture explorer | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/3cab46522cbbd42171c6670a48b9f71c4c379a52 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
Kiến trúc Workspace Abstraction và hộp cát cô lập lỗi (Preview Sandbox) là hai cột mốc kỹ thuật quan trọng giúp trang Components System bền vững, không ảnh hưởng đến hiệu năng hay độ tin cậy của toàn bộ nền tảng quản trị CVerify. Việc chuẩn hóa mô hình đồ thị node/edge ngay từ đầu giúp việc tích hợp vẽ sơ đồ cây phân rã liên kết bằng React Flow trở nên mạch lạc và dễ dàng mở rộng cho AST scanning trong tương lai.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

| Hạng mục                    | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú                                                            |
| --------------------------- | :-----------: | :----------: | :-------------: | :-----------: | ------------------------------------------------------------------ |
| Phân tích yêu cầu           |               |              |        x        |               | Phân tích các mối quan hệ đồ thị và các nguy cơ crash runtime      |
| Viết user story/use case    |       x       |              |                 |               |                                                                    |
| Thiết kế database           |       x       |              |                 |               | Sử dụng dynamic registry, không đổi db schema vật lý               |
| Thiết kế kiến trúc hệ thống |               |              |                 |       x       | Thiết kế mô hình Workspace Abstraction và Sandbox Isolation        |
| Thiết kế giao diện          |               |              |        x        |               | Thiết kế Linear/Raycast dark theme và side panel inspector         |
| Code frontend               |               |              |                 |       x       | Code cấu trúc view, stores, providers và dependency graph          |
| Code backend                |               |      x       |                 |               | Chỉ bổ sung quyền components:system:read vào permissions-registry  |
| Debug lỗi                   |               |              |        x        |               | Khắc phục lỗi build TypeScript (this.props.children & Lucide icon) |
| Viết test case              |       x       |              |                 |               |                                                                    |
| Kiểm thử sản phẩm           |               |      x       |                 |               | Chạy build và chạy thủ công xác thực luồng bảo mật                 |
| Tối ưu code                 |               |              |        x        |               | Tích hợp IntersectionObserver và React.memo để hoãn render preview |
| Viết báo cáo                |       x       |              |                 |               |                                                                    |
| Làm slide thuyết trình      |       x       |              |                 |               |                                                                    |

---

## 6. Các lỗi hoặc hạn chế từ AI

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
| --: | ----------------- | -------------- | ------------------- |
|   1 | AI viết sai cú pháp truy cập thuộc tính trong React Class Component ở PreviewErrorBoundary: `return this.children;`. | Quá trình chạy build `npm run build` ném ra lỗi biên dịch: `Property 'children' does not exist on type 'PreviewErrorBoundary'.` | Sửa lại thành `return this.props.children;` để tương thích hoàn toàn với React 19 và TypeScript. |
|   2 | Sử dụng icon `Settings` trong mã JSX nhưng lại bỏ quên không khai báo import từ thư viện `lucide-react`. | Next.js production build compiler báo lỗi không tìm thấy định nghĩa Settings: `Cannot find name 'Settings'.` | Bổ sung `Settings` vào danh sách import Lucide ở đầu tệp `components-system-view.tsx`. |

---

## 7. Kiểm chứng kết quả AI

### Nội dung kiểm chứng

```text
Kiểm chứng kết quả thông qua:
1. Thực hiện lệnh build sản phẩm thực tế `npm run build` trên thư mục client. Kết quả biên dịch hoàn tất thành công, không sinh bất kỳ cảnh báo kiểu (type warning) hay lỗi biên dịch nào.
2. Kiểm thử bảo mật (Permission-Based Access Control): Sử dụng tài khoản thông thường (role USER) truy cập trực tiếp URL `/admin/components`, giao diện lập tức chặn và hiển thị màn hình cảnh báo "Access Revoked" đẹp mắt kèm nút quay lại. Đăng nhập bằng tài khoản ADMIN sở hữu quyền "components:system:read" thì truy cập bình thường.
3. Kiểm thử hộp cát lỗi (Sandbox Isolation): Thử nghiệm chạy render mô phỏng, kéo thả và tương tác với các component có trạng thái (OtpInput, Button, Card). Các component hoạt động trơn tru, thay đổi theme/device mượt mà và không gây ra hiện tượng giật lag nhờ có IntersectionObserver trì hoãn render.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

```text
- Thiết kế hệ thống Workspace Abstraction tổng quát để sẵn sàng nhân rộng cho các workspace quản trị khác.
- Trực tiếp debug các lỗi type check của AI liên quan đến React 19 class components và imports thiếu của Lucide icons.
- Bảo mật thông tin bằng cách cấu hình lại git remote để gỡ bỏ Personal Access Token khỏi plain-text config ngay sau khi đẩy code.
```

### 8.2. Đối với bài nhóm

| Thành viên            | MSSV     | Nhiệm vụ chính                                                     | Có sử dụng AI không? | Minh chứng đóng góp |
| --------------------- | -------- | ------------------------------------------------------------------ | -------------------- | ------------------- |
| Đoàn Thế Lực          | DE200523 | Thiết kế kiến trúc, cài đặt Registry, Graph, Sandbox và trang view | Có                   | https://github.com/fptu-se-su26/swp391-su26-ai-audit-project-swp391_se20a02_group-05/commit/3cab46522cbbd42171c6670a48b9f71c4c379a52 |
| Nguyễn Hoàng Ngọc Ánh | DE200147 | Kiểm thử truy cập bảo mật và đánh giá hiệu năng render             | Không                |                     |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI giúp phác thảo nhanh chóng hệ thống boilerplate đồ thị React Flow phức tạp cùng toạ độ phân rã Atomic Design, viết nhanh mã code thô cho các danh mục registry giúp tiết kiệm thời gian đáng kể.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Không sử dụng đề xuất của AI về việc truy cập raw children trong Class Component vì nó sai lệch cú pháp trên React 19, gây đổ vỡ biên dịch TypeScript. Nhóm đã tự viết lớp bọc provider để bảo toàn tính đóng gói.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Bằng cách chạy lệnh production build `npm run build` nghiêm ngặt và kiểm thử hộp đen thủ công mọi kịch bản tương tác (fuzzy search, Ctrl+K navigation, theme toggle) trên trình duyệt.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần dựng toạ độ hiển thị sơ đồ liên kết node/edge của các thành phần trong React Flow sẽ mất cực kỳ nhiều thời gian tính toán và căn chỉnh giao diện thủ công nếu không được AI phác thảo nhanh cấu trúc.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Học được cách tổ chức các mô-đun ứng dụng thành các không gian làm việc (Workspaces) chuyên biệt giúp hệ thống lớn luôn giữ được tính cô lập, dễ bảo trì và dễ phân quyền ở mức độ granular.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Việc biên dịch thử nghiệm môi trường production (`npm run build`) là bắt buộc để rà soát các hạt sạn nhỏ của AI. Tuyệt đối không được chủ quan tin tưởng hoàn toàn vào mã nguồn do AI sinh ra mà bỏ qua bước kiểm tra nghiêm ngặt của compiler và linter.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
| ----------------------- | ------------- |
| Nguyễn Hoàng Ngọc Ánh   | 2026-05-29    |
