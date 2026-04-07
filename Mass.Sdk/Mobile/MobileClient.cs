using Mass.Sdk.Desktop.Models;
using Mass.Sdk.Mobile.Models;
using Mass.Sdk.Models.Account;
using RestSharp;

namespace Mass.Sdk.Mobile;

public class MobileClient(MassClient client)
{
    public async Task<MobileSession> LoginCookies(string cookies)
        => new (client, await client.Request<SessionDto>(new RestRequest("/api/mobile/login/cookies", Method.Post)
            .AddParameter("cookies", cookies)));
    
    public async Task<MobileSession> Login163(string email, string password)
        => new (client, await client.Request<SessionDto>(new RestRequest("/api/mobile/login/163", Method.Post)
            .AddParameter("email", email)
            .AddParameter("password", password)));
    
    public async Task<MobileSession> LoginMobile(string mobile, string password)
        => new (client, await client.Request<SessionDto>(new RestRequest("/api/mobile/login/mobile", Method.Post)
            .AddParameter("mobile", mobile)
            .AddParameter("password", password)));
    
    public async Task SendSms(string mobile)
        => await client.Request(new RestRequest("/api/mobile/login/mobile/send", Method.Post)
            .AddParameter("mobile", mobile));
    
    public async Task<MobileSession> LoginSms(string mobile, string code)
        => new (client, await client.Request<SessionDto>(new RestRequest("/api/mobile/login/mobile/verify", Method.Post)
            .AddParameter("mobile", mobile)
            .AddParameter("code", code)));
    
    public async Task<MobileSession> Login4399Com(string username, string password)
        => new (client, await client.Request<SessionDto>(new RestRequest("/api/mobile/login/4399com", Method.Post)
            .AddParameter("username", username)
            .AddParameter("password", password)));
    
    public async Task<MobileSession> Login4399ComRandom()
        => new (client, await client.Request<SessionDto>(new RestRequest("/api/mobile/login/random-4399com", Method.Post)));
}