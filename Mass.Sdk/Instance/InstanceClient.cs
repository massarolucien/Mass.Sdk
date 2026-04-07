using Mass.Sdk.Instance.Models;
using Mass.Sdk.Models;
using RestSharp;

namespace Mass.Sdk.Instance;

public class InstanceClient(MassClient client)
{
    public async Task<MassInstance[]> GetList()
        => await client.Request<MassInstance[]>(new RestRequest("/api/instance/list"));

    public async Task CloseAll()
        => await client.Request(new RestRequest("/api/instance/close-all", Method.Post));

    public async Task Close(string gameId, string roleName)
        => await client.Request(new RestRequest("/api/instance/close", Method.Post)
            .AddParameter("gameId", gameId)
            .AddParameter("roleName", roleName));

    public async Task Close(long instanceId)
        => await client.Request(new RestRequest($"/api/instance/{instanceId}/close", Method.Post));
}