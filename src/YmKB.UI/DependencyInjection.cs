using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;
using YmKB.UI.Providers;
using YmKB.UI.Services;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI;

public static class DependencyInjection
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            // .AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7219") });
            .AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5045") });
        
        services.AddScoped<IKnowledgeBaseService, MockKnowledgeBaseService>();
        services.AddScoped<IDocumentService, MockDocumentService>();
        services.AddScoped<IApplicationService, MockApplicationService>();
        services.AddScoped<IAIModelService, AIModelService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
        services.AddScoped<LayoutService>();
        services.AddScoped<ICommonDialogService, CommonDialogService>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddBlazoredLocalStorage();
        services.AddAuthorizationCore();
        services.AddMudBlazors(configuration);
    }

    private static void AddMudBlazors(this IServiceCollection services, IConfiguration config)
    {
        services.AddMudServicesWithExtensions(mudConfig =>
        {
            MudGlobal.InputDefaults.ShrinkLabel = true;
            MudGlobal.UnhandledExceptionHandler = Console.WriteLine;
            mudConfig.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            mudConfig.SnackbarConfiguration.NewestOnTop = false;
            mudConfig.SnackbarConfiguration.ShowCloseIcon = true;
            mudConfig.SnackbarConfiguration.VisibleStateDuration = 3000;
            mudConfig.SnackbarConfiguration.HideTransitionDuration = 500;
            mudConfig.SnackbarConfiguration.ShowTransitionDuration = 500;
            mudConfig.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
    }
}