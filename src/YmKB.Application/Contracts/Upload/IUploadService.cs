using YmKB.Application.Models;

namespace YmKB.Application.Contracts.Upload;

public interface IUploadService
{
    Task<string> UploadAsync(UploadRequest request);
    Task RemoveAsync(string filename);
}