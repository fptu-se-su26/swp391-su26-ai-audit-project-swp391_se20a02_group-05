using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CVerify.API.Infrastructure.Persistence;

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

        // 1b. Environment-guarded destructive reset (Development or specific environments only)
        var resetDbEnv = Environment.GetEnvironmentVariable("RESET_DATABASE");
        bool shouldReset = string.Equals(resetDbEnv, "true", StringComparison.OrdinalIgnoreCase);
        if (shouldReset)
        {
            const string dropSql = @"
                DROP TABLE IF EXISTS messages CASCADE;
                DROP TABLE IF EXISTS conversations CASCADE;
                DROP TABLE IF EXISTS audit_logs CASCADE;
                DROP TABLE IF EXISTS outbox_messages CASCADE;
                DROP TABLE IF EXISTS reset_password_tokens CASCADE;
                DROP TABLE IF EXISTS verification_tokens CASCADE;
                DROP TABLE IF EXISTS refresh_tokens CASCADE;
                DROP TABLE IF EXISTS role_permissions CASCADE;
                DROP TABLE IF EXISTS permissions CASCADE;
                DROP TABLE IF EXISTS user_roles CASCADE;
                DROP TABLE IF EXISTS verification_links CASCADE;
                DROP TABLE IF EXISTS otp_verifications CASCADE;
                DROP TABLE IF EXISTS organization_members CASCADE;
                DROP TABLE IF EXISTS organizations CASCADE;
                DROP TABLE IF EXISTS password_credentials CASCADE;
                DROP TABLE IF EXISTS auth_providers CASCADE;
                DROP TABLE IF EXISTS users CASCADE;
                DROP TABLE IF EXISTS roles CASCADE;
                DROP TYPE IF EXISTS user_status CASCADE;
            ";
            await context.Database.ExecuteSqlRawAsync(dropSql);
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
                id UUID PRIMARY KEY,
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
                id UUID PRIMARY KEY,
                email CITEXT NOT NULL,
                password_hash TEXT,
                full_name VARCHAR(255) NOT NULL,
                avatar_url TEXT,
                status user_status NOT NULL DEFAULT 'EMAIL_VERIFY_PENDING',
                email_verified_at TIMESTAMP WITH TIME ZONE,
                last_login_at TIMESTAMP WITH TIME ZONE,
                last_login_ip INET,
                failed_attempts INT DEFAULT 0,
                last_failed_at TIMESTAMP WITH TIME ZONE,
                lock_until TIMESTAMP WITH TIME ZONE,
                session_version INTEGER NOT NULL DEFAULT 1,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                deleted_at TIMESTAMP WITH TIME ZONE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email_active ON users(email) WHERE deleted_at IS NULL;

            -- Stores authentication provider linkage information
            CREATE TABLE IF NOT EXISTS auth_providers (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL,
                provider_name VARCHAR(50) NOT NULL,
                provider_key VARCHAR(255) NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                deleted_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_auth_providers_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS idx_auth_providers_key_active ON auth_providers(provider_name, provider_key) WHERE deleted_at IS NULL;
            CREATE UNIQUE INDEX IF NOT EXISTS idx_auth_providers_user_type_active ON auth_providers(user_id, provider_name) WHERE deleted_at IS NULL;

            -- Stores active/inactive historical password credentials to prevent reuse
            CREATE TABLE IF NOT EXISTS password_credentials (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL,
                password_hash TEXT NOT NULL,
                is_active BOOLEAN NOT NULL DEFAULT TRUE,
                revoked_at TIMESTAMP WITH TIME ZONE,
                revoked_reason VARCHAR(255),
                password_changed_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                deleted_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_password_credentials_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Stores verified organization workspaces
            CREATE TABLE IF NOT EXISTS organizations (
                id UUID PRIMARY KEY,
                name VARCHAR(255) NOT NULL,
                tax_code VARCHAR(50) NOT NULL,
                email VARCHAR(255) NOT NULL,
                username VARCHAR(100) NOT NULL,
                is_verified BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                deleted_at TIMESTAMP WITH TIME ZONE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS idx_organizations_username_active ON organizations(username) WHERE deleted_at IS NULL;
            CREATE UNIQUE INDEX IF NOT EXISTS idx_organizations_tax_code_active ON organizations(tax_code) WHERE deleted_at IS NULL;

            -- Stores organization workspace membership records
            CREATE TABLE IF NOT EXISTS organization_members (
                id UUID PRIMARY KEY,
                organization_id UUID NOT NULL,
                user_id UUID NOT NULL,
                role VARCHAR(50) NOT NULL,
                joined_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT fk_organization_members_organization FOREIGN KEY (organization_id) REFERENCES organizations(id) ON DELETE CASCADE,
                CONSTRAINT fk_organization_members_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Stores challenge-based OTP verifications
            CREATE TABLE IF NOT EXISTS otp_verifications (
                id UUID PRIMARY KEY,
                challenge_id UUID NOT NULL,
                email VARCHAR(255) NOT NULL,
                otp_hash VARCHAR(255) NOT NULL,
                purpose VARCHAR(100) NOT NULL,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                consumed_at TIMESTAMP WITH TIME ZONE,
                attempts INTEGER DEFAULT 0,
                last_attempt_at TIMESTAMP WITH TIME ZONE,
                resend_count INTEGER DEFAULT 0,
                last_sent_at TIMESTAMP WITH TIME ZONE,
                last_resent_at TIMESTAMP WITH TIME ZONE
            );
            CREATE INDEX IF NOT EXISTS idx_otp_verifications_challenge_id ON otp_verifications(challenge_id);
            CREATE INDEX IF NOT EXISTS idx_otp_verifications_email ON otp_verifications(email);

            -- Stores telemetry-tracked verification links for company email ownership
            CREATE TABLE IF NOT EXISTS verification_links (
                id UUID PRIMARY KEY,
                email VARCHAR(255) NOT NULL,
                tax_code VARCHAR(50),
                company_name VARCHAR(255),
                token_hash VARCHAR(255) NOT NULL,
                purpose VARCHAR(100) NOT NULL,
                user_id UUID,
                organization_id UUID,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                consumed_at TIMESTAMP WITH TIME ZONE,
                consumed_by_ip VARCHAR(45),
                consumed_by_user_agent VARCHAR(500),
                deleted_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_verification_links_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
                CONSTRAINT fk_verification_links_organization FOREIGN KEY (organization_id) REFERENCES organizations(id) ON DELETE SET NULL
            );
            CREATE INDEX IF NOT EXISTS idx_verification_links_active ON verification_links(token_hash) WHERE deleted_at IS NULL AND consumed_at IS NULL;

            -- Maps users to roles (Many-to-Many relationship)
            CREATE TABLE IF NOT EXISTS user_roles (
                user_id UUID NOT NULL,
                role_id UUID NOT NULL,
                assigned_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
                PRIMARY KEY (user_id, role_id),
                CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
                CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE
            );

            -- Backward-compatibility patch block: migrate old single user role layout if detected
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'role_id'
                ) THEN
                    -- Copy existing single-role relations to junction table
                    INSERT INTO user_roles (user_id, role_id)
                    SELECT id, role_id FROM users
                    ON CONFLICT DO NOTHING;

                    -- Drop foreign key constraint on users table
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'fk_users_role'
                    ) THEN
                        ALTER TABLE users DROP CONSTRAINT fk_users_role;
                    END IF;

                    -- Safely drop the old role_id column
                    ALTER TABLE users DROP COLUMN role_id;
                END IF;

                -- Safely provision session_version column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'session_version'
                ) THEN
                    ALTER TABLE users ADD COLUMN session_version INTEGER NOT NULL DEFAULT 1;
                END IF;

                -- Safely provision failed_attempts column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'failed_attempts'
                ) THEN
                    ALTER TABLE users ADD COLUMN failed_attempts INTEGER NOT NULL DEFAULT 0;
                END IF;

                -- Safely provision last_failed_at column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'last_failed_at'
                ) THEN
                    ALTER TABLE users ADD COLUMN last_failed_at TIMESTAMP WITH TIME ZONE;
                END IF;

                -- Safely provision lock_until column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'lock_until'
                ) THEN
                    ALTER TABLE users ADD COLUMN lock_until TIMESTAMP WITH TIME ZONE;
                END IF;

                -- Safely provision email_verified_at column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'email_verified_at'
                ) THEN
                    ALTER TABLE users ADD COLUMN email_verified_at TIMESTAMP WITH TIME ZONE;
                END IF;

                -- Safely provision last_login_at column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'last_login_at'
                ) THEN
                    ALTER TABLE users ADD COLUMN last_login_at TIMESTAMP WITH TIME ZONE;
                END IF;

                -- Safely provision last_login_ip column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'last_login_ip'
                ) THEN
                    ALTER TABLE users ADD COLUMN last_login_ip INET;
                END IF;

                -- Safely provision avatar_url column if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'users' AND column_name = 'avatar_url'
                ) THEN
                    ALTER TABLE users ADD COLUMN avatar_url TEXT;
                END IF;

                -- Safely provision assigned_at column to role_permissions junction table if missing
                IF EXISTS (
                    SELECT 1 FROM information_schema.tables WHERE table_name = 'role_permissions'
                ) AND NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'role_permissions' AND column_name = 'assigned_at'
                ) THEN
                    ALTER TABLE role_permissions ADD COLUMN assigned_at TIMESTAMP WITH TIME ZONE DEFAULT NOW();
                END IF;

                -- Safely provision assigned_at column to user_roles junction table if missing
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name = 'user_roles' AND column_name = 'assigned_at'
                ) THEN
                    ALTER TABLE user_roles ADD COLUMN assigned_at TIMESTAMP WITH TIME ZONE DEFAULT NOW();
                END IF;
            END $$;

            -- Granular permissions using a hierarchical naming convention
            CREATE TABLE IF NOT EXISTS permissions (
                id UUID PRIMARY KEY,
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
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL,
                token VARCHAR(255) NOT NULL,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                revoked_at TIMESTAMP WITH TIME ZONE,
                replaced_by_token VARCHAR(255),
                user_agent VARCHAR(500),
                ip_address VARCHAR(45),
                session_id UUID NOT NULL,
                remember_me BOOLEAN NOT NULL DEFAULT FALSE,
                replaced_by_token_id UUID,
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

            -- Ensure session_id column exists on refresh_tokens
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name='refresh_tokens' AND column_name='session_id'
                ) THEN
                    ALTER TABLE refresh_tokens ADD COLUMN session_id UUID NOT NULL;
                END IF;
            END $$;

            -- Ensure remember_me column exists on refresh_tokens
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name='refresh_tokens' AND column_name='remember_me'
                ) THEN
                    ALTER TABLE refresh_tokens ADD COLUMN remember_me BOOLEAN NOT NULL DEFAULT FALSE;
                END IF;
            END $$;

            -- Ensure replaced_by_token_id column exists on refresh_tokens
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_name='refresh_tokens' AND column_name='replaced_by_token_id'
                ) THEN
                    ALTER TABLE refresh_tokens ADD COLUMN replaced_by_token_id UUID;
                END IF;
            END $$;

            -- Create explicit indexes for high performance query lookups
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON refresh_tokens(user_id);
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_session_id ON refresh_tokens(session_id);
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expires_at ON refresh_tokens(expires_at);

            -- Manages one-time-use email verification tokens
            CREATE TABLE IF NOT EXISTS verification_tokens (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL,
                token_hash VARCHAR(255) NOT NULL UNIQUE,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                consumed_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_verification_tokens_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Manages one-time-use password reset tokens
            CREATE TABLE IF NOT EXISTS reset_password_tokens (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL,
                token_hash VARCHAR(255) NOT NULL UNIQUE,
                expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                consumed_at TIMESTAMP WITH TIME ZONE,
                CONSTRAINT fk_reset_password_tokens_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Outbox Pattern Table for reliable asynchronous email delivery
            CREATE TABLE IF NOT EXISTS outbox_messages (
                id UUID PRIMARY KEY,
                type VARCHAR(100) NOT NULL,
                payload TEXT NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                processed_at TIMESTAMP WITH TIME ZONE,
                error TEXT
            );

            -- Security Audit Logs Table for tracking major events
            CREATE TABLE IF NOT EXISTS audit_logs (
                id UUID PRIMARY KEY,
                user_id UUID,
                event_type VARCHAR(100) NOT NULL,
                description TEXT NOT NULL,
                ip_address VARCHAR(45),
                user_agent VARCHAR(500),
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT fk_audit_logs_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
            );

            -- Manages chat conversation sessions with the AI Assistant
            CREATE TABLE IF NOT EXISTS conversations (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL,
                title VARCHAR(255) NOT NULL DEFAULT 'New Conversation',
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT fk_conversations_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
            );

            -- Stores individual messages in a conversation
            CREATE TABLE IF NOT EXISTS messages (
                id UUID PRIMARY KEY,
                conversation_id UUID NOT NULL,
                role VARCHAR(50) NOT NULL,
                content TEXT NOT NULL,
                streaming_state VARCHAR(50) NOT NULL DEFAULT 'Pending',
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT fk_messages_conversation FOREIGN KEY (conversation_id) REFERENCES conversations(id) ON DELETE CASCADE
            );

            -- Optimized Indexes
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token ON refresh_tokens(token);
            CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user ON refresh_tokens(user_id);
            CREATE INDEX IF NOT EXISTS idx_verification_tokens_active ON verification_tokens(token_hash) WHERE consumed_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_reset_password_tokens_active ON reset_password_tokens(token_hash) WHERE consumed_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_outbox_messages_pending ON outbox_messages(created_at) WHERE processed_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_users_active ON users(status) WHERE deleted_at IS NULL;
            CREATE INDEX IF NOT EXISTS idx_permissions_hierarchy ON permissions (name varchar_pattern_ops);
            CREATE INDEX IF NOT EXISTS idx_conversations_user_id ON conversations(user_id);
            CREATE INDEX IF NOT EXISTS idx_messages_conversation_id_created_at ON messages(conversation_id, created_at);

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

            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'tr_conversations_timestamp') THEN
                    CREATE TRIGGER tr_conversations_timestamp BEFORE UPDATE ON conversations 
                        FOR EACH ROW EXECUTE PROCEDURE fn_update_timestamp();
                END IF;
            END $$;

            -- Initial Data (Seeding)
            -- Handled dynamically via permissions-registry.json mapping inside CVerify.Core,
            -- but bootstrap placeholders are kept for standard seed continuity.
            INSERT INTO roles (id, name, display_name, description, is_system)
            VALUES 
                ('018fc35b-1c5c-7b8a-9a2d-3e4f5a6b7c8d'::uuid, 'SUPER_ADMIN', 'System Administrator', 'Root access to all modules', TRUE),
                ('018fc35b-1c5d-7b8a-9a2d-3e4f5a6b7c8d'::uuid, 'USER', 'General User', 'Basic application access', TRUE)
            ON CONFLICT (name) DO NOTHING;

            INSERT INTO permissions (id, name, display_name, description, module, is_system)
            VALUES 
                ('018fc35b-1c5e-7b8a-9a2d-3e4f5a6b7c8d'::uuid, '*:*:*', 'Global Wildcard', 'Full access to every module and feature', 'system', TRUE)
            ON CONFLICT (name) DO NOTHING;

            INSERT INTO role_permissions (role_id, permission_id)
            SELECT r.id, p.id FROM roles r, permissions p 
            WHERE r.name = 'SUPER_ADMIN' AND p.name = '*:*:*'
            ON CONFLICT DO NOTHING;

            -- Provision the master administrator account if it doesn't exist
            INSERT INTO users (
                id,
                email, 
                password_hash, 
                full_name, 
                status, 
                email_verified_at
            )
            SELECT 
                '018fc35b-1c5f-7b8a-9a2d-3e4f5a6b7c8d'::uuid,
                'admin@system.com',
                crypt('SuperAdminPassword123', gen_salt('bf', 10)),
                'System Administrator',
                'ACTIVE',
                NOW()
            WHERE NOT EXISTS (SELECT 1 FROM users WHERE email = 'admin@system.com');

            -- Seed the master administrator role mapping if not present
            INSERT INTO user_roles (user_id, role_id)
            SELECT 
                (SELECT id FROM users WHERE email = 'admin@system.com'),
                (SELECT id FROM roles WHERE name = 'SUPER_ADMIN')
            ON CONFLICT DO NOTHING;
        ";

        await context.Database.ExecuteSqlRawAsync(sql);

        // Clear Npgsql connection pools to force reload of system types (like citext and user_status enum) 
        // created during database initialization, preventing System.NotSupportedException.
        Npgsql.NpgsqlConnection.ClearAllPools();

        // 3. Dynamic seed from permissions-registry.json
        var registryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "permissions-registry.json");
        if (!File.Exists(registryPath))
        {
            // Fallback for dev environment directly
            registryPath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "permissions-registry.json");
        }

        if (File.Exists(registryPath))
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(registryPath);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
                
                // Seed all permissions from the modules section
                if (doc.RootElement.TryGetProperty("modules", out var modulesElement))
                {
                    foreach (var moduleProperty in modulesElement.EnumerateObject())
                    {
                        var moduleName = moduleProperty.Name;
                        foreach (var permElement in moduleProperty.Value.EnumerateArray())
                        {
                            var name = permElement.GetProperty("name").GetString();
                            var displayName = permElement.GetProperty("displayName").GetString();
                            var description = permElement.GetProperty("description").GetString();
                            
                            var sqlSeedPermission = @"
                                INSERT INTO permissions (id, name, display_name, description, module, is_system)
                                VALUES (@id, @name, @displayName, @description, @module, TRUE)
                                ON CONFLICT (name) DO UPDATE 
                                SET display_name = EXCLUDED.display_name, description = EXCLUDED.description, module = EXCLUDED.module;";
                                
                            await context.Database.ExecuteSqlRawAsync(sqlSeedPermission, 
                                new Npgsql.NpgsqlParameter("@id", Guid.CreateVersion7()),
                                new Npgsql.NpgsqlParameter("@name", name),
                                new Npgsql.NpgsqlParameter("@displayName", displayName),
                                new Npgsql.NpgsqlParameter("@description", description ?? (object)DBNull.Value),
                                new Npgsql.NpgsqlParameter("@module", moduleName));
                        }
                    }
                }
                
                // Seed all roles and map their permissions
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var roleProperty in rolesElement.EnumerateObject())
                    {
                        var roleName = roleProperty.Name;
                        var roleDisplayName = roleProperty.Value.GetProperty("displayName").GetString();
                        var roleDescription = roleProperty.Value.GetProperty("description").GetString();
                        
                        var sqlSeedRole = @"
                            INSERT INTO roles (id, name, display_name, description, is_system, is_active)
                            VALUES (@id, @name, @displayName, @description, TRUE, TRUE)
                            ON CONFLICT (name) DO UPDATE 
                            SET display_name = EXCLUDED.display_name, description = EXCLUDED.description;";
                            
                        await context.Database.ExecuteSqlRawAsync(sqlSeedRole,
                            new Npgsql.NpgsqlParameter("@id", Guid.CreateVersion7()),
                            new Npgsql.NpgsqlParameter("@name", roleName),
                            new Npgsql.NpgsqlParameter("@displayName", roleDisplayName),
                            new Npgsql.NpgsqlParameter("@description", roleDescription ?? (object)DBNull.Value));

                        var roleId = await context.Roles
                            .Where(r => r.Name == roleName)
                            .Select(r => r.Id)
                            .FirstOrDefaultAsync();

                        if (roleId != Guid.Empty)
                        {
                            // Parse permissions assigned to this role in registry
                            var permissionsList = new List<string>();
                            if (roleProperty.Value.TryGetProperty("permissions", out var permsElement))
                            {
                                foreach (var permVal in permsElement.EnumerateArray())
                                {
                                    permissionsList.Add(permVal.GetString()!);
                                }
                            }
                            
                            // Get all permission IDs for this role
                            var dbPermissionIds = await context.Permissions
                                .Where(p => permissionsList.Contains(p.Name))
                                .Select(p => p.Id)
                                .ToListAsync();
                                
                            // Clear existing role-permissions mapping for this role, then rebuild it
                            var sqlClear = "DELETE FROM role_permissions WHERE role_id = @roleId;";
                            await context.Database.ExecuteSqlRawAsync(sqlClear, new Npgsql.NpgsqlParameter("@roleId", roleId));
                            
                            foreach (var permId in dbPermissionIds)
                            {
                                var sqlLink = "INSERT INTO role_permissions (role_id, permission_id) VALUES (@roleId, @permId) ON CONFLICT DO NOTHING;";
                                await context.Database.ExecuteSqlRawAsync(sqlLink, 
                                    new Npgsql.NpgsqlParameter("@roleId", roleId),
                                    new Npgsql.NpgsqlParameter("@permId", permId));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PermissionSeeding] Error dynamically seeding registry: {ex.Message}");
            }
        }
    }
}
