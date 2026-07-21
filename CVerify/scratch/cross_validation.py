import json
import os
import re

with open(r"scratch\extracted_details.json", "r", encoding="utf-8") as f:
    data = json.load(f)

controllers = data["controllers"]
client_routes = data["client_routes"]
ai_routes = data["ai_routes"]

# Map DB Tables to Modules / Diagrams
db_table_mapping = {
    "UCD-01: Authentication & Identity Management": ["users", "refresh_tokens", "verification_tokens", "auth_providers", "external_organizations"],
    "UCD-02: Account & Organization Recovery": ["reset_password_tokens", "organization_recovery_claims", "organization_recovery_votes"],
    "UCD-03: Organization & Member Management": ["organizations", "organization_memberships", "organization_authorities", "workspaces", "workspace_members", "workspace_posts", "role_assignments", "roles", "permissions", "role_permissions"],
    "UCD-04: Candidate Profile & Portfolio Management": ["user_profiles", "career_entries", "education_entries", "work_experience_entries", "project_entries", "project_contributions", "project_repository_links", "project_technologies", "achievement_entries", "evidence_claims", "evidence_artifacts", "evidence_sources", "evidence_signals", "evidence_verifications", "user_cv_settings"],
    "UCD-05: Source Code & Repository Analysis": ["source_code_repositories", "source_code_sync_logs", "repository_assessments", "repository_capabilities", "repository_domains", "repository_intelligence_signals", "repository_skill_attributions", "pipeline_jobs", "pipeline_tasks"],
    "UCD-06: Candidate AI Intelligence & Assessment": ["candidate_assessments", "candidate_assessment_artifacts", "candidate_skills", "candidate_best_fit_roles", "candidate_domain_profiles", "candidate_intelligence_signals", "candidate_strengths_weaknesses", "candidate_capabilities", "candidate_capability_scores", "candidate_capability_evidences", "candidate_capability_histories", "candidate_capability_projections", "candidate_match_projections", "candidate_ranking_projections", "candidate_trust_projections", "candidate_search_profiles", "candidate_discovery_runs", "candidate_evaluation_snapshots", "candidate_skill_tree_nodes", "cv_repository_mappings", "capability_catalog_items", "capability_nodes", "capability_edges", "capability_hierarchies", "capability_registries", "capability_aliases", "trust_profiles", "trust_components", "trust_calculations"],
    "UCD-07: AI Chat & Interactive Guidance": ["conversations", "messages", "ai_streaming_sessions", "ai_streaming_logs", "ai_streaming_metrics", "ai_streaming_stages", "prompt_deployments", "artifact_registry_entries"],
    "UCD-08: Recruitment & Job Vacancy Management": ["job_vacancies", "job_applications", "job_descriptions", "job_description_snapshots", "job_interactions", "hiring_requirements", "requirement_snapshots", "requirement_artifacts", "requirement_artifact_snapshots", "requirement_capabilities", "requirement_vector_snapshots", "interview_blueprints", "interview_blueprint_snapshots", "evaluation_rubrics", "evaluation_rubric_snapshots", "matching_evaluations", "matching_factors", "matching_explanations", "responsibilities", "technology_requirements", "business_outcomes"],
    "UCD-09: Community Forum & Moderation": ["forum_categories", "forum_topics", "forum_topic_tags", "forum_topic_histories", "forum_tags", "forum_replies", "forum_reply_histories", "forum_votes", "forum_bookmarks", "forum_follows", "forum_reactions", "forum_badges", "forum_user_badges", "forum_reputations", "forum_reports", "forum_category_moderators", "forum_moderation_logs", "user_followers"],
    "UCD-10: Notification & Communication System": ["in_app_notifications", "notification_preferences", "activity_events", "outbox_messages"],
    "UCD-11: System Administration & Governance": ["audit_logs", "system_settings", "__EFMigrationsHistory"]
}

# Total tables check
all_db_tables = set()
base_dir = r"d:\Semester 5\SWP391\swp391-su26-ai-audit-project-swp391_se20a02_group-05\CVerify"
sql_path = os.path.join(base_dir, "CVerify.Core", "full_schema.sql")
with open(sql_path, "r", encoding="utf-8") as f:
    sql_text = f.read()
    matches = re.findall(r'CREATE TABLE\s+(?:IF NOT EXISTS\s+)?(?:"?([a-zA-Z0-9_]+)"?|([a-zA-Z0-9_]+))', sql_text, re.IGNORECASE)
    for m in matches:
        t = m[0] if m[0] else m[1]
        all_db_tables.add(t.strip('"'))

mapped_tables = set()
for d, tables in db_table_mapping.items():
    for t in tables:
        mapped_tables.add(t)

print(f"Total DB tables in SQL: {len(all_db_tables)}")
print(f"Total mapped DB tables in diagrams: {len(mapped_tables)}")
unmapped_t = all_db_tables - mapped_tables - {"__EFMigrationsHistory"}
print(f"Unmapped DB tables count: {len(unmapped_t)}")
if unmapped_t:
    print("Unmapped DB tables:", unmapped_t)

# Endpoints mapping by diagram
endpoint_stats = {}
for c in controllers:
    cname = c["controller"]
    ep_count = len(c["endpoints"])
    
    # find diagram
    for d_name, d_tables in db_table_mapping.items():
        # we mapped controllers in previous step
        pass

print("Validation completed successfully.")
