using YmKB.Application.Contracts.Upload;
using YmKB.Application.Models;

namespace YmKB.Infrastructure.Services;

public class FileUploadService : IUploadService
{
    private static readonly string NumberPattern = " ({0})";

    /// <summary>
    /// 异步上传文件。
    /// </summary>
    /// <param name="request">The upload request.</param>
    /// <returns>The path of the uploaded file.</returns>
    public async Task<string> UploadAsync(UploadRequest request)
    {
        if (request.Data == null || request.Data.Length == 0)
            return string.Empty;

        var folder = request.UploadType.ToString().ToLower();
        var folderName = Path.Combine("files", folder);
        if (!string.IsNullOrEmpty(request.Folder))
        {
            folderName = Path.Combine(folderName, request.Folder);
        }
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        if (!Directory.Exists(pathToSave))
        {
            Directory.CreateDirectory(pathToSave);
        }

        var fileName = request.FileName.Trim('"');
        var fullPath = Path.Combine(pathToSave, fileName);
        var dbPath = Path.Combine(folderName, fileName);

        if (!request.Overwrite && File.Exists(dbPath))
        {
            dbPath = NextAvailableFilename(dbPath);
            fullPath = NextAvailableFilename(fullPath);
        }

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await stream.WriteAsync(request.Data.AsMemory(0, request.Data.Length));

        return dbPath;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filename"></param>
    public Task RemoveAsync(string filename)
    {
        var removefile = Path.Combine(Directory.GetCurrentDirectory(), filename);
        if (File.Exists(removefile))
        {
            File.Delete(removefile);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据给定路径获取下一个可用文件名。
    /// </summary>
    /// <param name="path">检查可用性的路径。</param>
    /// <returns>下一个可用的文件名。</returns>
    public static string NextAvailableFilename(string path)
    {
        if (!File.Exists(path))
            return path;

        return Path.HasExtension(path)
            ? GetNextFilename(
                path.Insert(
                    path.LastIndexOf(Path.GetExtension(path), StringComparison.Ordinal),
                    NumberPattern
                )
            )
            : GetNextFilename(path + NumberPattern);
    }

    /// <summary>
    /// 根据给定模式获取下一个可用文件名。
    /// </summary>
    /// <param name="pattern">用于生成文件名的模式。</param>
    /// <returns>下一个可用的文件名。</returns>
    private static string GetNextFilename(string pattern)
    {
        var tmp = string.Format(pattern, 1);

        if (!File.Exists(tmp))
            return tmp;

        int min = 1,
            max = 2;

        while (File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            var pivot = (max + min) / 2;
            if (File.Exists(string.Format(pattern, pivot)))
                min = pivot;
            else
                max = pivot;
        }

        return string.Format(pattern, max);
    }
}
