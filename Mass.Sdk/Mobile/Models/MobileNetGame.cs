using System.Text.Json.Serialization;
using Mass.Sdk.Mobile.Interfaces;

namespace Mass.Sdk.Mobile.Models;

public class MobileNetGame : IMobileGame
{
    [JsonPropertyName("res_name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("brief")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("title_image_url")]
    public string ImageUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("entity_id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("online_num")]
    public int PlayerCount { get; set; }
}