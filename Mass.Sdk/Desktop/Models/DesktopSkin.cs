using System.Text.Json.Serialization;

namespace Mass.Sdk.Desktop.Models;

public class DesktopSkin
{
    [JsonPropertyName("entity_id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("brief_summary")]
    public string BriefSummary { get; set; } = string.Empty;
    
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;
}