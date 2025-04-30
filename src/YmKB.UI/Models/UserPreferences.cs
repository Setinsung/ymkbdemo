namespace YmKB.UI.Models;

public class UserPreferences
{
    /// <summary>
    /// 记住我
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    /// 明暗模式
    /// </summary>
    public DarkLightModeType DarkLightMode { get; set; }
}


public enum DarkLightModeType
{
    System = 0,
    Light = 1,
    Dark = 2
}
