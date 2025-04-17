using System.Security.Claims;

namespace YmKB.Application.Contracts.Identity;

/// <summary>
/// 用于设置当前登录用户的信息
/// </summary>
public interface ICurrentUserContextSetter
{
    /// <summary>
    /// 设置当前登录用户的信息
    /// </summary>
    /// <param name="user"> 要设置的用户信息 </param>
    void SetCurrentUser(ClaimsPrincipal user);
    
    /// <summary>
    /// 清除当前登录用户的信息
    /// </summary>
    void Clear();
}
