using Mass.Sdk.Models;
using RestSharp;

namespace Mass.Sdk.Desktop.Models;

public class DesktopSession(MassClient client, string userId)
{
    public string UserId => userId;
    public async Task<DesktopNetGame[]> GetNetGames()
        => await client.Request<DesktopNetGame[]>(new RestRequest($"/api/desktop/{userId}/net-game/list"));
    
    public async Task<DesktopRentalGame[]> GetRentalGames()
        => await client.Request<DesktopRentalGame[]>(new RestRequest($"/api/desktop/{userId}/rental-game/list"));
    
    public async Task<Page<DesktopSkin>> GetSkins(int page)
        => await client.Request<Page<DesktopSkin>>(new RestRequest($"/api/desktop/{userId}/skin/list")
            .AddQueryParameter("page", page));
    
    public async Task<Page<DesktopSkin>> GetOwnedSkins(int page)
        => await client.Request<Page<DesktopSkin>>(new RestRequest($"/api/desktop/{userId}/skin/owned-list")
            .AddQueryParameter("page", page));

    public async Task<DesktopNetGameCharacter[]> GetNetGameCharacters(string gameId)
        => await client.Request<DesktopNetGameCharacter[]>(new RestRequest($"/api/desktop/{userId}/net-game/{gameId}/list"));
    
    public async Task<DesktopRentalGameCharacter[]> GetRentalGameCharacters(string gameId)
        => await client.Request<DesktopRentalGameCharacter[]>(new RestRequest($"/api/desktop/{userId}/rental-game/{gameId}/list"));
    
    public async Task AddNetGameCharacter(string gameId, string name)
        => await client.Request(new RestRequest($"/api/desktop/{userId}/net-game/{gameId}/add", Method.Post)
            .AddParameter("name", name));
    
    public async Task AddRentalGameCharacter(string gameId, string name)
        => await client.Request(new RestRequest($"/api/desktop/{userId}/rental-game/{gameId}/add", Method.Post)
            .AddParameter("name", name));
    
    public async Task SetSkin(string itemId)
        => await client.Request(new RestRequest($"/api/desktop/{userId}/skin/{itemId}/set", Method.Post));
    
    public async Task<int> StartNetGameProxy(string gameId, string name)
        => await client.Request<int>(new RestRequest($"/api/desktop/{userId}/net-game/{gameId}/{name}/start-proxy", Method.Post));
    
    public async Task<int> StartRentalGameProxy(string gameId, string name, string? password = null)
        => await client.Request<int>(new RestRequest($"/api/desktop/{userId}/rental-game/{gameId}/{name}/start-proxy", Method.Post)
            .AddParameter("password", password));
}