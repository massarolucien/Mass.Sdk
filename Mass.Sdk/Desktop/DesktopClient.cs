using Mass.Sdk.Desktop.Models;
using RestSharp;

namespace Mass.Sdk.Desktop;

public class DesktopClient(MassClient client)
{
    public async Task<DesktopSession> LoginCookies(string cookies)
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/cookies", Method.Post)
            .AddParameter("cookies", cookies)));
    
    public async Task<DesktopSession> Login163(string email, string password)
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/163", Method.Post)
            .AddParameter("email", email)
            .AddParameter("password", password)));
    
    public async Task<DesktopSession> LoginMobile(string mobile, string password)
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/mobile", Method.Post)
            .AddParameter("mobile", mobile)
            .AddParameter("password", password)));
    
    public async Task SendSms(string mobile)
        => await client.Request(new RestRequest("/api/desktop/login/mobile/send", Method.Post)
            .AddParameter("mobile", mobile));
    
    public async Task<DesktopSession> LoginSms(string mobile, string code)
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/mobile/verify", Method.Post)
            .AddParameter("mobile", mobile)
            .AddParameter("code", code)));
    
    public async Task<DesktopSession> Login4399Pc(string username, string password)
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/4399pc", Method.Post)
            .AddParameter("username", username)
            .AddParameter("password", password)));
    
    public async Task<DesktopSession> Login4399Com(string username, string password)
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/4399com", Method.Post)
            .AddParameter("username", username)
            .AddParameter("password", password)));
    
    public async Task<DesktopSession> Login4399ComRandom()
        => new (client, await client.Request<string>(new RestRequest("/api/desktop/login/random-4399com", Method.Post)));
}