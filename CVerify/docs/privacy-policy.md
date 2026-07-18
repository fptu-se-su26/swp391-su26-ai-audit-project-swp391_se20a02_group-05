# CVerify Privacy Policy

**Effective Date:** July 18, 2026  
**Version:** 1.0.0-GA  
**Last Updated:** July 18, 2026  

---

## Document Control & Version History

| Version | Date | Authors | Status | Description of Revisions |
| :--- | :--- | :--- | :--- | :--- |
| `1.0.0-GA` | July 18, 2026 | CVerify Compliance Team | Active | Initial publication for enterprise operations. |

---

## 1. Introduction & Corporate Identity

### Purpose
To identify the legal entities operating the CVerify platform and declare our commitment to candidate and corporate data privacy.

### Description
This Privacy Policy governs the processing of personal data by **CVerify Joint Stock Company** ("CVerify", "we", "us", or "our"), a corporate entity registered under the laws of the Socialist Republic of Vietnam. CVerify acts as a Data Controller for candidate profile information created directly on our platform and as a Data Processor on behalf of employer organizations using CVerify workspaces.

### Examples
- *Example 1:* When a candidate registers at `cverify.com`, CVerify acts as the Data Controller.
- *Example 2:* When a candidate submits an application to a specific job vacancy posted by an employer under a workspace, the employer acts as the Data Controller, and CVerify acts as the Data Processor.

### Exceptions
This policy does not apply to third-party websites, integrations, or services linked on CVerify that are not owned or controlled by CVerify.

### User Rights
Users have the right to know the identity and contact details of the legal entity processing their personal data.

### Company Responsibility
CVerify is responsible for maintaining transparent legal structures and providing clear, accessible information regarding our identity and processing activities.

### Security Note
Corporate identity records and legal documentation are kept on secure internal systems, isolated from applicant-facing databases.

---

## 2. Scope & Applicability

### Purpose
To define the categories of individuals whose personal data is processed by CVerify and establish the boundaries of this policy.

### Description
This policy applies to all visitors, registered candidates, organization owners, administrators, recruiters, interviewers, and partners interacting with the CVerify platform, websites, and applications.

### Examples
- *Example 1:* A developer visiting our homepage to read our blog.
- *Example 2:* A recruiter conducting an interview evaluation using our Workspace portal.

### Exceptions
This policy does not cover employees of CVerify J.S.C., whose data is governed by internal HR privacy frameworks.

### User Rights
Users have the right to receive confirmation on whether their data falls within the scope of CVerify's processing activities.

### Company Responsibility
CVerify ensures that all digital interfaces (portals, APIs, widgets) clearly display links to this policy.

### Security Note
Identity scopes are parsed and enforced at the Next.js Edge Middleware layer to prevent lateral access across roles.

---

## 3. Key Legal Definitions

### Purpose
To establish a clear glossary of legal and technical terms used throughout this document.

### Description
Unified definitions ensure that users, developers, and auditors share a common understanding of the system's operational elements.

### Examples
- **Candidate:** A developer user registering to build a profile or verify source code.
- **Trust Score:** An automated capability indicator calculated by CVerify's background ranking engine.
- **Lizard AST:** The static code complexity analyzer utilized to evaluate source files.
- **MinHash LSH:** The clone-detection algorithm used to identify code duplicates.

### Exceptions
Where localized regulations (e.g., CCPA/GDPR) define terms differently, those legal definitions override internal definitions for users in those jurisdictions.

### User Rights
Users have the right to request clarification on any technical or legal terms used in this policy.

### Company Responsibility
CVerify will maintain an accurate, up-to-date glossary of definitions aligned with international standards.

### Security Note
Security classifications and role mappings inside our database schemas align directly with these defined terms.

---

## 4. Core Privacy Principles

### Purpose
To align CVerify's development and business operations with global data protection principles.

### Description
CVerify adheres to the principles of lawfulness, fairness, transparency, purpose limitation, data minimization, accuracy, storage limitation, integrity, and confidentiality.

### Examples
- *Example 1:* We do not collect candidate location data if the profile builder does not require it.
- *Example 2:* Cloned source files are deleted immediately after static code evaluation.

### Exceptions
Emergency system maintenance or forensics investigations may temporarily defer standard dpo schedules to protect system integrity.

### User Rights
Users can object to any processing activities that they believe violate these core principles.

