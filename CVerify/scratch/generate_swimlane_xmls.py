import xml.etree.ElementTree as ET
import xml.dom.minidom as minidom
import os

out_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify\drawio_diagrams"
os.makedirs(out_dir, exist_ok=True)

def prettify(elem):
    xml_str = ET.tostring(elem, encoding="utf-8")
    dom = minidom.parseString(xml_str)
    return dom.toprettyxml(indent="  ", encoding="utf-8").decode("utf-8")

# ==============================================================================
# SL-01: Business — Registration & Verification (Exact match to User's Image)
# ==============================================================================
def create_sl01_xml():
    mxfile = ET.Element("mxfile", {"host": "app.diagrams.net", "agent": "CVerify", "version": "21.0.0"})
    diag = ET.SubElement(mxfile, "diagram", {"id": "sl-01", "name": "Business — Registration & Verification"})
    model = ET.SubElement(diag, "mxGraphModel", {
        "dx": "1400", "dy": "900", "grid": "1", "gridSize": "10", "guides": "1",
        "tooltips": "1", "connect": "1", "arrows": "1", "fold": "1", "page": "1",
        "pageScale": "1", "pageWidth": "1000", "pageHeight": "1400", "math": "0", "shadow": "0"
    })
    root = ET.SubElement(model, "root")
    ET.SubElement(root, "mxCell", {"id": "0"})
    ET.SubElement(root, "mxCell", {"id": "1", "parent": "0"})

    # Header / Main Swimlane Container
    header = ET.SubElement(root, "mxCell", {
        "id": "main_container",
        "value": "Business — registration &amp; verification",
        "style": "shape=swimlane;html=1;childLayout=stackLayout;startSize=30;horizontal=1;horizontalStack=1;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=14;collapsible=0;expand=0;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(header, "mxGeometry", {"x": "40", "y": "40", "width": "920", "height": "1300", "as": "geometry"})

    # Lanes
    lane1 = ET.SubElement(root, "mxCell", {
        "id": "lane_biz", "value": "Business (Org Admin)",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container"
    })
    ET.SubElement(lane1, "mxGeometry", {"x": "0", "y": "30", "width": "300", "height": "1270", "as": "geometry"})

    lane2 = ET.SubElement(root, "mxCell", {
        "id": "lane_admin", "value": "Super Admin",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container"
    })
    ET.SubElement(lane2, "mxGeometry", {"x": "300", "y": "30", "width": "300", "height": "1270", "as": "geometry"})

    lane3 = ET.SubElement(root, "mxCell", {
        "id": "lane_sys", "value": "CVerify System + AI",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container"
    })
    ET.SubElement(lane3, "mxGeometry", {"x": "600", "y": "30", "width": "320", "height": "1270", "as": "geometry"})

    # --- Nodes inside Lane 1 (Business Admin) ---
    start = ET.SubElement(root, "mxCell", {
        "id": "node_start", "value": "",
        "style": "ellipse;html=1;fillColor=#000000;strokeColor=#000000;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(start, "mxGeometry", {"x": "175", "y": "90", "width": "30", "height": "30", "as": "geometry"})

    act1 = ET.SubElement(root, "mxCell", {
        "id": "act_reg", "value": "Đăng ký: Company + Tax code (MST)\n+ Step 1 đồng ý ToS",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act1, "mxGeometry", {"x": "70", "y": "150", "width": "240", "height": "50", "as": "geometry"})

    act_req_tos = ET.SubElement(root, "mxCell", {
        "id": "act_req_tos", "value": "Yêu cầu đồng ý điều khoản",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_req_tos, "mxGeometry", {"x": "90", "y": "270", "width": "200", "height": "45", "as": "geometry"})

    act_err_mst = ET.SubElement(root, "mxCell", {
        "id": "act_err_mst", "value": "Báo lỗi MST - nhập lại",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_err_mst, "mxGeometry", {"x": "90", "y": "430", "width": "200", "height": "45", "as": "geometry"})

    act_up_gpkd = ET.SubElement(root, "mxCell", {
        "id": "act_up_gpkd", "value": "Up Giấy ĐKKD (GPKD)",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_up_gpkd, "mxGeometry", {"x": "90", "y": "590", "width": "200", "height": "45", "as": "geometry"})

    act_login = ET.SubElement(root, "mxCell", {
        "id": "act_login", "value": "Đăng nhập (Pass/Magic/OTP/MFA) — BR-07",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_login, "mxGeometry", {"x": "70", "y": "900", "width": "240", "height": "50", "as": "geometry"})

    act_create_ws = ET.SubElement(root, "mxCell", {
        "id": "act_create_ws", "value": "Tạo Workspace (Phòng ban/Dự án, BR-10)",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_create_ws, "mxGeometry", {"x": "70", "y": "980", "width": "240", "height": "50", "as": "geometry"})

    act_invite = ET.SubElement(root, "mxCell", {
        "id": "act_invite", "value": "Mời đối tác / Member / Phân quyền (OAuth)",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_invite, "mxGeometry", {"x": "70", "y": "1060", "width": "240", "height": "50", "as": "geometry"})

    act_pub_job = ET.SubElement(root, "mxCell", {
        "id": "act_pub_job", "value": "Publish JD lên Job Board (BR-17)",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_pub_job, "mxGeometry", {"x": "70", "y": "1140", "width": "240", "height": "50", "as": "geometry"})

    end_node = ET.SubElement(root, "mxCell", {
        "id": "node_end", "value": "",
        "style": "ellipse;html=1;shape=endState;fillColor=#000000;strokeColor=#000000;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(end_node, "mxGeometry", {"x": "175", "y": "1230", "width": "30", "height": "30", "as": "geometry"})

    # --- Nodes inside Lane 2 (Super Admin) ---
    act_review = ET.SubElement(root, "mxCell", {
        "id": "act_review", "value": "Review HSKD và GPKD, Giấy UQ / Mã ĐK",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_review, "mxGeometry", {"x": "370", "y": "660", "width": "240", "height": "50", "as": "geometry"})

    dec_approve = ET.SubElement(root, "mxCell", {
        "id": "dec_approve", "value": "Duyệt HSKD?",
        "style": "rhombus;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(dec_approve, "mxGeometry", {"x": "430", "y": "740", "width": "120", "height": "60", "as": "geometry"})

    act_set_tier2 = ET.SubElement(root, "mxCell", {
        "id": "act_set_tier2", "value": "Set Tier -> ACTIVE",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_set_tier2, "mxGeometry", {"x": "390", "y": "830", "width": "200", "height": "45", "as": "geometry"})

    # --- Nodes inside Lane 3 (CVerify System + AI) ---
    dec_tos = ET.SubElement(root, "mxCell", {
        "id": "dec_tos", "value": "Đồng ý ToS & Privacy Policy?",
        "style": "rhombus;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=10;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(dec_tos, "mxGeometry", {"x": "700", "y": "150", "width": "160", "height": "70", "as": "geometry"})

    act_check_mst = ET.SubElement(root, "mxCell", {
        "id": "act_check_mst", "value": "Auto check MST (Tax API)",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_check_mst, "mxGeometry", {"x": "680", "y": "260", "width": "200", "height": "45", "as": "geometry"})

    dec_mst = ET.SubElement(root, "mxCell", {
        "id": "dec_mst", "value": "MST Hợp lệ?",
        "style": "rhombus;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(dec_mst, "mxGeometry", {"x": "720", "y": "330", "width": "120", "height": "60", "as": "geometry"})

    act_create_pending = ET.SubElement(root, "mxCell", {
        "id": "act_create_pending", "value": "Tạo Org account PENDING, hash Password & Gửi OTP/Magic",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_create_pending, "mxGeometry", {"x": "660", "y": "420", "width": "240", "height": "55", "as": "geometry"})

    dec_tier = ET.SubElement(root, "mxCell", {
        "id": "dec_tier", "value": "Check Tier?",
        "style": "rhombus;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(dec_tier, "mxGeometry", {"x": "720", "y": "500", "width": "120", "height": "60", "as": "geometry"})

    act_set_tier1 = ET.SubElement(root, "mxCell", {
        "id": "act_set_tier1", "value": "Set Member Tier 1",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_set_tier1, "mxGeometry", {"x": "680", "y": "590", "width": "200", "height": "45", "as": "geometry"})

    act_email_rej = ET.SubElement(root, "mxCell", {
        "id": "act_email_rej", "value": "Email lý do -> QTK PENDING",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_email_rej, "mxGeometry", {"x": "680", "y": "745", "width": "200", "height": "45", "as": "geometry"})

    act_notify = ET.SubElement(root, "mxCell", {
        "id": "act_notify", "value": "Notify + Welcome email (Mật khẩu, Mã kích hoạt)",
        "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(act_notify, "mxGeometry", {"x": "660", "y": "830", "width": "240", "height": "50", "as": "geometry"})

    # Horizontal Join / Fork Bars
    fork1 = ET.SubElement(root, "mxCell", {
        "id": "fork1", "value": "",
        "style": "line;strokeWidth=4;html=1;fillColor=#000000;strokeColor=#000000;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(fork1, "mxGeometry", {"x": "200", "y": "645", "width": "400", "height": "10", "as": "geometry"})

    join1 = ET.SubElement(root, "mxCell", {
        "id": "join1", "value": "",
        "style": "line;strokeWidth=4;html=1;fillColor=#000000;strokeColor=#000000;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(join1, "mxGeometry", {"x": "400", "y": "890", "width": "450", "height": "10", "as": "geometry"})

    # --- Edges / Connectors ---
    def add_edge(src, tgt, label="", style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=classic;strokeColor=#000000;fontSize=10;"):
        e = ET.SubElement(root, "mxCell", {
            "id": f"e_{src}_{tgt}", "value": label, "style": style,
            "edge": "1", "parent": "1", "source": src, "target": tgt
        })
        ET.SubElement(e, "mxGeometry", {"relative": "1", "as": "geometry"})

    add_edge("node_start", "act_reg")
    add_edge("act_reg", "dec_tos")
    add_edge("dec_tos", "act_req_tos", "No")
    add_edge("act_req_tos", "dec_tos")
    add_edge("dec_tos", "act_check_mst", "Yes")
    add_edge("act_check_mst", "dec_mst")
    add_edge("dec_mst", "act_err_mst", "No")
    add_edge("act_err_mst", "act_check_mst")
    add_edge("dec_mst", "act_create_pending", "Yes")
    add_edge("act_create_pending", "dec_tier")
    add_edge("dec_tier", "act_set_tier1", "Tier 1")
    add_edge("dec_tier", "act_up_gpkd", "Tier 2")
    add_edge("act_up_gpkd", "fork1")
    add_edge("fork1", "act_review")
    add_edge("act_review", "dec_approve")
    add_edge("dec_approve", "act_email_rej", "Reject")
    add_edge("dec_approve", "act_set_tier2", "Approve")
    add_edge("act_set_tier2", "join1")
    add_edge("act_notify", "join1")
    add_edge("join1", "act_login")
    add_edge("act_login", "act_create_ws")
    add_edge("act_create_ws", "act_invite")
    add_edge("act_invite", "act_pub_job")
    add_edge("act_pub_job", "node_end")

    with open(os.path.join(out_dir, "swimlane_01_business_registration_verification.xml"), "w", encoding="utf-8") as f:
        f.write(prettify(mxfile))
    with open(os.path.join(out_dir, "swimlane_01_business_registration_verification.drawio"), "w", encoding="utf-8") as f:
        f.write(prettify(mxfile))

# ==============================================================================
# SL-04: Candidate — Source Code & Git Repository AI Audit Swimlane
# ==============================================================================
def create_sl04_xml():
    mxfile = ET.Element("mxfile", {"host": "app.diagrams.net", "agent": "CVerify", "version": "21.0.0"})
    diag = ET.SubElement(mxfile, "diagram", {"id": "sl-04", "name": "Source Code & Git Repository AI Audit"})
    model = ET.SubElement(diag, "mxGraphModel", {
        "dx": "1400", "dy": "900", "grid": "1", "gridSize": "10", "guides": "1",
        "tooltips": "1", "connect": "1", "arrows": "1", "fold": "1", "page": "1",
        "pageScale": "1", "pageWidth": "1200", "pageHeight": "1300", "math": "0", "shadow": "0"
    })
    root = ET.SubElement(model, "root")
    ET.SubElement(root, "mxCell", {"id": "0"})
    ET.SubElement(root, "mxCell", {"id": "1", "parent": "0"})

    header = ET.SubElement(root, "mxCell", {
        "id": "main_container_sl04",
        "value": "Candidate — Source Code &amp; Git Repository AI Audit Flow",
        "style": "shape=swimlane;html=1;childLayout=stackLayout;startSize=30;horizontal=1;horizontalStack=1;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=14;collapsible=0;expand=0;",
        "vertex": "1", "parent": "1"
    })
    ET.SubElement(header, "mxGeometry", {"x": "40", "y": "40", "width": "1120", "height": "1200", "as": "geometry"})

    lane1 = ET.SubElement(root, "mxCell", {
        "id": "l_cand", "value": "Candidate (Developer)",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container_sl04"
    })
    ET.SubElement(lane1, "mxGeometry", {"x": "0", "y": "30", "width": "280", "height": "1170", "as": "geometry"})

    lane2 = ET.SubElement(root, "mxCell", {
        "id": "l_core", "value": "CVerify Core API (.NET)",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container_sl04"
    })
    ET.SubElement(lane2, "mxGeometry", {"x": "280", "y": "30", "width": "280", "height": "1170", "as": "geometry"})

    lane3 = ET.SubElement(root, "mxCell", {
        "id": "l_worker", "value": "Background Worker Queue",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container_sl04"
    })
    ET.SubElement(lane3, "mxGeometry", {"x": "560", "y": "30", "width": "280", "height": "1170", "as": "geometry"})

    lane4 = ET.SubElement(root, "mxCell", {
        "id": "l_ai", "value": "CVerify.AI Engine (Python)",
        "style": "shape=swimlane;html=1;startSize=30;swimlaneHead=0;swimlaneBody=0;top=0;left=0;bottom=0;right=0;collapsible=0;dropTarget=0;fillColor=#ffffff;strokeColor=#000000;fontStyle=1;fontSize=12;",
        "vertex": "1", "parent": "main_container_sl04"
    })
    ET.SubElement(lane4, "mxGeometry", {"x": "840", "y": "30", "width": "280", "height": "1170", "as": "geometry"})

    # Nodes
    start = ET.SubElement(root, "mxCell", {"id": "g_start", "value": "", "style": "ellipse;html=1;fillColor=#000000;strokeColor=#000000;", "vertex": "1", "parent": "1"})
    ET.SubElement(start, "mxGeometry", {"x": "165", "y": "90", "width": "30", "height": "30", "as": "geometry"})

    a1 = ET.SubElement(root, "mxCell", {"id": "g_connect", "value": "Kết nối GitHub/GitLab OAuth", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a1, "mxGeometry", {"x": "70", "y": "150", "width": "220", "height": "45", "as": "geometry"})

    a2 = ET.SubElement(root, "mxCell", {"id": "g_sync", "value": "Đồng bộ Remote Repositories", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a2, "mxGeometry", {"x": "350", "y": "150", "width": "220", "height": "45", "as": "geometry"})

    a3 = ET.SubElement(root, "mxCell", {"id": "g_trigger", "value": "Chọn Repo & Yêu cầu Audit AI", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a3, "mxGeometry", {"x": "70", "y": "250", "width": "220", "height": "45", "as": "geometry"})

    a4 = ET.SubElement(root, "mxCell", {"id": "g_create_job", "value": "Tạo Pipeline Job (Pending)", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a4, "mxGeometry", {"x": "350", "y": "250", "width": "220", "height": "45", "as": "geometry"})

    a5 = ET.SubElement(root, "mxCell", {"id": "g_consume_job", "value": "Quét Pending Job & Clone Code", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a5, "mxGeometry", {"x": "630", "y": "350", "width": "220", "height": "45", "as": "geometry"})

    a6 = ET.SubElement(root, "mxCell", {"id": "g_ast", "value": "Trích xuất Cấu trúc AST & File Stats", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a6, "mxGeometry", {"x": "630", "y": "440", "width": "220", "height": "45", "as": "geometry"})

    a7 = ET.SubElement(root, "mxCell", {"id": "g_ai_analyze", "value": "Phân tích Tech Stack, Patterns\n& Commit Skill Attributions", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a7, "mxGeometry", {"x": "910", "y": "440", "width": "220", "height": "50", "as": "geometry"})

    a8 = ET.SubElement(root, "mxCell", {"id": "g_save_res", "value": "Lưu Assessment Report & Capabilities vào DB", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a8, "mxGeometry", {"x": "350", "y": "550", "width": "220", "height": "50", "as": "geometry"})

    a9 = ET.SubElement(root, "mxCell", {"id": "g_view_rep", "value": "Xem Báo cáo Audit Codebase Dashboard", "style": "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#000000;fontSize=11;", "vertex": "1", "parent": "1"})
    ET.SubElement(a9, "mxGeometry", {"x": "70", "y": "660", "width": "220", "height": "45", "as": "geometry"})

    end_sl04 = ET.SubElement(root, "mxCell", {"id": "g_end", "value": "", "style": "ellipse;html=1;shape=endState;fillColor=#000000;strokeColor=#000000;", "vertex": "1", "parent": "1"})
    ET.SubElement(end_sl04, "mxGeometry", {"x": "165", "y": "760", "width": "30", "height": "30", "as": "geometry"})

    # Edges
    def add_e(src, tgt, label=""):
        e = ET.SubElement(root, "mxCell", {"id": f"eg_{src}_{tgt}", "value": label, "style": "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=classic;strokeColor=#000000;fontSize=10;", "edge": "1", "parent": "1", "source": src, "target": tgt})
        ET.SubElement(e, "mxGeometry", {"relative": "1", "as": "geometry"})

    add_e("g_start", "g_connect")
    add_e("g_connect", "g_sync")
    add_e("g_sync", "g_trigger")
    add_e("g_trigger", "g_create_job")
    add_e("g_create_job", "g_consume_job")
    add_e("g_consume_job", "g_ast")
    add_e("g_ast", "g_ai_analyze")
    add_e("g_ai_analyze", "g_save_res")
    add_e("g_save_res", "g_view_rep")
    add_e("g_view_rep", "g_end")

    with open(os.path.join(out_dir, "swimlane_04_source_code_git_audit.xml"), "w", encoding="utf-8") as f:
        f.write(prettify(mxfile))
    with open(os.path.join(out_dir, "swimlane_04_source_code_git_audit.drawio"), "w", encoding="utf-8") as f:
        f.write(prettify(mxfile))

if __name__ == "__main__":
    create_sl01_xml()
    create_sl04_xml()
    print("SUCCESS: Generated Swimlane Activity Diagrams in Draw.io format!")
