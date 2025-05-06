namespace YmKB.UI.ConstantConfigs;

public class AppSettings
{
    public const string KEY = nameof(AppSettings);
    public required string AppName { get; set; }
    public required string Version { get; set; }
    public required string ServiceBaseUrl { get; set; }
}