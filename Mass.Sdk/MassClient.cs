using Mass.Sdk.Desktop;
using Mass.Sdk.Helpers;
using Mass.Sdk.Mobile;
using Mass.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Mass.Sdk;

public class MassClient : IDisposable
{
    public MassClient(string baseUrl)
    {
        _client = new RestClient(new RestClientOptions(baseUrl), configureSerialization: s =>
        {
            s.UseSystemTextJson(JsonHelper.SnakeCaseOptions);
        });
        Desktop = new DesktopClient(this);
        Mobile = new MobileClient(this);
    }
    private readonly RestClient _client;

    public async Task Request(RestRequest request)
        => await Request<object>(request);
    
    public async Task<T> Request<T>(RestRequest request)
    {
        var response = await _client.ExecuteAsync<ApiResponse<T>>(request);
        if (!response.IsSuccessful || response.Data is null)
            throw new HttpRequestException("请求失败", response.ErrorException);
        if (response.Data.Code != 200)
            throw new HttpRequestException(response.Data.Msg);
        return response.Data.Data;
    }

    public async Task<bool> Ping()
    {
        try
        {
            await Request(new RestRequest("/api/base/ping"));
            return true;
        }
        catch {  }
        return false;
    }
    
    public static async Task<MassClient> FindAsync(int startPort = 23333, int tryTimes = 10)
    {
        var port = startPort;
        
        MassClient? client;
        do
        {
            client = new MassClient($"http://127.0.0.1:{port}");
            if (await client.Ping()) break;
            client.Dispose();
            client = null;
            port++;
        } while (port < startPort + tryTimes);

        if (client is null) throw new ApplicationException("没有找到 Mass 本地服务");
        return client;
    }
    
    public async Task MassLogin(string token)
        => await Request(new RestRequest("/api/security/login", Method.Post)
            .AddParameter("token", token));
    
    public readonly DesktopClient Desktop;
    public readonly MobileClient Mobile;
    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}