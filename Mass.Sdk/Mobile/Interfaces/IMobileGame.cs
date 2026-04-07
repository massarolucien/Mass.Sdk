namespace Mass.Sdk.Mobile.Interfaces;

public interface IMobileGame
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PlayerCount { get; set; }
    public string ImageUrl { get; set; }
}