CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;
CREATE TABLE activity_events (
    id uuid NOT NULL,
    correlation_id uuid NOT NULL,
    causation_id uuid,
    organization_id uuid,
    actor_user_id uuid,
    event_type character varying(100) NOT NULL,
    resource_type character varying(50) NOT NULL,
    resource_id uuid,
    visibility character varying(30) NOT NULL,
    is_projected boolean NOT NULL,
    payload_json jsonb,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_activity_events PRIMARY KEY (id),
    CONSTRAINT fk_activity_events_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES organizations (id) ON DELETE CASCADE,
    CONSTRAINT fk_activity_events_users_actor_user_id FOREIGN KEY (actor_user_id) REFERENCES users (id) ON DELETE SET NULL
);

CREATE TABLE notification_preferences (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    notification_type character varying(100) NOT NULL,
    channel character varying(20) NOT NULL,
    is_enabled boolean NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_notification_preferences PRIMARY KEY (id),
    CONSTRAINT fk_notification_preferences_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE in_app_notifications (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    activity_event_id uuid,
    notification_type character varying(100) NOT NULL,
    resource_type character varying(50) NOT NULL,
    resource_id uuid,
    payload_json jsonb,
    is_read boolean NOT NULL,
    is_aggregated boolean NOT NULL,
    aggregate_key character varying(255),
    created_at timestamp with time zone NOT NULL,
    read_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_in_app_notifications PRIMARY KEY (id),
    CONSTRAINT fk_in_app_notifications_activity_events_activity_event_id FOREIGN KEY (activity_event_id) REFERENCES activity_events (id) ON DELETE SET NULL,
    CONSTRAINT fk_in_app_notifications_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX idx_activity_events_correlation ON activity_events (correlation_id);

CREATE INDEX idx_activity_events_org_created ON activity_events (organization_id, created_at);

CREATE INDEX ix_activity_events_actor_user_id ON activity_events (actor_user_id);

CREATE INDEX idx_in_app_notifications_aggregate ON in_app_notifications (user_id, aggregate_key) WHERE is_read = FALSE AND deleted_at IS NULL;

CREATE INDEX idx_in_app_notifications_user_id ON in_app_notifications (user_id);

CREATE INDEX idx_in_app_notifications_user_unread ON in_app_notifications (user_id, is_read) WHERE deleted_at IS NULL;

CREATE INDEX ix_in_app_notifications_activity_event_id ON in_app_notifications (activity_event_id);

CREATE UNIQUE INDEX idx_user_notification_prefs ON notification_preferences (user_id, notification_type, channel);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260611091911_AddNotificationSystem', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE workspace_posts (
    id uuid NOT NULL,
    organization_id uuid NOT NULL,
    created_by_user_id uuid NOT NULL,
    category character varying(100) NOT NULL,
    content text NOT NULL,
    images text[] NOT NULL,
    likes integer NOT NULL,
    shares_count integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_workspace_posts PRIMARY KEY (id),
    CONSTRAINT fk_workspace_posts_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES organizations (id) ON DELETE CASCADE,
    CONSTRAINT fk_workspace_posts_users_created_by_user_id FOREIGN KEY (created_by_user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_workspace_posts_created_by_user_id ON workspace_posts (created_by_user_id);

CREATE INDEX ix_workspace_posts_organization_id ON workspace_posts (organization_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260614071252_AddWorkspacePostsTable', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE job_vacancies (
    id uuid NOT NULL,
    organization_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    department character varying(100) NOT NULL,
    workplace_type character varying(50) NOT NULL,
    city character varying(100) NOT NULL,
    type character varying(50) NOT NULL,
    salary character varying(100) NOT NULL,
    salary_min_max character varying(100) NOT NULL,
    headcount integer NOT NULL,
    gender character varying(50) NOT NULL,
    experience character varying(100) NOT NULL,
    degree character varying(100) NOT NULL,
    category character varying(200) NOT NULL,
    description text[] NOT NULL,
    requirements text[] NOT NULL,
    benefits text[] NOT NULL,
    tags text[] NOT NULL,
    skills text[] NOT NULL,
    cover_url character varying(2048) NOT NULL,
    images text[] NOT NULL,
    is_active boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_job_vacancies PRIMARY KEY (id),
    CONSTRAINT fk_job_vacancies_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES organizations (id) ON DELETE CASCADE
);

CREATE INDEX ix_job_vacancies_organization_id ON job_vacancies (organization_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260614100549_AddJobVacanciesTable', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE work_experience_entries ADD is_leadership boolean NOT NULL DEFAULT FALSE;

ALTER TABLE user_profiles ADD last_profile_update_at timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE organizations ADD core_values text;

ALTER TABLE organizations ADD founded text;

ALTER TABLE organizations ADD mission text;

ALTER TABLE organizations ADD vision text;

CREATE TABLE candidate_assessments (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    status character varying(50) NOT NULL,
    overall_score double precision NOT NULL,
    career_level character varying(20),
    career_level_label character varying(50),
    primary_tendency character varying(50),
    primary_working_style character varying(50),
    summary_headline character varying(500),
    summary_paragraph character varying(2000),
    pipeline_version character varying(20) NOT NULL,
    assessment_schema_version character varying(20) NOT NULL,
    last_profile_update_at timestamp with time zone NOT NULL,
    last_repository_analysis_at timestamp with time zone NOT NULL,
    last_assessment_at timestamp with time zone,
    failed_stage character varying(100),
    failure_reason text,
    version integer NOT NULL,
    created_at_utc timestamp with time zone NOT NULL,
    completed_at_utc timestamp with time zone,
    CONSTRAINT pk_candidate_assessments PRIMARY KEY (id),
    CONSTRAINT fk_candidate_assessments_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE candidate_assessment_artifacts (
    id uuid NOT NULL,
    assessment_id uuid NOT NULL,
    artifact_type character varying(100) NOT NULL,
    json_data text NOT NULL,
    created_at_utc timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_assessment_artifacts PRIMARY KEY (id),
    CONSTRAINT fk_candidate_assessment_artifacts_candidate_assessments_assess FOREIGN KEY (assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE
);

CREATE INDEX idx_candidate_assessment_artifacts_assessment_id ON candidate_assessment_artifacts (assessment_id);

CREATE UNIQUE INDEX ux_candidate_assessment_artifacts_type ON candidate_assessment_artifacts (assessment_id, artifact_type);

CREATE INDEX idx_candidate_assessments_user_id ON candidate_assessments (user_id);

CREATE UNIQUE INDEX ux_candidate_assessments_user_version ON candidate_assessments (user_id, version);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260615093611_AddCandidateAssessments', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE source_code_repositories ADD external_organization_id uuid;

CREATE TABLE external_organizations (
    id uuid NOT NULL,
    auth_provider_id uuid NOT NULL,
    external_id character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    login character varying(255) NOT NULL,
    type character varying(50) NOT NULL,
    avatar_url character varying(1000),
    html_url character varying(1000),
    description character varying(2000),
    is_active boolean NOT NULL,
    last_synced_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_external_organizations PRIMARY KEY (id),
    CONSTRAINT fk_external_organizations_auth_providers_auth_provider_id FOREIGN KEY (auth_provider_id) REFERENCES auth_providers (id) ON DELETE CASCADE
);

CREATE INDEX ix_source_code_repositories_external_organization_id ON source_code_repositories (external_organization_id);

CREATE UNIQUE INDEX idx_external_organizations_provider_external_active ON external_organizations (auth_provider_id, external_id);

ALTER TABLE source_code_repositories ADD CONSTRAINT fk_source_code_repositories_external_organizations_external_or FOREIGN KEY (external_organization_id) REFERENCES external_organizations (id) ON DELETE SET NULL;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260615141609_AddExternalOrganizations', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE project_entries (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    name character varying(255) NOT NULL,
    role character varying(255),
    description character varying(2000) NOT NULL,
    start_date timestamp with time zone,
    end_date timestamp with time zone,
    is_currently_working boolean NOT NULL,
    verification_level integer NOT NULL,
    verification_status integer NOT NULL,
    verified_at timestamp with time zone,
    verification_metadata_json jsonb,
    display_order integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_project_entries PRIMARY KEY (id),
    CONSTRAINT fk_project_entries_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE project_contributions (
    id uuid NOT NULL,
    project_entry_id uuid NOT NULL,
    content character varying(1000) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_project_contributions PRIMARY KEY (id),
    CONSTRAINT fk_project_contributions_project_entries_project_entry_id FOREIGN KEY (project_entry_id) REFERENCES project_entries (id) ON DELETE CASCADE
);

CREATE TABLE project_repository_links (
    id uuid NOT NULL,
    project_entry_id uuid NOT NULL,
    source_code_repository_id uuid NOT NULL,
    linked_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_project_repository_links PRIMARY KEY (id),
    CONSTRAINT fk_project_repository_links_project_entries_project_entry_id FOREIGN KEY (project_entry_id) REFERENCES project_entries (id) ON DELETE CASCADE,
    CONSTRAINT fk_project_repository_links_source_code_repositories_source_co FOREIGN KEY (source_code_repository_id) REFERENCES source_code_repositories (id) ON DELETE CASCADE
);

CREATE TABLE project_technologies (
    id uuid NOT NULL,
    project_entry_id uuid NOT NULL,
    name character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_project_technologies PRIMARY KEY (id),
    CONSTRAINT fk_project_technologies_project_entries_project_entry_id FOREIGN KEY (project_entry_id) REFERENCES project_entries (id) ON DELETE CASCADE
);

CREATE INDEX idx_project_contributions_project_id ON project_contributions (project_entry_id);

CREATE INDEX idx_project_entries_user_id ON project_entries (user_id);

CREATE UNIQUE INDEX idx_project_repo_links_unique ON project_repository_links (project_entry_id, source_code_repository_id);

CREATE INDEX ix_project_repository_links_source_code_repository_id ON project_repository_links (source_code_repository_id);

CREATE INDEX idx_project_technologies_project_id ON project_technologies (project_entry_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260615152001_AddProjectEntriesAndLinks', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE candidate_assessments ADD cv_id uuid;

ALTER TABLE candidate_assessments ADD model_version character varying(100);

ALTER TABLE candidate_assessments ADD prompt_version character varying(50);

CREATE TABLE repository_assessments (
    id uuid NOT NULL,
    repository_id uuid NOT NULL,
    analysis_job_id uuid NOT NULL,
    status character varying(30) NOT NULL,
    commit_sha character varying(100) NOT NULL,
    overall_score double precision NOT NULL,
    tech_stack jsonb,
    patterns jsonb,
    quality_metrics jsonb,
    json_data jsonb,
    model_version character varying(100),
    prompt_version character varying(50),
    assessment_schema_version character varying(20),
    pipeline_version character varying(20),
    created_at_utc timestamp with time zone NOT NULL,
    completed_at_utc timestamp with time zone,
    CONSTRAINT pk_repository_assessments PRIMARY KEY (id)
);

CREATE INDEX idx_repository_assessments_job_id ON repository_assessments (analysis_job_id);

CREATE INDEX idx_repository_assessments_repo_id ON repository_assessments (repository_id);

CREATE INDEX ux_repository_assessments_repo_sha ON repository_assessments (repository_id, commit_sha);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260615171115_AddRepositoryAssessments', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE repository_capabilities (
    id uuid NOT NULL,
    repository_assessment_id uuid NOT NULL,
    name character varying(100) NOT NULL,
    category character varying(50) NOT NULL,
    confidence double precision NOT NULL,
    maturity character varying(30) NOT NULL,
    difficulty_score double precision NOT NULL,
    score double precision NOT NULL,
    evidence_json jsonb,
    assessment_version character varying(20) NOT NULL,
    analysis_version character varying(20) NOT NULL,
    model_version character varying(100) NOT NULL,
    prompt_version character varying(50) NOT NULL,
    CONSTRAINT pk_repository_capabilities PRIMARY KEY (id)
);

CREATE TABLE repository_domains (
    id uuid NOT NULL,
    repository_assessment_id uuid NOT NULL,
    domain_name character varying(100) NOT NULL,
    weight double precision NOT NULL,
    confidence double precision NOT NULL,
    evidence_count integer NOT NULL,
    supporting_signals jsonb,
    assessment_version character varying(20) NOT NULL,
    analysis_version character varying(20) NOT NULL,
    model_version character varying(100) NOT NULL,
    prompt_version character varying(50) NOT NULL,
    CONSTRAINT pk_repository_domains PRIMARY KEY (id)
);

CREATE TABLE repository_intelligence_signals (
    id uuid NOT NULL,
    repository_assessment_id uuid NOT NULL,
    scope_signal double precision NOT NULL,
    complexity_signal double precision NOT NULL,
    ownership_signal double precision NOT NULL,
    leadership_signal double precision NOT NULL,
    consistency_signal double precision NOT NULL,
    last_updated_utc timestamp with time zone NOT NULL,
    assessment_version character varying(20) NOT NULL,
    analysis_version character varying(20) NOT NULL,
    model_version character varying(100) NOT NULL,
    prompt_version character varying(50) NOT NULL,
    CONSTRAINT pk_repository_intelligence_signals PRIMARY KEY (id)
);

CREATE TABLE repository_skill_attributions (
    id uuid NOT NULL,
    repository_assessment_id uuid NOT NULL,
    skill_name character varying(100) NOT NULL,
    contribution_weight double precision NOT NULL,
    confidence double precision NOT NULL,
    verification_level character varying(30) NOT NULL,
    assessment_version character varying(20) NOT NULL,
    analysis_version character varying(20) NOT NULL,
    model_version character varying(100) NOT NULL,
    prompt_version character varying(50) NOT NULL,
    CONSTRAINT pk_repository_skill_attributions PRIMARY KEY (id)
);

CREATE INDEX idx_repository_capabilities_assessment_id ON repository_capabilities (repository_assessment_id);

CREATE UNIQUE INDEX ux_repository_capabilities_assessment_id_name ON repository_capabilities (repository_assessment_id, name);

CREATE INDEX idx_repository_domains_assessment_id ON repository_domains (repository_assessment_id);

CREATE UNIQUE INDEX ux_repository_domains_assessment_id_domain ON repository_domains (repository_assessment_id, domain_name);

CREATE UNIQUE INDEX ux_repository_intelligence_signals_assessment_id ON repository_intelligence_signals (repository_assessment_id);

CREATE INDEX idx_repository_skill_attributions_assessment_id ON repository_skill_attributions (repository_assessment_id);

CREATE UNIQUE INDEX ux_repository_skill_attributions_assessment_id_skill ON repository_skill_attributions (repository_assessment_id, skill_name);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260616080806_AddRepositoryIntelligenceTables', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE candidate_assessments ADD execution_strength double precision NOT NULL DEFAULT 0.0;

ALTER TABLE candidate_assessments ADD leadership_potential double precision NOT NULL DEFAULT 0.0;

ALTER TABLE candidate_assessments ADD technical_breadth double precision NOT NULL DEFAULT 0.0;

ALTER TABLE candidate_assessments ADD technical_depth double precision NOT NULL DEFAULT 0.0;

ALTER TABLE candidate_assessments ADD trust_level double precision NOT NULL DEFAULT 0.0;

CREATE TABLE candidate_best_fit_roles (
    id uuid NOT NULL,
    candidate_assessment_id uuid NOT NULL,
    role_title character varying(100) NOT NULL,
    match_score double precision NOT NULL,
    confidence double precision NOT NULL,
    rank integer NOT NULL,
    matching_engine_version character varying(20) NOT NULL,
    evidence jsonb,
    engine_metadata jsonb,
    CONSTRAINT pk_candidate_best_fit_roles PRIMARY KEY (id),
    CONSTRAINT fk_candidate_best_fit_roles_candidate_assessments_candidate_as FOREIGN KEY (candidate_assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE
);

CREATE TABLE candidate_domain_profiles (
    id uuid NOT NULL,
    candidate_assessment_id uuid NOT NULL,
    domain_name character varying(100) NOT NULL,
    score double precision NOT NULL,
    confidence double precision NOT NULL,
    seniority character varying(50) NOT NULL,
    supporting_evidence jsonb,
    CONSTRAINT pk_candidate_domain_profiles PRIMARY KEY (id),
    CONSTRAINT fk_candidate_domain_profiles_candidate_assessments_candidate_a FOREIGN KEY (candidate_assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE
);

CREATE TABLE candidate_intelligence_signals (
    id uuid NOT NULL,
    candidate_assessment_id uuid NOT NULL,
    scope_signal double precision NOT NULL,
    complexity_signal double precision NOT NULL,
    ownership_signal double precision NOT NULL,
    leadership_signal double precision NOT NULL,
    consistency_signal double precision NOT NULL,
    delivery_signal double precision NOT NULL,
    engineering_maturity_signal double precision NOT NULL,
    problem_solving_signal double precision NOT NULL,
    last_updated_utc timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_intelligence_signals PRIMARY KEY (id),
    CONSTRAINT fk_candidate_intelligence_signals_candidate_assessments_candid FOREIGN KEY (candidate_assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE
);

CREATE TABLE candidate_skills (
    id uuid NOT NULL,
    candidate_assessment_id uuid NOT NULL,
    skill_name character varying(100) NOT NULL,
    score double precision NOT NULL,
    confidence double precision NOT NULL,
    level character varying(50) NOT NULL,
    evidence_sources jsonb,
    CONSTRAINT pk_candidate_skills PRIMARY KEY (id),
    CONSTRAINT fk_candidate_skills_candidate_assessments_candidate_assessment FOREIGN KEY (candidate_assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE
);

CREATE TABLE candidate_strengths_weaknesses (
    id uuid NOT NULL,
    candidate_assessment_id uuid NOT NULL,
    finding_type character varying(20) NOT NULL,
    topic character varying(150) NOT NULL,
    description character varying(1000) NOT NULL,
    evidence jsonb,
    CONSTRAINT pk_candidate_strengths_weaknesses PRIMARY KEY (id),
    CONSTRAINT fk_candidate_strengths_weaknesses_candidate_assessments_candid FOREIGN KEY (candidate_assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE
);

CREATE INDEX ix_candidate_best_fit_roles_candidate_assessment_id ON candidate_best_fit_roles (candidate_assessment_id);

CREATE INDEX ix_candidate_domain_profiles_candidate_assessment_id ON candidate_domain_profiles (candidate_assessment_id);

CREATE INDEX ix_candidate_intelligence_signals_candidate_assessment_id ON candidate_intelligence_signals (candidate_assessment_id);

CREATE INDEX ix_candidate_skills_candidate_assessment_id ON candidate_skills (candidate_assessment_id);

CREATE INDEX ix_candidate_strengths_weaknesses_candidate_assessment_id ON candidate_strengths_weaknesses (candidate_assessment_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260616082519_AddCandidateIntelligenceTablesPhase2', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE job_vacancies ADD metadata text;

CREATE TABLE cv_repository_mappings (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    source_code_repository_id uuid NOT NULL,
    reference_source character varying(50) NOT NULL,
    reference_entity_id uuid,
    indexed_at_utc timestamp with time zone NOT NULL,
    CONSTRAINT pk_cv_repository_mappings PRIMARY KEY (id),
    CONSTRAINT fk_cv_repository_mappings_source_code_repositories_source_code FOREIGN KEY (source_code_repository_id) REFERENCES source_code_repositories (id) ON DELETE CASCADE
);

CREATE INDEX idx_cv_repository_mappings_repo_id ON cv_repository_mappings (source_code_repository_id);

CREATE INDEX idx_cv_repository_mappings_user_id ON cv_repository_mappings (user_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260620101455_AddMetadataToJobVacancy', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE job_vacancies ADD hiring_requirement_id uuid;

CREATE TABLE hiring_requirements (
    id uuid NOT NULL,
    organization_id uuid NOT NULL,
    workspace_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    department character varying(100) NOT NULL,
    seniority character varying(50) NOT NULL,
    workplace_type character varying(50) NOT NULL,
    city character varying(100),
    employment_type character varying(50) NOT NULL,
    salary_min numeric,
    salary_max numeric,
    currency character varying(10),
    timezone_range character varying(100),
    degree_requirement character varying(100),
    benefits text[] NOT NULL,
    language_requirements text[] NOT NULL,
    headcount integer NOT NULL,
    status character varying(20) NOT NULL,
    version integer NOT NULL,
    hiring_reason character varying(100),
    business_problem character varying(2000),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_hiring_requirements PRIMARY KEY (id),
    CONSTRAINT fk_hiring_requirements_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES organizations (id) ON DELETE CASCADE,
    CONSTRAINT fk_hiring_requirements_workspaces_workspace_id FOREIGN KEY (workspace_id) REFERENCES workspaces (id) ON DELETE CASCADE
);

CREATE TABLE business_outcomes (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    text character varying(1000) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_business_outcomes PRIMARY KEY (id),
    CONSTRAINT fk_business_outcomes_hiring_requirements_hiring_requirement_id FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE evaluation_rubrics (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    capability_weights jsonb,
    scoring_rules jsonb,
    evidence_requirements jsonb,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_evaluation_rubrics PRIMARY KEY (id),
    CONSTRAINT fk_evaluation_rubrics_hiring_requirements_hiring_requirement_id FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE interview_blueprints (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    capability_questions jsonb,
    dimensions jsonb,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_interview_blueprints PRIMARY KEY (id),
    CONSTRAINT fk_interview_blueprints_hiring_requirements_hiring_requirement FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE requirement_capabilities (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    capability_id character varying(100) NOT NULL,
    name character varying(255) NOT NULL,
    category character varying(100) NOT NULL,
    priority text NOT NULL,
    ownership_level text NOT NULL,
    expected_proficiency integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_requirement_capabilities PRIMARY KEY (id),
    CONSTRAINT fk_requirement_capabilities_hiring_requirements_hiring_require FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE requirement_snapshots (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    version integer NOT NULL,
    snapshotted_at timestamp with time zone NOT NULL,
    title character varying(255) NOT NULL,
    department character varying(100) NOT NULL,
    seniority character varying(50) NOT NULL,
    workplace_type character varying(50) NOT NULL,
    city character varying(100),
    employment_type character varying(50) NOT NULL,
    salary_min numeric,
    salary_max numeric,
    currency character varying(10),
    timezone_range character varying(100),
    degree_requirement character varying(100),
    benefits text[] NOT NULL,
    language_requirements text[] NOT NULL,
    headcount integer NOT NULL,
    hiring_reason character varying(100),
    business_problem character varying(2000),
    business_outcomes_json jsonb,
    responsibilities_json jsonb,
    capabilities_json jsonb,
    technology_requirements_json jsonb,
    CONSTRAINT pk_requirement_snapshots PRIMARY KEY (id),
    CONSTRAINT fk_requirement_snapshots_hiring_requirements_hiring_requiremen FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE responsibilities (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    text character varying(1000) NOT NULL,
    priority text NOT NULL,
    ownership_level text NOT NULL,
    is_leadership boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_responsibilities PRIMARY KEY (id),
    CONSTRAINT fk_responsibilities_hiring_requirements_hiring_requirement_id FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE technology_requirements (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    name character varying(100) NOT NULL,
    priority text NOT NULL,
    sfia_level integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_technology_requirements PRIMARY KEY (id),
    CONSTRAINT fk_technology_requirements_hiring_requirements_hiring_requirem FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE TABLE evidence_signals (
    id uuid NOT NULL,
    requirement_capability_id uuid NOT NULL,
    signal_type character varying(100) NOT NULL,
    expected_metric character varying(255) NOT NULL,
    rationale character varying(1000),
    metadata jsonb,
    CONSTRAINT pk_evidence_signals PRIMARY KEY (id),
    CONSTRAINT fk_evidence_signals_requirement_capabilities_requirement_capab FOREIGN KEY (requirement_capability_id) REFERENCES requirement_capabilities (id) ON DELETE CASCADE
);

CREATE TABLE evaluation_rubric_snapshots (
    requirement_snapshot_id uuid NOT NULL,
    capability_weights jsonb,
    scoring_rules jsonb,
    evidence_requirements jsonb,
    snapshotted_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_evaluation_rubric_snapshots PRIMARY KEY (requirement_snapshot_id),
    CONSTRAINT fk_evaluation_rubric_snapshots_requirement_snapshots_requireme FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id) ON DELETE CASCADE
);

CREATE TABLE interview_blueprint_snapshots (
    requirement_snapshot_id uuid NOT NULL,
    capability_questions jsonb,
    dimensions jsonb,
    snapshotted_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_interview_blueprint_snapshots PRIMARY KEY (requirement_snapshot_id),
    CONSTRAINT fk_interview_blueprint_snapshots_requirement_snapshots_require FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id) ON DELETE CASCADE
);

CREATE TABLE job_description_snapshots (
    requirement_snapshot_id uuid NOT NULL,
    markdown_content text NOT NULL,
    snapshotted_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_job_description_snapshots PRIMARY KEY (requirement_snapshot_id),
    CONSTRAINT fk_job_description_snapshots_requirement_snapshots_requirement FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id) ON DELETE CASCADE
);

CREATE TABLE requirement_vector_snapshots (
    requirement_snapshot_id uuid NOT NULL,
    vector real[] NOT NULL,
    dimension integer NOT NULL,
    snapshotted_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_requirement_vector_snapshots PRIMARY KEY (requirement_snapshot_id),
    CONSTRAINT fk_requirement_vector_snapshots_requirement_snapshots_requirem FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id) ON DELETE CASCADE
);

CREATE INDEX ix_job_vacancies_hiring_requirement_id ON job_vacancies (hiring_requirement_id);

CREATE INDEX idx_business_outcomes_hr_id ON business_outcomes (hiring_requirement_id);

CREATE INDEX idx_evaluation_rubrics_hr_id ON evaluation_rubrics (hiring_requirement_id);

CREATE INDEX idx_evidence_signals_cap_id ON evidence_signals (requirement_capability_id);

CREATE INDEX idx_hiring_requirements_org_id ON hiring_requirements (organization_id);

CREATE INDEX idx_hiring_requirements_workspace_id ON hiring_requirements (workspace_id);

CREATE INDEX idx_interview_blueprints_hr_id ON interview_blueprints (hiring_requirement_id);

CREATE INDEX idx_requirement_capabilities_hr_id ON requirement_capabilities (hiring_requirement_id);

CREATE INDEX idx_requirement_snapshots_hr_id ON requirement_snapshots (hiring_requirement_id);

CREATE INDEX idx_responsibilities_hr_id ON responsibilities (hiring_requirement_id);

CREATE INDEX idx_technology_requirements_hr_id ON technology_requirements (hiring_requirement_id);

ALTER TABLE job_vacancies ADD CONSTRAINT fk_job_vacancies_hiring_requirements_hiring_requirement_id FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260620103710_AddHiringRequirementSystem', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE job_descriptions (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    markdown_content text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_job_descriptions PRIMARY KEY (id),
    CONSTRAINT fk_job_descriptions_hiring_requirements_hiring_requirement_id FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE INDEX ix_job_descriptions_hiring_requirement_id ON job_descriptions (hiring_requirement_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260620104410_AddDraftJobDescription', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE capability_catalog_items (
    capability_id character varying(100) NOT NULL,
    display_name character varying(255) NOT NULL,
    category character varying(100) NOT NULL,
    description character varying(1000) NOT NULL,
    skills text[] NOT NULL,
    expected_evidence text[] NOT NULL,
    workspace_id uuid,
    status character varying(20) NOT NULL,
    is_custom boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_capability_catalog_items PRIMARY KEY (capability_id),
    CONSTRAINT fk_capability_catalog_items_workspaces_workspace_id FOREIGN KEY (workspace_id) REFERENCES workspaces (id)
);

CREATE INDEX ix_capability_catalog_items_workspace_id ON capability_catalog_items (workspace_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260620120213_AddCapabilityCatalogTaxonomy', '10.0.9');

COMMIT;

START TRANSACTION;
DROP TABLE job_description_snapshots;

DROP TABLE job_descriptions;

CREATE TABLE requirement_artifact_snapshots (
    id uuid NOT NULL,
    requirement_snapshot_id uuid NOT NULL,
    artifact_type character varying(100) NOT NULL,
    markdown_content text NOT NULL,
    structured_content_json jsonb,
    snapshotted_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_requirement_artifact_snapshots PRIMARY KEY (id),
    CONSTRAINT fk_requirement_artifact_snapshots_requirement_snapshots_requir FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id) ON DELETE CASCADE
);

CREATE TABLE requirement_artifacts (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    artifact_type character varying(100) NOT NULL,
    markdown_content text NOT NULL,
    structured_content_json jsonb,
    status character varying(50) NOT NULL,
    model_info character varying(100),
    prompt_template_id character varying(100),
    prompt_version character varying(50),
    prompt_hash character varying(100),
    generation_metadata_json jsonb,
    regeneration_history_json jsonb,
    generation_timestamp timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_requirement_artifacts PRIMARY KEY (id),
    CONSTRAINT fk_requirement_artifacts_hiring_requirements_hiring_requiremen FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE
);

CREATE INDEX ix_requirement_artifact_snapshots_requirement_snapshot_id ON requirement_artifact_snapshots (requirement_snapshot_id);

CREATE INDEX ix_requirement_artifacts_hiring_requirement_id ON requirement_artifacts (hiring_requirement_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260620140210_AddRequirementArtifacts', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE job_vacancies ADD acquisition_strategy character varying(50) NOT NULL DEFAULT '';

ALTER TABLE job_vacancies ADD discovery_profile_json jsonb;

ALTER TABLE job_vacancies ADD requirement_snapshot_id uuid;

ALTER TABLE job_vacancies ADD status character varying(50) NOT NULL DEFAULT '';

CREATE INDEX ix_job_vacancies_requirement_snapshot_id ON job_vacancies (requirement_snapshot_id);

ALTER TABLE job_vacancies ADD CONSTRAINT fk_job_vacancies_requirement_snapshots_requirement_snapshot_id FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621102139_AddJobVacancyWorkflowRedesign', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE job_vacancies DROP CONSTRAINT fk_job_vacancies_hiring_requirements_hiring_requirement_id;

ALTER TABLE job_vacancies DROP CONSTRAINT fk_job_vacancies_requirement_snapshots_requirement_snapshot_id;

ALTER TABLE job_vacancies ADD CONSTRAINT fk_job_vacancies_hiring_requirements_hiring_requirement_id FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE;

ALTER TABLE job_vacancies ADD CONSTRAINT fk_job_vacancies_requirement_snapshots_requirement_snapshot_id FOREIGN KEY (requirement_snapshot_id) REFERENCES requirement_snapshots (id) ON DELETE SET NULL;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621103138_AddJobVacancyCascadeDelete', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE requirement_snapshots ADD auto_close_rule integer NOT NULL DEFAULT 0;

ALTER TABLE requirement_snapshots ADD candidates_needed_count integer;

ALTER TABLE requirement_snapshots ADD end_date timestamp with time zone;

ALTER TABLE requirement_snapshots ADD is_manually_closed boolean NOT NULL DEFAULT FALSE;

ALTER TABLE requirement_snapshots ADD is_salary_negotiable boolean NOT NULL DEFAULT FALSE;

ALTER TABLE requirement_snapshots ADD salary_period integer NOT NULL DEFAULT 0;

ALTER TABLE requirement_snapshots ADD start_date timestamp with time zone;

ALTER TABLE hiring_requirements ADD auto_close_rule integer NOT NULL DEFAULT 0;

ALTER TABLE hiring_requirements ADD candidates_needed_count integer;

ALTER TABLE hiring_requirements ADD end_date timestamp with time zone;

ALTER TABLE hiring_requirements ADD is_manually_closed boolean NOT NULL DEFAULT FALSE;

ALTER TABLE hiring_requirements ADD is_salary_negotiable boolean NOT NULL DEFAULT FALSE;

ALTER TABLE hiring_requirements ADD salary_period integer NOT NULL DEFAULT 0;

ALTER TABLE hiring_requirements ADD start_date timestamp with time zone;

CREATE TABLE candidate_discovery_runs (
    id uuid NOT NULL,
    hiring_requirement_id uuid NOT NULL,
    triggered_by_id uuid NOT NULL,
    started_at timestamp with time zone NOT NULL,
    completed_at timestamp with time zone,
    status integer NOT NULL,
    candidates_found_count integer NOT NULL,
    match_quality_summary character varying(500),
    error_message text,
    raw_results_json text,
    CONSTRAINT pk_candidate_discovery_runs PRIMARY KEY (id),
    CONSTRAINT fk_candidate_discovery_runs_hiring_requirements_hiring_require FOREIGN KEY (hiring_requirement_id) REFERENCES hiring_requirements (id) ON DELETE CASCADE,
    CONSTRAINT fk_candidate_discovery_runs_users_triggered_by_id FOREIGN KEY (triggered_by_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX idx_candidate_discovery_runs_requirement_id ON candidate_discovery_runs (hiring_requirement_id);

CREATE INDEX idx_candidate_discovery_runs_triggered_by_id ON candidate_discovery_runs (triggered_by_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621172337_AddHiringLifecycleAndDiscoveryRuns', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE candidate_discovery_runs DROP CONSTRAINT fk_candidate_discovery_runs_users_triggered_by_id;

ALTER TABLE candidate_discovery_runs ALTER COLUMN triggered_by_id DROP NOT NULL;

ALTER TABLE candidate_discovery_runs ADD CONSTRAINT fk_candidate_discovery_runs_users_triggered_by_id FOREIGN KEY (triggered_by_id) REFERENCES users (id) ON DELETE SET NULL;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621173554_MakeTriggeredByIdNullable', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE capability_registries (
    capability_id character varying(100) NOT NULL,
    display_name character varying(255) NOT NULL,
    category character varying(100) NOT NULL,
    description character varying(1000) NOT NULL,
    taxonomy_version character varying(50) NOT NULL,
    capability_version character varying(20) NOT NULL,
    status character varying(30) NOT NULL,
    deprecated_by_id character varying(100),
    effective_date timestamp with time zone NOT NULL,
    migration_mappings jsonb,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_capability_registries PRIMARY KEY (capability_id),
    CONSTRAINT fk_capability_registries_capability_registries_deprecated_by_id FOREIGN KEY (deprecated_by_id) REFERENCES capability_registries (capability_id) ON DELETE SET NULL
);

CREATE TABLE capability_aliases (
    alias_name character varying(100) NOT NULL,
    canonical_id character varying(100) NOT NULL,
    CONSTRAINT pk_capability_aliases PRIMARY KEY (alias_name),
    CONSTRAINT fk_capability_aliases_capability_registries_canonical_id FOREIGN KEY (canonical_id) REFERENCES capability_registries (capability_id) ON DELETE CASCADE
);

CREATE TABLE capability_hierarchies (
    parent_id character varying(100) NOT NULL,
    child_id character varying(100) NOT NULL,
    CONSTRAINT pk_capability_hierarchies PRIMARY KEY (parent_id, child_id),
    CONSTRAINT fk_capability_hierarchies_capability_registries_child_id FOREIGN KEY (child_id) REFERENCES capability_registries (capability_id) ON DELETE CASCADE,
    CONSTRAINT fk_capability_hierarchies_capability_registries_parent_id FOREIGN KEY (parent_id) REFERENCES capability_registries (capability_id) ON DELETE CASCADE
);

CREATE INDEX ix_capability_aliases_canonical_id ON capability_aliases (canonical_id);

CREATE INDEX ix_capability_hierarchies_child_id ON capability_hierarchies (child_id);

CREATE INDEX ix_capability_registries_deprecated_by_id ON capability_registries (deprecated_by_id);

CREATE INDEX ix_capability_registries_status ON capability_registries (status);

CREATE INDEX ix_capability_registries_taxonomy_version ON capability_registries (taxonomy_version);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621175628_AddCapabilityRegistryAndGraphSchema', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE EXTENSION IF NOT EXISTS citext;
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE candidate_match_projections (
    candidate_id uuid NOT NULL,
    profile_summary character varying(1000),
    normalized_capabilities uuid[] NOT NULL,
    last_projected_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_match_projections PRIMARY KEY (candidate_id),
    CONSTRAINT fk_candidate_match_projections_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE candidate_search_profiles (
    candidate_id uuid NOT NULL,
    full_name character varying(255) NOT NULL,
    headline character varying(255),
    location character varying(100),
    trust_score integer NOT NULL,
    trust_tier character varying(30) NOT NULL,
    capabilities_json jsonb NOT NULL,
    search_embedding real[] NOT NULL,
    last_projected_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_search_profiles PRIMARY KEY (candidate_id),
    CONSTRAINT fk_candidate_search_profiles_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE capability_nodes (
    id uuid NOT NULL,
    name character varying(150) NOT NULL,
    slug character varying(150) NOT NULL,
    description character varying(1000),
    category character varying(50) NOT NULL,
    vector_embedding real[],
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_capability_nodes PRIMARY KEY (id)
);

CREATE TABLE evidence_sources (
    id uuid NOT NULL,
    name character varying(150) NOT NULL,
    provider_type character varying(50) NOT NULL,
    is_active boolean NOT NULL,
    connection_config jsonb,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_evidence_sources PRIMARY KEY (id)
);

CREATE TABLE matching_evaluations (
    id uuid NOT NULL,
    job_vacancy_id uuid NOT NULL,
    candidate_id uuid NOT NULL,
    aggregate_score integer NOT NULL,
    confidence_level character varying(30) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_matching_evaluations PRIMARY KEY (id),
    CONSTRAINT fk_matching_evaluations_job_vacancies_job_vacancy_id FOREIGN KEY (job_vacancy_id) REFERENCES job_vacancies (id) ON DELETE CASCADE,
    CONSTRAINT fk_matching_evaluations_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE trust_profiles (
    id uuid NOT NULL,
    target_entity_id uuid NOT NULL,
    target_type character varying(30) NOT NULL,
    recalculated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_trust_profiles PRIMARY KEY (id)
);

CREATE TABLE candidate_capabilities (
    id uuid NOT NULL,
    candidate_id uuid NOT NULL,
    capability_node_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_capabilities PRIMARY KEY (id),
    CONSTRAINT fk_candidate_capabilities_capability_nodes_capability_node_id FOREIGN KEY (capability_node_id) REFERENCES capability_nodes (id) ON DELETE CASCADE,
    CONSTRAINT fk_candidate_capabilities_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE capability_edges (
    source_node_id uuid NOT NULL,
    target_node_id uuid NOT NULL,
    relationship_type character varying(50) NOT NULL,
    weight double precision NOT NULL,
    CONSTRAINT pk_capability_edges PRIMARY KEY (source_node_id, target_node_id, relationship_type),
    CONSTRAINT fk_capability_edges_capability_nodes_source_node_id FOREIGN KEY (source_node_id) REFERENCES capability_nodes (id) ON DELETE CASCADE,
    CONSTRAINT fk_capability_edges_capability_nodes_target_node_id FOREIGN KEY (target_node_id) REFERENCES capability_nodes (id) ON DELETE CASCADE
);

CREATE TABLE evidence_artifacts (
    id uuid NOT NULL,
    source_id uuid NOT NULL,
    external_identifier character varying(500) NOT NULL,
    artifact_type character varying(50) NOT NULL,
    payload jsonb NOT NULL,
    cryptographic_signature character varying(512),
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_evidence_artifacts PRIMARY KEY (id),
    CONSTRAINT fk_evidence_artifacts_evidence_sources_source_id FOREIGN KEY (source_id) REFERENCES evidence_sources (id) ON DELETE CASCADE
);

CREATE TABLE matching_factors (
    id uuid NOT NULL,
    matching_evaluation_id uuid NOT NULL,
    factor_name character varying(100) NOT NULL,
    factor_score integer NOT NULL,
    weight double precision NOT NULL,
    CONSTRAINT pk_matching_factors PRIMARY KEY (id),
    CONSTRAINT fk_matching_factors_matching_evaluations_matching_evaluation_id FOREIGN KEY (matching_evaluation_id) REFERENCES matching_evaluations (id) ON DELETE CASCADE
);

CREATE TABLE candidate_trust_projections (
    candidate_id uuid NOT NULL,
    trust_profile_id uuid NOT NULL,
    aggregate_score integer NOT NULL,
    trust_tier character varying(30) NOT NULL,
    last_updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_trust_projections PRIMARY KEY (candidate_id),
    CONSTRAINT fk_candidate_trust_projections_trust_profiles_trust_profile_id FOREIGN KEY (trust_profile_id) REFERENCES trust_profiles (id) ON DELETE CASCADE,
    CONSTRAINT fk_candidate_trust_projections_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE trust_calculations (
    id uuid NOT NULL,
    trust_profile_id uuid NOT NULL,
    aggregate_score integer NOT NULL,
    calculation_details jsonb NOT NULL,
    calculated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_trust_calculations PRIMARY KEY (id),
    CONSTRAINT fk_trust_calculations_trust_profiles_trust_profile_id FOREIGN KEY (trust_profile_id) REFERENCES trust_profiles (id) ON DELETE CASCADE
);

CREATE TABLE trust_components (
    id uuid NOT NULL,
    trust_profile_id uuid NOT NULL,
    component_name character varying(100) NOT NULL,
    component_score integer NOT NULL,
    weight double precision NOT NULL,
    explanation_metadata jsonb,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_trust_components PRIMARY KEY (id),
    CONSTRAINT fk_trust_components_trust_profiles_trust_profile_id FOREIGN KEY (trust_profile_id) REFERENCES trust_profiles (id) ON DELETE CASCADE
);

CREATE TABLE candidate_capability_histories (
    id uuid NOT NULL,
    candidate_capability_id uuid NOT NULL,
    proficiency_score double precision NOT NULL,
    recorded_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_capability_histories PRIMARY KEY (id),
    CONSTRAINT fk_candidate_capability_histories_candidate_capabilities_candi FOREIGN KEY (candidate_capability_id) REFERENCES candidate_capabilities (id) ON DELETE CASCADE
);

CREATE TABLE candidate_capability_scores (
    candidate_capability_id uuid NOT NULL,
    expertise_level character varying(50) NOT NULL,
    proficiency_score double precision NOT NULL,
    recency_index double precision NOT NULL,
    calculated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_capability_scores PRIMARY KEY (candidate_capability_id),
    CONSTRAINT fk_candidate_capability_scores_candidate_capabilities_candidat FOREIGN KEY (candidate_capability_id) REFERENCES candidate_capabilities (id) ON DELETE CASCADE
);

CREATE TABLE candidate_capability_evidences (
    candidate_capability_id uuid NOT NULL,
    evidence_artifact_id uuid NOT NULL,
    added_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_capability_evidences PRIMARY KEY (candidate_capability_id, evidence_artifact_id),
    CONSTRAINT fk_candidate_capability_evidences_candidate_capabilities_candi FOREIGN KEY (candidate_capability_id) REFERENCES candidate_capabilities (id) ON DELETE CASCADE,
    CONSTRAINT fk_candidate_capability_evidences_evidence_artifacts_evidence_ FOREIGN KEY (evidence_artifact_id) REFERENCES evidence_artifacts (id) ON DELETE CASCADE
);

CREATE TABLE evidence_claims (
    id uuid NOT NULL,
    candidate_id uuid NOT NULL,
    evidence_artifact_id uuid NOT NULL,
    assertion_type character varying(50) NOT NULL,
    confidence_score double precision NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_evidence_claims PRIMARY KEY (id),
    CONSTRAINT fk_evidence_claims_evidence_artifacts_evidence_artifact_id FOREIGN KEY (evidence_artifact_id) REFERENCES evidence_artifacts (id) ON DELETE CASCADE,
    CONSTRAINT fk_evidence_claims_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE matching_explanations (
    id uuid NOT NULL,
    matching_evaluation_id uuid NOT NULL,
    explanation_type character varying(50) NOT NULL,
    capability_node_id uuid,
    assertion_text text NOT NULL,
    supporting_artifact_id uuid,
    CONSTRAINT pk_matching_explanations PRIMARY KEY (id),
    CONSTRAINT fk_matching_explanations_capability_nodes_capability_node_id FOREIGN KEY (capability_node_id) REFERENCES capability_nodes (id),
    CONSTRAINT fk_matching_explanations_evidence_artifacts_supporting_artifac FOREIGN KEY (supporting_artifact_id) REFERENCES evidence_artifacts (id),
    CONSTRAINT fk_matching_explanations_matching_evaluations_matching_evaluat FOREIGN KEY (matching_evaluation_id) REFERENCES matching_evaluations (id) ON DELETE CASCADE
);

CREATE TABLE evidence_verifications (
    id uuid NOT NULL,
    evidence_claim_id uuid NOT NULL,
    verification_type character varying(50) NOT NULL,
    status character varying(30) NOT NULL,
    verification_log jsonb,
    verified_at timestamp with time zone,
    expires_at timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_evidence_verifications PRIMARY KEY (id),
    CONSTRAINT fk_evidence_verifications_evidence_claims_evidence_claim_id FOREIGN KEY (evidence_claim_id) REFERENCES evidence_claims (id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX ix_candidate_capabilities_candidate_id_capability_node_id ON candidate_capabilities (candidate_id, capability_node_id);

CREATE INDEX ix_candidate_capabilities_capability_node_id ON candidate_capabilities (capability_node_id);

CREATE INDEX ix_candidate_capability_evidences_evidence_artifact_id ON candidate_capability_evidences (evidence_artifact_id);

CREATE INDEX ix_candidate_capability_histories_candidate_capability_id_reco ON candidate_capability_histories (candidate_capability_id, recorded_at);

CREATE INDEX ix_candidate_trust_projections_trust_profile_id ON candidate_trust_projections (trust_profile_id);

CREATE INDEX ix_capability_edges_target_node_id ON capability_edges (target_node_id);

CREATE UNIQUE INDEX ix_capability_nodes_slug ON capability_nodes (slug);

CREATE INDEX ix_evidence_artifacts_source_id_external_identifier ON evidence_artifacts (source_id, external_identifier);

CREATE UNIQUE INDEX ix_evidence_claims_candidate_id_evidence_artifact_id ON evidence_claims (candidate_id, evidence_artifact_id);

CREATE INDEX ix_evidence_claims_evidence_artifact_id ON evidence_claims (evidence_artifact_id);

CREATE INDEX ix_evidence_verifications_evidence_claim_id ON evidence_verifications (evidence_claim_id);

CREATE INDEX ix_matching_evaluations_candidate_id ON matching_evaluations (candidate_id);

CREATE UNIQUE INDEX ix_matching_evaluations_job_vacancy_id_candidate_id ON matching_evaluations (job_vacancy_id, candidate_id);

CREATE INDEX ix_matching_explanations_capability_node_id ON matching_explanations (capability_node_id);

CREATE INDEX ix_matching_explanations_matching_evaluation_id ON matching_explanations (matching_evaluation_id);

CREATE INDEX ix_matching_explanations_supporting_artifact_id ON matching_explanations (supporting_artifact_id);

CREATE INDEX ix_matching_factors_matching_evaluation_id ON matching_factors (matching_evaluation_id);

CREATE INDEX ix_trust_calculations_trust_profile_id ON trust_calculations (trust_profile_id);

CREATE INDEX ix_trust_components_trust_profile_id ON trust_components (trust_profile_id);

CREATE INDEX ix_trust_profiles_target_entity_id_target_type ON trust_profiles (target_entity_id, target_type);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621183546_AddTalentIntelligenceGraph', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE job_applications (
    id uuid NOT NULL,
    job_vacancy_id uuid NOT NULL,
    candidate_id uuid NOT NULL,
    status character varying(50) NOT NULL,
    gaps_snapshot_json jsonb,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_job_applications PRIMARY KEY (id),
    CONSTRAINT fk_job_applications_job_vacancies_job_vacancy_id FOREIGN KEY (job_vacancy_id) REFERENCES job_vacancies (id) ON DELETE CASCADE,
    CONSTRAINT fk_job_applications_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE job_interactions (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    job_vacancy_id uuid NOT NULL,
    interaction_type character varying(30) NOT NULL,
    interaction_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_job_interactions PRIMARY KEY (id),
    CONSTRAINT fk_job_interactions_job_vacancies_job_vacancy_id FOREIGN KEY (job_vacancy_id) REFERENCES job_vacancies (id) ON DELETE CASCADE,
    CONSTRAINT fk_job_interactions_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX idx_job_vacancies_published_active ON job_vacancies (status, is_active) WHERE status = 'Published' AND is_active = TRUE;

CREATE INDEX ix_job_applications_candidate_id ON job_applications (candidate_id);

CREATE UNIQUE INDEX ix_job_applications_job_vacancy_id_candidate_id ON job_applications (job_vacancy_id, candidate_id);

CREATE INDEX ix_job_interactions_job_vacancy_id ON job_interactions (job_vacancy_id);

CREATE INDEX ix_job_interactions_user_id_interaction_type ON job_interactions (user_id, interaction_type);

CREATE UNIQUE INDEX ix_job_interactions_user_id_job_vacancy_id_interaction_type ON job_interactions (user_id, job_vacancy_id, interaction_type);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621185145_AddPublicJobsAndInteractions', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE job_applications ADD eligibility_snapshot_json jsonb;

CREATE TABLE candidate_capability_projections (
    candidate_id uuid NOT NULL,
    capabilities_json jsonb NOT NULL,
    projected_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_capability_projections PRIMARY KEY (candidate_id),
    CONSTRAINT fk_candidate_capability_projections_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE candidate_evaluation_snapshots (
    candidate_id uuid NOT NULL,
    profile_completeness double precision NOT NULL,
    identity_trust_score double precision NOT NULL,
    evidence_trust_score double precision NOT NULL,
    verification_state character varying(50) NOT NULL,
    evaluated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_evaluation_snapshots PRIMARY KEY (candidate_id),
    CONSTRAINT fk_candidate_evaluation_snapshots_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260621194522_AddCandidateEvaluationSnapshotsAndProjections', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE candidate_ranking_projections (
    candidate_id uuid NOT NULL,
    full_name character varying(255) NOT NULL,
    username character varying(32),
    bio character varying(500),
    headline character varying(255),
    location character varying(100),
    avatar_url character varying(1000),
    composite_score double precision NOT NULL,
    ai_score double precision NOT NULL,
    trust_score double precision NOT NULL,
    profile_completeness double precision NOT NULL,
    evidence_trust_score double precision NOT NULL,
    verified_repo_count integer NOT NULL,
    total_stars_count integer NOT NULL,
    total_forks_count integer NOT NULL,
    verified_contribution_count integer NOT NULL,
    top_capabilities_json jsonb,
    primary_domain character varying(100),
    career_level_label character varying(50),
    followers_count integer NOT NULL,
    following_count integer NOT NULL,
    available_for_hire boolean NOT NULL,
    open_to_work_status character varying(20) NOT NULL,
    global_rank_position integer NOT NULL,
    previous_global_rank_position integer NOT NULL,
    last_updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_candidate_ranking_projections PRIMARY KEY (candidate_id),
    CONSTRAINT fk_candidate_ranking_projections_users_candidate_id FOREIGN KEY (candidate_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE user_followers (
    follower_id uuid NOT NULL,
    followee_id uuid NOT NULL,
    followed_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_user_followers PRIMARY KEY (follower_id, followee_id),
    CONSTRAINT fk_user_followers_users_followee_id FOREIGN KEY (followee_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT fk_user_followers_users_follower_id FOREIGN KEY (follower_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_user_followers_followee_id ON user_followers (followee_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260622183209_AddRankingAndFollows', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE workspaces ADD description character varying(1000);

ALTER TABLE workspaces ADD owner_id uuid;


                UPDATE workspaces w 
                SET owner_id = COALESCE(
                    (SELECT user_id FROM organization_memberships om WHERE om.organization_id = w.organization_id AND om.role = 'OWNER' AND om.status = 'active' LIMIT 1),
                    (SELECT user_id FROM organization_memberships om WHERE om.organization_id = w.organization_id AND om.status = 'active' LIMIT 1)
                );
            


                UPDATE workspaces
                SET owner_id = (SELECT id FROM users LIMIT 1)
                WHERE owner_id IS NULL;
            

ALTER TABLE workspaces ALTER COLUMN owner_id SET NOT NULL;

CREATE INDEX ix_workspaces_owner_id ON workspaces (owner_id);

ALTER TABLE workspaces ADD CONSTRAINT fk_workspaces_users_owner_id FOREIGN KEY (owner_id) REFERENCES users (id) ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260623181310_AddWorkspaceDescriptionAndOwner', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE candidate_skill_tree_nodes (
    id uuid NOT NULL,
    candidate_assessment_id uuid NOT NULL,
    parent_id uuid,
    display_name character varying(100) NOT NULL,
    category character varying(100) NOT NULL,
    proficiency_level character varying(50) NOT NULL,
    confidence_score double precision NOT NULL,
    estimated_experience_months double precision NOT NULL,
    supporting_evidence jsonb,
    CONSTRAINT pk_candidate_skill_tree_nodes PRIMARY KEY (id),
    CONSTRAINT fk_candidate_skill_tree_nodes_candidate_assessments_candidate_ FOREIGN KEY (candidate_assessment_id) REFERENCES candidate_assessments (id) ON DELETE CASCADE,
    CONSTRAINT fk_candidate_skill_tree_nodes_candidate_skill_tree_nodes_paren FOREIGN KEY (parent_id) REFERENCES candidate_skill_tree_nodes (id) ON DELETE RESTRICT
);

CREATE INDEX ix_candidate_skill_tree_nodes_candidate_assessment_id ON candidate_skill_tree_nodes (candidate_assessment_id);

CREATE INDEX ix_candidate_skill_tree_nodes_parent_id ON candidate_skill_tree_nodes (parent_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260628104720_AddCandidateSkillTreeNodes', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE user_profiles ADD ai_suggestions_json text;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260628120042_AddAiSuggestionsJsonToUserProfile', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE ai_streaming_sessions (
    id uuid NOT NULL,
    pipeline_id character varying(100) NOT NULL,
    user_id uuid,
    workspace_id uuid,
    status character varying(50) NOT NULL,
    progress double precision NOT NULL,
    current_step character varying(100),
    model_name character varying(100),
    provider character varying(100),
    started_at timestamp with time zone,
    completed_at timestamp with time zone,
    total_cost_usd numeric(10,6),
    total_input_tokens integer,
    total_output_tokens integer,
    error_message character varying(2000),
    summary_data jsonb,
    expected_outputs jsonb,
    pipeline_version character varying(50) NOT NULL,
    created_at_utc timestamp with time zone NOT NULL,
    last_updated_utc timestamp with time zone NOT NULL,
    CONSTRAINT pk_ai_streaming_sessions PRIMARY KEY (id),
    CONSTRAINT fk_ai_streaming_sessions_users_user_id FOREIGN KEY (user_id) REFERENCES users (id),
    CONSTRAINT fk_ai_streaming_sessions_workspaces_workspace_id FOREIGN KEY (workspace_id) REFERENCES workspaces (id)
);

CREATE TABLE ai_streaming_logs (
    id uuid NOT NULL,
    session_id uuid NOT NULL,
    stage_id character varying(100),
    log_level character varying(20) NOT NULL,
    component character varying(100),
    message text NOT NULL,
    timestamp timestamp with time zone NOT NULL,
    CONSTRAINT pk_ai_streaming_logs PRIMARY KEY (id),
    CONSTRAINT fk_ai_streaming_logs_ai_streaming_sessions_session_id FOREIGN KEY (session_id) REFERENCES ai_streaming_sessions (id) ON DELETE CASCADE
);

CREATE TABLE ai_streaming_metrics (
    id uuid NOT NULL,
    session_id uuid NOT NULL,
    stage_id character varying(100),
    metric_name character varying(100) NOT NULL,
    metric_value double precision NOT NULL,
    timestamp timestamp with time zone NOT NULL,
    CONSTRAINT pk_ai_streaming_metrics PRIMARY KEY (id),
    CONSTRAINT fk_ai_streaming_metrics_ai_streaming_sessions_session_id FOREIGN KEY (session_id) REFERENCES ai_streaming_sessions (id) ON DELETE CASCADE
);

CREATE TABLE ai_streaming_stages (
    id uuid NOT NULL,
    session_id uuid NOT NULL,
    stage_id character varying(100) NOT NULL,
    stage_name character varying(200) NOT NULL,
    parent_stage_id character varying(100),
    status character varying(50) NOT NULL,
    progress double precision NOT NULL,
    description character varying(1000),
    details jsonb,
    started_at timestamp with time zone,
    completed_at timestamp with time zone,
    duration_ms bigint,
    retry_count integer NOT NULL,
    CONSTRAINT pk_ai_streaming_stages PRIMARY KEY (id),
    CONSTRAINT fk_ai_streaming_stages_ai_streaming_sessions_session_id FOREIGN KEY (session_id) REFERENCES ai_streaming_sessions (id) ON DELETE CASCADE
);

CREATE INDEX ix_ai_streaming_logs_session_id ON ai_streaming_logs (session_id);

CREATE INDEX ix_ai_streaming_metrics_session_id ON ai_streaming_metrics (session_id);

CREATE INDEX ix_ai_streaming_sessions_user_id ON ai_streaming_sessions (user_id);

CREATE INDEX ix_ai_streaming_sessions_workspace_id ON ai_streaming_sessions (workspace_id);

CREATE INDEX ix_ai_streaming_stages_session_id ON ai_streaming_stages (session_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260628143050_AddAiStreamingTables', '10.0.9');

COMMIT;

START TRANSACTION;

                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'candidate_assessments' AND column_name = 'calculation_mode') THEN
                        ALTER TABLE candidate_assessments ADD COLUMN calculation_mode VARCHAR(50);
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'candidate_assessments' AND column_name = 'clone_risk_classification') THEN
                        ALTER TABLE candidate_assessments ADD COLUMN clone_risk_classification VARCHAR(50);
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'candidate_assessments' AND column_name = 'evidence_completeness') THEN
                        ALTER TABLE candidate_assessments ADD COLUMN evidence_completeness VARCHAR(50);
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'candidate_assessments' AND column_name = 'input_feature_set_hash') THEN
                        ALTER TABLE candidate_assessments ADD COLUMN input_feature_set_hash VARCHAR(100);
                    END IF;
                END $$;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260629103220_AddCandidateAssessmentMetadataColumns', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE user_profiles ALTER COLUMN bio TYPE character varying(1000);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260629114108_UpdateBioMaxLength', '10.0.9');

COMMIT;

START TRANSACTION;
ALTER TABLE candidate_assessments ADD professional_bio character varying(1000);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260629120025_AddProfessionalBioToAssessment', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE forum_badges (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(500) NOT NULL,
    icon_name character varying(50) NOT NULL,
    criteria_code character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_badges PRIMARY KEY (id)
);

CREATE TABLE forum_categories (
    id uuid NOT NULL,
    organization_id uuid,
    name character varying(100) NOT NULL,
    slug character varying(100) NOT NULL,
    description character varying(500),
    icon_name character varying(50),
    display_order integer NOT NULL,
    is_private boolean NOT NULL,
    is_archived boolean NOT NULL,
    required_role character varying(50),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_forum_categories PRIMARY KEY (id),
    CONSTRAINT fk_forum_categories_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES organizations (id)
);

CREATE TABLE forum_moderation_logs (
    id uuid NOT NULL,
    moderator_id uuid NOT NULL,
    target_type character varying(50) NOT NULL,
    target_id uuid NOT NULL,
    action character varying(50) NOT NULL,
    reason character varying(500),
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_moderation_logs PRIMARY KEY (id),
    CONSTRAINT fk_forum_moderation_logs_users_moderator_id FOREIGN KEY (moderator_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_reputations (
    user_id uuid NOT NULL,
    points integer NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_reputations PRIMARY KEY (user_id),
    CONSTRAINT fk_forum_reputations_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_tags (
    id uuid NOT NULL,
    name character varying(50) NOT NULL,
    slug character varying(50) NOT NULL,
    is_archived boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_tags PRIMARY KEY (id)
);

CREATE TABLE forum_user_badges (
    user_id uuid NOT NULL,
    badge_id uuid NOT NULL,
    awarded_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_user_badges PRIMARY KEY (user_id, badge_id),
    CONSTRAINT fk_forum_user_badges_forum_badges_badge_id FOREIGN KEY (badge_id) REFERENCES forum_badges (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_user_badges_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_category_moderators (
    category_id uuid NOT NULL,
    user_id uuid NOT NULL,
    assigned_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_category_moderators PRIMARY KEY (category_id, user_id),
    CONSTRAINT fk_forum_category_moderators_forum_categories_category_id FOREIGN KEY (category_id) REFERENCES forum_categories (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_category_moderators_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_topics (
    id uuid NOT NULL,
    category_id uuid NOT NULL,
    organization_id uuid,
    author_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    slug character varying(255) NOT NULL,
    content text NOT NULL,
    ai_excerpt text,
    view_count integer NOT NULL,
    reply_count integer NOT NULL,
    score integer NOT NULL,
    is_pinned boolean NOT NULL,
    is_locked boolean NOT NULL,
    is_solved boolean NOT NULL,
    is_featured boolean NOT NULL,
    is_archived boolean NOT NULL,
    is_pending_review boolean NOT NULL,
    last_activity_at timestamp with time zone NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_forum_topics PRIMARY KEY (id),
    CONSTRAINT fk_forum_topics_forum_categories_category_id FOREIGN KEY (category_id) REFERENCES forum_categories (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_topics_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES organizations (id),
    CONSTRAINT fk_forum_topics_users_author_id FOREIGN KEY (author_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_bookmarks (
    topic_id uuid NOT NULL,
    user_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_bookmarks PRIMARY KEY (topic_id, user_id),
    CONSTRAINT fk_forum_bookmarks_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_bookmarks_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_follows (
    topic_id uuid NOT NULL,
    user_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_follows PRIMARY KEY (topic_id, user_id),
    CONSTRAINT fk_forum_follows_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_follows_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_replies (
    id uuid NOT NULL,
    topic_id uuid NOT NULL,
    author_id uuid NOT NULL,
    parent_reply_id uuid,
    content text NOT NULL,
    quote_text character varying(2000),
    is_accepted_solution boolean NOT NULL,
    score integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_forum_replies PRIMARY KEY (id),
    CONSTRAINT fk_forum_replies_forum_replies_parent_reply_id FOREIGN KEY (parent_reply_id) REFERENCES forum_replies (id),
    CONSTRAINT fk_forum_replies_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_replies_users_author_id FOREIGN KEY (author_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_topic_histories (
    id uuid NOT NULL,
    topic_id uuid NOT NULL,
    edited_by_id uuid NOT NULL,
    title character varying(255) NOT NULL,
    content text NOT NULL,
    edited_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_topic_histories PRIMARY KEY (id),
    CONSTRAINT fk_forum_topic_histories_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_topic_histories_users_edited_by_id FOREIGN KEY (edited_by_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_topic_tags (
    topic_id uuid NOT NULL,
    tag_id uuid NOT NULL,
    CONSTRAINT pk_forum_topic_tags PRIMARY KEY (topic_id, tag_id),
    CONSTRAINT fk_forum_topic_tags_forum_tags_tag_id FOREIGN KEY (tag_id) REFERENCES forum_tags (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_topic_tags_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE
);

CREATE TABLE forum_reactions (
    id uuid NOT NULL,
    topic_id uuid,
    reply_id uuid,
    user_id uuid NOT NULL,
    reaction_type character varying(50) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_reactions PRIMARY KEY (id),
    CONSTRAINT fk_forum_reactions_forum_replies_reply_id FOREIGN KEY (reply_id) REFERENCES forum_replies (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_reactions_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_reactions_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_reply_histories (
    id uuid NOT NULL,
    reply_id uuid NOT NULL,
    edited_by_id uuid NOT NULL,
    content text NOT NULL,
    edited_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_reply_histories PRIMARY KEY (id),
    CONSTRAINT fk_forum_reply_histories_forum_replies_reply_id FOREIGN KEY (reply_id) REFERENCES forum_replies (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_reply_histories_users_edited_by_id FOREIGN KEY (edited_by_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE forum_reports (
    id uuid NOT NULL,
    topic_id uuid,
    reply_id uuid,
    reported_user_id uuid,
    reporter_user_id uuid NOT NULL,
    reason character varying(500) NOT NULL,
    status character varying(30) NOT NULL,
    resolution_notes character varying(1000),
    created_at timestamp with time zone NOT NULL,
    resolved_at timestamp with time zone,
    resolved_by_id uuid,
    CONSTRAINT pk_forum_reports PRIMARY KEY (id),
    CONSTRAINT fk_forum_reports_forum_replies_reply_id FOREIGN KEY (reply_id) REFERENCES forum_replies (id),
    CONSTRAINT fk_forum_reports_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id),
    CONSTRAINT fk_forum_reports_users_reported_user_id FOREIGN KEY (reported_user_id) REFERENCES users (id),
    CONSTRAINT fk_forum_reports_users_reporter_user_id FOREIGN KEY (reporter_user_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_reports_users_resolved_by_id FOREIGN KEY (resolved_by_id) REFERENCES users (id)
);

CREATE TABLE forum_votes (
    id uuid NOT NULL,
    topic_id uuid,
    reply_id uuid,
    user_id uuid NOT NULL,
    vote_type character varying(20) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_forum_votes PRIMARY KEY (id),
    CONSTRAINT fk_forum_votes_forum_replies_reply_id FOREIGN KEY (reply_id) REFERENCES forum_replies (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_votes_forum_topics_topic_id FOREIGN KEY (topic_id) REFERENCES forum_topics (id) ON DELETE CASCADE,
    CONSTRAINT fk_forum_votes_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_forum_bookmarks_user_id ON forum_bookmarks (user_id);

CREATE INDEX ix_forum_categories_organization_id ON forum_categories (organization_id);

CREATE INDEX ix_forum_category_moderators_user_id ON forum_category_moderators (user_id);

CREATE INDEX ix_forum_follows_user_id ON forum_follows (user_id);

CREATE INDEX ix_forum_moderation_logs_moderator_id ON forum_moderation_logs (moderator_id);

CREATE INDEX ix_forum_reactions_reply_id ON forum_reactions (reply_id);

CREATE INDEX ix_forum_reactions_topic_id ON forum_reactions (topic_id);

CREATE INDEX ix_forum_reactions_user_id ON forum_reactions (user_id);

CREATE INDEX ix_forum_replies_author_id ON forum_replies (author_id);

CREATE INDEX ix_forum_replies_parent_reply_id ON forum_replies (parent_reply_id);

CREATE INDEX ix_forum_replies_topic_id_parent_reply_id_created_at ON forum_replies (topic_id, parent_reply_id, created_at);

CREATE INDEX ix_forum_reply_histories_edited_by_id ON forum_reply_histories (edited_by_id);

CREATE INDEX ix_forum_reply_histories_reply_id ON forum_reply_histories (reply_id);

CREATE INDEX ix_forum_reports_reply_id ON forum_reports (reply_id);

CREATE INDEX ix_forum_reports_reported_user_id ON forum_reports (reported_user_id);

CREATE INDEX ix_forum_reports_reporter_user_id ON forum_reports (reporter_user_id);

CREATE INDEX ix_forum_reports_resolved_by_id ON forum_reports (resolved_by_id);

CREATE INDEX ix_forum_reports_topic_id ON forum_reports (topic_id);

CREATE UNIQUE INDEX ix_forum_tags_name ON forum_tags (name);

CREATE UNIQUE INDEX ix_forum_tags_slug ON forum_tags (slug);

CREATE INDEX ix_forum_topic_histories_edited_by_id ON forum_topic_histories (edited_by_id);

CREATE INDEX ix_forum_topic_histories_topic_id ON forum_topic_histories (topic_id);

CREATE INDEX ix_forum_topic_tags_tag_id ON forum_topic_tags (tag_id);

CREATE INDEX ix_forum_topics_author_id ON forum_topics (author_id);

CREATE INDEX ix_forum_topics_category_id_is_pinned_created_at ON forum_topics (category_id, is_pinned, created_at);

CREATE INDEX ix_forum_topics_organization_id_created_at ON forum_topics (organization_id, created_at);

CREATE UNIQUE INDEX ix_forum_topics_slug ON forum_topics (slug);

CREATE INDEX ix_forum_user_badges_badge_id ON forum_user_badges (badge_id);

CREATE INDEX ix_forum_votes_reply_id ON forum_votes (reply_id);

CREATE INDEX ix_forum_votes_topic_id ON forum_votes (topic_id);

CREATE INDEX ix_forum_votes_user_id ON forum_votes (user_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260701142634_AddForumModule', '10.0.9');

COMMIT;

START TRANSACTION;
CREATE TABLE user_cv_settings (
    user_id uuid NOT NULL,
    cv_template_id character varying(50) NOT NULL,
    cv_theme_color character varying(50),
    is_cv_published boolean NOT NULL,
    cv_layout_config_json text,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_user_cv_settings PRIMARY KEY (user_id),
    CONSTRAINT fk_user_cv_settings_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260707095049_AddUserCvSettings', '10.0.9');

COMMIT;

