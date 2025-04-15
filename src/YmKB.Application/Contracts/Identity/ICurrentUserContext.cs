using System.Security.Claims;

namespace YmKB.Application.Contracts.Identity;

public interface ICurrentUserContext
{
    /// <summary>
    /// 用于获取当前登录用户的信息
    /// </summary>
    /// <returns>返回当前用户的身份信息</returns>
    ClaimsPrincipal? GetCurrentUser();

    /// <summary>
    /// 用于设置当前登录用户的信息
    /// </summary>
    /// <param name="user"> 要设置的用户信息 </param>
    void Set(ClaimsPrincipal? user);
    
    /// <summary>
    /// 用于清除当前登录用户的信息
    /// </summary>
    void Clear();
}
