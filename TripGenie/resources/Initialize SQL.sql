-- =========================================================
-- EXTENSIONS
-- =========================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =========================================================
-- USER STATUS ENUM
-- =========================================================

CREATE TYPE user_status AS ENUM (
    'EMAIL_VERIFY_PENDING',
    'ACTIVE',
    'SUSPENDED',
    'BANNED',
    'DELETED'
);



-- =========================================================
-- ROLES TABLE
-- =========================================================

CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL UNIQUE,

    display_name VARCHAR(100) NOT NULL,
    description TEXT,

    -- System role cannot be deleted easily
    is_system BOOLEAN NOT NULL DEFAULT FALSE,

    -- Soft disable role
    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMP WITH TIME ZONE
);



-- =========================================================
-- USERS TABLE
-- =========================================================

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
	
    role_id UUID NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,


    full_name VARCHAR(255) NOT NULL,
    avatar_url TEXT,

    status user_status NOT NULL DEFAULT 'EMAIL_VERIFY_PENDING',

    email_verified_at TIMESTAMP WITH TIME ZONE,
    last_login_at TIMESTAMP WITH TIME ZONE,

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMP WITH TIME ZONE,

    -- =====================================================
    -- CONSTRAINTS
    -- =====================================================

    CONSTRAINT fk_users_role
        FOREIGN KEY (role_id)
        REFERENCES roles(id)
        ON DELETE RESTRICT
);


-- =========================================================
-- PERMISSIONS TABLE
-- =========================================================

CREATE TABLE permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(150) NOT NULL UNIQUE,

    display_name VARCHAR(150) NOT NULL,
    description TEXT,

    -- Grouping/module
    module VARCHAR(50) NOT NULL,

    -- System permission
    is_system BOOLEAN NOT NULL DEFAULT TRUE,

    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMP WITH TIME ZONE
);



-- =========================================================
-- ROLE PERMISSIONS TABLE
-- =========================================================

CREATE TABLE role_permissions (
    role_id UUID NOT NULL,
    permission_id UUID NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

    PRIMARY KEY (role_id, permission_id),

    CONSTRAINT fk_role_permissions_role
        FOREIGN KEY (role_id)
        REFERENCES roles(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_role_permissions_permission
        FOREIGN KEY (permission_id)
        REFERENCES permissions(id)
        ON DELETE CASCADE
);



-- =========================================================
-- INDEXES
-- =========================================================

CREATE INDEX idx_users_email
ON users(email);

CREATE INDEX idx_users_role_id
ON users(role_id);

CREATE INDEX idx_users_status
ON users(status);

CREATE INDEX idx_roles_name
ON roles(name);

CREATE INDEX idx_permissions_name
ON permissions(name);

CREATE INDEX idx_permissions_module
ON permissions(module);

-- =========================================================
-- DEFAULT ROLES
-- =========================================================

INSERT INTO roles (
    name,
    display_name,
    description,
    is_system
)
VALUES
(
    'USER',
    'User',
    'Default system user',
    TRUE
),
(
    'SUPER_ADMIN',
    'System Administrator',
    'System administrator with full access',
    TRUE
);

-- =========================================================
-- DEFAULT PERMISSIONS
-- =========================================================

INSERT INTO permissions (
    name,
    display_name,
    description,
    module
)
VALUES
(
    '*',
    'Full System Access',
    'Grant all permissions across the entire system',
    'system'
);

-- =========================================================
-- ASSIGN ALL PERMISSIONS TO ADMIN
-- =========================================================

INSERT INTO role_permissions (
    role_id,
    permission_id
)
SELECT
    r.id,
    p.id
FROM roles r
CROSS JOIN permissions p
WHERE r.name = 'ADMIN';