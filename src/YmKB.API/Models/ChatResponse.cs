namespace YmKB.API.Models;

public class ChatResponse
{
    public string Message { get; set; } = null!;
    public string ConversationId { get; set; } = null!;
    public List<ChatReference> References { get; set; } = new();
}

public class ChatReference
{
    public string Content { get; set; } = null!;
    public string Source { get; set; } = null!;
}
