namespace YmKB.UI.Models;

public class Application
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // 对话应用 或 知识库
    public string ConversationModel { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
    public bool EnablePromptSuggestion { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 0.95;
    public string ApiPlugins { get; set; } = string.Empty;
    public string NativePlugins { get; set; } = string.Empty;
}
