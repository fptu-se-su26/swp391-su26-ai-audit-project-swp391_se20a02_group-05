import json

with open(r"scratch\extracted_details.json", "r", encoding="utf-8") as f:
    data = json.load(f)

client_routes = data["client_routes"]
print(f"Total Client Routes: {len(client_routes)}")

# Categorize client routes
route_groups = {}
for r in client_routes:
    parts = r.split('/')
    group = parts[1] if len(parts) > 1 else "root"
    if group not in route_groups:
        route_groups[group] = []
    route_groups[group].append(r)

for g, routes in route_groups.items():
    print(f"\nGroup [{g}]: ({len(routes)} routes)")
    for r in routes:
        print("  ", r)
