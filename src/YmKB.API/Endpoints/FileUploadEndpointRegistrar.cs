using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using YmKB.Application.Contracts.Upload;
using YmKB.Application.Models;

namespace YmKB.API.Endpoints;

public class FileUploadEndpointRegistrar : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/fileManagement")
            .WithTags("File Upload")
            .RequireAuthorization();

        group
            .MapGet(
                "/antiforgeryToken",
                (HttpContext context, [FromServices] IAntiforgery antiforgery) =>
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    return TypedResults.Ok(
                        new AntiforgeryTokenResponse(
                            tokens.CookieToken,
                            tokens.RequestToken,
                            tokens.HeaderName
                        )
                    );
                }
            )
            .Produces<AntiforgeryTokenResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("获取 Antiforgery Token")
            .WithDescription("检索新的防伪令牌，用于验证后续请求。");

        group
            .MapPost(
                "/upload",
                async ([FromForm] FileUploadRequest request, HttpContext context) =>
                {
                    var response = new List<FileUploadResponse>();
                    var uploadService = context
                        .RequestServices
                        .GetRequiredService<IUploadService>();
                    // 构造 URL 以访问文件
                    var requestScheme = context.Request.Scheme; // 'http' or 'https'
                    var requestHost = context.Request.Host.Value; // 'host:port'
                    foreach (var file in request.Files)
                    {
                        // var uniqueFileName = $"{Guid.CreateVersion7().ToString()}_{file.FileName}";
                        var filestream = file.OpenReadStream();
                        var stream = new MemoryStream();
                        await filestream.CopyToAsync(stream);
                        stream.Position = 0;
                        var size = stream.Length;
                        var uploadRequest = new UploadRequest(
                            file.FileName,
                            UploadType.Document,
                            stream.ToArray(),
                            request.Overwrite,
                            request.Folder
                        );
                        var result = await uploadService.UploadAsync(uploadRequest);
                        var fileUrl = result.StartsWith("https://")
                            ? result
                            : $"{requestScheme}://{requestHost}/{result.Replace("\\", "/")}";
                        response.Add(
                            new FileUploadResponse
                            {
                                FileName = file.FileName,
                                Path = result,
                                Url = fileUrl,
                                Size = size
                            }
                        );
                    }
                    return TypedResults.Ok(response);
                }
            )
            .Accepts<FileUploadRequest>("multipart/form-data")
            .Produces<IEnumerable<FileUploadResponse>>()
            .DisableAntiforgery()
            .WithMetadata(new ConsumesAttribute("multipart/form-data"))
            .WithSummary("将文件上传到服务器")
            .WithDescription("允许将多个文件上传到服务器上的特定文件夹。");

        group
            .MapPost(
                "/image",
                async ([FromForm] ImageUploadRequest request, HttpContext context) =>
                {
                    var response = new List<FileUploadResponse>();
                    var uploadService = context
                        .RequestServices
                        .GetRequiredService<IUploadService>();
                    // 构造 URL 以访问文件
                    var requestScheme = context.Request.Scheme; // 'http' or 'https'
                    var requestHost = context.Request.Host.Value; // 'host:port'
                    foreach (var file in request.Files)
                    {
                        var filestream = file.OpenReadStream();
                        var imgstream = new MemoryStream();
                        await filestream.CopyToAsync(imgstream);
                        imgstream.Position = 0;
                        if (request.CropSize_Width != null && request.CropSize_Height != null)
                        {
                            using var outStream = new MemoryStream();
                            using var image = await Image.LoadAsync(imgstream);
                            image.Mutate(
                                i =>
                                    i.Resize(
                                        new ResizeOptions
                                        {
                                            Mode = ResizeMode.Crop,
                                            Size = new Size(
                                                (int)request.CropSize_Width,
                                                (int)request.CropSize_Height
                                            )
                                        }
                                    )
                            );
                            await image.SaveAsync(outStream, PngFormat.Instance);
                            var result = await uploadService.UploadAsync(
                                new UploadRequest(
                                    file.FileName,
                                    UploadType.Images,
                                    outStream.ToArray(),
                                    request.Overwrite,
                                    request.Folder
                                )
                            );
                            var fileUrl = result.StartsWith("https://")
                                ? result
                                : $"{requestScheme}://{requestHost}/{result.Replace("\\", "/")}";
                            response.Add(
                                new FileUploadResponse
                                {
                                    Url = fileUrl,
                                    Path = result,
                                    Size = imgstream.Length
                                }
                            );
                        }
                        else
                        {
                            var uploadRequest = new UploadRequest(
                                file.FileName,
                                UploadType.Images,
                                imgstream.ToArray(),
                                request.Overwrite,
                                request.Folder
                            );
                            var result = await uploadService.UploadAsync(uploadRequest);
                            var fileUrl = result.StartsWith("https://")
                                ? result
                                : $"{requestScheme}://{requestHost}/{result.Replace("\\", "/")}";
                            response.Add(
                                new FileUploadResponse
                                {
                                    Url = fileUrl,
                                    Path = result,
                                    Size = imgstream.Length
                                }
                            );
                        }
                    }
                    return TypedResults.Ok(response);
                }
            )
            .Accepts<ImageUploadRequest>("multipart/form-data")
            .Produces<IEnumerable<FileUploadResponse>>()
            .WithMetadata(new ConsumesAttribute("multipart/form-data"))
            .WithSummary("使用裁剪选项将图像上传到服务器")
            .WithDescription(
                "允许将多个具有可选裁剪选项的图像文件上传到服务器上的特定文件夹。"
            );

        // group
        //     .MapGet(
        //         "/",
        //         async Task<Results<FileStreamHttpResult, ValidationProblem, NotFound<string>>> (
        //             [FromQuery] string path,
        //             HttpContext context
        //         ) =>
        //         {
        //             if (path.Contains("..") || path.StartsWith("/") || path.StartsWith("\\"))
        //             {
        //                 var validationProblem = TypedResults.ValidationProblem(
        //                     new Dictionary<string, string[]>
        //                     {
        //                         { "path", ["Invalid file path."] }
        //                     }
        //                 );
        //                 return validationProblem;
        //             }
        //
        //             var baseDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
        //             var filePath = Path.GetFullPath(Path.Combine(baseDirectory, path));
        //             if (!filePath.StartsWith(baseDirectory))
        //             {
        //                 var validationProblem = TypedResults.ValidationProblem(
        //                     new Dictionary<string, string[]>
        //                     {
        //                         { "path", ["Invalid file path."] }
        //                     }
        //                 );
        //                 return validationProblem;
        //             }
        //             if (!File.Exists(filePath))
        //             {
        //                 return TypedResults.NotFound($"File '{filePath}' does not exist.");
        //             }
        //             FileStream fileStream;
        //             try
        //             {
        //                 fileStream = new FileStream(
        //                     filePath,
        //                     FileMode.Open,
        //                     FileAccess.Read,
        //                     FileShare.Read,
        //                     bufferSize: 4096,
        //                     useAsync: true
        //                 );
        //             }
        //             catch (Exception ex)
        //             {
        //                 var validationProblem = TypedResults.ValidationProblem(
        //                     new Dictionary<string, string[]>
        //                     {
        //                         {
        //                             "file",
        //                             [
        //                                 "An error occurred while accessing the file.",
        //                                 ex.Message
        //                             ]
        //                         }
        //                     }
        //                 );
        //                 return validationProblem;
        //             }
        //             var contentType = GetContentType(filePath);
        //             var fileName = Path.GetFileName(filePath);
        //             return TypedResults.File(fileStream, contentType, fileName);
        //         }
        //     )
        //     .WithSummary("从服务器下载或预览文件")
        //     .WithDescription(
        //         "允许客户端通过指定文件夹和文件名来下载或预览文件。"
        //     );

        group
            .MapDelete(
                "/",
                async Task<Results<NoContent, ValidationProblem, NotFound<string>>> (
                    [FromQuery] string path,
                    HttpContext context
                ) =>
                {
                    if (path.Contains("..") || path.StartsWith("/") || path.StartsWith("\\"))
                    {
                        var validationProblem = TypedResults.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                { "path", ["Invalid file path."] }
                            }
                        );
                        return validationProblem;
                    }
                    var baseDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
                    var filePath = Path.GetFullPath(Path.Combine(baseDirectory, path));
                    if (!filePath.StartsWith(baseDirectory))
                    {
                        var validationProblem = TypedResults.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                { "path", ["Invalid file path."] }
                            }
                        );
                        return validationProblem;
                    }
                    if (!File.Exists(filePath))
                    {
                        return TypedResults.NotFound($"File '{path}' does not exist.");
                    }
                    try
                    {
                        File.Delete(filePath);
                        return TypedResults.NoContent();
                    }
                    catch (Exception ex)
                    {
                        var validationProblem = TypedResults.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                {
                                    "file",
                                    [
                                        "An error occurred while deleting the file.",
                                        ex.Message
                                    ]
                                }
                            }
                        );
                        return validationProblem;
                    }
                }
            )
            .WithSummary("从服务器中删除文件")
            .WithDescription(
                "允许客户端通过指定文件夹和文件名来删除文件。"
            );
    }

    private static string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }

    public class FileUploadRequest
    {
        /// <summary>
        /// 表示将文件上传到特定文件夹的请求。
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "文件夹名称不能超过 255 个字符。")]
        [Description("应上传文件的文件夹路径。")]
        public string Folder { get; set; } = string.Empty;

        [Description("指示是否覆盖现有文件。")]
        public bool Overwrite { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "必须至少上传一个文件。")]
        [Description("要上传的文件列表。")]
        public IFormFileCollection Files { get; set; } = new FormFileCollection();
    }

    public class FileUploadResponse
    {
        /// <summary>
        /// 文件名称。
        /// </summary>
        [Description("文件名称。")]
        public string FileName { get; set; }
        
        /// <summary>
        /// 用于访问上传文件的完整 URL。
        /// </summary>
        [Description("用于访问上传文件的完整 URL。")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 保存上传文件的路径。
        /// </summary>
        [Description("保存上传文件的路径。")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 上传文件的大小（以字节为单位）。
        /// </summary>
        [Description("上传文件的大小（以字节为单位）。")]
        public long Size { get; set; }
    }

    public class ImageUploadRequest
    {
        /// <summary>
        /// 表示将图像文件上传到特定文件夹的请求，并提供裁剪选项。
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "文件夹名称不能超过 255 个字符。")]
        [Description("应上传文件的文件夹路径。")]
        public string Folder { get; set; } = string.Empty;

        [Description("指示是否覆盖现有文件。")]
        public bool Overwrite { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "必须至少上传一个文件。")]
        [Description("要上传的文件列表。")]
        public IFormFileCollection Files { get; set; } = new FormFileCollection();

        [Description("上传图像宽度所需的裁剪大小。")]
        public int? CropSize_Width { get; set; }

        [Description("上传图像高度所需的裁剪大小。")]
        public int? CropSize_Height { get; set; }
    }

    public record AntiforgeryTokenResponse(
        string? CookieToken,
        string? RequestToken,
        string? HeaderName
    );

    public class CropSize
    {
        /// <summary>
        /// 表示裁剪图像所需的大小。
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "宽度必须大于 0。")]
        [Description("裁剪图像的宽度。")]
        public int Width { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "高度必须大于 0。")]
        [Description("裁剪图像的高度。")]
        public int Height { get; set; }
    }
}
