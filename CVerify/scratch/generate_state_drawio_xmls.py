import xml.etree.ElementTree as ET
import xml.dom.minidom as minidom
import os

out_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify\drawio_diagrams"
os.makedirs(out_dir, exist_ok=True)

def prettify(elem):
    xml_str = ET.tostring(elem, encoding="utf-8")
    dom = minidom.parseString(xml_str)
    return dom.toprettyxml(indent="  ", encoding="utf-8").decode("utf-8")

def create_state_diagram(file_prefix, diag_id, diag_title, boundary_title, nodes_data, edges_data, total_w=950, total_h=800):
    mxfile = ET.Element("mxfile", {"host": "app.diagrams.net", "agent": "CVerify", "version": "21.0.0"})
    diag = ET.SubElement(mxfile, "diagram", {"id": diag_id, "name": diag_title})
    model = ET.SubElement(diag, "mxGraphModel", {
        "dx": "1200", "dy": "800", "grid": "1", "gridSize": "10", "guides": "1",
        "tooltips": "1", "connect": "1", "arrows": "1", "fold": "1", "page": "1",
        "pageScale": "1", "pageWidth": str(total_w + 100), "pageHeight": str(total_h + 100), "math": "0", "shadow": "0"
    })
    root = ET.SubElement(model, "root")
    ET.SubElement(root, "mxCell", {"id": "0"})
    ET.SubElement(root, "mxCell", {"id": "1", "parent": "0"})

    b_cell = ET.SubElement(root, "mxCell", {
        "id": f"boundary_{diag_id}",
        "value": boundary_title,
        "style": "shape=swimlane;whiteSpace=wrap;html=1;startSize=35;collapsible=0;recursiveResize=0;expand=0;fontStyle=1;fontSize=14;fillColor=#f8f9fa;strokeColor=#6c757d;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(b_cell, "mxGeometry", {"x": "40", "y": "40", "width": str(total_w), "height": str(total_h), "as": "geometry"})

    for n in nodes_data:
        ntype = n.get("type", "state")
        if ntype == "start":
            style = "ellipse;html=1;fillColor=#000000;strokeColor=#000000;"
        elif ntype == "end":
            style = "ellipse;html=1;shape=endState;fillColor=#000000;strokeColor=#000000;"
        elif ntype == "choice":
            style = "rhombus;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;align=center;"
        else: # state
            style = "rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;fontSize=11;fontStyle=1;align=center;"

        cell = ET.SubElement(root, "mxCell", {
            "id": n["id"], "value": n.get("value", ""),
            "style": style, "vertex": "1", "parent": "1"
        })
        ET.SubElement(cell, "mxGeometry", {
            "x": str(n["x"]), "y": str(n["y"]), "width": str(n["width"]), "height": str(n["height"]), "as": "geometry"
        })

    for e in edges_data:
        edge_cell = ET.SubElement(root, "mxCell", {
            "id": f"e_{e['source']}_{e['target']}",
            "value": e.get("label", ""),
            "style": "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=classic;strokeColor=#333333;fontSize=10;fontStyle=1;",
            "edge": "1", "parent": "1",
            "source": e["source"], "target": e["target"]
        })
        ET.SubElement(edge_cell, "mxGeometry", {"relative": "1", "as": "geometry"})

    xml_filepath = os.path.join(out_dir, f"{file_prefix}.xml")
    drawio_filepath = os.path.join(out_dir, f"{file_prefix}.drawio")
    
    formatted_xml = prettify(mxfile)
    with open(xml_filepath, "w", encoding="utf-8") as f:
        f.write(formatted_xml)
    with open(drawio_filepath, "w", encoding="utf-8") as f:
        f.write(formatted_xml)

# STD-01
create_state_diagram(
    file_prefix="state_01_candidate_assessment_lifecycle",
    diag_id="std-01", diag_title="Candidate Assessment Lifecycle",
    boundary_title="Candidate Assessment State Machine (candidate_assessments)",
    nodes_data=[
        {"id": "st_start1", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_queued", "type": "state", "value": "State: Queued\n(Waiting in Processing Queue)", "x": 200, "y": 110, "width": 220, "height": 50},
        {"id": "s_extracting", "type": "state", "value": "State: ExtractingCV\n(Parsing Document & Structure)", "x": 500, "y": 110, "width": 220, "height": 50},
        {"id": "s_auditing", "type": "state", "value": "State: AuditingRepositories\n(Analyzing Code AST & Commits)", "x": 500, "y": 230, "width": 220, "height": 50},
        {"id": "s_calculating", "type": "state", "value": "State: CalculatingTrustScore\n(Computing SkillTree & Trust Score)", "x": 500, "y": 350, "width": 220, "height": 50},
        {"id": "s_completed", "type": "state", "value": "State: Completed\n(Assessment Report Active)", "x": 200, "y": 350, "width": 220, "height": 50},
        {"id": "s_failed", "type": "state", "value": "State: Failed\n(Error Logged / Retrying Allowed)", "x": 500, "y": 470, "width": 220, "height": 50},
        {"id": "st_end1", "type": "end", "x": 100, "y": 360, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start1", "target": "s_queued", "label": "Trigger Assessment"},
        {"source": "s_queued", "target": "s_extracting", "label": "Worker Polls Job"},
        {"source": "s_extracting", "target": "s_auditing", "label": "CV Parsed Success"},
        {"source": "s_extracting", "target": "s_failed", "label": "CV Format Error"},
        {"source": "s_auditing", "target": "s_calculating", "label": "Repo Audit Success"},
        {"source": "s_auditing", "target": "s_failed", "label": "Git Audit Error"},
        {"source": "s_calculating", "target": "s_completed", "label": "Pipeline Success"},
        {"source": "s_calculating", "target": "s_failed", "label": "AI Engine Exception"},
        {"source": "s_failed", "target": "s_queued", "label": "Re-trigger / Retry"},
        {"source": "s_completed", "target": "st_end1", "label": "Finish"}
    ], total_w=800, total_h=580
)

# STD-02
create_state_diagram(
    file_prefix="state_02_repository_audit_job_lifecycle",
    diag_id="std-02", diag_title="Repository Audit Job Lifecycle",
    boundary_title="Repository Audit Job State Machine (pipeline_jobs)",
    nodes_data=[
        {"id": "st_start2", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_created2", "type": "state", "value": "State: Created\n(Record Saved in DB)", "x": 200, "y": 110, "width": 200, "height": 50},
        {"id": "s_queued2", "type": "state", "value": "State: Queued\n(Waiting for Worker)", "x": 470, "y": 110, "width": 200, "height": 50},
        {"id": "s_cloning2", "type": "state", "value": "State: CloningRepo\n(Cloning Git Repository)", "x": 740, "y": 110, "width": 200, "height": 50},
        {"id": "s_ast2", "type": "state", "value": "State: ExtractingAST\n(Parsing AST & File Stats)", "x": 740, "y": 230, "width": 200, "height": 50},
        {"id": "s_ai2", "type": "state", "value": "State: DispatchedToAI\n(Processing in CVerify.AI)", "x": 470, "y": 230, "width": 200, "height": 50},
        {"id": "s_completed2", "type": "state", "value": "State: Completed\n(Assessment Result Saved)", "x": 200, "y": 230, "width": 200, "height": 50},
        {"id": "s_failed2", "type": "state", "value": "State: Failed\n(Audit Job Error)", "x": 470, "y": 350, "width": 200, "height": 50},
        {"id": "st_end2", "type": "end", "x": 100, "y": 240, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start2", "target": "s_created2", "label": "POST /analyze"},
        {"source": "s_created2", "target": "s_queued2", "label": "DB Commit"},
        {"source": "s_queued2", "target": "s_cloning2", "label": "Worker Consume"},
        {"source": "s_cloning2", "target": "s_ast2", "label": "Clone OK"},
        {"source": "s_cloning2", "target": "s_failed2", "label": "Clone Error"},
        {"source": "s_ast2", "target": "s_ai2", "label": "AST Ready"},
        {"source": "s_ast2", "target": "s_failed2", "label": "Syntax Error"},
        {"source": "s_ai2", "target": "s_completed2", "label": "AI Result Received"},
        {"source": "s_ai2", "target": "s_failed2", "label": "AI Timeout"},
        {"source": "s_failed2", "target": "s_queued2", "label": "Retry Job"},
        {"source": "s_completed2", "target": "st_end2", "label": "Finish"}
    ], total_w=980, total_h=450
)

# STD-03
create_state_diagram(
    file_prefix="state_03_organization_verification_account_status",
    diag_id="std-03", diag_title="Organization Verification & Account Status",
    boundary_title="Organization Status State Machine (organizations)",
    nodes_data=[
        {"id": "st_start3", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_unver3", "type": "state", "value": "State: Unverified\n(Newly Created Org Account)", "x": 200, "y": 110, "width": 220, "height": 50},
        {"id": "s_pending3", "type": "state", "value": "State: PendingVerification\n(GPKD Uploaded, Awaiting Admin)", "x": 500, "y": 110, "width": 240, "height": 50},
        {"id": "s_ver3", "type": "state", "value": "State: Verified\n(Active Verified Org Badge)", "x": 500, "y": 240, "width": 240, "height": 50},
        {"id": "s_rej3", "type": "state", "value": "State: Rejected\n(Invalid GPKD Documents)", "x": 200, "y": 240, "width": 220, "height": 50},
        {"id": "s_susp3", "type": "state", "value": "State: Suspended\n(Account Locked by Super Admin)", "x": 500, "y": 360, "width": 240, "height": 50},
        {"id": "st_end3", "type": "end", "x": 100, "y": 250, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start3", "target": "s_unver3", "label": "Create Org"},
        {"source": "s_unver3", "target": "s_pending3", "label": "Submit GPKD"},
        {"source": "s_pending3", "target": "s_ver3", "label": "Admin Approve"},
        {"source": "s_pending3", "target": "s_rej3", "label": "Admin Reject"},
        {"source": "s_rej3", "target": "s_pending3", "label": "Re-submit Docs"},
        {"source": "s_ver3", "target": "s_susp3", "label": "Policy Violation"},
        {"source": "s_susp3", "target": "s_ver3", "label": "Unlock Account"},
        {"source": "s_ver3", "target": "st_end3", "label": "Active Org"}
    ], total_w=780, total_h=460
)

# STD-04
create_state_diagram(
    file_prefix="state_04_org_recovery_claim_dispute",
    diag_id="std-04", diag_title="Org Recovery Claim & Dispute Lifecycle",
    boundary_title="Org Recovery Claim State Machine (organization_recovery_claims)",
    nodes_data=[
        {"id": "st_start4", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_init4", "type": "state", "value": "State: Initiated\n(L2 Claim Requested)", "x": 200, "y": 110, "width": 200, "height": 50},
        {"id": "s_evidence4", "type": "state", "value": "State: EvidenceUploaded\n(Legal Docs Uploaded)", "x": 470, "y": 110, "width": 200, "height": 50},
        {"id": "s_voting4", "type": "state", "value": "State: VotingActive\n(Member Voting Period 7-Days)", "x": 740, "y": 110, "width": 200, "height": 50},
        {"id": "s_review4", "type": "state", "value": "State: UnderSuperAdminReview\n(Awaiting Admin Decision)", "x": 740, "y": 240, "width": 200, "height": 50},
        {"id": "s_approved4", "type": "state", "value": "State: Approved\n(Ownership Transferred)", "x": 470, "y": 240, "width": 200, "height": 50},
        {"id": "s_rejected4", "type": "state", "value": "State: Rejected\n(Claim Refused)", "x": 470, "y": 360, "width": 200, "height": 50},
        {"id": "s_expired4", "type": "state", "value": "State: Expired\n(No Vote Consensus)", "x": 740, "y": 360, "width": 200, "height": 50},
        {"id": "st_end4", "type": "end", "x": 200, "y": 250, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start4", "target": "s_init4", "label": "Initiate Claim"},
        {"source": "s_init4", "target": "s_evidence4", "label": "Upload Docs"},
        {"source": "s_evidence4", "target": "s_voting4", "label": "Dispatch Vote Invitations"},
        {"source": "s_voting4", "target": "s_review4", "label": "Voting Ends / Consensus"},
        {"source": "s_voting4", "target": "s_expired4", "label": "Voting Expired"},
        {"source": "s_review4", "target": "s_approved4", "label": "Admin Approve"},
        {"source": "s_review4", "target": "s_rejected4", "label": "Admin Reject"},
        {"source": "s_approved4", "target": "st_end4", "label": "Reclaim Success"},
        {"source": "s_rejected4", "target": "st_end4", "label": "Finish"},
        {"source": "s_expired4", "target": "st_end4", "label": "Finish"}
    ], total_w=980, total_h=460
)

# STD-05
create_state_diagram(
    file_prefix="state_05_job_vacancy_lifecycle",
    diag_id="std-05", diag_title="Job Vacancy Lifecycle",
    boundary_title="Job Vacancy State Machine (job_vacancies)",
    nodes_data=[
        {"id": "st_start5", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_draft5", "type": "state", "value": "State: Draft\n(Raw JD Soạn thảo thô)", "x": 200, "y": 110, "width": 200, "height": 50},
        {"id": "s_req5", "type": "state", "value": "State: RequirementConfigured\n(AI JD Parsed & Configured)", "x": 470, "y": 110, "width": 220, "height": 50},
        {"id": "s_snap5", "type": "state", "value": "State: SnapshotCreated\n(Requirement Snapshot Locked)", "x": 740, "y": 110, "width": 220, "height": 50},
        {"id": "s_pub5", "type": "state", "value": "State: Published\n(Active on Job Board /jobs)", "x": 740, "y": 240, "width": 220, "height": 50},
        {"id": "s_closed5", "type": "state", "value": "State: Closed\n(Applications Closed)", "x": 470, "y": 240, "width": 220, "height": 50},
        {"id": "s_arch5", "type": "state", "value": "State: Archived\n(Hidden from Public Portal)", "x": 200, "y": 240, "width": 200, "height": 50},
        {"id": "st_end5", "type": "end", "x": 100, "y": 250, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start5", "target": "s_draft5", "label": "Create Entry"},
        {"source": "s_draft5", "target": "s_req5", "label": "AI Parse & Save"},
        {"source": "s_req5", "target": "s_snap5", "label": "Lock Snapshot"},
        {"source": "s_snap5", "target": "s_pub5", "label": "Publish Job"},
        {"source": "s_pub5", "target": "s_closed5", "label": "Close Application"},
        {"source": "s_closed5", "target": "s_pub5", "label": "Re-open Job"},
        {"source": "s_closed5", "target": "s_arch5", "label": "Archive Job"},
        {"source": "s_arch5", "target": "st_end5", "label": "Finish"}
    ], total_w=980, total_h=350
)

# STD-06
create_state_diagram(
    file_prefix="state_06_job_application_pipeline_lifecycle",
    diag_id="std-06", diag_title="Job Application Pipeline Lifecycle",
    boundary_title="Job Application Pipeline State Machine (job_applications)",
    nodes_data=[
        {"id": "st_start6", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_sub6", "type": "state", "value": "State: Submitted\n(Application Received)", "x": 200, "y": 110, "width": 200, "height": 50},
        {"id": "s_scoring6", "type": "state", "value": "State: AIScoring\n(Calculating FitScore %)", "x": 470, "y": 110, "width": 200, "height": 50},
        {"id": "s_screened6", "type": "state", "value": "State: Screened\n(Passed Screening Stage)", "x": 740, "y": 110, "width": 200, "height": 50},
        {"id": "s_interview6", "type": "state", "value": "State: InterviewScheduled\n(Interview Round Active)", "x": 740, "y": 240, "width": 200, "height": 50},
        {"id": "s_offered6", "type": "state", "value": "State: Offered\n(Job Offer Extended)", "x": 470, "y": 240, "width": 200, "height": 50},
        {"id": "s_rejected6", "type": "state", "value": "State: Rejected\n(Application Refused)", "x": 470, "y": 360, "width": 200, "height": 50},
        {"id": "s_withdrawn6", "type": "state", "value": "State: Withdrawn\n(Candidate Retracted)", "x": 200, "y": 360, "width": 200, "height": 50},
        {"id": "st_end6", "type": "end", "x": 200, "y": 250, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start6", "target": "s_sub6", "label": "Apply Job"},
        {"source": "s_sub6", "target": "s_scoring6", "label": "Auto Match"},
        {"source": "s_scoring6", "target": "s_screened6", "label": "FitScore OK"},
        {"source": "s_screened6", "target": "s_interview6", "label": "Schedule Interview"},
        {"source": "s_interview6", "target": "s_offered6", "label": "Pass Interview"},
        {"source": "s_interview6", "target": "s_rejected6", "label": "Fail Interview"},
        {"source": "s_screened6", "target": "s_rejected6", "label": "Reject Candidate"},
        {"source": "s_sub6", "target": "s_withdrawn6", "label": "Candidate Withdraw"},
        {"source": "s_offered6", "target": "st_end6", "label": "Hire Success"},
        {"source": "s_rejected6", "target": "st_end6", "label": "Finish"},
        {"source": "s_withdrawn6", "target": "st_end6", "label": "Finish"}
    ], total_w=980, total_h=460
)

# STD-07
create_state_diagram(
    file_prefix="state_07_community_forum_moderation",
    diag_id="std-07", diag_title="Community Forum Content Moderation Lifecycle",
    boundary_title="Forum Topic Content State Machine (forum_topics)",
    nodes_data=[
        {"id": "st_start7", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_active7", "type": "state", "value": "State: Active\n(Topic Published on Forum)", "x": 200, "y": 110, "width": 220, "height": 50},
        {"id": "s_reported7", "type": "state", "value": "State: Reported\n(Flagged by Community Users)", "x": 500, "y": 110, "width": 220, "height": 50},
        {"id": "s_queue7", "type": "state", "value": "State: InModerationQueue\n(Under Review by Moderator)", "x": 500, "y": 230, "width": 220, "height": 50},
        {"id": "s_approved7", "type": "state", "value": "State: Approved\n(Keep & Dismiss Reports)", "x": 200, "y": 230, "width": 220, "height": 50},
        {"id": "s_removed7", "type": "state", "value": "State: Removed\n(Soft-Deleted for Violation)", "x": 500, "y": 350, "width": 220, "height": 50},
        {"id": "st_end7", "type": "end", "x": 100, "y": 240, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start7", "target": "s_active7", "label": "Post Topic"},
        {"source": "s_active7", "target": "s_reported7", "label": "Report Flagged"},
        {"source": "s_reported7", "target": "s_queue7", "label": "Push to Queue"},
        {"source": "s_queue7", "target": "s_approved7", "label": "Mod Approve"},
        {"source": "s_queue7", "target": "s_removed7", "label": "Mod Delete"},
        {"source": "s_approved7", "target": "s_active7", "label": "Restore Active"},
        {"source": "s_removed7", "target": "st_end7", "label": "Finish"}
    ], total_w=780, total_h=450
)

# STD-08
create_state_diagram(
    file_prefix="state_08_outbox_transactional_message",
    diag_id="std-08", diag_title="Outbox Transactional Message Lifecycle",
    boundary_title="Outbox Message State Machine (outbox_messages)",
    nodes_data=[
        {"id": "st_start8", "type": "start", "x": 100, "y": 120, "width": 30, "height": 30},
        {"id": "s_pending8", "type": "state", "value": "State: Pending\n(Saved in DB Transaction)", "x": 200, "y": 110, "width": 220, "height": 50},
        {"id": "s_locked8", "type": "state", "value": "State: Locked\n(Acquired by Worker Lock)", "x": 500, "y": 110, "width": 220, "height": 50},
        {"id": "s_sent8", "type": "state", "value": "State: Sent\n(Email Delivered Successfully)", "x": 500, "y": 230, "width": 220, "height": 50},
        {"id": "s_failed8", "type": "state", "value": "State: Failed\n(Gateway Error / Retrying)", "x": 200, "y": 230, "width": 220, "height": 50},
        {"id": "s_dead8", "type": "state", "value": "State: DeadLetter\n(Max Retries Exceeded = 5)", "x": 200, "y": 350, "width": 220, "height": 50},
        {"id": "st_end8", "type": "end", "x": 500, "y": 360, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "st_start8", "target": "s_pending8", "label": "Insert Outbox"},
        {"source": "s_pending8", "target": "s_locked8", "label": "Worker Poll"},
        {"source": "s_locked8", "target": "s_sent8", "label": "SMTP 200 OK"},
        {"source": "s_locked8", "target": "s_failed8", "label": "SMTP Error"},
        {"source": "s_failed8", "target": "s_pending8", "label": "Retries < 5"},
        {"source": "s_failed8", "target": "s_dead8", "label": "Retries >= 5"},
        {"source": "s_sent8", "target": "st_end8", "label": "Finish"},
        {"source": "s_dead8", "target": "st_end8", "label": "Alert Admin"}
    ], total_w=780, total_h=450
)

print("SUCCESS: Generated ALL 8 State Machine Diagrams in XML & DRAWIO format!")
