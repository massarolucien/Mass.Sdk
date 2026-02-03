using System.Text.Json;

namespace Mass.Sdk.Helpers;

public static class JsonHelper
{
    public static readonly JsonSerializerOptions SnakeCaseOptions = new (JsonSerializerDefaults.General)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    public static string SerializeSnakeCase(object obj) => JsonSerializer.Serialize(obj, SnakeCaseOptions);
}