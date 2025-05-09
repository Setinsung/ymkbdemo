namespace YmKB.Application.Features.KnowledgeDbs.DTOs;

public class SearchedVectorsDto
{
    /// <summary>
    /// 搜索耗时
    /// </summary>
    public double ElapsedTime { get; set; }

    /// <summary>
    /// 搜索结果列表
    /// </summary>
    public List<SearchVectorItem> Results { get; set; } = [];
}

public class SearchVectorItem
{
    /// <summary>
    /// 文本内容
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// 路径
    /// </summary>
    public string? Path { get; set; }
    
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// 相关性
    /// </summary>
    public double Relevance { get; set; }
    
    /// <summary>
    /// 知识库文档Id
    /// </summary>
    public string KbDocFileId { get; set; }
    
    public string DocumentId { get; set; }

}