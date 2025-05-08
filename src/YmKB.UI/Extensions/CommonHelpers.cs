namespace YmKB.UI.Extensions;

public static class CommonHelpers
{
    
    // 截断文本
    public static string TruncateText(string? text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text ?? "";
        }
        return text[..(maxLength - 3)] + "...";
    }

    // 格式化文件大小
    public static string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        int i = 0;
        double dblSByte = bytes;
        if (bytes > 1024)
        {
            for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
        }
        return $"{dblSByte:N2} {suffixes[i]}";
    }
    
    public static string FormatDateTimeOst(DateTimeOffset dto)
    {
        DateTimeOffset adjustedDateTime = dto.AddHours(8);
        string customFormat = adjustedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        return customFormat;
    }
    
    public static string CombinePathWithBaseUrl(string serviceBaseUrl, string localPath)
    {
        localPath = localPath.TrimStart('\\');
        localPath = localPath.Replace(@"\\", "/");
        serviceBaseUrl = serviceBaseUrl.TrimEnd('/');
        return $"{serviceBaseUrl}/{localPath}";
    }
}