### Company Responsibility
CVerify must design features with "Privacy by Design and by Default" (PbD) configurations.

### Security Note
Internal code review controls check that new database migrations do not violate data minimization rules.

---

## 5. Data Protection Officer (DPO) & Governance

### Purpose
To designate individuals responsible for data privacy compliance and provide a point of contact for users.

### Description
CVerify has appointed a Data Protection Officer (DPO) to oversee compliance with GDPR, CCPA, and PDPA. Users can contact the DPO directly regarding privacy matters.

### Examples
- *Example 1:* Submitting an email to `dpo@cverify.com` to request a data portability export.
- *Example 2:* Filing a request for a manual review of an automated evaluation.

### Exceptions
General support tickets not related to personal data are handled by the customer support team, not the DPO.

### User Rights
Users have the right to communicate directly with the DPO regarding any personal data concerns.

### Company Responsibility
CVerify must grant the DPO independent authority to investigate internal compliance anomalies.

### Security Note
Communication with the DPO is encrypted in transit and stored in a restricted legal workspace.

---

## 6. Categories of Personal Data Processed

### Purpose
To specify the types of personal data we collect during your use of CVerify.

### Description
CVerify processes general account credentials, portfolio profiles, developer metrics, and technical system telemetry.

| Category | Elements |
| :--- | :--- |
| **Identity Data** | Name, email address, password hashes, Google SSO Subject ID. |
| **Portfolio Data** | Work experience logs, education history, skills, project titles, repository URLs. |
| **Technical Telemetry** | IP addresses, browser User-Agent strings, access logs, correlation IDs. |

### Examples
- *Example 1:* Storing `john.doe@gmail.com` as the account email.
- *Example 2:* Keeping a copy of the developer's bio: "Full Stack Engineer specialized in Go."

### Exceptions
We do not collect government identification numbers unless required for company corporate verification.

### User Rights
Users can request a complete list of personal data categories held by CVerify at any time.

### Company Responsibility
CVerify must maintain an accurate database schema record indicating where each category of personal data is stored.

### Security Note
Database tables containing candidate profile details are isolated using logical relational foreign key constraints.

---

## 7. Sensitive Personal Data Disclosures

### Purpose
To outline CVerify's strict limitations regarding the processing of special categories of personal data.

### Description
CVerify does not collect sensitive personal data such as political opinions, religious beliefs, union memberships, health details, genetic profiles, or sexual orientation.

### Examples
- *Example 1:* If a candidate uploads a resume containing their religious affiliation, the system CV parser is configured to ignore these details.
- *Example 2:* Government registration IDs submitted during company validation are strictly restricted.

### Exceptions
Government registration proofs or corporate tax identification numbers are collected strictly from business representatives during company verification processes.

### User Rights
Users have the right to demand the erasure of any sensitive data accidentally uploaded to the platform.

### Company Responsibility
CVerify must configure its CV parser algorithms to omit and purge special categories of data.

### Security Note
Uploaded corporate licensing documents are stored in Cloudflare R2 and access-restricted to audit logs.

---

## 8. Methods of Data Collection

### Purpose
To explain how data enters the CVerify environment.

### Description
CVerify collects personal data through direct input forms, automatic logging systems, and external OAuth Git platform integrations.

### Examples
- *Direct:* Filling out the "Add Work Experience" modal.
- *Automatic:* Logging your IP address when you login to protect against brute-force attacks.
- *Integrations:* Syncing commit history from GitHub.

### Exceptions
Manually entered projects that are not linked to Git do not trigger automatic synchronization.

### User Rights
Users can choose to deny permissions for external integrations (e.g. by not linking GitHub).

### Company Responsibility
CVerify must present clear consent overlays prior to executing OAuth redirect actions.

### Security Note
OAuth exchange routes target verified domains (e.g. `api.github.com`) using secure HTTPS handshakes.

---

## 9. Legal Bases for Processing (GDPR Mapping)

### Purpose
To establish the legal justifications for processing personal data under GDPR Article 6.

### Description
We only process personal data when we have a valid legal basis, including contract performance, consent, legal obligation, and legitimate interests.

### Examples
- **Contract Performance:** Processing credentials to let you login and access CVerify.
- **Consent:** Storing your search preferences on the dashboard.
- **Legitimate Interest:** Analyzing system IP logs to block DDoS attacks.

### Exceptions
Processing is restricted if you withdraw your consent for specific optional features (e.g. marketing newsletters).

