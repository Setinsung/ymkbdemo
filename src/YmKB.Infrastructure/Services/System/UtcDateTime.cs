using YmKB.Application.Contracts;

namespace YmKB.Infrastructure.Services;

public class UtcDateTime : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}

