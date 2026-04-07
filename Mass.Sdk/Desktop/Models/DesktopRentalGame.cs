using System.Text.Json.Serialization;
using Mass.Sdk.Desktop.Enums;
using Mass.Sdk.Desktop.Interfaces;
using Mass.Sdk.Serialization;

namespace Mass.Sdk.Desktop.Models;

public class DesktopRentalGame : IDesktopGame
{
    [JsonPropertyName("entity_id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("player_count")]
    public uint PlayerCount { get; set; }

    [JsonPropertyName("like_num")]
    public uint LikeCount { get; set; }

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("server_name")]
    public string ServerName { get; set; } = string.Empty;

    [JsonPropertyName("visibility")]
    public DesktopVisibilityStatus Visibility { get; set; }

    [JsonPropertyName("has_pwd")]
    public bool HasPassword { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("server_type")]
    public DesktopServerType ServerType { get; set; }

    [JsonPropertyName("status")]
    public DesktopServerStatus Status { get; set; }

    [JsonPropertyName("capacity")]
    public uint Capacity { get; set; }

    [JsonPropertyName("mc_version")]
    public string McVersion { get; set; } = string.Empty;

    [JsonPropertyName("owner_id")]
    public long OwnerId { get; set; }

    [JsonPropertyName("world_id")]
    public string WorldId { get; set; } = string.Empty;

    [JsonPropertyName("min_level")]
    public string MinLevel { get; set; } = string.Empty;

    [JsonPropertyName("pvp")]
    public bool IsPvpEnabled { get; set; }

    [JsonPropertyName("icon_index")]
    public uint IconIndex { get; set; }

    [JsonPropertyName("offset")]
    public string? Offset { get; set; }
}