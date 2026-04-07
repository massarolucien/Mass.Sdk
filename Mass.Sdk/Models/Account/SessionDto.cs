namespace Mass.Sdk.Models.Account;

public class SessionDto
{
    public required string UserId { get; set; }
    
    public required string Cookies { get; set; }
    
    public required string Nickname { get; set; }
    
    public required AccountInfoDto Info { get; set; }
}