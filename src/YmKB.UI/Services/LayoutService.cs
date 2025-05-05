using MudBlazor;
using YmKB.UI.ConstantConfigs;
using YmKB.UI.Models;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

/// <summary>
/// 表示管理应用程序的布局和主题偏好的服务。
/// </summary>
public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences? _userPreferences;

    // 系统默认是否深色模式
    private bool _systemIsDarkMode;

    /// <summary>
    /// 引发 MajorUpdateOccurred 事件
    /// </summary>
    private void OnMajorUpdateOccurred() => MajorUpdateOccurred?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 发生重大更新时触发的事件。
    /// </summary>
    public event EventHandler? MajorUpdateOccurred;

    /// <summary>
    /// 获取或设置是否启用深色模式。
    /// </summary>
    public bool IsDarkMode { get; set; }

    /// <summary>
    /// 获取或设置是否显示导航菜单。
    /// </summary>
    public bool DrawerOpen { get; set; }

    /// <summary>
    /// 获取或设置是否可以最小化侧边菜单抽屉。
    /// </summary>
    public bool CanMiniSideMenuDrawer { get; set; } = true;
    
    public DarkLightModeType CurrentDarkLightMode { get; private set; } = DarkLightModeType.Light;
    public MudTheme CurrentTheme { get; private set; }

    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService =
            userPreferencesService
            ?? throw new ArgumentNullException(nameof(userPreferencesService));
        CurrentTheme = Theme.AppTheme;
    }

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        OnMajorUpdateOccurred();
    }
    public void ToggleDrawerOpen()
    {
        DrawerOpen = !DrawerOpen;
        OnMajorUpdateOccurred();
    }

    /// <summary>
    /// 将用户偏好应用于布局。
    /// </summary>
    /// <param name="systemIsDarkMode">该值指示深色模式是否为系统默认主题。</param>
    public async Task ApplyUserPreferences(bool systemIsDarkMode)
    {
        _systemIsDarkMode = systemIsDarkMode;
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            CurrentDarkLightMode = _userPreferences.DarkLightMode;
            IsDarkMode = CurrentDarkLightMode switch
            {
                DarkLightModeType.Dark => true,
                DarkLightModeType.Light => false,
                DarkLightModeType.System => systemIsDarkMode,
                _ => IsDarkMode
            };
        }
        // 如果无用户偏好，则使用系统默认
        else
        {
            IsDarkMode = systemIsDarkMode;
            _userPreferences = new UserPreferences { DarkLightMode = DarkLightModeType.System };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
    }

    /// <summary>
    /// 处理系统首选项更改事件。
    /// </summary>
    /// <param name="newMode">系统首选项的新值。</param>
    /// <returns>表示异步作的任务。</returns>
    public async Task OnSystemPreferenceChanged(bool newMode)
    {
        _systemIsDarkMode = newMode;

        if (CurrentDarkLightMode == DarkLightModeType.System)
        {
            IsDarkMode = newMode;
            OnMajorUpdateOccurred();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 在深色/浅色模式选项之间循环。
    /// </summary>
    /// <returns>表示异步作的任务。</returns>
    public async Task CycleDarkLightModeAsync()
    {
        switch (CurrentDarkLightMode)
        {
            case DarkLightModeType.System:
                CurrentDarkLightMode = DarkLightModeType.Light;
                IsDarkMode = false;
                break;
            case DarkLightModeType.Light:
                CurrentDarkLightMode = DarkLightModeType.Dark;
                IsDarkMode = true;
                break;
            case DarkLightModeType.Dark:
                CurrentDarkLightMode = DarkLightModeType.System;
                IsDarkMode = _systemIsDarkMode;
                break;
        }

        _userPreferences!.DarkLightMode = CurrentDarkLightMode;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccurred();
    }
}
