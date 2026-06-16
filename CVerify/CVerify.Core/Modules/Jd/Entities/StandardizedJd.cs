using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CVerify.API.Modules.Shared.Domain.Entities;

namespace CVerify.API.Modules.Jd.Entities;

public sealed class StandardizedJd
{
    [Key]
    [MaxLength(64)]
    public string Id { get; set; } = string.Empty;

    public Guid OwnerUserId { get; set; }

    [ForeignKey(nameof(OwnerUserId))]
    public User OwnerUser { get; set; } = null!;

    [MaxLength(255)]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Seniority { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [Column(TypeName = "decimal(18,2)")]
    public decimal SalaryMin { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SalaryMax { get; set; }

    [Column(TypeName = "jsonb")]
    public string StructuredJson { get; set; } = "{}";

    public string HumanReadableText { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
