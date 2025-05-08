using YmKB.Domain.Entities;

namespace YmKB.Application.Features.KbDocFiles.DTOs;

public class KbDocFileDto
{
    public string Id { get; set; }
    public string KbId { get; set; }
    public string FileName { get; set; }
    public string Url { get; set; }
    public string Type { get; set; }
    public long Size { get; set; }
    public int? DataCount { get; set; }
    public QuantizationState? Status { get; set; }
    public SegmentPattern SegmentPattern { get; set; }
    public DateTime Created { get; set; }
}