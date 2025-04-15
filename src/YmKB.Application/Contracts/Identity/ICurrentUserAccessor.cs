using System.Security.Claims;

namespace YmKB.Application.Contracts.Identity;

/// <summary>
/// 提供对当前用户身份信息的访问。
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// 表示当前用户的身份信息。
    /// </summary>
    ClaimsPrincipal? User { get; }
    
    /// <summary>
    /// 表示当前用户的唯一标识符。
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// 表示当前用户所属的租户标识符。
    /// </summary>
    string? TenantId { get; }
}
