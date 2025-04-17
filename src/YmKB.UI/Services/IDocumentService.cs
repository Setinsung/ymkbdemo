using YmKB.UI.Models;

namespace YmKB.UI.Services;

public interface IDocumentService
{
    Task<(List<Document> Items, int TotalCount)> GetDocumentsAsync(
        string? searchText,
        int page,
        int pageSize
    );
    Task<Document?> GetDocumentAsync(string id);
    Task<bool> DeleteDocumentAsync(string id);
    Task<Document> CreateDocumentAsync(Document document);
}