### User Rights
Users can request the specific legal basis assigned to any processing operation.

### Company Responsibility
CVerify must log and record the legal basis mappings for all software modules.

### Security Note
Legal bases are recorded within our ROPA (Record of Processing Activities) database.

---

## 10. Git Repository Analysis & Ephemeral Clones

### Purpose
To outline how CVerify analyzes codebases to evaluate technical capabilities.

### Description
CVerify analyzes connected repositories to verify developer contributions. The FastAPI AI microservice ephemerally clones selected repositories to a transient disk volume. Once the static analysis runs (calculating AST metrics via Lizard and plagiarism vectors), the code folder is permanently deleted.

```
[Candidate Git Repo] ---> [Secure Clone (tmpfs)] ---> [Lizard AST & MinHash Analysis] ---> [Delete Clone]
                                                                                               │
                                                                                               ▼
                                                                                     [Save Report Metadata]
```

### Examples
- *Example 1:* An applicant links `github.com/user/project-alpha`. CVerify clones it, extracts files, and measures function lengths.
- *Example 2:* The system removes the cloned codebase directory instantly after saving the AST report.

### Exceptions
If an analysis job encounters a critical timeout, a fallback handler forces the deletion of all cloned files.

### User Rights
Candidates have the right to select which repositories are analyzed and disconnect them at any time.

### Company Responsibility
CVerify guarantees that it will never persist or store candidate raw source code on long-term databases.

### Security Note
Ephemeral clones are conducted in isolated worker directories using transient storage structures (such as tmpfs).

---

## 11. Code Provenance & Plagiarism Analysis (MinHash LSH)

### Purpose
To identify code duplicates, plagiarism, and AI-generated source segments.

### Description
CVerify calculates code signatures using the DataSketch MinHash LSH engine. This generates digital hashes of code fragments to identify files copied from public repositories or AI generation pools.

### Examples
- *Example 1:* A developer copies a repository from a public online course. The MinHash index flags the file as 95% identical to a public signature.
- *Example 2:* Matching hashes are saved in the DB, and the candidate's trust score decreases.

### Exceptions
Standard library boilerplate files (e.g. template initializers) are skipped by the plagiarism parser.

### User Rights
Candidates can view the specific files that triggered plagiarism flags in their trust dashboard.

### Company Responsibility
CVerify must ensure that clone-detection algorithms minimize false positives by filtering standard libraries.

### Security Note
Only numerical MinHash signatures are stored in the PostgreSQL database; raw file contents are discarded.

---

## 12. Interactive AI Career Counselor Chat

### Purpose
To provide real-time career advice and CV optimizations.

### Description
CVerify offers a career counselor chat. The chat interface sends prompts and context window details (limited to the last 10 messages) to an external FastAPI parser connecting to Anthropic Claude. Conversations are persisted in PostgreSQL.

### Examples
- *Example 1:* Candidate asks: "How can I improve my Rust rating?" The system fetches the last 10 chat messages to maintain the context.
- *Example 2:* The prompt is sent to FastAPI with secure HMAC signatures.

### Exceptions
Deleted conversations are marked as soft-deleted in the database and hidden from the UI.

### User Rights
Candidates have the right to clear their chat history, which deletes conversation logs.

### Company Responsibility
CVerify must enforce a 10-message context window limit to optimize data minimization.

### Security Note
Communication between core servers and the FastAPI AI engine is signed using cryptographic HMAC headers.

---

## 13. Automated Decision-Making & Trust Score Transparency

### Purpose
To explain how Trust Scores are computed and declare compliance with GDPR Article 22.

### Description
CVerify calculates a Composite Trust Score based on:
`CompositeScore = (AiScore * 0.35) + (TrustScore * 0.35) + (Completeness * 0.15) + (OssImpactScore * 0.15)`
These scores represent automated evaluations. However, CVerify does not make hiring decisions; recruiters must review candidates manually.

### Examples
- *Example 1:* A candidate's score drops due to unverified certificates.
- *Example 2:* A recruiter views the candidate's rating profile and makes a hiring choice.

### Exceptions
Automated checks do not apply to general guest profiles.

### User Rights
Candidates have the right to request a manual review of their Trust Score calculations.

### Company Responsibility
CVerify must design dashboard interfaces to make the scoring calculations transparent to candidates.

### Security Note
All computations are executed backend-side. Results are protected against modification using DB constraint checks.

