using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace YmKB.Infrastructure.Persistence.Conversions;

public static class ValueConversionExtensions
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        var converter = new ValueConverter<T, string>(
            v => JsonSerializer.Serialize(v, Options),
            v => string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize<T>(v, Options)
        );

        var comparer = new ValueComparer<T>(
            (l, r) => JsonSerializer.Serialize(l, Options) == JsonSerializer.Serialize(r, Options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, Options).GetHashCode(),
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, Options), Options)
        );

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}
