using YmKB.Domain.Entities;

namespace YmKB.Application.Features.AIModels.DTOs;

public class AIModelDto
{
    public string Id { get; set; }

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
    public string ModelDescription { get; set; } = "";
}