---

## 14. Profile Visibility & Leaderboard Controls

### Purpose
To give candidates control over how their data is displayed to recruiters.

### Description
Candidates can configure their profile visibility settings:
- **Public:** Profile and Trust Score are viewable on public leaderboards.
- **Private:** Profile is only viewable to the candidate.
- **Restricted:** Profile is only viewable to recruiters in selected workspaces.

### Examples
- *Example 1:* Candidate toggles profile visibility to "Private," removing their name from leaderboards.
- *Example 2:* Recruiters search for candidates with scores above 80% on the talent pool list.

### Exceptions
Aggregated platform metrics do not contain identifiable candidate PII.

### User Rights
Candidates can change their profile visibility at any time via settings.

### Company Responsibility
CVerify must default new candidate profiles to "Private" (Privacy by Default).

### Security Note
Profile visibility states are enforced at the database query level to prevent access leaks.

---

## 15. Developer Forum & Community Posts

### Purpose
To manage public contributions on community boards.

### Description
CVerify hosts a developer discussion forum. Forum topics and replies are public. If you delete your account, your contributions are anonymized rather than deleted to keep discussions readable.

### Examples
- *Example 1:* Candidate posts a thread about ASP.NET Core v10.
- *Example 2:* Candidate deletes their account. The post author changes to "Anonymous User".

### Exceptions
Posts containing PII or violating community standards are moderated and removed.

### User Rights
Users can edit or delete their forum posts at any time.

### Company Responsibility
CVerify must provide moderation controls to report and remove inappropriate content.

### Security Note
Forum database structures use cascade delete options for author profiles, renaming deleted IDs to null values.

---

## 16. Company Verification & Organization Reclaims

### Purpose
To verify the legitimacy of hiring organizations.

### Description
Companies must submit registration files or domain proofs to verify their business profile. If a domain ownership dispute occurs, CVerify conducts audit checks.

### Examples
- *Example 1:* An owner uploads a business license PDF to verify their company.
- *Example 2:* CVerify verifies the uploaded proof and marks the company profile as "Verified".

### Exceptions
Personal developer profiles do not require company verification.

### User Rights
Company owners can request access to files submitted for verification.

### Company Responsibility
CVerify must review submitted corporate licensing documentation within a reasonable timeframe.

### Security Note
Verification uploads are saved in isolated R2 directories and deleted after validation.

---

## 17. Billing & Payment Gateway Processing (Stripe)

### Purpose
To handle subscription invoices and payments.

### Description
CVerify integrates with Stripe. We do not store credit card numbers on our servers. Stripe processes payments, and we receive transaction tokens, invoice details, and billing histories.

### Examples
- *Example 1:* Finance Manager upgrades the workspace to the Enterprise tier.
- *Example 2:* Stripe sends a secure webhook callback, and CVerify updates the subscription status.

### Exceptions
Manual invoicing is available for enterprise clients.

### User Rights
Users can access, download, and request correction of billing records.

### Company Responsibility
CVerify must ensure all billing APIs comply with PCI-DSS standards.

### Security Note
Payment routes are secured using SSL/TLS, and webhooks require Stripe signature checks.

---

## 18. Web CDN, Firewall Proxy, and Traffic logs

### Purpose
To protect CVerify servers and monitor network security.

### Description
CVerify proxies domain traffic through Cloudflare CDN. Cloudflare monitors traffic to prevent DDoS attacks, SQL injections, and malicious requests. Cloudflare logs visitor IP addresses, browser agents, and request histories.

### Examples
- *Example 1:* Cloudflare blocks a malicious script attempting to access `/api/auth/register`.
- *Example 2:* System logs record IP addresses to protect against brute-force login attempts.

### Exceptions
Traffic logs are not linked to candidate profiles unless security incident investigations occur.

### User Rights
Users can review the information collected by Cloudflare in accordance with Cloudflare's privacy policy.

### Company Responsibility
CVerify must configure log retention policies to delete technical logs when they are no longer needed.

### Security Note
System telemetry logs are stored in encrypted directories and rotated automatically.

---

## 19. Cloudflare R2 Secure Document Storage

### Purpose
To secure files uploaded by candidates.

### Description
Candidate uploads (CV PDFs, certificates, screenshot files) are stored in Cloudflare R2 bucket storage. Files are assigned randomized UUID v7 keys. Public access is disabled, and downloads are restricted to 1-hour pre-signed tokens.

