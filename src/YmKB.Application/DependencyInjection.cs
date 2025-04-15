using System.Reflection;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using YmKB.Application.Pipeline;

namespace YmKB.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FusionCacheRefreshBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MessageValidatorBehaviour<,>));
        
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        
        return services;
    }
}
