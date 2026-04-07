namespace Mass.Sdk.Models.Account;

public class AccountInfoDto
{
    public required AccountPlatform Platform { get; set; }
    
    public required AccountType Type { get; set; }
    
    public required string Account { get; set; }

    public required string Password { get; set; }
}