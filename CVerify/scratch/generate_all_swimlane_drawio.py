import xml.etree.ElementTree as ET
import xml.dom.minidom as minidom
import os

out_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify\drawio_diagrams"
os.makedirs(out_dir, exist_ok=True)

def prettify(elem):
    xml_str = ET.tostring(elem, encoding="utf-8")
    dom = minidom.parseString(xml_str)
    return dom.toprettyxml(indent="  ", encoding="utf-8").decode("utf-8")

def create_swimlane_diagram(file_prefix, diag_id, diag_title, header_title, lanes_data, nodes_data, edges_data, total_w=950, total_h=1300):
    mxfile = ET.Element("mxfile", {"host": "app.diagrams.net", "agent": "CVerify", "version": "21.0.0"})
    diag = ET.SubElement(mxfile, "diagram", {"id": diag_id, "name": diag_title})
    model = ET.SubElement(diag, "mxGraphModel", {
        "dx": "1400", "dy": "900", "grid": "1", "gridSize": "10", "guides": "1",
        "tooltips": "1", "connect": "1", "arrows": "1", "fold": "1", "page": "1",
        "pageScale": "1", "pageWidth": str(total_w + 100), "pageHeight": str(total_h + 100), "math": "0", "shadow": "0"
    })
    root = ET.SubElement(model, "root")
    ET.SubElement(root, "mxCell", {"id": "0"})
    ET.SubElement(root, "mxCell", {"id": "1", "parent": "0"})

    # Header / Swimlane Container
    header = ET.SubElement(root, "mxCell", {
        "id": f"main_container_{diag_id}",
        "value": header_title,
        "style": "shape=swimlane;html=1;childLayout=stackLayout;startSize=30;horizontal=1;horizontalStack=1;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=14;collapsible=0;expand=0;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(header, "mxGeometry", {"x": "40", "y": "40", "width": str(total_w), "height": str(total_h), "as": "geometry"})

    # Lanes
    cur_x = 0
    for lane in lanes_data:
        l_cell = ET.SubElement(root, "mxCell", {
            "id": lane["id"], "value": lane["name"],
            "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
            "vertex": "1", "parent": f"main_container_{diag_id}"
        })
        ET.SubElement(l_cell, "mxGeometry", {"x": str(cur_x), "y": "30", "width": str(lane["width"]), "height": str(total_h - 30), "as": "geometry"})
        cur_x += lane["width"]

    # Nodes
    for n in nodes_data:
        ntype = n.get("type", "activity")
        if ntype == "start":
            style = "ellipse;html=1;fillColor=#000000;strokeColor=#000000;"
        elif ntype == "end":
            style = "ellipse;html=1;shape=endState;fillColor=#000000;strokeColor=#000000;"
        elif ntype == "decision":
            style = "rhombus;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;align=center;"
        elif ntype == "fork" or ntype == "join":
            style = "line;strokeWidth=4;html=1;fillColor=#000000;strokeColor=#000000;"
        else: # activity
            style = "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;align=center;"

        cell = ET.SubElement(root, "mxCell", {
            "id": n["id"], "value": n.get("value", ""),
            "style": style, "vertex": "1", "parent": "1"
        })
        ET.SubElement(cell, "mxGeometry", {
            "x": str(n["x"]), "y": str(n["y"]), "width": str(n["width"]), "height": str(n["height"]), "as": "geometry"
        })

    # Edges
    for e in edges_data:
        edge_cell = ET.SubElement(root, "mxCell", {
            "id": f"e_{e['source']}_{e['target']}",
            "value": e.get("label", ""),
            "style": "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=classic;strokeColor=#000000;fontSize=10;",
            "edge": "1", "parent": "1",
            "source": e["source"], "target": e["target"]
        })
        ET.SubElement(edge_cell, "mxGeometry", {"relative": "1", "as": "geometry"})

    # Output files
    xml_filepath = os.path.join(out_dir, f"{file_prefix}.xml")
    drawio_filepath = os.path.join(out_dir, f"{file_prefix}.drawio")
    
    formatted_xml = prettify(mxfile)
    with open(xml_filepath, "w", encoding="utf-8") as f:
        f.write(formatted_xml)
    with open(drawio_filepath, "w", encoding="utf-8") as f:
        f.write(formatted_xml)

# ==============================================================================
# SL-02: Organization Level-2 Recovery & Dispute Voting Flow
# ==============================================================================
create_swimlane_diagram(
    file_prefix="swimlane_02_account_org_level2_recovery_dispute",
    diag_id="sl-02",
    diag_title="Org Level-2 Recovery & Dispute Voting",
    header_title="Organization — Level-2 Recovery &amp; Dispute Voting Flow",
    lanes_data=[
        {"id": "l_claimant", "name": "Claimant (User)", "width": 300},
        {"id": "l_members", "name": "Org Members", "width": 300},
        {"id": "l_admin", "name": "Super Admin", "width": 300},
        {"id": "l_sys2", "name": "CVerify System Core", "width": 300}
    ],
    nodes_data=[
        {"id": "n_start2", "type": "start", "x": 175, "y": 90, "width": 30, "height": 30},
        {"id": "a_req_l2", "type": "activity", "value": "Yêu cầu Tranh chấp Khôi phục L2\n(Initiate Org Recovery L2)", "x": 70, "y": 150, "width": 240, "height": 50},
        {"id": "a_up_evid", "type": "activity", "value": "Tải lên Bằng chứng Pháp lý (Giấy ĐKKD, Con dấu)", "x": 70, "y": 240, "width": 240, "height": 50},
        {"id": "a_create_claim", "type": "activity", "value": "Tạo bản ghi Claim PENDING & Phát Event Bỏ phiếu", "x": 970, "y": 240, "width": 240, "height": 50},
        {"id": "a_notify_mems", "type": "activity", "value": "Gửi Mail Thông báo Tranh chấp cho Thành viên", "x": 970, "y": 330, "width": 240, "height": 50},
        {"id": "a_vote", "type": "activity", "value": "Truy cập Màn hình Bỏ phiếu\n(Vote Approve / Reject Claim)", "x": 370, "y": 420, "width": 240, "height": 50},
        {"id": "a_count_vote", "type": "activity", "value": "Tổng hợp Tỷ lệ Đồng thuận Bỏ phiếu", "x": 970, "y": 420, "width": 240, "height": 50},
        {"id": "a_admin_review", "type": "activity", "value": "Super Admin Thẩm định Hồ sơ Tranh chấp & Vote Result", "x": 670, "y": 520, "width": 240, "height": 50},
        {"id": "d_admin_approve", "type": "decision", "value": "Phê duyệt Claim?", "x": 730, "y": 610, "width": 120, "height": 60},
        {"id": "a_reclaim_master", "type": "activity", "value": "Chuyển giao Quyền Master Admin cho Claimant", "x": 970, "y": 700, "width": 240, "height": 50},
        {"id": "a_reject_claim", "type": "activity", "value": "Từ chối Yêu cầu & Khóa Tài khoản Báo cáo Sai", "x": 970, "y": 780, "width": 240, "height": 50},
        {"id": "a_track_res", "type": "activity", "value": "Nhận Thông báo Kết quả Khôi phục Doanh nghiệp", "x": 70, "y": 860, "width": 240, "height": 50},
        {"id": "n_end2", "type": "end", "x": 175, "y": 960, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "n_start2", "target": "a_req_l2"},
        {"source": "a_req_l2", "target": "a_up_evid"},
        {"source": "a_up_evid", "target": "a_create_claim"},
        {"source": "a_create_claim", "target": "a_notify_mems"},
        {"source": "a_notify_mems", "target": "a_vote"},
        {"source": "a_vote", "target": "a_count_vote"},
        {"source": "a_count_vote", "target": "a_admin_review"},
        {"source": "a_admin_review", "target": "d_admin_approve"},
        {"source": "d_admin_approve", "target": "a_reclaim_master", "label": "Approve"},
        {"source": "d_admin_approve", "target": "a_reject_claim", "label": "Reject"},
        {"source": "a_reclaim_master", "target": "a_track_res"},
        {"source": "a_reject_claim", "target": "a_track_res"},
        {"source": "a_track_res", "target": "n_end2"}
    ],
    total_w=1200, total_h=1050
)

# ==============================================================================
# SL-05: Candidate AI Assessment & Skill Tree Pipeline Flow
# ==============================================================================
create_swimlane_diagram(
    file_prefix="swimlane_05_candidate_ai_assessment_skill_tree",
    diag_id="sl-05",
    diag_title="Candidate AI Assessment & Skill Tree Pipeline",
    header_title="Candidate — AI Assessment Pipeline &amp; Skill Tree Flow",
    lanes_data=[
        {"id": "l_cand5", "name": "Candidate", "width": 280},
        {"id": "l_core5", "name": "Assessment Controller (.NET)", "width": 280},
        {"id": "l_ai5", "name": "CVerify.AI Engine (Python)", "width": 300},
        {"id": "l_db5", "name": "PostgreSQL Database", "width": 280}
    ],
    nodes_data=[
        {"id": "n_start5", "type": "start", "x": 165, "y": 90, "width": 30, "height": 30},
        {"id": "a_trigger_assess", "type": "activity", "value": "Yêu cầu Đánh giá AI Tổng thể (Queue Assessment)", "x": 70, "y": 150, "width": 220, "height": 50},
        {"id": "a_create_assess_rec", "type": "activity", "value": "Tạo Candidate Assessment Record (Processing)", "x": 350, "y": 150, "width": 220, "height": 50},
        {"id": "a_fetch_data", "type": "activity", "value": "Fetch Profile, CV & Repo Audit Data", "x": 930, "y": 150, "width": 220, "height": 50},
        {"id": "a_run_skill_tree", "type": "activity", "value": "Giai đoạn 1: Build Interactive Skill Tree Matrix", "x": 630, "y": 260, "width": 240, "height": 50},
        {"id": "a_calc_trust", "type": "activity", "value": "Giai đoạn 2: Compute Profile Trust Score (0-100%)", "x": 630, "y": 350, "width": 240, "height": 50},
        {"id": "a_fit_roles", "type": "activity", "value": "Giai đoạn 3: Analyze Strengths & Best-Fit Roles", "x": 630, "y": 440, "width": 240, "height": 50},
        {"id": "a_save_assess", "type": "activity", "value": "Save Assessment Report & Capabilities JSON", "x": 930, "y": 530, "width": 220, "height": 50},
        {"id": "a_view_dashboard", "type": "activity", "value": "Hiển thị Cây kỹ năng 3D & Báo cáo Audit Năng lực", "x": 70, "y": 630, "width": 220, "height": 50},
        {"id": "n_end5", "type": "end", "x": 165, "y": 730, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "n_start5", "target": "a_trigger_assess"},
        {"source": "a_trigger_assess", "target": "a_create_assess_rec"},
        {"source": "a_create_assess_rec", "target": "a_fetch_data"},
        {"source": "a_fetch_data", "target": "a_run_skill_tree"},
        {"source": "a_run_skill_tree", "target": "a_calc_trust"},
        {"source": "a_calc_trust", "target": "a_fit_roles"},
        {"source": "a_fit_roles", "target": "a_save_assess"},
        {"source": "a_save_assess", "target": "a_view_dashboard"},
        {"source": "a_view_dashboard", "target": "n_end5"}
    ],
    total_w=1140, total_h=820
)

# ==============================================================================
# SL-07: Job Vacancy, AI Requirement & Publishing Flow
# ==============================================================================
create_swimlane_diagram(
    file_prefix="swimlane_07_job_vacancy_ai_jd_parser_publishing",
    diag_id="sl-07",
    diag_title="Job Vacancy, AI Requirement & Publishing",
    header_title="Recruitment — Job Vacancy, AI Requirement &amp; Publishing Flow",
    lanes_data=[
        {"id": "l_emp7", "name": "Employer / Recruiter", "width": 280},
        {"id": "l_job_api7", "name": "Job Vacancy API (.NET)", "width": 280},
        {"id": "l_parser7", "name": "CVerify.AI JD Parser Engine", "width": 300},
        {"id": "l_portal7", "name": "Public Job Board", "width": 280}
    ],
    nodes_data=[
        {"id": "n_start7", "type": "start", "x": 165, "y": 90, "width": 30, "height": 30},
        {"id": "a_paste_jd", "type": "activity", "value": "Nhập tiêu đề & Dán văn bản Mô tả công việc (JD thô)", "x": 70, "y": 150, "width": 220, "height": 50},
        {"id": "a_req_parse", "type": "activity", "value": "Gửi JD text yêu cầu trích xuất tiêu chuẩn", "x": 350, "y": 150, "width": 220, "height": 50},
        {"id": "a_ai_parse_jd", "type": "activity", "value": "AI bóc tách Skill Must-have, Nice-to-have\n& Khung tiêu chuẩn chấm điểm", "x": 630, "y": 250, "width": 240, "height": 55},
        {"id": "a_review_req", "type": "activity", "value": "Xem phác thảo & Tinh chỉnh trọng số kỹ năng", "x": 70, "y": 350, "width": 220, "height": 50},
        {"id": "a_click_pub", "type": "activity", "value": "Nhấn Save & Publish Job Vacancy", "x": 70, "y": 440, "width": 220, "height": 50},
        {"id": "a_create_snapshot", "type": "activity", "value": "Khóa Requirement Snapshot & Tạo Job Record", "x": 350, "y": 440, "width": 220, "height": 50},
        {"id": "a_pub_portal", "type": "activity", "value": "Xuất bản tin tuyển dụng lên Public Job Board (/jobs)", "x": 930, "y": 540, "width": 220, "height": 50},
        {"id": "n_end7", "type": "end", "x": 1025, "y": 640, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "n_start7", "target": "a_paste_jd"},
        {"source": "a_paste_jd", "target": "a_req_parse"},
        {"source": "a_req_parse", "target": "a_ai_parse_jd"},
        {"source": "a_ai_parse_jd", "target": "a_review_req"},
        {"source": "a_review_req", "target": "a_click_pub"},
        {"source": "a_click_pub", "target": "a_create_snapshot"},
        {"source": "a_create_snapshot", "target": "a_pub_portal"},
        {"source": "a_pub_portal", "target": "n_end7"}
    ],
    total_w=1140, total_h=720
)

# ==============================================================================
# SL-08: Candidate Application & AI Job-Fit Ranking Flow
# ==============================================================================
create_swimlane_diagram(
    file_prefix="swimlane_08_candidate_application_ai_fit_ranking",
    diag_id="sl-08",
    diag_title="Candidate Application & AI Job-Fit Ranking",
    header_title="Recruitment — Candidate Application &amp; AI Job-Fit Ranking Flow",
    lanes_data=[
        {"id": "l_cand8", "name": "Candidate", "width": 280},
        {"id": "l_rec8", "name": "Recruiter / Employer", "width": 280},
        {"id": "l_app_api8", "name": "Application API (.NET)", "width": 280},
        {"id": "l_match8", "name": "CVerify.AI Matching Engine", "width": 300}
    ],
    nodes_data=[
        {"id": "n_start8", "type": "start", "x": 165, "y": 90, "width": 30, "height": 30},
        {"id": "a_apply_job", "type": "activity", "value": "Nhấn Apply & Nộp Hồ sơ CV ứng tuyển Job", "x": 70, "y": 150, "width": 220, "height": 50},
        {"id": "a_save_app", "type": "activity", "value": "INSERT job_applications (Status = Submitted)", "x": 630, "y": 150, "width": 220, "height": 50},
        {"id": "a_run_matching", "type": "activity", "value": "Tính toán Vector Cosine Similarity & Domain Weight", "x": 930, "y": 250, "width": 240, "height": 50},
        {"id": "a_gen_factors", "type": "activity", "value": "Sinh FitScore (%) & Giải thích Match Explanation JSON", "x": 930, "y": 340, "width": 240, "height": 50},
        {"id": "a_save_eval", "type": "activity", "value": "INSERT matching_evaluations record", "x": 630, "y": 430, "width": 220, "height": 50},
        {"id": "a_view_pipeline", "type": "activity", "value": "Mở Recruitment Pipeline Dashboard", "x": 350, "y": 520, "width": 220, "height": 50},
        {"id": "a_rank_apps", "type": "activity", "value": "Truy vấn danh sách Ứng viên ORDER BY FitScore DESC", "x": 630, "y": 520, "width": 220, "height": 50},
        {"id": "a_review_ranked", "type": "activity", "value": "Xem danh sách Ứng viên đã xếp hạng AI & Lý do Fit", "x": 350, "y": 620, "width": 220, "height": 50},
        {"id": "n_end8", "type": "end", "x": 445, "y": 720, "width": 30, "height": 30}
    ],
    edges_data=[
        {"source": "n_start8", "target": "a_apply_job"},
        {"source": "a_apply_job", "target": "a_save_app"},
        {"source": "a_save_app", "target": "a_run_matching"},
        {"source": "a_run_matching", "target": "a_gen_factors"},
        {"source": "a_gen_factors", "target": "a_save_eval"},
        {"source": "a_save_eval", "target": "a_view_pipeline"},
        {"source": "a_view_pipeline", "target": "a_rank_apps"},
        {"source": "a_rank_apps", "target": "a_review_ranked"},
        {"source": "a_review_ranked", "target": "n_end8"}
    ],
    total_w=1140, total_h=800
)

print("SUCCESS: All Swimlane Activity Diagram XML & DRAWIO files created!")
