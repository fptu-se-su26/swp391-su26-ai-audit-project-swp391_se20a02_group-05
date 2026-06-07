---
description: This workflow automates the complete development and AI audit documentation process for the CVerify project. It must only be executed when explicitly requested by the user and must never run automatically.
---

Step 1 — Manual Invocation Validation
Before doing anything:
• Confirm that the workflow was explicitly requested by the user.
• Never trigger this workflow automatically.
• If the workflow was not directly requested, terminate immediately.

---

Step 2 — Validate File Changes
Inspect all current file changes.
Determine whether the modified files belong to the CVerify project.
Eligible changes include:
• Source code
• Configuration files
• Documentation
• Database-related files
• Infrastructure files
• Project assets
If the file changes are unrelated to the CVerify project:
The detected file changes do not belong to the CVerify project.

Workflow terminated.
Stop the workflow immediately.

---

Step 3 — Analyze Changes and Generate Commit Message
Review all:
• Added files
• Modified files
• Deleted files
• Renamed files
Generate a semantic commit message that accurately reflects the implementation.
Use appropriate commit prefixes:
feat:
fix:
refactor:
design:
test:
docs:
chore:
The commit message must describe the actual changes made.

---

Step 4 — Determine Branch Type
Analyze the purpose of the changes.
Select one of the following branch categories:
feature/
bugfix/
hotfix/
design/
refactor/
test/
doc/
Branch type definitions:
• feature → New functionality
• bugfix → Bug resolution
• hotfix → Urgent production fix
• design → UI/UX updates
• refactor → Internal code improvements
• test → Automated testing changes
• doc → Documentation updates

---

Step 5 — Create Working Branch
Always create the branch from:
CVerify-uat
Generate a descriptive branch name using:
<type>/<change-description>
Examples:
feature/github-oauth-linking
bugfix/avatar-upload-failure
refactor/auth-service-cleanup
design/settings-page-redesign
doc/organization-recovery-workflow
Requirements:
• Use kebab-case.
• Branch name must reflect the actual implementation.
• Avoid generic names.

---

Step 6 — Commit and Push Changes
Perform the following actions:

1. Checkout CVerify-uat.
2. Pull the latest changes.
3. Create the new branch.
4. Stage all relevant files.
5. Commit using the generated commit message.
6. Push the branch to the remote repository.

---

Step 7 — Create Source Code Pull Request
Create a Pull Request using:
.github/pull_request_template.md
Populate all sections based on the actual implementation, including:
• Summary
• Purpose
• Scope
• Testing
• Impact
• Risks
• Additional Notes
Do not leave template sections incomplete unless they are genuinely not applicable.

---

Step 8 — Assign Reviewer, Assignee, and Labels
After creating the Pull Request:
Reviewer:
LucFr1746
Assignee:
Current authenticated GitHub user
Apply all relevant labels based on the implementation:
feature
bug
hotfix
design
refactor
documentation
test
backend
frontend
database
security
oauth
settings
audit

---

Step 9 — Store Implementation Context
Record and retain:
• Branch name
• Commit message
• Pull Request title
• Pull Request description
• Modified files
• Business purpose
• Technical purpose
This information will be used later during AI audit documentation generation.

---

Step 10 — Resolve Documentation Identity
Switch to the AI Audit Documentation workflow.
Determine the user’s documentation identity.
Example:
GitHub Username: LucFr1746
Documentation Branch: doc/DoanTheLuc
Documentation Folder: docs/DoanTheLuc
If the mapping is unknown:
Request confirmation from the user.
Required information:
GitHub Username
Documentation Branch
Documentation Folder
Store the confirmed mapping for future executions.

---

Step 11 — Review Existing Audit Documentation
Read and analyze:
docs/AI_AUDIT_LOG.md
docs/CHANGELOG.md
docs/PROMPTS.md
docs/REFLECTION.md
Additionally:
• Inspect previous audit folders belonging to the same user.
• Review formatting conventions.
• Review writing style and structure.
The goal is to maintain consistency across all audit records.

---

Step 12 — Generate Audit Documentation Package
Using the stored implementation context:
Generate:
AI_AUDIT_LOG.md
CHANGELOG.md
PROMPTS.md
REFLECTION.md
Requirements:
• Reflect actual implementation work.
• Reflect actual prompts used.
• Reflect actual decisions made.
• Reflect actual technical outcomes.
• Do not invent missing information.

---

Step 13 — Create Sequential Audit Folder
Navigate to the user’s documentation directory.
Example:
docs/DoanTheLuc/
Find the highest existing audit folder number.
Examples:
#1
#2
#3
#4
Create the next available folder.
Example:
#5
Place the four generated audit files inside that folder.
If uncertain about formatting:
Review previous audit folders before generating content.

---

Step 14 — Commit Audit Documentation
Checkout the user’s documentation branch.
Example:
doc/NguyenHoangNgocAnh
Commit the audit package.
Commit message format:
docs(audit): add audit package #<number>
Push the documentation branch.

---

Step 15 — Create Audit Pull Request
Create a second Pull Request.
Source Branch:
doc/DoanTheLuc
Target Branch:
main
Use:
.github/pull_request_template.md
Populate the template using the generated audit documentation.

---

Step 16 — Assign Audit Review Metadata
Reviewer:
nhnnanh
Assignee:
Current authenticated GitHub user
Labels:
documentation
audit

---

Completion Criteria
The workflow is considered complete only when all of the following are successfully finished:
• Source code branch created.
• Source code committed and pushed.
• Source code Pull Request created.
• Reviewer assigned.
• Labels assigned.
• Implementation context recorded.
• Audit documentation generated.
• Audit folder created.
• Audit files committed and pushed.
• Audit Pull Request created.
• Audit reviewer assigned.
• Audit labels assigned.