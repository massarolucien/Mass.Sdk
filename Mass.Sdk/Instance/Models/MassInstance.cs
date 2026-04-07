using System.Text.Json.Serialization;
using Mass.Sdk.Serialization;

namespace Mass.Sdk.Instance.Models;

[JsonConverter(typeof(InstanceConverter))]
public class MassInstance
{
    public string UserId { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public long Id { get; set; }
    
    public DateTime LaunchTime { get; set; }
}