using YmKB.UI.Models;

namespace YmKB.UI.Services;

public class MockDocumentService : IDocumentService
{
    private static readonly List<Document> _documents =
        new()
        {
            new Document
            {
                FileName = "沪科技生物 选择性必修1【高清教材】.PDF",
                FileType = "PDF",
                FileSize = 4_020_000, // 4.02 MB
                UploadTime = DateTime.Parse("2025-04-17 13:59:37")
            },
            new Document
            {
                FileName = ".gitignore",
                FileType = "gitignore",
                FileSize = 6_700, // 6.7 KB
                UploadTime = DateTime.Parse("2025-04-16 01:16:02")
            },
            new Document
            {
                FileName = "nginx11.txt",
                FileType = "txt",
                FileSize = 2_680, // 2.68 KB
                UploadTime = DateTime.Parse("2025-04-15 16:49:24")
            },
            new Document
            {
                FileName = "g7.jpg",
                FileType = "jpg",
                FileSize = 84_040, // 84.04 KB
                UploadTime = DateTime.Parse("2025-04-15 16:46:08")
            }
        };

    public Task<Document> CreateDocumentAsync(Document document)
    {
        _documents.Add(document);
        return Task.FromResult(document);
    }

    public Task<bool> DeleteDocumentAsync(string id)
    {
        var doc = _documents.FirstOrDefault(x => x.Id == id);
        if (doc != null)
        {
            _documents.Remove(doc);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<Document?> GetDocumentAsync(string id)
    {
        var doc = _documents.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(doc);
    }

    public Task<(List<Document> Items, int TotalCount)> GetDocumentsAsync(
        string? searchText,
        int page,
        int pageSize
    )
    {
        var query = _documents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(
                x => x.FileName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            );
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(x => x.UploadTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((items, totalCount));
    }
}
