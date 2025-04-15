using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YmKB.Infrastructure.Persistence;

public class BlazorContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly IServiceProvider _provider;

    public BlazorContextFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public TContext CreateDbContext()
    {
        var scope = _provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TContext>();
    }
}
