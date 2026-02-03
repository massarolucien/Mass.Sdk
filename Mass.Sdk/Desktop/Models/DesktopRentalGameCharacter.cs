using System.Text.Json.Serialization;
using Mass.Sdk.Desktop.Interfaces;
using Mass.Sdk.Serialization;

namespace Mass.Sdk.Desktop.Models;

public class DesktopRentalGameCharacter : IDesktopGameCharacter
{
    [JsonPropertyName("server_id")]
    public string GameId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(TimestampConverter))]
    [JsonPropertyName("create_ts")]
    public DateTime CreateTime { get; set; }
}