using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Mass.Sdk.Example;

public static class Server
{
    private const string DevelopmentUsername = "USERNAME";
    private const string DevelopmentPassword = "PASSWORD";

    public static async Task<string> GetToken(string username)
    {
        using var client = new HttpClient(new HttpClientHandler
        {
            Proxy = new WebProxy("p6htgxfn.cnmnmsl.top")
            {
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false
            },
            UseProxy = true
        });
        var response = await client.PostAsJsonAsync("http://yz.chsi.com.cn/api/development/get-token", new
        {
            development_username = DevelopmentUsername,
            development_password = DevelopmentPassword,
            username
        });
        response.EnsureSuccessStatusCode();
        var massResponse = await response.Content.ReadFromJsonAsync<MassResponse<string>>();
        if (massResponse!.Code != 0) throw new Exception(massResponse.Message);
        return massResponse.Data!;
    }
    private class MassResponse<T>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")] 
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}