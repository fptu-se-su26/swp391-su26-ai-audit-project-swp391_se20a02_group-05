using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVerify.API.Core.Entities;

public class Organization
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string TaxCode { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = null!;

    public bool IsVerified { get; set; } = false;

    public int VerificationLevel { get; set; } = 0; // 0 = Unverified/Onboarding, 1 = Legal Verified, 2 = Domain Verified, 3 = Domain Ownership Verified

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? DeletedAt { get; set; }

    public virtual ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
}
