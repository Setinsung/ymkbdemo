using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Serialization.Form;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Kiota.Serialization.Multipart;
using Microsoft.Kiota.Serialization.Text;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;
using YMKB.UI.APIs;
using YmKB.UI.ConstantConfigs;
using YmKB.UI.Handlers;
using YmKB.UI.Providers;
using YmKB.UI.Services;
using YmKB.UI.Services.Contracts;
using YmKB.UI.Services.JsInterops;

namespace YmKB.UI;

public static class DependencyInjection
{
    public static void AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // App配置信息
        var appSettings = configuration.GetSection(AppSettings.KEY).Get<AppSettings>();
        services.AddSingleton(appSettings!);

        // common service
        services.AddScoped<AuthenticationService>();
        services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
        services.AddScoped<LayoutService>();
        services.AddScoped<ICommonDialogService, CommonDialogService>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IndexedDbCache>();

        // Httpclient Kiota ApiClient Service
        services.AddScoped<JwtAuthorizationMessageHandler>();
        services.AddScoped<IAuthenticationProvider, AnonymousAuthenticationProvider>();
        services
            .AddHttpClient<IRequestAdapter, HttpClientRequestAdapter>(
                client => client.BaseAddress = new Uri(appSettings.ServiceBaseUrl)
            )
            .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();
        // "http://localhost:5045"
        // "https://localhost:7219"
        services.AddScoped<ApiClient>();
        services.AddScoped<ApiClientServiceProxy>();

        // mudblazor
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
