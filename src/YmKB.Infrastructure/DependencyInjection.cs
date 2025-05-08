using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using YmKB.Application.Contracts;
using YmKB.Application.Contracts.Identity;
using YmKB.Application.Contracts.Persistence;
using YmKB.Application.Contracts.Upload;
using YmKB.Domain.Abstractions.Identities;
using YmKB.Infrastructure.Configurations;
using YmKB.Infrastructure.Persistence;
using YmKB.Infrastructure.Persistence.Interceptors;
using YmKB.Infrastructure.Persistence.Seed;
using YmKB.Infrastructure.Services;
using ZiggyCreatures.Caching.Fusion;

namespace YmKB.Infrastructure;

public static class DependencyInjection
{
    private const string DATABASE_SETTINGS_KEY = "DatabaseSettings";
    private const string NPGSQL_ENABLE_LEGACY_TIMESTAMP_BEHAVIOR =
        "Npgsql.EnableLegacyTimestampBehavior";
    private const string MSSQL_MIGRATIONS_ASSEMBLY = "YmKB.Migrators.MSSQL";
    private const string SQLITE_MIGRATIONS_ASSEMBLY = "YmKB.Migrators.SQLite";
    private const string POSTGRESQL_MIGRATIONS_ASSEMBLY = "YmKB.Migrators.PostgreSQL";
    private const string USE_IN_MEMORY_DATABASE_KEY = "UseInMemoryDatabase";
    private const string IN_MEMORY_DATABASE_NAME = "YmKBDb";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IAuthService, AuthService>();
        
        services
            .AddScoped<IUploadService, FileUploadService>()
            .AddScoped<IDateTime, UtcDateTime>()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddScoped<ICurrentUserAccessor, CurrentUserAccessor>()
            .AddScoped<ICurrentUserContextSetter, CurrentUserContextSetter>()
            .AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        services.AddDatabase(configuration);
        services.AddFusionCacheService();
        services.AddIdentityService(configuration);
        services.AddAIService(configuration);
        return services;
    }

    // 添加ai相关服务
    private static IServiceCollection AddAIService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<QdrantSettings>(configuration.GetSection("QdrantSettings"));
        services.AddSingleton<AIKernelCreateService>();
        return services;
    }
    
    private static IServiceCollection AddIdentityService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])
                    )
                };
            });
        return services;
    }

    // 向服务集合中添加 Fusion 缓存服务。
    // 首先添加内存缓存服务，然后配置 Fusion 缓存的默认选项，包括缓存持续时间、故障安全选项、工厂超时选项等。
    private static IServiceCollection AddFusionCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services
            .AddFusionCache()
            .WithDefaultEntryOptions(
                new FusionCacheEntryOptions
                {
                    // 缓存持续时间
                    Duration = TimeSpan.FromMinutes(120),
                    // 故障安全选项
                    IsFailSafeEnabled = true,
                    FailSafeMaxDuration = TimeSpan.FromHours(8),
                    FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                    // 工厂最大执行时间
                    FactorySoftTimeout = TimeSpan.FromSeconds(10),
                    FactoryHardTimeout = TimeSpan.FromSeconds(30),
                    AllowTimedOutFactoryBackgroundCompletion = true,
                }
            );
        return services;
    }

    // 向服务集合中添加数据库相关的服务。
    // 首先从配置中读取数据库设置并绑定到 DatabaseSettings 对象，然后添加审计实体拦截器和领域事件调度拦截器。
    // 根据配置决定是否使用内存数据库，如果使用内存数据库则配置相应的数据库上下文，否则根据数据库类型配置不同的数据库连接，
    // 最后添加数据库上下文工厂、应用程序数据库上下文和数据库上下文初始化器。
    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .Configure<DatabaseSettings>(configuration.GetSection(DATABASE_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        services
            .AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>()
            .AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        if (configuration.GetValue<bool>(USE_IN_MEMORY_DATABASE_KEY))
        {
            services.AddDbContext<ApplicationDbContext>(
                (p, options) =>
                {
                    options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME);
                    options.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                    options.EnableSensitiveDataLogging();
                }
            );
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(
                (p, m) =>
                {
                    var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                    m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                    m.UseExceptionProcessor(databaseSettings.DBProvider);
                    m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
                }
            );
        }

        services.AddScoped<
            IDbContextFactory<ApplicationDbContext>,
            BlazorContextFactory<ApplicationDbContext>
        >();
        services.AddScoped<IApplicationDbContext>(
            provider =>
                provider
                    .GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                    .CreateDbContext()
        );
        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }

    // 根据传入的数据库类型（如 PostgreSQL、MSSQL、SQLite），设置相应的数据库连接选项，
    // 包括迁移程序集和命名约定（对于 PostgreSQL）。如果数据库类型不支持，则抛出异常。
    private static DbContextOptionsBuilder UseDatabase(
        this DbContextOptionsBuilder builder,
        string dbProvider,
        string connectionString
    )
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                AppContext.SetSwitch(NPGSQL_ENABLE_LEGACY_TIMESTAMP_BEHAVIOR, true);
                return builder
                    .UseNpgsql(
                        connectionString,
                        e => e.MigrationsAssembly(POSTGRESQL_MIGRATIONS_ASSEMBLY)
                    )
                    .UseSnakeCaseNamingConvention();

            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(
                    connectionString,
                    e => e.MigrationsAssembly(MSSQL_MIGRATIONS_ASSEMBLY)
                );

            case DbProviderKeys.SqLite:
                return builder.UseSqlite(
                    connectionString,
                    e => e.MigrationsAssembly(SQLITE_MIGRATIONS_ASSEMBLY)
                );

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    // 根据传入的数据库类型（如 PostgreSQL、MSSQL、SQLite），添加相应的数据库异常处理器扩展。
    // 如果数据库类型不支持，则抛出异常。
    private static DbContextOptionsBuilder UseExceptionProcessor(
        this DbContextOptionsBuilder builder,
        string dbProvider
    )
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                EntityFramework
                    .Exceptions
                    .PostgreSQL
                    .ExceptionProcessorExtensions
                    .UseExceptionProcessor(builder);
                return builder;

            case DbProviderKeys.SqlServer:
                EntityFramework
                    .Exceptions
                    .SqlServer
                    .ExceptionProcessorExtensions
                    .UseExceptionProcessor(builder);
                return builder;

            case DbProviderKeys.SqLite:
                EntityFramework
                    .Exceptions
                    .Sqlite
                    .ExceptionProcessorExtensions
                    .UseExceptionProcessor(builder);
                return builder;

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    public static async Task InitializeDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var initializer = scope
            .ServiceProvider
            .GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitialiseAsync().ConfigureAwait(false);

        var env = host.Services.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            await initializer.SeedAsync().ConfigureAwait(false);
        }
    }
}

internal class DbProviderKeys
{
    public const string Npgsql = "postgresql";
    public const string SqlServer = "mssql";
    public const string SqLite = "sqlite";
}
