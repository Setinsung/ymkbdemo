using System.Text.Json;
using System.Text.Json.Serialization;

namespace YmKB.UI.ConstantConfigs;

public static class JsonSerializerConfigs
{
    public static JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}