using Microsoft.EntityFrameworkCore;
using TripGenie.API.Core.Entities;
using Npgsql;

namespace TripGenie.API.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
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

        // Configure User -> Role (Many-to-One)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

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
    }
}
