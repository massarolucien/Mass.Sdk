using System.Text.Json.Serialization;
using Mass.Sdk.Desktop.Enums;
using Mass.Sdk.Desktop.Interfaces;

namespace Mass.Sdk.Desktop.Models;

public class DesktopNetGame : IDesktopGame
{
    [JsonPropertyName("entity_id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("online_count")] 
    public uint PlayerCount { get; set; }

    [JsonPropertyName("like_num")]
    public uint LikeCount { get; set; }

    [JsonPropertyName("title_image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("brief_summary")]
    public string Summary { get; set; } = string.Empty;
    
    [JsonPropertyName("download_num")]
    public int DownloadCount { get; set; }
    
    [JsonPropertyName("mc_version_id")]
    public DesktopGameVersion GameVersionId { get; set; }
}