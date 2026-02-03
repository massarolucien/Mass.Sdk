using Mass.Sdk;
using Mass.Sdk.Helpers;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 初始化 Logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .CreateLogger();
        
        var massClient = await MassClient.FindAsync();
        
        // 登录 Mass
        await massClient.MassLogin("bUSNBWWVT+G5OC/jKU3IWd2X8xec5e20WBv8RnUjxWzV3030/6EyH3f5YvIL+mHaFo6+CSoo46u5uIMs9d/pJS4cXaks6c1UYSPjanbqSQuIaAkYLnLRp28DhCoHDEVOO9ja0zRlWzkZcHW0HiYG8RCeIJ8VUltzDLR60df0hLzeDsR95u6uouqiga1HJ7lHcBWwvj48R5rclnzrGX8lUGvVQ5hWkWd0KsuGtpE38VYEEKg6DU8axnY2uVxSZ+xWprEorbVSurBawh+Hn8iHi/eqo568O4q0UvZa3SL4tdsBmPv0KRvAVnl90FGhsYY770plJARarZN1/ma8RTrR9w==");
        Log.Information("成功登录Mass");
        
        // 随机小号登录
        var session = await massClient.Desktop.RandomLogin();
        Log.Information("成功登录 {UserId}", session.UserId);
        
        // 获取网络服务器列表
        var netGames = await session.GetNetGames();
        var heypixelGame = netGames.First(n => n.Name.Contains("布吉岛"));
        Log.Information("{Name} {Id}", heypixelGame.Name, heypixelGame.Id);

        // 添加随机角色        
        await session.AddNetGameCharacter(heypixelGame.Id, RandomHelper.GetString(10));
        
        // 获取角色列表
        var characters = await session.GetNetGameCharacters(heypixelGame.Id);
        var character = characters.First();
        
        // 启动代理服务
        var port = await session.StartNetGameProxy(heypixelGame.Id, character.Name);
        Log.Information("代理服务启动在 {Address}", $"127.0.0.1:{port}");
    }
}