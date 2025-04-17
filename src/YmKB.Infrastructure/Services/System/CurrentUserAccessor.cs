using System.Security.Claims;
using YmKB.Application.Contracts.Identity;
using YmKB.Infrastructure.Extensions;

namespace YmKB.Infrastructure.Services;


/// <summary>
/// 提供对当前用户的会话信息的访问权限。
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly ICurrentUserContext _currentUserContext;
    
    public CurrentUserAccessor(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// 获取当前用户的会话信息。
    /// </summary>
    public ClaimsPrincipal? User => _currentUserContext.GetCurrentUser();

    public string? UserId => User?.GetUserId();

    public string? TenantId => User?.GetTenantId();
}