### Examples
- *Example 1:* A recruiter clicks a certificate link. The backend generates a pre-signed URL valid for 1 hour.
- *Example 2:* The recruiter attempts to use the link after 2 hours, and the request is denied.

### Exceptions
Publicly visible avatars do not require pre-signed URLs.

### User Rights
Candidates can delete their uploaded files, which triggers a dpo cleanup.

### Company Responsibility
CVerify must configure buckets to restrict public scanning.

### Security Note
Object keys utilize random UUIDs, preventing directory traversal attacks.

---

## 20. Third-Party sub-Processors & Data Transfers

### Purpose
To disclose third-party processors and international data transfers.

### Description
CVerify uses sub-processors (Stripe, Cloudflare, Anthropic, SendGrid) to deliver services. Data may be transferred to and processed in the United States and other regions.

### Examples
- *Example 1:* SendGrid delivers account validation emails.
- *Example 2:* Candidate data is sent to Anthropic API for capability evaluations.

### Exceptions
Data transfers do not occur for users who decline integrations.

### User Rights
Users can request a list of active sub-processors.

### Company Responsibility
CVerify must sign Standard Contractual Clauses (SCCs) to protect international data transfers.

### Security Note
API connections to sub-processors enforce HTTPS TLS 1.3 encryption.

---

## 21. Data Minimization & Retention Schedules

### Purpose
To limit data processing to necessary parameters and establish retention schedules.

### Description
CVerify retains personal data in accordance with our schedules. When data exceeds retention limits, it is deleted or anonymized.

| Data Class | Retention Term |
| :--- | :--- |
| **Active Profiles** | Lifespan of active account. |
| **Transient Code** | Deleted immediately after analysis. |
| **System Security Logs** | 180 days. |
| **Billing Invoices** | 7 years (tax requirement). |

### Examples
- *Example 1:* Cloned source files are deleted immediately after static code evaluation.
- *Example 2:* Log archives are rotated out of storage after 180 days.

### Exceptions
Retention terms are extended if required for legal disputes or regulatory audits.

### User Rights
Users can request the deletion of their personal data in accordance with our retention policy.

### Company Responsibility
CVerify must run background cron jobs to delete expired data records.

### Security Note
Physical deletion commands overwrite storage registers to prevent data recovery.

---

## 22. Database Soft-Deletion & Deactivation Grace Periods

### Purpose
To explain the deletion lifecycle and account deactivation grace periods.

### Description
When you delete your account, it enters a `DELETION_PENDING` state. We retain data for a 10-day grace period to allow self-reactivation. After 10 days, the database record is deleted or anonymized.

### Examples
- *Example 1:* Candidate requests deletion, then changes their mind after 3 days. They login and reactivate their account.
- *Example 2:* The 10-day period expires, and the backend deletes the user record and associated R2 files.

### Exceptions
Billing invoices are retained for 7 years to comply with tax requirements.

### User Rights
Candidates can request immediate hard deletion, which bypasses the 10-day grace period.

### Company Responsibility
CVerify must process deletion requests and remove files from R2 buckets after the grace period.

### Security Note
Reactivation tokens are stored in Redis with a 10-minute expiration limit.

---

## 23. Data Encryption & Cryptographic Hash Matrix

### Purpose
To secure personal data at rest and in transit.

### Description
CVerify encrypts personal data. We hash passwords and tokens using secure algorithms.

| Element | Algorithm |
| :--- | :--- |
| **Passwords** | BCrypt. |
| **Tokens** | SHA-256. |
| **Git Tokens** | AES-256-GCM. |
| **Transit** | TLS 1.3 / HTTPS. |

### Examples
- *Example 1:* Passwords are hashed using BCrypt.
- *Example 2:* GitHub tokens are stored encrypted using AES-256-GCM.

### Exceptions
Public forum posts do not require encryption at rest.

### User Rights
Users have the right to request information about our encryption standards.

### Company Responsibility
CVerify must update cryptographic configurations when vulnerabilities are identified.

### Security Note
Encryption keys are managed securely and rotated periodically.

---

## 24. Dockerized Container Isolation & Infrastructure Hardening

### Purpose
To protect our servers and database environments.

### Description
CVerify isolation structures separate applications into segmented network layers. Next.js edge proxies have no direct access to PostgreSQL databases or Redis caches.

