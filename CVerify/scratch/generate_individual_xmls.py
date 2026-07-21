import xml.etree.ElementTree as ET
import xml.dom.minidom as minidom
import os

diagrams_data = [
    {
        "filename": "ucd_01_authentication_identity.xml",
        "id": "ucd-01",
        "title": "UCD-01 Authentication & Identity Management Diagram",
        "boundary": "Authentication & Identity Management System",
        "left_actors": [
            {"id": "act_guest", "name": "Guest", "x": 50, "y": 180},
            {"id": "act_user", "name": "User", "x": 50, "y": 480}
        ],
        "right_actors": [
            {"id": "act_oauth", "name": "Google/GitHub OAuth", "x": 960, "y": 200},
            {"id": "act_email", "name": "Email Service", "x": 960, "y": 480}
        ],
        "use_cases": [
            {"id": "uc_01", "name": "UC-AUTH-01: Register via Email", "x": 250, "y": 80},
            {"id": "uc_02", "name": "UC-AUTH-02: Verify Email Address", "x": 580, "y": 80},
            {"id": "uc_03", "name": "UC-AUTH-03: Login with Password", "x": 250, "y": 170},
            {"id": "uc_04", "name": "UC-AUTH-04: Login with Google OAuth", "x": 580, "y": 170},
            {"id": "uc_05", "name": "UC-AUTH-05: Login with GitHub OAuth", "x": 580, "y": 260},
            {"id": "uc_06", "name": "UC-AUTH-06: Refresh Access Token", "x": 415, "y": 340},
            {"id": "uc_07", "name": "UC-AUTH-07: Logout", "x": 250, "y": 430},
            {"id": "uc_08", "name": "UC-AUTH-08: Revoke All Tokens", "x": 580, "y": 430},
            {"id": "uc_09", "name": "UC-AUTH-09: Reactivate Account", "x": 250, "y": 520},
            {"id": "uc_10", "name": "UC-AUTH-10: Switch Actor Context", "x": 580, "y": 520},
            {"id": "uc_11", "name": "UC-AUTH-11: Company Onboarding Setup", "x": 250, "y": 610},
            {"id": "uc_12", "name": "UC-AUTH-12: Workspace Initial Setup", "x": 580, "y": 610}
        ],
        "edges": [
            {"source": "act_guest", "target": "uc_01", "type": "assoc"},
            {"source": "act_guest", "target": "uc_03", "type": "assoc"},
            {"source": "act_guest", "target": "uc_04", "type": "assoc"},
            {"source": "act_guest", "target": "uc_05", "type": "assoc"},
            {"source": "act_user", "target": "uc_06", "type": "assoc"},
            {"source": "act_user", "target": "uc_07", "type": "assoc"},
            {"source": "act_user", "target": "uc_08", "type": "assoc"},
            {"source": "act_user", "target": "uc_09", "type": "assoc"},
            {"source": "act_user", "target": "uc_10", "type": "assoc"},
            {"source": "act_user", "target": "uc_11", "type": "assoc"},
            {"source": "act_oauth", "target": "uc_04", "type": "assoc"},
            {"source": "act_oauth", "target": "uc_05", "type": "assoc"},
            {"source": "uc_02", "target": "act_email", "type": "assoc"},
            {"source": "uc_01", "target": "uc_02", "type": "include"},
            {"source": "uc_04", "target": "uc_06", "type": "include"},
            {"source": "uc_05", "target": "uc_06", "type": "include"},
            {"source": "uc_03", "target": "uc_10", "type": "extend"},
            {"source": "uc_11", "target": "uc_12", "type": "include"}
        ]
    },
    {
        "filename": "ucd_02_account_recovery.xml",
        "id": "ucd-02",
        "title": "UCD-02 Account & Organization Recovery Diagram",
        "boundary": "Account & Organization Recovery System",
        "left_actors": [
            {"id": "act_user_rec", "name": "Guest / User", "x": 50, "y": 200},
            {"id": "act_org_member", "name": "Org Member", "x": 50, "y": 480}
        ],
        "right_actors": [
            {"id": "act_admin_rec", "name": "Super Admin", "x": 960, "y": 200},
            {"id": "act_email_rec", "name": "Email Service", "x": 960, "y": 480}
        ],
        "use_cases": [
            {"id": "uc_r01", "name": "UC-REC-01: Request Password Reset", "x": 250, "y": 80},
            {"id": "uc_r02", "name": "UC-REC-02: Verify Reset Token/OTP", "x": 580, "y": 80},
            {"id": "uc_r03", "name": "UC-REC-03: Reset Password", "x": 415, "y": 170},
            {"id": "uc_r04", "name": "UC-REC-04: Initiate Org Recovery L1", "x": 250, "y": 260},
            {"id": "uc_r05", "name": "UC-REC-05: Initiate Org Recovery L2 Dispute", "x": 250, "y": 350},
            {"id": "uc_r06", "name": "UC-REC-06: Upload Ownership Evidence", "x": 580, "y": 350},
            {"id": "uc_r07", "name": "UC-REC-07: Vote on Recovery Claim", "x": 250, "y": 440},
            {"id": "uc_r08", "name": "UC-REC-08: Track Claim Status", "x": 580, "y": 440},
            {"id": "uc_r09", "name": "UC-REC-09: Approve L2 Claim (Super Admin)", "x": 250, "y": 550},
            {"id": "uc_r10", "name": "UC-REC-10: Reject L2 Claim (Super Admin)", "x": 580, "y": 550},
            {"id": "uc_r11", "name": "UC-REC-11: Reclaim Org Ownership", "x": 415, "y": 640}
        ],
        "edges": [
            {"source": "act_user_rec", "target": "uc_r01", "type": "assoc"},
            {"source": "act_user_rec", "target": "uc_r04", "type": "assoc"},
            {"source": "act_user_rec", "target": "uc_r05", "type": "assoc"},
            {"source": "act_user_rec", "target": "uc_r08", "type": "assoc"},
            {"source": "act_org_member", "target": "uc_r07", "type": "assoc"},
            {"source": "act_admin_rec", "target": "uc_r09", "type": "assoc"},
            {"source": "act_admin_rec", "target": "uc_r10", "type": "assoc"},
            {"source": "uc_r01", "target": "act_email_rec", "type": "assoc"},
            {"source": "uc_r01", "target": "uc_r02", "type": "include"},
            {"source": "uc_r02", "target": "uc_r03", "type": "include"},
            {"source": "uc_r05", "target": "uc_r06", "type": "include"},
            {"source": "uc_r05", "target": "uc_r07", "type": "extend"},
            {"source": "uc_r09", "target": "uc_r11", "type": "include"}
        ]
    },
    {
        "filename": "ucd_03_organization_workspace.xml",
        "id": "ucd-03",
        "title": "UCD-03 Organization & Workspace Management Diagram",
        "boundary": "Organization & Workspace Management System",
        "left_actors": [
            {"id": "act_org_admin", "name": "Org Admin", "x": 50, "y": 220},
            {"id": "act_org_mem", "name": "Org Member", "x": 50, "y": 520}
        ],
        "right_actors": [
            {"id": "act_invitee", "name": "Guest / Invitee", "x": 960, "y": 220},
            {"id": "act_s_admin", "name": "Super Admin", "x": 960, "y": 520}
        ],
        "use_cases": [
            {"id": "uc_o01", "name": "UC-ORG-01: Update Org Profile & Branding", "x": 250, "y": 70},
            {"id": "uc_o02", "name": "UC-ORG-02: Submit Org Verification", "x": 250, "y": 140},
            {"id": "uc_o03", "name": "UC-ORG-03: Approve/Reject Org Verification", "x": 580, "y": 140},
            {"id": "uc_o04", "name": "UC-ORG-04: Create Custom Org Role", "x": 250, "y": 210},
            {"id": "uc_o05", "name": "UC-ORG-05: Assign Permissions to Role", "x": 580, "y": 210},
            {"id": "uc_o06", "name": "UC-ORG-06: Manage Org Roles", "x": 250, "y": 280},
            {"id": "uc_o07", "name": "UC-ORG-07: Invite Member via Email", "x": 250, "y": 350},
            {"id": "uc_o08", "name": "UC-ORG-08: Accept/Decline Org Invitation", "x": 580, "y": 350},
            {"id": "uc_o09", "name": "UC-ORG-09: Assign Role to Member", "x": 250, "y": 420},
            {"id": "uc_o10", "name": "UC-ORG-10: Manage/Remove Member", "x": 580, "y": 420},
            {"id": "uc_o11", "name": "UC-ORG-11: Create Workspace", "x": 250, "y": 490},
            {"id": "uc_o12", "name": "UC-ORG-12: Manage Workspace Settings", "x": 580, "y": 490},
            {"id": "uc_o13", "name": "UC-ORG-13: Manage Workspace Members", "x": 250, "y": 560},
            {"id": "uc_o14", "name": "UC-ORG-14: Create Workspace Post", "x": 580, "y": 560},
            {"id": "uc_o15", "name": "UC-ORG-15: Edit/Delete Workspace Post", "x": 250, "y": 630},
            {"id": "uc_o16", "name": "UC-ORG-16: View Workspace List", "x": 580, "y": 630},
            {"id": "uc_o17", "name": "UC-ORG-17: Business Roles Management", "x": 250, "y": 700},
            {"id": "uc_o18", "name": "UC-ORG-18: View Org Permission Audit", "x": 580, "y": 700}
        ],
        "edges": [
            {"source": "act_org_admin", "target": "uc_o01", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o02", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o04", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o06", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o07", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o09", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o11", "type": "assoc"},
            {"source": "act_org_admin", "target": "uc_o18", "type": "assoc"},
            {"source": "act_org_mem", "target": "uc_o13", "type": "assoc"},
            {"source": "act_org_mem", "target": "uc_o14", "type": "assoc"},
            {"source": "act_org_mem", "target": "uc_o16", "type": "assoc"},
            {"source": "act_invitee", "target": "uc_o08", "type": "assoc"},
            {"source": "act_s_admin", "target": "uc_o03", "type": "assoc"},
            {"source": "uc_o02", "target": "uc_o03", "type": "extend"},
            {"source": "uc_o04", "target": "uc_o05", "type": "include"},
            {"source": "uc_o07", "target": "uc_o08", "type": "include"},
            {"source": "uc_o11", "target": "uc_o13", "type": "include"}
        ]
    },
    {
        "filename": "ucd_04_candidate_profile_evidence.xml",
        "id": "ucd-04",
        "title": "UCD-04 Candidate Profile & Evidence Verification Diagram",
        "boundary": "Candidate Profile & Evidence Verification System",
        "left_actors": [
            {"id": "act_cand_p", "name": "Candidate", "x": 50, "y": 280}
        ],
        "right_actors": [
            {"id": "act_recruiter_p", "name": "Recruiter / Employer", "x": 960, "y": 200},
            {"id": "act_admin_p", "name": "Super Admin / System", "x": 960, "y": 500}
        ],
        "use_cases": [
            {"id": "uc_p01", "name": "UC-PROF-01: Manage Personal Profile & Bio", "x": 250, "y": 70},
            {"id": "uc_p02", "name": "UC-PROF-02: Upload & Manage CV Files", "x": 580, "y": 70},
            {"id": "uc_p03", "name": "UC-PROF-03: Manage CV Privacy Settings", "x": 250, "y": 150},
            {"id": "uc_p04", "name": "UC-PROF-04: Manage Education Entries", "x": 580, "y": 150},
            {"id": "uc_p05", "name": "UC-PROF-05: Manage Work Experience Entries", "x": 250, "y": 230},
            {"id": "uc_p06", "name": "UC-PROF-06: Set Leadership Flag", "x": 580, "y": 230},
            {"id": "uc_p07", "name": "UC-PROF-07: Manage Project Entries", "x": 250, "y": 310},
            {"id": "uc_p08", "name": "UC-PROF-08: Add Project Contributions", "x": 580, "y": 310},
            {"id": "uc_p09", "name": "UC-PROF-09: Add Project Technologies", "x": 250, "y": 390},
            {"id": "uc_p10", "name": "UC-PROF-10: Manage Achievement Entries", "x": 580, "y": 390},
            {"id": "uc_p11", "name": "UC-PROF-11: Upload Verification Evidence", "x": 250, "y": 470},
            {"id": "uc_p12", "name": "UC-PROF-12: Link Evidence to Profile Claim", "x": 580, "y": 470},
            {"id": "uc_p13", "name": "UC-PROF-13: Validate & Verify Evidence Claims", "x": 250, "y": 550},
            {"id": "uc_p14", "name": "UC-PROF-14: Update Verification Level Status", "x": 580, "y": 550},
            {"id": "uc_p15", "name": "UC-PROF-15: View Public Developer Profile", "x": 250, "y": 630},
            {"id": "uc_p16", "name": "UC-PROF-16: Export Formatted CV", "x": 580, "y": 630}
        ],
        "edges": [
            {"source": "act_cand_p", "target": "uc_p01", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p02", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p03", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p04", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p05", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p07", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p10", "type": "assoc"},
            {"source": "act_cand_p", "target": "uc_p11", "type": "assoc"},
            {"source": "act_recruiter_p", "target": "uc_p15", "type": "assoc"},
            {"source": "act_admin_p", "target": "uc_p13", "type": "assoc"},
            {"source": "uc_p05", "target": "uc_p06", "type": "extend"},
            {"source": "uc_p07", "target": "uc_p08", "type": "include"},
            {"source": "uc_p07", "target": "uc_p09", "type": "include"},
            {"source": "uc_p11", "target": "uc_p12", "type": "include"},
            {"source": "uc_p13", "target": "uc_p14", "type": "include"},
            {"source": "uc_p15", "target": "uc_p16", "type": "extend"}
        ]
    },
    {
        "filename": "ucd_05_source_code_git_analysis.xml",
        "id": "ucd-05",
        "title": "UCD-05 Source Code & Git Repository Analysis Diagram",
        "boundary": "Source Code & Git Repository Analysis System",
        "left_actors": [
            {"id": "act_cand_git", "name": "Candidate", "x": 50, "y": 250},
            {"id": "act_worker_git", "name": "Background Worker", "x": 50, "y": 500}
        ],
        "right_actors": [
            {"id": "act_provider_git", "name": "Git Provider (GitHub/GitLab)", "x": 960, "y": 200},
            {"id": "act_ai_git", "name": "CVerify.AI Engine", "x": 960, "y": 480}
        ],
        "use_cases": [
            {"id": "uc_g01", "name": "UC-GIT-01: Connect Source Code Provider", "x": 250, "y": 80},
            {"id": "uc_g02", "name": "UC-GIT-02: Sync Remote Repositories", "x": 580, "y": 80},
            {"id": "uc_g03", "name": "UC-GIT-03: View Repository List & Status", "x": 250, "y": 170},
            {"id": "uc_g04", "name": "UC-GIT-04: Link Repository to Profile Project", "x": 580, "y": 170},
            {"id": "uc_g05", "name": "UC-GIT-05: Trigger Repository Audit Job", "x": 250, "y": 260},
            {"id": "uc_g06", "name": "UC-GIT-06: Clone & Extract Code AST", "x": 580, "y": 260},
            {"id": "uc_g07", "name": "UC-GIT-07: Dispatch Payload to CVerify.AI Engine", "x": 250, "y": 350},
            {"id": "uc_g08", "name": "UC-GIT-08: Analyze Tech Stack & Patterns", "x": 580, "y": 350},
            {"id": "uc_g09", "name": "UC-GIT-09: Extract Code Intelligence Signals", "x": 250, "y": 440},
            {"id": "uc_g10", "name": "UC-GIT-10: Attribute Skill Contributions via Commits", "x": 580, "y": 440},
            {"id": "uc_g11", "name": "UC-GIT-11: Track AI Analysis Job Progress", "x": 250, "y": 530},
            {"id": "uc_g12", "name": "UC-GIT-12: View Repository Assessment Report", "x": 580, "y": 530},
            {"id": "uc_g13", "name": "UC-GIT-13: Cancel / Rerun Failed Analysis Job", "x": 250, "y": 620},
            {"id": "uc_g14", "name": "UC-GIT-14: Disconnect Git Provider", "x": 580, "y": 620}
        ],
        "edges": [
            {"source": "act_cand_git", "target": "uc_g01", "type": "assoc"},
            {"source": "act_cand_git", "target": "uc_g03", "type": "assoc"},
            {"source": "act_cand_git", "target": "uc_g04", "type": "assoc"},
            {"source": "act_cand_git", "target": "uc_g05", "type": "assoc"},
            {"source": "act_cand_git", "target": "uc_g11", "type": "assoc"},
            {"source": "act_cand_git", "target": "uc_g12", "type": "assoc"},
            {"source": "act_cand_git", "target": "uc_g14", "type": "assoc"},
            {"source": "act_worker_git", "target": "uc_g06", "type": "assoc"},
            {"source": "act_provider_git", "target": "uc_g02", "type": "assoc"},
            {"source": "act_ai_git", "target": "uc_g07", "type": "assoc"},
            {"source": "uc_g01", "target": "uc_g02", "type": "include"},
            {"source": "uc_g05", "target": "uc_g06", "type": "include"},
            {"source": "uc_g05", "target": "uc_g07", "type": "include"},
            {"source": "uc_g07", "target": "uc_g08", "type": "include"},
            {"source": "uc_g07", "target": "uc_g09", "type": "include"},
            {"source": "uc_g07", "target": "uc_g10", "type": "include"},
            {"source": "uc_g12", "target": "uc_g11", "type": "extend"}
        ]
    },
    {
        "filename": "ucd_06_candidate_ai_assessment.xml",
        "id": "ucd-06",
        "title": "UCD-06 Candidate AI Assessment & Intelligence Diagram",
        "boundary": "Candidate AI Assessment & Intelligence System",
        "left_actors": [
            {"id": "act_cand_intel", "name": "Candidate", "x": 50, "y": 200},
            {"id": "act_employer_intel", "name": "Employer / Org Admin", "x": 50, "y": 500}
        ],
        "right_actors": [
            {"id": "act_ai_intel", "name": "CVerify.AI Engine", "x": 960, "y": 200},
            {"id": "act_admin_intel", "name": "Super Admin", "x": 960, "y": 500}
        ],
        "use_cases": [
            {"id": "uc_i01", "name": "UC-INT-01: Queue Candidate for Assessment", "x": 250, "y": 70},
            {"id": "uc_i02", "name": "UC-INT-02: Run Assessment Pipeline", "x": 580, "y": 70},
            {"id": "uc_i03", "name": "UC-INT-03: Build Interactive Skill Tree", "x": 250, "y": 150},
            {"id": "uc_i04", "name": "UC-INT-04: Calculate Candidate Trust Score", "x": 580, "y": 150},
            {"id": "uc_i05", "name": "UC-INT-05: Analyze Strengths & Weaknesses", "x": 250, "y": 230},
            {"id": "uc_i06", "name": "UC-INT-06: Project Best-Fit Career Roles", "x": 580, "y": 230},
            {"id": "uc_i07", "name": "UC-INT-07: View AI Assessment Report", "x": 250, "y": 310},
            {"id": "uc_i08", "name": "UC-INT-08: Re-trigger Assessment", "x": 580, "y": 310},
            {"id": "uc_i09", "name": "UC-INT-09: Override Trust Score (Admin)", "x": 250, "y": 390},
            {"id": "uc_i10", "name": "UC-INT-10: View Assessment Version History", "x": 580, "y": 390},
            {"id": "uc_i11", "name": "UC-INT-11: Search Talent Pool with Filters", "x": 250, "y": 470},
            {"id": "uc_i12", "name": "UC-INT-12: Rank Candidates against Job", "x": 580, "y": 470},
            {"id": "uc_i13", "name": "UC-INT-13: Analyze Skill Gap Analysis", "x": 250, "y": 550},
            {"id": "uc_i14", "name": "UC-INT-14: Save Candidate to Talent Pool", "x": 580, "y": 550},
            {"id": "uc_i15", "name": "UC-INT-15: Export Candidate Audit Report", "x": 250, "y": 630},
            {"id": "uc_i16", "name": "UC-INT-16: Browse Capability Catalog", "x": 580, "y": 630},
            {"id": "uc_i17", "name": "UC-INT-17: Compare Candidate Profiles", "x": 415, "y": 700}
        ],
        "edges": [
            {"source": "act_cand_intel", "target": "uc_i03", "type": "assoc"},
            {"source": "act_cand_intel", "target": "uc_i04", "type": "assoc"},
            {"source": "act_cand_intel", "target": "uc_i07", "type": "assoc"},
            {"source": "act_employer_intel", "target": "uc_i11", "type": "assoc"},
            {"source": "act_employer_intel", "target": "uc_i12", "type": "assoc"},
            {"source": "act_employer_intel", "target": "uc_i14", "type": "assoc"},
            {"source": "act_employer_intel", "target": "uc_i17", "type": "assoc"},
            {"source": "act_ai_intel", "target": "uc_i02", "type": "assoc"},
            {"source": "act_admin_intel", "target": "uc_i09", "type": "assoc"},
            {"source": "uc_i01", "target": "uc_i02", "type": "include"},
            {"source": "uc_i02", "target": "uc_i03", "type": "include"},
            {"source": "uc_i02", "target": "uc_i04", "type": "include"},
            {"source": "uc_i02", "target": "uc_i05", "type": "include"},
            {"source": "uc_i02", "target": "uc_i06", "type": "include"},
            {"source": "uc_i11", "target": "uc_i12", "type": "include"},
            {"source": "uc_i07", "target": "uc_i15", "type": "extend"}
        ]
    },
    {
        "filename": "ucd_07_ai_chat_interview.xml",
        "id": "ucd-07",
        "title": "UCD-07 AI Interactive Chat & Interview Guidance Diagram",
        "boundary": "AI Interactive Chat & Interview Guidance System",
        "left_actors": [
            {"id": "act_cand_chat", "name": "Candidate / User", "x": 50, "y": 300}
        ],
        "right_actors": [
            {"id": "act_ai_chat", "name": "CVerify.AI Engine (LLM)", "x": 960, "y": 200},
            {"id": "act_admin_chat", "name": "Super Admin", "x": 960, "y": 500}
        ],
        "use_cases": [
            {"id": "uc_c01", "name": "UC-CHAT-01: Create New AI Chat Session", "x": 250, "y": 80},
            {"id": "uc_c02", "name": "UC-CHAT-02: Send Message to AI Assistant", "x": 580, "y": 80},
            {"id": "uc_c03", "name": "UC-CHAT-03: Stream Realtime AI Response (SSE)", "x": 415, "y": 170},
            {"id": "uc_c04", "name": "UC-CHAT-04: View Conversation History", "x": 250, "y": 260},
            {"id": "uc_c05", "name": "UC-CHAT-05: Load Conversation Details", "x": 580, "y": 260},
            {"id": "uc_c06", "name": "UC-CHAT-06: Delete Conversation History", "x": 250, "y": 350},
            {"id": "uc_c07", "name": "UC-CHAT-07: Initiate AI Mock Interview", "x": 580, "y": 350},
            {"id": "uc_c08", "name": "UC-CHAT-08: Configure Interview Blueprint", "x": 250, "y": 450},
            {"id": "uc_c09", "name": "UC-CHAT-09: Monitor AI Streaming Metrics", "x": 580, "y": 450},
            {"id": "uc_c10", "name": "UC-CHAT-10: Manage Prompt Deployments", "x": 415, "y": 550}
        ],
        "edges": [
            {"source": "act_cand_chat", "target": "uc_c01", "type": "assoc"},
            {"source": "act_cand_chat", "target": "uc_c02", "type": "assoc"},
            {"source": "act_cand_chat", "target": "uc_c04", "type": "assoc"},
            {"source": "act_cand_chat", "target": "uc_c07", "type": "assoc"},
            {"source": "act_ai_chat", "target": "uc_c03", "type": "assoc"},
            {"source": "act_admin_chat", "target": "uc_c09", "type": "assoc"},
            {"source": "act_admin_chat", "target": "uc_c10", "type": "assoc"},
            {"source": "uc_c02", "target": "uc_c03", "type": "include"},
            {"source": "uc_c07", "target": "uc_c01", "type": "include"},
            {"source": "uc_c08", "target": "uc_c10", "type": "include"}
        ]
    },
    {
        "filename": "ucd_08_recruitment_job_vacancy.xml",
        "id": "ucd-08",
        "title": "UCD-08 Recruitment & Job Vacancy Management Diagram",
        "boundary": "Recruitment & Job Vacancy Management System",
        "left_actors": [
            {"id": "act_cand_job", "name": "Guest / Candidate", "x": 50, "y": 200},
            {"id": "act_recruiter_job", "name": "Recruiter / Org Admin", "x": 50, "y": 500}
        ],
        "right_actors": [
            {"id": "act_ai_job", "name": "CVerify.AI Engine", "x": 960, "y": 300}
        ],
        "use_cases": [
            {"id": "uc_j01", "name": "UC-JOB-01: Create Job Vacancy", "x": 250, "y": 70},
            {"id": "uc_j02", "name": "UC-JOB-02: Update Job Vacancy", "x": 580, "y": 70},
            {"id": "uc_j03", "name": "UC-JOB-03: Toggle Job Vacancy Status", "x": 250, "y": 140},
            {"id": "uc_j04", "name": "UC-JOB-04: Delete Job Vacancy", "x": 580, "y": 140},
            {"id": "uc_j05", "name": "UC-JOB-05: Create Hiring Requirement Profile", "x": 250, "y": 210},
            {"id": "uc_j06", "name": "UC-JOB-06: Extract Capability Requirements via AI", "x": 580, "y": 210},
            {"id": "uc_j07", "name": "UC-JOB-07: Configure Interview Blueprint", "x": 250, "y": 280},
            {"id": "uc_j08", "name": "UC-JOB-08: Setup Evaluation Rubrics", "x": 580, "y": 280},
            {"id": "uc_j09", "name": "UC-JOB-09: Create Requirement Snapshot", "x": 250, "y": 350},
            {"id": "uc_j10", "name": "UC-JOB-10: Publish Job to Public Portal", "x": 580, "y": 350},
            {"id": "uc_j11", "name": "UC-JOB-11: Search Public Jobs", "x": 250, "y": 430},
            {"id": "uc_j12", "name": "UC-JOB-12: View Public Job Details", "x": 580, "y": 430},
            {"id": "uc_j13", "name": "UC-JOB-13: Apply for Job Vacancy", "x": 250, "y": 510},
            {"id": "uc_j14", "name": "UC-JOB-14: Withdraw Job Application", "x": 580, "y": 510},
            {"id": "uc_j15", "name": "UC-JOB-15: Manage Job Applications", "x": 250, "y": 590},
            {"id": "uc_j16", "name": "UC-JOB-16: Evaluate Candidate-Job Fit Score", "x": 580, "y": 590},
            {"id": "uc_j17", "name": "UC-JOB-17: Advance Application Pipeline Stage", "x": 250, "y": 670},
            {"id": "uc_j18", "name": "UC-JOB-18: View Matching Explanation Breakdown", "x": 580, "y": 670}
        ],
        "edges": [
            {"source": "act_cand_job", "target": "uc_j11", "type": "assoc"},
            {"source": "act_cand_job", "target": "uc_j12", "type": "assoc"},
            {"source": "act_cand_job", "target": "uc_j13", "type": "assoc"},
            {"source": "act_cand_job", "target": "uc_j14", "type": "assoc"},
            {"source": "act_recruiter_job", "target": "uc_j01", "type": "assoc"},
            {"source": "act_recruiter_job", "target": "uc_j05", "type": "assoc"},
            {"source": "act_recruiter_job", "target": "uc_j10", "type": "assoc"},
            {"source": "act_recruiter_job", "target": "uc_j15", "type": "assoc"},
            {"source": "act_recruiter_job", "target": "uc_j17", "type": "assoc"},
            {"source": "act_ai_job", "target": "uc_j06", "type": "assoc"},
            {"source": "act_ai_job", "target": "uc_j16", "type": "assoc"},
            {"source": "uc_j01", "target": "uc_j05", "type": "include"},
            {"source": "uc_j05", "target": "uc_j06", "type": "include"},
            {"source": "uc_j10", "target": "uc_j09", "type": "include"},
            {"source": "uc_j13", "target": "uc_j16", "type": "include"}
        ]
    },
    {
        "filename": "ucd_09_community_forum.xml",
        "id": "ucd-09",
        "title": "UCD-09 Community Forum & Moderation Diagram",
        "boundary": "Community Forum & Moderation System",
        "left_actors": [
            {"id": "act_forum_user", "name": "Forum User / Candidate", "x": 50, "y": 250},
            {"id": "act_forum_mod", "name": "Forum Moderator", "x": 50, "y": 520}
        ],
        "right_actors": [
            {"id": "act_admin_forum", "name": "Super Admin", "x": 960, "y": 250},
            {"id": "act_auto_filter", "name": "Automated Filter", "x": 960, "y": 520}
        ],
        "use_cases": [
            {"id": "uc_f01", "name": "UC-FORUM-01: Browse Categories & Topics", "x": 250, "y": 70},
            {"id": "uc_f02", "name": "UC-FORUM-02: Create Forum Topic", "x": 580, "y": 70},
            {"id": "uc_f03", "name": "UC-FORUM-03: Edit Forum Topic", "x": 250, "y": 150},
            {"id": "uc_f04", "name": "UC-FORUM-04: Delete Forum Topic", "x": 580, "y": 150},
            {"id": "uc_f05", "name": "UC-FORUM-05: Create Reply / Comment", "x": 250, "y": 230},
            {"id": "uc_f06", "name": "UC-FORUM-06: Edit/Delete Reply", "x": 580, "y": 230},
            {"id": "uc_f07", "name": "UC-FORUM-07: Vote Upvote / Downvote", "x": 250, "y": 310},
            {"id": "uc_f08", "name": "UC-FORUM-08: React to Post", "x": 580, "y": 310},
            {"id": "uc_f09", "name": "UC-FORUM-09: Bookmark Topic", "x": 250, "y": 390},
            {"id": "uc_f10", "name": "UC-FORUM-10: Follow Topic/Author", "x": 580, "y": 390},
            {"id": "uc_f11", "name": "UC-FORUM-11: Report Content", "x": 250, "y": 470},
            {"id": "uc_f12", "name": "UC-FORUM-12: View Moderation Queue", "x": 580, "y": 470},
            {"id": "uc_f13", "name": "UC-FORUM-13: Approve/Reject Reported Content", "x": 250, "y": 550},
            {"id": "uc_f14", "name": "UC-FORUM-14: Manage Categories & Tags", "x": 580, "y": 550},
            {"id": "uc_f15", "name": "UC-FORUM-15: View Reputation & Badges", "x": 415, "y": 630}
        ],
        "edges": [
            {"source": "act_forum_user", "target": "uc_f01", "type": "assoc"},
            {"source": "act_forum_user", "target": "uc_f02", "type": "assoc"},
            {"source": "act_forum_user", "target": "uc_f05", "type": "assoc"},
            {"source": "act_forum_user", "target": "uc_f07", "type": "assoc"},
            {"source": "act_forum_user", "target": "uc_f11", "type": "assoc"},
            {"source": "act_forum_mod", "target": "uc_f12", "type": "assoc"},
            {"source": "act_forum_mod", "target": "uc_f13", "type": "assoc"},
            {"source": "act_admin_forum", "target": "uc_f14", "type": "assoc"},
            {"source": "act_auto_filter", "target": "uc_f12", "type": "assoc"},
            {"source": "uc_f02", "target": "uc_f11", "type": "extend"},
            {"source": "uc_f11", "target": "uc_f12", "type": "include"},
            {"source": "uc_f12", "target": "uc_f13", "type": "include"},
            {"source": "uc_f05", "target": "uc_f15", "type": "include"}
        ]
    },
    {
        "filename": "ucd_10_notification_communication.xml",
        "id": "ucd-10",
        "title": "UCD-10 Notification & Communication System Diagram",
        "boundary": "Notification & Communication System",
        "left_actors": [
            {"id": "act_user_notif", "name": "User / Candidate", "x": 50, "y": 250},
            {"id": "act_worker_notif", "name": "Outbox Worker", "x": 50, "y": 500}
        ],
        "right_actors": [
            {"id": "act_email_gateway", "name": "Email Gateway (SMTP)", "x": 960, "y": 250},
            {"id": "act_admin_notif", "name": "Super Admin", "x": 960, "y": 500}
        ],
        "use_cases": [
            {"id": "uc_n01", "name": "UC-NOTIF-01: View In-App Notifications", "x": 250, "y": 100},
            {"id": "uc_n02", "name": "UC-NOTIF-02: Mark Notification as Read", "x": 580, "y": 100},
            {"id": "uc_n03", "name": "UC-NOTIF-03: Mark All Notifications as Read", "x": 250, "y": 200},
            {"id": "uc_n04", "name": "UC-NOTIF-04: Manage Notification Preferences", "x": 580, "y": 200},
            {"id": "uc_n05", "name": "UC-NOTIF-05: Aggregate Notifications", "x": 415, "y": 300},
            {"id": "uc_n06", "name": "UC-NOTIF-06: Process Outbox Email Messages", "x": 250, "y": 420},
            {"id": "uc_n07", "name": "UC-NOTIF-07: Send Test Email (Admin)", "x": 580, "y": 420},
            {"id": "uc_n08", "name": "UC-NOTIF-08: Render Email Templates", "x": 415, "y": 520}
        ],
        "edges": [
            {"source": "act_user_notif", "target": "uc_n01", "type": "assoc"},
            {"source": "act_user_notif", "target": "uc_n04", "type": "assoc"},
            {"source": "act_worker_notif", "target": "uc_n06", "type": "assoc"},
            {"source": "act_email_gateway", "target": "uc_n06", "type": "assoc"},
            {"source": "act_admin_notif", "target": "uc_n07", "type": "assoc"},
            {"source": "uc_n01", "target": "uc_n02", "type": "extend"},
            {"source": "uc_n01", "target": "uc_n05", "type": "include"},
            {"source": "uc_n06", "target": "uc_n08", "type": "include"}
        ]
    },
    {
        "filename": "ucd_11_system_administration.xml",
        "id": "ucd-11",
        "title": "UCD-11 System Administration & Governance Diagram",
        "boundary": "System Administration & Governance System",
        "left_actors": [
            {"id": "act_super_admin", "name": "Super Admin", "x": 50, "y": 350}
        ],
        "right_actors": [
            {"id": "act_sys_monitor", "name": "System Monitor", "x": 960, "y": 350}
        ],
        "use_cases": [
            {"id": "uc_a01", "name": "UC-ADM-01: Manage Global Users", "x": 250, "y": 70},
            {"id": "uc_a02", "name": "UC-ADM-02: Lock/Unlock Account", "x": 580, "y": 70},
            {"id": "uc_a03", "name": "UC-ADM-03: Assign/Revoke System Roles", "x": 250, "y": 150},
            {"id": "uc_a04", "name": "UC-ADM-04: Manage System Roles", "x": 580, "y": 150},
            {"id": "uc_a05", "name": "UC-ADM-05: Manage Global Permissions", "x": 250, "y": 230},
            {"id": "uc_a06", "name": "UC-ADM-06: View Audit Logs", "x": 580, "y": 230},
            {"id": "uc_a07", "name": "UC-ADM-07: View System Analytics", "x": 250, "y": 310},
            {"id": "uc_a08", "name": "UC-ADM-08: Check System Health Status", "x": 580, "y": 310},
            {"id": "uc_a09", "name": "UC-ADM-09: Monitor Server Metrics", "x": 250, "y": 390},
            {"id": "uc_a10", "name": "UC-ADM-10: Purge Cache / Temp Data", "x": 580, "y": 390},
            {"id": "uc_a11", "name": "UC-ADM-11: Manage System Settings", "x": 250, "y": 470},
            {"id": "uc_a12", "name": "UC-ADM-12: Check DB & Integration Health", "x": 580, "y": 470},
            {"id": "uc_a13", "name": "UC-ADM-13: Impersonate User Account", "x": 415, "y": 550}
        ],
        "edges": [
            {"source": "act_super_admin", "target": "uc_a01", "type": "assoc"},
            {"source": "act_super_admin", "target": "uc_a04", "type": "assoc"},
            {"source": "act_super_admin", "target": "uc_a06", "type": "assoc"},
            {"source": "act_super_admin", "target": "uc_a11", "type": "assoc"},
            {"source": "act_super_admin", "target": "uc_a13", "type": "assoc"},
            {"source": "act_sys_monitor", "target": "uc_a08", "type": "assoc"},
            {"source": "act_sys_monitor", "target": "uc_a09", "type": "assoc"},
            {"source": "uc_a01", "target": "uc_a03", "type": "include"},
            {"source": "uc_a04", "target": "uc_a05", "type": "include"},
            {"source": "uc_a08", "target": "uc_a12", "type": "include"}
        ]
    }
]

