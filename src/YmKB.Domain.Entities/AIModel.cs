using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

/// <summary>
/// 模型配置
/// </summary>
public class AIModel : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// 模型类型
    /// </summary>
    public AIModelType AIModelType { get; set; } = AIModelType.Chat;
    /// <summary>
    /// 模型地址
    /// </summary>
    public string Endpoint { get; set; } = "";
    /// <summary>
    /// 模型名称
    /// </summary>
    public string ModelName { get; set; } = "";
    /// <summary>
    /// apikey
    /// </summary>
    public string ModelKey { get; set; } = "";
    
    /// <summary>
    /// 模型描述
    /// </summary>
    public string ModelDescription { get; set; }= "";

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}

public enum AIModelType
{
    /// <summary>
    /// Chat模型
    /// </summary>
    Chat,
    /// <summary>
    /// 向量化模型
    /// </summary>
    Embedding
}