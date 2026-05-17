using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace TripGenie.API.Core.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid RoleId { get; set; }
    
    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;

    [Required]
    [Column(TypeName = "citext")]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    [Required]
    public UserStatus Status { get; set; } = UserStatus.EMAIL_VERIFY_PENDING;

    public DateTimeOffset? EmailVerifiedAt { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public IPAddress? LastLoginIp { get; set; } // Map to INET in Postgres

    public int FailedAttempts { get; set; }

    public DateTimeOffset? LastFailedAt { get; set; }

    public DateTimeOffset? LockUntil { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? DeletedAt { get; set; }

    [ConcurrencyCheck]
    public uint Version { get; set; } // Map PostgreSQL xmin system column

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
