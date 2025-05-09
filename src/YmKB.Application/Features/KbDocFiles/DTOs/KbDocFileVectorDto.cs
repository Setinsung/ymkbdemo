namespace YmKB.Application.Features.KbDocFiles.DTOs;

public class KbDocFileVectorDto
{
    /// <summary>
    /// 索引
    /// </summary>
    public int Index { get; set; }
    
    public string Id { get; set; }

    /// <summary>
    /// 原始内容
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// 关联知识文档
    /// </summary>
    public string KbDocFileId { get; set; }
}