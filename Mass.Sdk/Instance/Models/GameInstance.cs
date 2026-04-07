using System.Text.Json.Serialization;
using Mass.Sdk.Serialization;

namespace Mass.Sdk.Instance.Models;

public class GameInstance : MassInstance
{
    public int? ProcessId { get; set; }
}