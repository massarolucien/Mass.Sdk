using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mass.Sdk.Serialization;

public class StringBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => 
                bool.TryParse(reader.GetString(), out var b) || reader.GetString() == "1" || reader.GetString() == "yes",
            JsonTokenType.Number =>
                reader.GetInt32() == 1,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            _ => throw new JsonException("Invalid boolean value.")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "1" : "0");
    }
}