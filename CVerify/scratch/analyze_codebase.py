import os
import re
import json

base_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify"
core_dir = os.path.join(base_dir, "CVerify.Core")
client_dir = os.path.join(base_dir, "client", "src")
ai_dir = os.path.join(base_dir, "CVerify.AI", "app")

controller_details = []

for root, dirs, files in os.walk(core_dir):
    if any(x in root for x in ["bin", "obj", ".git"]):
        continue
    for file in files:
        if file.endswith("Controller.cs"):
            filepath = os.path.join(root, file)
            module = os.path.basename(os.path.dirname(os.path.dirname(filepath)))
            
            with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
                content = f.read()
                
            # Class route
            class_route_match = re.search(r'\[Route\("([^"]+)"\)\]', content)
            base_route = class_route_match.group(1) if class_route_match else ""
            
            # Controller class name
            cname_match = re.search(r'public class (\w+Controller)', content)
            cname = cname_match.group(1) if cname_match else file[:-3]
            
            # Endpoints
            methods = re.split(r'\n\s*\[Http', content)
            ep_list = []
            
            # Find all endpoint attributes
            pattern = r'\[(HttpGet|HttpPost|HttpPut|HttpDelete|HttpPatch)(?:\("([^"]*)"\))?\][\s\S]*?public\s+(?:async\s+)?(?:Task<)?(?:ActionResult<)?(?:IResult|IActionResult|[\w<>]+)\s+(\w+)\s*\('
            for match in re.finditer(pattern, content):
                http_verb = match.group(1)
                sub_route = match.group(2) or ""
                method_name = match.group(3)
                
                full_ep_route = base_route
                if sub_route:
                    if sub_route.startswith("/"):
                        full_ep_route = sub_route
                    else:
                        full_ep_route = f"{base_route}/{sub_route}"
                        
                ep_list.append({
                    "verb": http_verb,
                    "route": full_ep_route,
                    "method": method_name
                })
                
            controller_details.append({
                "module": module,
                "controller": cname,
                "file": os.path.relpath(filepath, base_dir),
                "base_route": base_route,
                "endpoints_count": len(ep_list),
                "endpoints": ep_list
            })

print(f"Total Controller detail count: {len(controller_details)}")
total_eps = sum(len(c["endpoints"]) for c in controller_details)
print(f"Total API Endpoints count: {total_eps}")

# Frontend Routes
client_routes = []
if os.path.exists(client_dir):
    for root, dirs, files in os.walk(client_dir):
        for file in files:
            if file in ["page.tsx", "page.jsx", "page.js"]:
                rel = os.path.relpath(root, os.path.join(client_dir, "app"))
                client_routes.append("/" + rel.replace("\\", "/"))

print(f"Frontend App Routes count: {len(client_routes)}")

# AI Routes
ai_routes = []
if os.path.exists(ai_dir):
    for root, dirs, files in os.walk(ai_dir):
        for file in files:
            if file.endswith(".py"):
                filepath = os.path.join(root, file)
                with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
                    content = f.read()
                matches = re.findall(r'@(?:router|app)\.(get|post|put|delete)\("([^"]+)"', content)
                for m in matches:
                    ai_routes.append((file, m[0].upper(), m[1]))

print(f"AI Service Routes count: {len(ai_routes)}")

with open(r"scratch\extracted_details.json", "w", encoding="utf-8") as f:
    json.dump({
        "controllers": controller_details,
        "client_routes": client_routes,
        "ai_routes": ai_routes
    }, f, indent=2)

print("Saved scratch\\extracted_details.json")