out_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify\drawio_diagrams"
os.makedirs(out_dir, exist_ok=True)

for diag in diagrams_data:
    mxfile = ET.Element("mxfile", {
        "host": "app.diagrams.net",
        "agent": "CVerify",
        "version": "21.0.0"
    })

    d_elem = ET.SubElement(mxfile, "diagram", {
        "id": diag["id"],
        "name": diag["title"]
    })
    
    model = ET.SubElement(d_elem, "mxGraphModel", {
        "dx": "1200", "dy": "800", "grid": "1", "gridSize": "10",
        "guides": "1", "tooltips": "1", "connect": "1", "arrows": "1",
        "fold": "1", "page": "1", "pageScale": "1",
        "pageWidth": "1169", "pageHeight": "827", "math": "0", "shadow": "0"
    })
    
    root = ET.SubElement(model, "root")
    ET.SubElement(root, "mxCell", {"id": "0"})
    ET.SubElement(root, "mxCell", {"id": "1", "parent": "0"})

    max_y = max(uc["y"] for uc in diag["use_cases"]) + 100
    boundary_h = max(max_y, 750)
    
    b_cell = ET.SubElement(root, "mxCell", {
        "id": f"boundary_{diag['id']}",
        "value": diag["boundary"],
        "style": "shape=swimlane;whiteSpace=wrap;html=1;startSize=35;collapsible=0;recursiveResize=0;expand=0;fontStyle=1;fontSize=14;fillColor=#f8f9fa;strokeColor=#6c757d;",
        "vertex": "1",
        "parent": "1"
    })
    ET.SubElement(b_cell, "mxGeometry", {
        "x": "180", "y": "20", "width": "740", "height": str(boundary_h), "as": "geometry"
    })

    for la in diag["left_actors"]:
        cell = ET.SubElement(root, "mxCell", {
            "id": la["id"],
            "value": la["name"],
            "style": "shape=umlActor;verticalLabelPosition=bottom;verticalAlign=top;html=1;outlineConnect=0;fillColor=#dae8fc;strokeColor=#6c8ebf;fontSize=12;fontStyle=1;",
            "vertex": "1",
            "parent": "1"
        })
        ET.SubElement(cell, "mxGeometry", {
            "x": str(la["x"]), "y": str(la["y"]), "width": "50", "height": "90", "as": "geometry"
        })

    for ra in diag["right_actors"]:
        cell = ET.SubElement(root, "mxCell", {
            "id": ra["id"],
            "value": ra["name"],
            "style": "shape=umlActor;verticalLabelPosition=bottom;verticalAlign=top;html=1;outlineConnect=0;fillColor=#fff2cc;strokeColor=#d6b656;fontSize=12;fontStyle=1;",
            "vertex": "1",
            "parent": "1"
        })
        ET.SubElement(cell, "mxGeometry", {
            "x": str(ra["x"]), "y": str(ra["y"]), "width": "50", "height": "90", "as": "geometry"
        })

    for uc in diag["use_cases"]:
        cell = ET.SubElement(root, "mxCell", {
            "id": uc["id"],
            "value": uc["name"],
            "style": "ellipse;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;fontSize=11;fontStyle=1;align=center;",
            "vertex": "1",
            "parent": "1"
        })
        ET.SubElement(cell, "mxGeometry", {
            "x": str(uc["x"]), "y": str(uc["y"]), "width": "220", "height": "60", "as": "geometry"
        })

    edge_idx = 0
    for edge in diag["edges"]:
        edge_idx += 1
        e_id = f"edge_{diag['id']}_{edge_idx}"
        
        if edge["type"] == "assoc":
            style = "endArrow=none;html=1;rounded=0;strokeWidth=1.5;strokeColor=#333333;"
            val = ""
        elif edge["type"] == "include":
            style = "endArrow=open;endSize=8;dashed=1;html=1;rounded=0;strokeWidth=1.2;strokeColor=#d79b00;fontSize=10;fontStyle=1;"
            val = "&lt;&lt;include&gt;&gt;"
        elif edge["type"] == "extend":
            style = "endArrow=open;endSize=8;dashed=1;html=1;rounded=0;strokeWidth=1.2;strokeColor=#b85450;fontSize=10;fontStyle=1;"
            val = "&lt;&lt;extend&gt;&gt;"

        cell = ET.SubElement(root, "mxCell", {
            "id": e_id,
            "value": val,
            "style": style,
            "edge": "1",
            "parent": "1",
            "source": edge["source"],
            "target": edge["target"]
        })
        ET.SubElement(cell, "mxGeometry", {
            "relative": "1", "as": "geometry"
        })

    xml_str = ET.tostring(mxfile, encoding="utf-8")
    dom = minidom.parseString(xml_str)
    pretty_xml = dom.toprettyxml(indent="  ", encoding="utf-8").decode("utf-8")

    filepath = os.path.join(out_dir, diag["filename"])
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(pretty_xml)
        
    drawio_filepath = os.path.join(out_dir, diag["filename"].replace(".xml", ".drawio"))
    with open(drawio_filepath, "w", encoding="utf-8") as f:
        f.write(pretty_xml)

print("SUCCESS: 11 individual XML and DRAWIO files generated in drawio_diagrams/")
