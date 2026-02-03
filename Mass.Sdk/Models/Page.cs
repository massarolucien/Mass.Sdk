namespace Mass.Sdk.Models;

public class Page<T>
{
    public required T[] Items { get; set; }
    public int TotalPage { get; set; }
}