```
[Internet] ---> [Next.js Client net] ---> [ASP.NET Core Core net] ---> [Postgres/Redis net]
                                                   │
                                                   ▼
                                          [FastAPI AI net]
```

### Examples
- *Example 1:* A vulnerability in Next.js does not expose the database.
- *Example 2:* Services run under non-root system users.

### Exceptions
Development environments may use simpler configurations.

### User Rights
Users have the right to a secure, audited hosting environment.

### Company Responsibility
CVerify must run system checks and monitor container configurations.

### Security Note
Docker configurations restrict container ulimits and log sizes (max 10MB).

---

## 25. Session Protection & Concurrency Tokens

### Purpose
To protect user sessions and prevent session hijacking.

### Description
CVerify manages sessions using HttpOnly, Secure, SameSite JWT and refresh token cookies. A 10-second grace window allows token rotation across multiple browser tabs without triggering reuse alerts.

### Examples
- *Example 1:* A candidate opens multiple dashboard tabs. The grace window prevents session expiration.
- *Example 2:* A reused token outside the grace window triggers an alert.

### Exceptions
Manual logout invalidates all session tokens.

### User Rights
Users can revoke active sessions via account settings.

### Company Responsibility
CVerify must configure token cookies to prevent scripting access (XSS).

### Security Note
Refresh tokens are stored hashed in PostgreSQL.

---

## 26. Security Auditing & Threat Logging

### Purpose
To detect and investigate security incidents.

### Description
CVerify records system events to audit logs. Logs capture user IDs, correlation IDs, timestamps, browser signatures, and client IP addresses.

### Examples
- *Example 1:* Logging the event `USER_LOGIN_FAILED_CREDENTIALS` when a login attempt fails.
- *Example 2:* Auditing access to workspace settings.

### Exceptions
Audit logs do not record plain passwords or payment information.

### User Rights
Users can request a copy of logs related to their account.

### Company Responsibility
CVerify must protect audit logs from modification.

### Security Note
Audit logs are stored in read-only tables and rotated regularly.

---

## 27. Data Breach Notification & Response (GDPR Art 33-34)

### Purpose
To establish a protocol for responding to data breaches.

### Description
In the event of a data breach, CVerify will notify supervisory authorities within 72 hours. If a breach is likely to result in a high risk to user rights, we will notify impacted users without undue delay.

### Examples
- *Example 1:* CVerify detects unauthorized access to the database and notifies the regulatory authority within 72 hours.
- *Example 2:* Impacted candidates are notified via email and dashboard alerts.

### Exceptions
Notification is not required if the compromised data was encrypted and remains unreadable.

### User Rights
Users have the right to be informed of data breaches affecting their personal data.

### Company Responsibility
CVerify must maintain an incident response plan and conduct team training.

### Security Note
Incident reports are stored in restricted directories.

---

## 28. Cookies Policy & Browser Storage Matrix

### Purpose
To disclose browser storage usage and cookies.

### Description
CVerify uses cookies and localStorage to manage sessions, CSRF protection, and preferences.

| Name | Type | Category | Purpose | Expiration |
| :--- | :--- | :--- | :--- | :--- |
| `cverify_session` | Cookie | Essential | JWT Session Auth | Session |
| `cverify_refresh_token`| Cookie | Essential | Token Rotation | 7 Days |
| `cverify_csrf` | Cookie | Security | CSRF Protection | Session |
| `cverify_lang` | LocalStorage | Preference| Language (vi/en) | Persistent |

### Examples
- *Example 1:* The session cookie checks that you remain signed in as you navigate pages.
- *Example 2:* Language settings are retrieved from localStorage.

### Exceptions
Optional marketing cookies are only set if the user consents.

### User Rights
Users can block or clear cookies using their browser settings.

### Company Responsibility
CVerify must provide a cookie consent banner for users in jurisdictions that require it.

### Security Note
Session cookies use `HttpOnly`, `Secure`, and `SameSite` flags.

---

## 29. Marketing Communications & Opt-Out Protocols

### Purpose
To manage marketing and promotional communications.

### Description
We only send promotional emails if you opt-in. You can opt-out of marketing communications at any time.

### Examples
- *Example 1:* Candidate checks the box to receive newsletters.
- *Example 2:* Candidate clicks the "Unsubscribe" link in a newsletter email.

### Exceptions
Transactional emails (verification links, invoices, password resets) do not support opt-out.

