using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

/// <summary>
/// chat聊天历史
/// </summary>
public class OnlyChatHistory : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// 知识库ID
    /// </summary>
    public string KbAppId { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType MessageType { get; set; }
    
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}


public enum MessageType
{
    User = 1,
    Assistant = 2
}