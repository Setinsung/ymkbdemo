using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Entities;

public class JsFunctionCall : BaseAuditableEntity, IAuditTrial, ISoftDelete
{
    /// <summary>
    /// function call 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// function call 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// js content
    /// </summary>
    public string ScriptContent { get; set; }

    /// <summary>
    /// 主函数名称
    /// </summary>
    public string MainFuncName { get; set; }

    /// <summary>
    /// 入参列表
    /// </summary>
    public List<JsFunctionParameter> Parameters { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}

public class JsFunctionParameter
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public string ParamName { get; set; }

    /// <summary>
    /// 参数描述
    /// </summary>
    public string ParamDescription { get; set; }
}
