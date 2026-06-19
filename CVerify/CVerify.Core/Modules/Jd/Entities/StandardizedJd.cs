using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVerify.API.Modules.Jd.Entities;

public sealed class StandardizedJd
{
    [Key]
    [MaxLength(64)]
    public string Id { get; set; } = string.Empty;

    // Stores either a UserId (regular users) or an OrganizationId (business accounts).
    // No FK constraint — business accounts have org IDs that are not in the users table.
    public Guid OwnerUserId { get; set; }

    [MaxLength(255)]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Seniority { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    [MaxLength(80)]
    public string EmploymentType { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(40)]
    public string WorkMode { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Industry { get; set; } = string.Empty;

    [MaxLength(40)]
    public string HiringPriority { get; set; } = string.Empty;

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
