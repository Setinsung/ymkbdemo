namespace YmKB.API.Endpoints;

/// <summary>
/// 定义注册端点路由的契约。
/// </summary>
public interface IEndpointRegistrar
{
    /// <summary>
    /// 为应用程序注册路由。
    /// </summary>
    /// <param name="routes">用于添加路由的 <see cref="IEndpointRouteBuilder"/> 对象。</param>
    void RegisterRoutes(IEndpointRouteBuilder routes);
}