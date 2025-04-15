using Microsoft.AspNetCore.Identity;
using YmKB.Domain.Abstractions.Common;

namespace YmKB.Domain.Abstractions.Identities;

/// <summary>
/// 表示应用程序用户
/// 该类扩展了 IdentityUser 的功能，增加了用户的额外信息，并支持审计功能。
/// </summary>
public class ApplicationUser : IdentityUser, IAuditableEntity
{
    /// <summary>
    /// 获取或设置用户的昵称。
    /// 昵称是用户在应用程序中使用的一个友好名称，可用于显示和识别。
    /// 如果未设置昵称，该值为 null。
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 获取或设置用户的身份验证提供者。
    /// 默认值为 "Local"，表示使用本地身份验证。
    /// 可以是其他值，如 "Google"、"Facebook" 等，表示使用第三方身份验证。
    /// </summary>
    public string? Provider { get; set; } = "Local";

    /// <summary>
    /// 获取或设置用户所属的租户 ID。
    /// 如果用户与某个租户相关联，该属性将包含租户的唯一标识符；否则为 null。
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// 获取或设置用户的头像 URL。
    /// 该 URL 指向用户的头像图片，可用于在应用程序中显示用户的头像。
    /// 如果未设置头像 URL，该值为 null。
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 获取或设置用户的刷新令牌。
    /// 刷新令牌用于在用户的访问令牌过期后获取新的访问令牌，以保持用户的会话状态。
    /// 如果未设置刷新令牌，该值为 null。
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 获取或设置刷新令牌的过期时间。
    /// 该时间表示刷新令牌的有效期限，超过该时间后刷新令牌将失效。
    /// </summary>
    public DateTime RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// 获取或设置用户所在的时区 ID。
    /// 时区 ID 用于将应用程序中的时间信息转换为用户所在时区的时间。
    /// 如果未设置时区 ID，该值为 null。
    /// </summary>
    public string? TimeZoneId { get; set; }

    /// <summary>
    /// 获取或设置用户使用的语言代码。
    /// 语言代码用于为用户提供符合其语言偏好的界面和内容。
    /// 如果未设置语言代码，该值为 null。
    /// </summary>
    public string? LanguageCode { get; set; }

    /// <summary>
    /// 获取或设置用户上级的 ID。
    /// 如果用户有上级，该属性将包含上级用户的唯一标识符；否则为 null。
    /// </summary>
    public string? SuperiorId { get; set; } = null;

    /// <summary>
    /// 获取或设置用户的上级对象。
    /// 如果用户有上级，该属性将引用上级用户对象；否则为 null。
    /// </summary>
    public ApplicationUser? Superior { get; set; } = null;

    /// <summary>
    /// 获取或设置用户记录的创建日期和时间。
    /// 如果用户记录已创建，此属性记录创建操作的发生时间；若未创建，则值为 null。
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// 获取或设置创建用户记录的用户标识。
    /// 如果用户记录已创建，此属性记录执行创建操作的用户标识信息；若未创建，则值为 null。
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 获取或设置用户记录最后一次修改的日期和时间。
    /// 如果用户记录有过修改，此属性记录最后一次修改操作的发生时间；若未修改过，则值为 null。
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// 获取或设置最后一次修改用户记录的用户标识。
    /// 如果用户记录有过修改，此属性记录执行最后一次修改操作的用户标识信息；若未修改过，则值为 null。
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
