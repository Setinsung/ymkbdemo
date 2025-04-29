using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;
using YmKB.UI.Contracts;
using YmKB.UI.Services;

namespace YmKB.UI;

public static class DependencyInjection
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<ICommonDialogService, CommonDialogService>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        services.AddBlazoredLocalStorage();
        services.AddMudBlazors(configuration);
    }
    
    public static void AddMudBlazors(this IServiceCollection services, IConfiguration config)
    {
        services.AddMudServicesWithExtensions(mudConfig =>
        {
            MudGlobal.InputDefaults.ShrinkLabel = true;
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