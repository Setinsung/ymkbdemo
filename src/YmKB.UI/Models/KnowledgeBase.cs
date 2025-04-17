namespace YmKB.UI.Models;

public class KnowledgeBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ConversationModel { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
    public int SegmentTokens { get; set; }
    public int LineTokens { get; set; }
    public int OverlapTokens { get; set; }
    public bool EnableOcr { get; set; }
    public int DocumentCount { get; set; }
    public int SegmentCount { get; set; }
}
