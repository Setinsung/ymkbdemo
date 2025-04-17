using System.Security.Claims;
using YmKB.Application.Contracts.Identity;
using YmKB.Infrastructure.Extensions;

namespace YmKB.Infrastructure.Services;


/// <summary>
/// Provides access to the current user's session information.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserAccessor"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
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
