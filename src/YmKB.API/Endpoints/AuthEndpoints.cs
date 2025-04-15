using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Contracts.Identity;
using YmKB.Application.Models.Identity;

namespace YmKB.API.Endpoints;

public class AuthEndpoints(ILogger<ProductEndpoins> logger) : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        routes
            .MapPost(
                "/login",
                async (
                    [FromServices] IAuthService authService,
                    [FromBody] AuthRequest authRequest
                ) => await authService.Login(authRequest))
            .Produces<AuthResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("登录")
            .WithDescription("用户输入用户名和密码，登录返回token");

        routes
            .MapPost(
                "/register",
                async (
                    [FromServices] IAuthService authService,
                    [FromBody] RegistrationRequest registrationRequest
                ) => await authService.Register(registrationRequest))
            .Produces<RegistrationResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("注册")
            .WithDescription("输入邮箱，用户名，密码，注册用户");
    }
}
