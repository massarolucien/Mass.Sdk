using Mass.Sdk.Desktop.Models;
using Mass.Sdk.Instance.Models;
using Mass.Sdk.Models;
using Mass.Sdk.Models.Account;
using RestSharp;

namespace Mass.Sdk.Mobile.Models;

public class MobileSession(MassClient client, SessionDto session) : DesktopSession(client, session)
{
    public new async Task<MobileNetGame[]> GetNetGames()
        => await Client.Request<MobileNetGame[]>(new RestRequest($"/api/mobile/{UserId}/net-game/list"));
    
    public async Task<GameInstance> StartNetCppGame(string gameId, IProgress<Progress>? callback = null)
        => await Client.Progress($"/api/mobile/{UserId}/net-game/{gameId}/start-game", callback);
}