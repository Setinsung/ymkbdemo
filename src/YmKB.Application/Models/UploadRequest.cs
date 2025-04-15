using System.ComponentModel;
using SixLabors.ImageSharp.Processing;

namespace YmKB.Application.Models;

public class UploadRequest(
    string fileName,
    UploadType uploadType,
    byte[] data,
    bool overwrite = false,
    string? folder = null,
    ResizeOptions? resizeOptions = null
)
{
    public string FileName { get; set; } = fileName;
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; } = uploadType;
    public bool Overwrite { get; set; } = overwrite;
    public byte[] Data { get; set; } = data;
    public string? Folder { get; set; } = folder;
    public ResizeOptions? ResizeOptions { get; set; } = resizeOptions;
}

public enum UploadType : byte
{
    [Description(@"Products")]
    Product,

    [Description(@"Images")]
    Images,

    [Description(@"ProfilePictures")]
    ProfilePicture,

    [Description(@"Documents")]
    Document
}
