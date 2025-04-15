using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace YmKB.Infrastructure.Persistence.Conversions;

public class StringListConverter : ValueConverter<List<string>, string>
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public StringListConverter()
        : base(
            v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions),
            v =>
                JsonSerializer.Deserialize<List<string>>(
                    string.IsNullOrEmpty(v) ? "[]" : v,
                    DefaultJsonSerializerOptions
                ) ?? new List<string>()
        ) { }
}
