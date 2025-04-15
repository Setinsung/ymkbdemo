using System.Security.Claims;
using YmKB.Application.Contracts.Identity;

namespace YmKB.Infrastructure.Services;


/// <summary>
/// 用于设置和清除当前用户上下文的服务。
/// </summary>
public class CurrentUserContextSetter : ICurrentUserContextSetter
{
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserContextSetter"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
    public CurrentUserContextSetter(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// 使用提供的会话信息设置当前用户上下文。
    /// </summary>
    /// <param name="user">The session information of the current user.</param>
    public void SetCurrentUser(ClaimsPrincipal user)
    {
        _currentUserContext.Set(user);
    }

    /// <summary>
    /// 清除当前用户上下文。
    /// </summary>
    public void Clear()
    {
        _currentUserContext.Clear();
    }
}