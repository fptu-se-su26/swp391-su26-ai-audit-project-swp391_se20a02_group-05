import os
import re

base_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify\CVerify.Core"

roles = set()
permissions = set()

for root, dirs, files in os.walk(base_dir):
    for file in files:
        if file.endswith(".cs") or file.endswith(".sql"):
            filepath = os.path.join(root, file)
            with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
                content = f.read()
                
                # Check permissions
                p_matches = re.findall(r'"([a-z_]+:[a-z_:]+)"', content)
                for p in p_matches:
                    permissions.add(p)
                    
                # Check system role references
                r_matches = re.findall(r'(SystemRoles|Roles|RoleNames)\.(\w+)', content)
                for r in r_matches:
                    roles.add(r[1])

print("System Roles found in code:")
for r in sorted(roles):
    print(" -", r)

print(f"\nTotal unique permission strings found: {len(permissions)}")
for p in sorted(list(permissions))[:30]:
    print(" -", p)
