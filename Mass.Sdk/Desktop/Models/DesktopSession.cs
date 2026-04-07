using Mass.Sdk.Instance.Models;
using Mass.Sdk.Models;
using Mass.Sdk.Models.Account;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;

namespace Mass.Sdk.Desktop.Models;

public class DesktopSession(MassClient client, SessionDto session)
{
    protected readonly MassClient Client = client;
    
    public readonly string UserId = session.UserId;
    
    public readonly string Cookies = session.Cookies;
    
    public readonly string Nickname = session.Nickname;
    
    public readonly AccountInfoDto Info = session.Info;
    
    public async Task<DesktopNetGame[]> GetNetGames()
        => await Client.Request<DesktopNetGame[]>(new RestRequest($"/api/desktop/{UserId}/net-game/list"));
    
    public async Task<DesktopRentalGame[]> GetRentalGames()
        => await Client.Request<DesktopRentalGame[]>(new RestRequest($"/api/desktop/{UserId}/rental-game/list"));
    
    public async Task<Page<DesktopSkin>> GetSkins(int page)
        => await Client.Request<Page<DesktopSkin>>(new RestRequest($"/api/desktop/{UserId}/skin/list")
            .AddQueryParameter("page", page));
    
    public async Task<Page<DesktopSkin>> GetOwnedSkins(int page)
        => await Client.Request<Page<DesktopSkin>>(new RestRequest($"/api/desktop/{UserId}/skin/owned-list")
            .AddQueryParameter("page", page));

    public async Task<DesktopNetGameCharacter[]> GetNetGameCharacters(string gameId)
        => await Client.Request<DesktopNetGameCharacter[]>(new RestRequest($"/api/desktop/{UserId}/net-game/{gameId}/list"));
    
    public async Task<DesktopRentalGameCharacter[]> GetRentalGameCharacters(string gameId)
        => await Client.Request<DesktopRentalGameCharacter[]>(new RestRequest($"/api/desktop/{UserId}/rental-game/{gameId}/list"));
    
    public async Task AddNetGameCharacter(string gameId, string name)
        => await Client.Request(new RestRequest($"/api/desktop/{UserId}/net-game/{gameId}/add", Method.Post)
            .AddParameter("name", name));
    
    public async Task AddRentalGameCharacter(string gameId, string name)
        => await Client.Request(new RestRequest($"/api/desktop/{UserId}/rental-game/{gameId}/add", Method.Post)
            .AddParameter("name", name));
    
    public async Task SetSkin(string itemId)
        => await Client.Request(new RestRequest($"/api/desktop/{UserId}/skin/{itemId}/set", Method.Post));
    
    public async Task<ProxyInstance> StartNetGameProxy(string gameId, string name)
        => await Client.Request<ProxyInstance>(new RestRequest($"/api/desktop/{UserId}/net-game/{gameId}/{name}/start-proxy", Method.Post));
    
    public async Task<ProxyInstance> StartRentalGameProxy(string gameId, string name, string? password = null)
        => await Client.Request<ProxyInstance>(new RestRequest($"/api/desktop/{UserId}/rental-game/{gameId}/{name}/start-proxy", Method.Post)
            .AddParameter("password", password));

    public async Task<GameInstance> StartNetJavaGame(string gameId, string name, IProgress<Progress>? callback = null)
        => await Client.Progress($"/api/desktop/{UserId}/net-game/{gameId}/{name}/start-game", callback);
    
    public async Task<GameInstance> StartRentalJavaGame(string gameId, string name, string? password = null, IProgress<Progress>? callback = null)
        => await Client.Progress($"/api/desktop/{UserId}/rental-game/{gameId}/{name}/start-game{(string.IsNullOrEmpty(password) ? string.Empty : $"?password={password}")}", callback);
}