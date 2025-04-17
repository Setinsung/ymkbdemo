namespace YmKB.UI.Models;

public class Document
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadTime { get; set; }

    public string FormattedFileSize
    {
        get
        {
            if (FileSize < 1024)
                return $"{FileSize} B";
            if (FileSize < 1024 * 1024)
                return $"{FileSize / 1024.0:F2} KB";
            if (FileSize < 1024 * 1024 * 1024)
                return $"{FileSize / (1024.0 * 1024.0):F2} MB";
            return $"{FileSize / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }
    }
}
