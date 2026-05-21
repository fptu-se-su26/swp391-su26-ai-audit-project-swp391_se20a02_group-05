using System;
using System.ComponentModel.DataAnnotations;

namespace CVerify.API.Core.Entities;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = null!;

    [Required]
    public string Payload { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ProcessedAt { get; set; }

    public string? Error { get; set; }
}
