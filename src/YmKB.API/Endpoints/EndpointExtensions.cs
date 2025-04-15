namespace YmKB.API.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpointDefinitions(this IEndpointRouteBuilder routes)
    {
        using var scope = routes.ServiceProvider.CreateScope();
        var registrars = scope.ServiceProvider.GetServices<IEndpointRegistrar>();
        foreach (var registrar in registrars)
        {
            registrar.RegisterRoutes(routes);
        }

        return routes;
    }
}
