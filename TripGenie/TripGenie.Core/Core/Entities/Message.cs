using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripGenie.API.Core.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ConversationId { get; set; }

    [ForeignKey(nameof(ConversationId))]
    public virtual Conversation Conversation { get; set; } = null!;

    [Required]
    public MessageRole Role { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public StreamingState StreamingState { get; set; } = StreamingState.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
