using Microsoft.EntityFrameworkCore;
using TripGenie.API.Core.Entities;
using TripGenie.API.Application.Exceptions;
using Npgsql;

namespace TripGenie.API.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            if (pgEx.ConstraintName?.Contains("users_email") == true || pgEx.Message.Contains("users") || pgEx.Detail?.Contains("email") == true)
            {
                throw new DuplicateEmailException("A user with this email address already exists.", ex);
            }
            throw;
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            if (pgEx.ConstraintName?.Contains("users_email") == true || pgEx.Message.Contains("users") || pgEx.Detail?.Contains("email") == true)
            {
                throw new DuplicateEmailException("A user with this email address already exists.", ex);
            }
            throw;
        }
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<VerificationToken> VerificationTokens => Set<VerificationToken>();
    public DbSet<ResetPasswordToken> ResetPasswordTokens => Set<ResetPasswordToken>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable PostgreSQL Extensions
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.HasPostgresExtension("pgcrypto");

        // Map Enum
        modelBuilder.HasPostgresEnum<UserStatus>();

        // Configure User <-> Role (Many-to-Many via user_roles junction table)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                j => j.HasOne<Role>().WithMany().HasForeignKey("role_id"),
                j => j.HasOne<User>().WithMany().HasForeignKey("user_id"),
                j =>
                {
                    j.Property<DateTimeOffset>("assigned_at").HasDefaultValueSql("NOW()");
                });

        // Configure Role <-> Permission (Many-to-Many)
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "role_permissions",
                j => j.HasOne<Permission>().WithMany().HasForeignKey("permission_id"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("role_id"),
                j =>
                {
                    j.Property<DateTimeOffset>("assigned_at").HasDefaultValueSql("NOW()");
                });

        // Configure VerificationToken -> User (Many-to-One Cascade)
        modelBuilder.Entity<VerificationToken>()
            .HasOne(vt => vt.User)
            .WithMany()
            .HasForeignKey(vt => vt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ResetPasswordToken -> User (Many-to-One Cascade)
        modelBuilder.Entity<ResetPasswordToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure AuditLog -> User (Many-to-One SetNull)
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);


        // Optimistic Concurrency Control mapping utilizing PostgreSQL xmin system column
        modelBuilder.Entity<User>()
            .Property(u => u.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        modelBuilder.Entity<VerificationToken>()
            .Property(vt => vt.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        modelBuilder.Entity<ResetPasswordToken>()
            .Property(rt => rt.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // Indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();

        // Optimized Hierarchy Index
        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Name)
            .HasMethod("btree") // default, but for hierarchy we might use varchar_pattern_ops in SQL
            .HasOperators("varchar_pattern_ops");
            
        // Status filter index
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Status)
            .HasFilter("deleted_at IS NULL")
            .HasDatabaseName("idx_users_active");

        // Optimized partial indexes for active tokens only (keeping indexes compact)
        modelBuilder.Entity<VerificationToken>()
            .HasIndex(vt => vt.TokenHash)
            .HasFilter("consumed_at IS NULL")
            .HasDatabaseName("idx_verification_tokens_active");

        modelBuilder.Entity<ResetPasswordToken>()
            .HasIndex(rt => rt.TokenHash)
            .HasFilter("consumed_at IS NULL")
            .HasDatabaseName("idx_reset_password_tokens_active");

        modelBuilder.Entity<OutboxMessage>()
            .HasIndex(om => om.CreatedAt)
            .HasFilter("processed_at IS NULL")
            .HasDatabaseName("idx_outbox_messages_pending");

        // Configure RefreshToken mapping and indexes explicitly
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.Property(t => t.SessionId)
                .HasColumnName("session_id")
                .IsRequired();
            entity.Property(t => t.RememberMe)
                .HasColumnName("remember_me")
                .HasDefaultValue(false)
                .IsRequired();
            entity.Property(t => t.ReplacedByTokenId)
                .HasColumnName("replaced_by_token_id");

            entity.HasIndex(t => t.UserId).HasDatabaseName("idx_refresh_tokens_user_id");
            entity.HasIndex(t => t.SessionId).HasDatabaseName("idx_refresh_tokens_session_id");
            entity.HasIndex(t => t.ExpiresAt).HasDatabaseName("idx_refresh_tokens_expires_at");
        });
    }
}
