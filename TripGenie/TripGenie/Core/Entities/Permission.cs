using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripGenie.API.Core.Entities;

public class Permission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!; // module:feature:action

    [Required]
    [MaxLength(150)]
    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Module { get; set; } = null!;

    public bool IsSystem { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
