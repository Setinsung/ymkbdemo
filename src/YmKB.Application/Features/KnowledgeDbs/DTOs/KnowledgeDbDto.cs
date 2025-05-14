namespace YmKB.Application.Features.KnowledgeDbs.DTOs;

public class KnowledgeDbDto
{
    public string Id { get; set; }
    
    public string? Icon { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string? ChatModelID { get; set; }

    public string? EmbeddingModelID { get; set; }

    public int MaxTokensPerParagraph { get; set; } = 299;

    public int MaxTokensPerLine { get; set; } = 99;

    public int OverlappingTokens { get; set; } = 49;

    public int? DocCount { get; set; }
    
    public int? WebDocCount { get; set; }
}