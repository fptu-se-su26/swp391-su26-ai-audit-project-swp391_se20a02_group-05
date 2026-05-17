-- =========================================================
-- 1. EXTENSIONS
-- =========================================================
-- Enable cryptographic functions for UUID generation and password hashing (e.g., bcrypt/blowfish)
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
-- Enable case-insensitive text data type, ideal for unique email addresses
CREATE EXTENSION IF NOT EXISTS "citext";

-- =========================================================
-- 2. FUNCTIONS & TRIGGERS
-- =========================================================
-- Generic trigger function to automatically update the 'updated_at' column on row modification
CREATE OR REPLACE FUNCTION fn_update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =========================================================
-- 3. ENUMS & TYPES
-- =========================================================
-- Defines the lifecycle states of a user account
CREATE TYPE user_status AS ENUM (
    'EMAIL_VERIFY_PENDING', -- Account created but email not yet confirmed
    'ACTIVE',               -- Fully functional account
    'SUSPENDED',            -- Temporarily disabled (e.g., by admin)
    'BANNED',               -- Permanently restricted
    'DELETED'               -- Soft-deleted account
);

-- =========================================================
-- 4. ROLES TABLE
-- =========================================================
-- Stores user roles for the Role-Based Access Control (RBAC) system
CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL UNIQUE,          -- Internal identifier (e.g., 'SUPER_ADMIN')
    display_name VARCHAR(100) NOT NULL,        -- User-friendly name
    description TEXT,
    
    is_system BOOLEAN NOT NULL DEFAULT FALSE,  -- Prevents deletion of core system roles
    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMP WITH TIME ZONE        -- Support for soft deletion
);

-- =========================================================
-- 5. USERS TABLE
-- =========================================================
-- Core table storing user credentials, profile data, and security logs
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    role_id UUID NOT NULL,
    
    -- Identity & Credentials
    email CITEXT NOT NULL UNIQUE,              -- Case-insensitive unique email
    password_hash TEXT NOT NULL,               -- Hashed password (never store plain text)
    full_name VARCHAR(255) NOT NULL,
    avatar_url TEXT,
    status user_status NOT NULL DEFAULT 'EMAIL_VERIFY_PENDING',

    -- Verification tracking
    email_verified_at TIMESTAMP WITH TIME ZONE,
    
    -- Security & Brute-force protection tracking
    last_login_at TIMESTAMP WITH TIME ZONE,
    last_login_ip INET,                        -- Stores IPv4/IPv6 addresses
    failed_attempts INT DEFAULT 0,             -- Counter for consecutive failed logins
    last_failed_at TIMESTAMP WITH TIME ZONE,   -- Timestamp of the last failed attempt
    lock_until TIMESTAMP WITH TIME ZONE,       -- Account lockout expiration time

    -- Audit trails
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMP WITH TIME ZONE,

    -- Restrict deletion of roles that are currently assigned to users
    CONSTRAINT fk_users_role FOREIGN KEY (role_id) 
        REFERENCES roles(id) ON DELETE RESTRICT
);

-- =========================================================
-- 6. PERMISSIONS TABLE
-- =========================================================
-- Granular permissions using a hierarchical naming convention
CREATE TABLE permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    -- Naming convention: module:feature:action (e.g., "auth:user:create")
    name VARCHAR(150) NOT NULL UNIQUE,
    display_name VARCHAR(150) NOT NULL,
    description TEXT,
    
    module VARCHAR(50) NOT NULL,               -- Logical grouping (e.g., 'auth', 'billing')
    is_system BOOLEAN NOT NULL DEFAULT TRUE,   -- Protects core application permissions

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- =========================================================
-- 7. JUNCTION TABLES & TOKENS
-- =========================================================
-- Maps permissions to roles (Many-to-Many relationship)
CREATE TABLE role_permissions (
    role_id UUID NOT NULL,
    permission_id UUID NOT NULL,
    assigned_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    PRIMARY KEY (role_id, permission_id),
    -- Cascades delete: if a role or permission is removed, the mapping is removed
    CONSTRAINT fk_role_permissions_role FOREIGN KEY (role_id) 
        REFERENCES roles(id) ON DELETE CASCADE,
    CONSTRAINT fk_role_permissions_permission FOREIGN KEY (permission_id) 
        REFERENCES permissions(id) ON DELETE CASCADE
);

-- Manages long-lived refresh tokens for maintaining user sessions securely
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    token VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    revoked_at TIMESTAMP WITH TIME ZONE,       -- Marks token as invalid before expiration
    replaced_by_token VARCHAR(255),            -- Tracks token rotation chains for security

    CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE CASCADE
);

-- Optimize queries for token validation and user session retrieval
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token);
CREATE INDEX idx_refresh_tokens_user ON refresh_tokens(user_id);

-- =========================================================
-- 8. TRIGGERS REGISTRATION
-- =========================================================
-- Attach the auto-update timestamp function to relevant tables
CREATE TRIGGER tr_roles_timestamp BEFORE UPDATE ON roles 
    FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();

CREATE TRIGGER tr_users_timestamp BEFORE UPDATE ON users 
    FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();

CREATE TRIGGER tr_permissions_timestamp BEFORE UPDATE ON permissions 
    FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();

-- =========================================================
-- 9. INITIAL DATA (SEEDING)
-- =========================================================

-- Bootstrap essential system roles
INSERT INTO roles (name, display_name, description, is_system)
VALUES 
    ('SUPER_ADMIN', 'System Administrator', 'Root access to all modules', TRUE),
    ('USER', 'General User', 'Basic application access', TRUE);

-- Bootstrap root wildcard permission
INSERT INTO permissions (name, display_name, description, module, is_system)
VALUES 
    ('*:*:*', 'Global Wildcard', 'Full access to every module and feature', 'system', TRUE);

-- Bind wildcard permission to the Super Admin role
INSERT INTO role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM roles r, permissions p 
WHERE r.name = 'SUPER_ADMIN' AND p.name = '*:*:*';

-- Provision the master administrator account
-- Uses Blowfish ('bf') algorithm with a cost factor of 10 for secure hashing
INSERT INTO users (
    role_id, 
    email, 
    password_hash, 
    full_name, 
    status, 
    email_verified_at
)
VALUES (
    (SELECT id FROM roles WHERE name = 'SUPER_ADMIN'),
    'admin@system.com',
    crypt('SuperAdminPassword123', gen_salt('bf', 10)),
    'System Administrator',
    'ACTIVE',
    NOW()
);

-- =========================================================
-- 10. OPTIMIZED INDEXES
-- =========================================================
-- Partial index to speed up queries fetching only active, non-deleted users
CREATE INDEX idx_users_active ON users(status) WHERE deleted_at IS NULL;

-- Operator class 'varchar_pattern_ops' optimizes LIKE queries (e.g., 'auth:%')
CREATE INDEX idx_permissions_hierarchy ON permissions (name varchar_pattern_ops);