using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbApps.DTOs;

public class KbAppDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public string Description { get; set; }

    public string? Icon { get; set; }

    public KbAppType KbAppType { get; set; }

    public string? ChatModelId { get; set; }

    public string? EmbeddingModelId { get; set; }

    public double Temperature { get; set; }

    public string? Prompt { get; set; }

    public string? ApiFunctionList { get; set; }

    public string? NativeFunctionList { get; set; }

    public string? KbIdList { get; set; }

    public double Relevance { get; set; }

    public int MaxAskPromptSize { get; set; }
    
    public int MaxMatchesCount { get; set; }

    public int AnswerTokens { get; set; }

    public string PromptTemplate { get; set; }
    
    public string? NoReplyFoundTemplate { get; set; }
}