### User Rights
Users can opt-out of marketing communications.

### Company Responsibility
CVerify must process opt-out requests within 10 business days.

### Security Note
Subscription preferences are stored in our PostgreSQL database.

---

## 30. Candidate Privacy Rights (GDPR/PIPEDA/PDPA)

### Purpose
To detail the legal rights available to candidates.

### Description
Candidates have rights under GDPR, PIPEDA, and PDPA, including the rights of access, rectification, erasure, portability, restriction, and objection.

### Examples
- *Access:* Requesting a file download containing your profile details.
- *Erasure:* Requesting the deletion of your account.
- *Portability:* Exporting your profile data in a structured JSON format.

### Exceptions
Requests may be denied if processing is required by law or to protect system security.

### User Rights
Users can exercise their rights by contacting the DPO.

### Company Responsibility
CVerify must respond to requests within 30 days.

### Security Note
Identity verification is required before we process rights requests.

---

## 31. California Privacy Rights (CCPA/CPRA)

### Purpose
To provide disclosures required under California law.

### Description
This section provides disclosures under the CCPA. CVerify does not sell or share personal information.

### Examples
- *Example 1:* A California resident requests a list of personal data categories collected by CVerify.
- *Example 2:* The user exercises their right to opt-out of profiling features.

### Exceptions
Publicly available records are not considered personal information under CCPA.

### User Rights
California residents have the right to know, delete, correct, and opt-out of the sale/sharing of their personal data.

### Company Responsibility
CVerify must provide a CCPA disclosure statement and response channels.

### Security Note
Request routes require multi-factor verification checks.

---

## 32. Children’s Online Privacy Protection

### Purpose
To protect the privacy of children.

### Description
CVerify is not intended for users under 16 years of age. We do not knowingly collect personal information from children.

### Examples
- *Example 1:* An applicant under 16 attempts to register, and the registration is denied.
- *Example 2:* A parent reports their child's account, and we delete the profile.

### Exceptions
Age limits may vary based on localized legal requirements.

### User Rights
Parents can contact the DPO to report and request deletion of underage accounts.

### Company Responsibility
CVerify must delete underage accounts when identified.

### Security Note
Deleted account profiles are permanently removed from databases and storage buckets.

---

## 33. Changes and Modifications to this Policy

### Purpose
To outline the procedure for revising this policy.

### Description
We may update this Privacy Policy. Modifications will be announced via email and highlighted on our dashboard with a 30-day notice period.

### Examples
- *Example 1:* CVerify updates its retention policy and notifies users via email.
- *Example 2:* The updated policy is published, and the version number increases.

### Exceptions
Minor formatting changes do not require a 30-day notice period.

### User Rights
Users can close their account if they do not agree to the updated policy.

### Company Responsibility
CVerify must maintain an archive of previous policy versions.

### Security Note
Policy updates are recorded in our version history log.

---

## 34. Contact Information & Dispute Resolution

### Purpose
To provide contact channels for privacy inquiries and disputes.

### Description
Users can contact our DPO regarding privacy questions or disputes.

**CVerify J.S.C. Compliance Office**  
Email: `dpo@cverify.com`  
Address: Level 5, Tech Tower, Hanoi, Vietnam.  

### Examples
- *Example 1:* Sending an email to `dpo@cverify.com` to report a potential data leak.
- *Example 2:* Contacting local data protection authorities regarding a complaint.

### Exceptions
General customer service inquiries should use the support portal.

### User Rights
Users can file complaints with national supervisory authorities.

### Company Responsibility
CVerify must review and respond to privacy inquiries.

### Security Note
Inquiries are logged in our secure ticket system.

---

## 35. Internal Link & Cross References

### Purpose
To establish navigation mappings across platform terms and settings.

### Description
Users can access settings, terms, and privacy tools using links on our dashboard.

### Examples
- [Terms of Service](file:///CVerify/docs/terms-of-service.md)
- [Workspace Settings Panel](file:///client/src/app/(candidate)/settings/page.tsx)
- [Profile Privacy Configurations](file:///client/src/app/(candidate)/settings/components/AccountTab.tsx#L568)

### Exceptions
Links are only accessible to authenticated users.

### User Rights
Users can request assistance if navigation links are broken.

### Company Responsibility
CVerify must verify that navigation links remain functional.

### Security Note
Link configurations are audited to prevent URL redirection vulnerabilities.
