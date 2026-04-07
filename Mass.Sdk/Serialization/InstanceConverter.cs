using System.Text.Json;
using System.Text.Json.Serialization;
using Mass.Sdk.Instance.Models;

namespace Mass.Sdk.Serialization;

public class InstanceConverter : JsonConverter<MassInstance>
{
    public override MassInstance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        string? type = null;
        if (root.TryGetProperty("type", out var typeElement))
            type = typeElement.GetString();
        return (type switch
        {
            "java" or "cpp" => root.Deserialize<GameInstance>(),
            "java_proxy" => root.Deserialize<ProxyInstance>(),
            _ => root.Deserialize<MassInstance>()
        })!;
    }

    public override void Write(Utf8JsonWriter writer, MassInstance value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}