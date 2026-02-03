namespace Mass.Sdk.Desktop.Interfaces;

public interface IDesktopGameCharacter
{
    public string GameId { get; set; }

    public string Name { get; set; }

    public DateTime CreateTime { get; set; }
}