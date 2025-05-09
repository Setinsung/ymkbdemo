using System.ComponentModel.DataAnnotations;

namespace YmKB.API.Models;

public class ChatRequest
{
    [Required]
    public string ApplicationId { get; set; } = null!;

    [Required]
    public string Message { get; set; } = null!;

    public string? ConversationId { get; set; }
}
