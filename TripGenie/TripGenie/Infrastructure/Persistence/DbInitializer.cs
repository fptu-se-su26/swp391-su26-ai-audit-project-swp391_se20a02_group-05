using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace TripGenie.API.Infrastructure.Persistence;

/// <summary>
/// A utility class responsible for automatic, idempotent database schema initialization and patching.
/// This ensures that the Postgres database matches our EF Core entities without manual database operations.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // 1. Fail fast if we cannot connect to the database
        if (!await context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException("Database connectivity check failed. Please ensure PostgreSQL is running and the connection string is correct.");
        }

        // 2. Execute idempotent PostgreSQL schema updates
        const string sql = @"
            -- Enable cryptographic functions for UUID generation and password hashing
            CREATE EXTENSION IF NOT EXISTS ""pgcrypto"";
            -- Enable case-insensitive text data type, ideal for unique email addresses
            CREATE EXTENSION IF NOT EXISTS ""citext"";

            -- Generic trigger function to automatically update the 'updated_at' column on row modification
            CREATE OR REPLACE FUNCTION fn_update_timestamp()
            RETURNS TRIGGER AS $$
            BEGIN
                NEW.updated_at = NOW();
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;

            -- Defines the lifecycle states of a user account
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_status') THEN
                    CREATE TYPE user_status AS ENUM (
                        'EMAIL_VERIFY_PENDING',
                        'ACTIVE',
                        'SUSPENDED',
                        'BANNED',
                        'DELETED'
                    );
                END IF;
            END $$;

            -- Stores user roles for the Role-Based Access Control (RBAC) system
            CREATE TABLE IF NOT EXISTS roles (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                name VARCHAR(50) NOT NULL UNIQUE,
                display_name VARCHAR(100) NOT NULL,
                description TEXT,
                is_system BOOLEAN NOT NULL DEFAULT FALSE,
                is_active BOOLEAN NOT NULL DEFAULT TRUE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                deleted_at TIMESTAMP WITH TIME ZONE
            );

            -- Core table storing user credentials, profile data, and security logs
            CREATE TABLE IF NOT EXISTS users (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                role_id UUID NOT NULL,
                email CITEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL,
                full_name VARCHAR(255) NOT NULL,
                avatar_url TEXT,
                status user_status NOT NULL DEFAULT 'EMAIL_VERIFY_PENDING',
                email_verified_at TIMESTAMP WITH TIME ZONE,
                last_login_at TIMESTAMP WITH TIME ZONE,
                last_login_ip INET,
                failed_attempts INT DEFAULT 0,
                last_failed_at TIMESTAMP WITH TIME ZONE,
                lock_until TIMESTAMP WITH TIME ZONE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                deleted_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_users_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE RESTRICT
            );

            -- Granular permissions using a hierarchical naming convention
            CREATE TABLE IF NOT EXISTS permissions (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                name VARCHAR(150) NOT NULL UNIQUE,
                display_name VARCHAR(150) NOT NULL,
                description TEXT,
                module VARCHAR(50) NOT NULL,
                is_system BOOLEAN NOT NULL DEFAULT TRUE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
            );

            -- Maps permissions to roles (Many-to-Many relationship)
            CREATE TABLE IF NOT EXISTS role_permissions (
                role_id UUID NOT NULL,
                permission_id UUID NOT NULL,
                assigned_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
                PRIMARY KEY (role_id, permission_id),
                CONSTRAINT fk_role_permissions_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE,
                CONSTRAINT fk_role_permissions_permission FOREIGN KEY (permission_id) REFERENCES permissions(id) ON DELETE CASCADE
            );

            -- Manages long-lived refresh tokens for maintaining user sessions securely
            CREATE TABLE IF NOT EXISTS refresh_tokens (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                user_id UUID NOT NULL,
                token VARCHAR(255) NOT NULL,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                revoked_at TIMESTAMP WITH TIME ZONE,
                replaced_by_token VARCHAR(255),
                user_agent VARCHAR(500),
                ip_address VARCHAR(45),
                CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Ensure ip_address column exists on refresh_tokens (in case table was created from older schema)
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name='refresh_tokens' AND column_name='ip_address'
                ) THEN
                    ALTER TABLE refresh_tokens ADD COLUMN ip_address VARCHAR(45);
                END IF;
            END $$;

            -- Ensure user_agent column exists on refresh_tokens
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name='refresh_tokens' AND column_name='user_agent'
                ) THEN
                    ALTER TABLE refresh_tokens ADD COLUMN user_agent VARCHAR(500);
                END IF;
            END $$;

            -- Manages one-time-use email verification tokens
            CREATE TABLE IF NOT EXISTS verification_tokens (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                user_id UUID NOT NULL,
                token_hash VARCHAR(255) NOT NULL UNIQUE,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                consumed_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_verification_tokens_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Manages one-time-use password reset tokens
            CREATE TABLE IF NOT EXISTS reset_password_tokens (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                user_id UUID NOT NULL,
                token_hash VARCHAR(255) NOT NULL UNIQUE,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                consumed_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_reset_password_tokens_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Outbox Pattern Table for reliable asynchronous email delivery
            CREATE TABLE IF NOT EXISTS outbox_messages (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                type VARCHAR(100) NOT NULL,
                payload TEXT NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                processed_at TIMESTAMP WITH TIME ZONE,
                error TEXT
            );

            -- Security Audit Logs Table for tracking major events
            CREATE TABLE IF NOT EXISTS audit_logs (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                user_id UUID,
                event_type VARCHAR(100) NOT NULL,
                description TEXT NOT NULL,
                ip_address VARCHAR(45),
                user_agent VARCHAR(500),
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT fk_audit_logs_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
            );

            -- Optimized Indexes
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token ON refresh_tokens(token);
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user ON refresh_tokens(user_id);
            CREATE INDEX IF NOT EXISTS idx_verification_tokens_active ON verification_tokens(token_hash) WHERE consumed_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_reset_password_tokens_active ON reset_password_tokens(token_hash) WHERE consumed_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_outbox_messages_pending ON outbox_messages(created_at) WHERE processed_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_users_active ON users(status) WHERE deleted_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_permissions_hierarchy ON permissions (name varchar_pattern_ops);

            -- Triggers Registration
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'tr_roles_timestamp') THEN
                    CREATE TRIGGER tr_roles_timestamp BEFORE UPDATE ON roles 
                        FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();
                END IF;
            END $$;

            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'tr_users_timestamp') THEN
                    CREATE TRIGGER tr_users_timestamp BEFORE UPDATE ON users 
                        FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();
                END IF;
            END $$;

            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'tr_permissions_timestamp') THEN
                    CREATE TRIGGER tr_permissions_timestamp BEFORE UPDATE ON permissions 
                        FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();
                END IF;
            END $$;

            -- Initial Data (Seeding)
            INSERT INTO roles (name, display_name, description, is_system)
            VALUES 
                ('SUPER_ADMIN', 'System Administrator', 'Root access to all modules', TRUE),
                ('USER', 'General User', 'Basic application access', TRUE)
            ON CONFLICT (name) DO NOTHING;

            INSERT INTO permissions (name, display_name, description, module, is_system)
            VALUES 
                ('*:*:*', 'Global Wildcard', 'Full access to every module and feature', 'system', TRUE)
            ON CONFLICT (name) DO NOTHING;

            INSERT INTO role_permissions (role_id, permission_id)
            SELECT r.id, p.id FROM roles r, permissions p 
            WHERE r.name = 'SUPER_ADMIN' AND p.name = '*:*:*'
            ON CONFLICT DO NOTHING;

            -- Provision the master administrator account if it doesn't exist
            INSERT INTO users (
                role_id, 
                email, 
                password_hash, 
                full_name, 
                status, 
                email_verified_at
            )
            SELECT 
                (SELECT id FROM roles WHERE name = 'SUPER_ADMIN'),
                'admin@system.com',
                crypt('SuperAdminPassword123', gen_salt('bf', 10)),
                'System Administrator',
                'ACTIVE',
                NOW()
            WHERE NOT EXISTS (SELECT 1 FROM users WHERE email = 'admin@system.com');
        ";

        await context.Database.ExecuteSqlRawAsync(sql);
    }
}
