using Mass.Sdk;
using Mass.Sdk.Example;
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
        
        // 查找本地Mass服务
        var massClient = await MassClient.FindAsync();
        
        // 从 Mass 服务端获取临时Token
        var token = await Server.GetToken("YOUR_USERNAME");
        
        // 登录 Mass
        await massClient.MassLogin(token);
        Log.Information("成功登录Mass");
        
        // 随机小号登录
        var session = await massClient.Desktop.Login4399ComRandom();
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