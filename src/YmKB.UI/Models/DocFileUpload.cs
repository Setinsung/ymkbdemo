using MudBlazor.Extensions.Components;
using YMKB.UI.APIs.Models;

namespace YmKB.UI.Models;

public class DocFileUpload
{
    public IList<UploadableFile> UploadRequests { get; set; } = [];

    public string? WebPageUrl { get; set; } = "";
    public string? WebPageName { get; set; } = "";

    public NullableOfSegmentPattern SegmentPattern { get; set; } = NullableOfSegmentPattern.Subsection;

}