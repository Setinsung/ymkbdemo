using System.Security.Claims;
using YmKB.Application.Contracts.Identity;

namespace YmKB.Infrastructure.Services;

/// <summary>
/// 表示当前用户上下文，保存会话信息。
/// </summary>
public class CurrentUserContext : ICurrentUserContext
{
    private static AsyncLocal<ClaimsPrincipal?> _currentUser = new();

    public ClaimsPrincipal? GetCurrentUser() => _currentUser.Value;

    public void Set(ClaimsPrincipal? user) => _currentUser.Value = user;
    public void Clear() => _currentUser.Value = null;
}
