using YmKB.Domain.Entities;

namespace YmKB.Application.Features.QuantizedLists.DTOs;

public class QuantizedListDto
{
    public string Id { get; set; }

    public string KbId { get; set; }
    
    public string FileName { get; set; }

    public string KbDocFileId { get; set; }

    public QuantizedListState Status { get; set; } = QuantizedListState.Pending;

    public string Remark { get; set; }
    
    public DateTime? Created { get; set; }
    public DateTime? ProcessTime { get; set; }

}