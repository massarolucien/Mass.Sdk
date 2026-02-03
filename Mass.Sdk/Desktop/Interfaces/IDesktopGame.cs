namespace Mass.Sdk.Desktop.Interfaces;

public interface IDesktopGame
{
    public string Id { get; set; }
    public string Name { get; set; }
    public uint PlayerCount { get; set; }
    public uint LikeCount { get; set; }
    public string ImageUrl { get; set; }
}