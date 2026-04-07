# Mass.Sdk 使用教程

## 1. 这份文档适合谁

`Mass.Sdk` 是一个面向 .NET 8 的 C# SDK，用来连接本地运行的 `Mass` 服务，并以编程方式完成：

- 发现本地 `Mass` 服务
- 使用外部签发的 token 登录 `Mass`
- 登录网易版 Minecraft 账号
- 查询网络服、租赁服、皮肤、角色
- 启动代理实例
- 启动游戏实例
- 获取实例启动进度

如果你准备在自己的工具、脚本、桌面程序、服务端程序里集成 `Mass`，这份文档就是给你的。

---

## 2. 先理解整体工作流

在开始之前，建议先把 `Mass.Sdk` 的调用链理解清楚：

1. 先启动本地 `Mass` 服务
2. 在你的 .NET 程序里创建 `MassClient`
3. 用 `MassClient.FindAsync()` 自动发现本地服务，或手动指定地址
4. 你自己从业务服务获取一个 `Mass token`
5. 调用 `MassClient.MassLogin(token)` 完成对本地 `Mass` 服务的登录
6. 通过 `Desktop` 或 `Mobile` 子客户端登录具体游戏账号
7. 拿到 `DesktopSession` / `MobileSession` 后继续查询游戏、角色、皮肤，或启动代理/游戏

这意味着：

- `Mass.Sdk` 不负责“签发 token”
- token 的签发流程应该放在调用者自己的服务端
- `Mass.Sdk` 负责“拿着 token 去登录本地 Mass 服务”
- 账号登录成功后，后续操作都围绕 `Session` 对象展开

---

## 3. 项目结构速览

`Mass.Sdk` 当前主要由下面几部分组成：

- `MassClient`
  - SDK 总入口，封装 REST 请求、SignalR 进度连接、服务探测、Mass 登录
- `DesktopClient`
  - 桌面版账号登录入口
- `MobileClient`
  - 手机版账号登录入口
- `InstanceClient`
  - 实例管理入口，可直接通过 `MassClient.Instance` 使用
- `DesktopSession`
  - 桌面版登录成功后的会话对象，可继续查服、加角色、换皮肤、启动代理、启动 Java 游戏
- `MobileSession`
  - 手机版登录成功后的会话对象，可继续查服、启动 Cpp 游戏
- `Models`
  - 各类响应模型、分页模型、进度模型、账号模型、实例模型

---

## 4. 运行前提

### 4.1 开发环境

- .NET SDK 8.0 或更高版本
- 本地可运行的 `Mass.LocalServer`
- 你自己的 token 获取方式
- 调用者自己的服务端授权接口

### 4.2 为什么必须先启动本地服务

`Mass.Sdk` 并不是直连远程业务系统，它主要是调用本地 `Mass` 服务暴露出来的 HTTP / SignalR 接口。

从源码可以看出：

- `MassClient.FindAsync()` 默认从 `127.0.0.1:23333` 开始扫描
- `Mass.LocalServer` 的默认端口也是 `23333`

所以最常见的使用方式就是：

1. 启动 `Mass.LocalServer`
2. 再运行你的 SDK 调用程序

---

## 5. 安装与引用

如果你的项目和仓库在一起，可以直接添加项目引用：

```xml
<ItemGroup>
  <ProjectReference Include="..\Mass.Sdk\Mass.Sdk.csproj" />
</ItemGroup>
```

如果你只是想运行仓库里的示例项目：

```powershell
dotnet run --project .\Mass.Sdk.Example\Mass.Sdk.Example.csproj
```

`Mass.Sdk.Example` 里的 `Server` 类不是“建议在客户端里直接请求开放平台”的意思，而是一个演示“如何向 Mass 开放平台申请 token”的最小示例。

在真实项目里，更推荐这样做：

1. 你的客户端把“当前软件用户名”发送给你自己的服务端
2. 你的服务端校验这个用户是否有权限使用你的软件
3. 你的服务端再去请求 Mass 开放平台换取 token
4. 你的服务端把 token 返回给客户端
5. 客户端再调用 `MassClient.MassLogin(token)`

这样做的好处是：

- 不会把开放平台的敏感凭据暴露给客户端
- 授权逻辑由调用者自己掌控
- 可以在自己的服务端做用户校验、封禁、额度控制、审计日志等业务处理

---

## 6. 第一个可运行示例

下面是一条最标准的主线：

```csharp
using Mass.Sdk;
using Mass.Sdk.Helpers;
using Mass.Sdk.Models;

var client = await MassClient.FindAsync();

var token = await GetMassTokenFromYourServerAsync("your-software-username");
await client.MassLogin(token);

var session = await client.Desktop.Login4399ComRandom();

var games = await session.GetNetGames();
var game = games.First();

await session.AddNetGameCharacter(game.Id, RandomHelper.GetString(10));

var characters = await session.GetNetGameCharacters(game.Id);
var character = characters.First();

var proxy = await session.StartNetGameProxy(game.Id, character.Name);

Console.WriteLine($"代理端口: {proxy.Port}");
```

这段代码做了几件事：

- 自动发现本地 `Mass` 服务
- 从你自己的服务端获取 token 并登录 `Mass`
- 使用一个随机 4399 账号登录桌面版
- 获取网络服列表
- 给目标游戏添加一个随机角色
- 获取角色列表
- 启动代理实例

---

## 7. 启动本地 `Mass` 服务

如果你在这个仓库里开发，可以直接运行：

```powershell
dotnet run --project .\Mass.LocalServer\Mass.LocalServer.csproj
```

通常情况下，服务会监听本地端口，SDK 默认从 `23333` 开始探测。

如果你没有使用默认端口，可以手动创建客户端：

```csharp
using var client = new MassClient("http://127.0.0.1:25000");
```

---

## 8. 创建 `MassClient`

### 8.1 自动发现服务

```csharp
var client = await MassClient.FindAsync();
```

默认行为：

- 起始端口：`23333`
- 尝试次数：`10`
- 实际扫描范围：`23333` 到 `23342`

也可以自定义：

```csharp
var client = await MassClient.FindAsync(startPort: 24000, tryTimes: 20);
```

### 8.2 手动指定地址

```csharp
var client = new MassClient("http://127.0.0.1:23333");
```

适合这些场景：

- 你明确知道服务地址
- 你不想走扫描逻辑
- 本地服务运行在非默认端口

### 8.3 服务健康检查

```csharp
var ok = await client.Ping();
Console.WriteLine(ok ? "服务可用" : "服务不可用");
```

---

## 9. `MassLogin` 是什么

### 9.1 它不是账号登录

很多人第一次看 SDK 会把这一步和“网易账号登录”混在一起。实际上：

- `MassLogin(token)` 登录的是本地 `Mass` 服务
- `Desktop.LoginXXX(...)` / `Mobile.LoginXXX(...)` 登录的才是具体账号

### 9.2 正确顺序

```csharp
var client = await MassClient.FindAsync();

var token = await GetMassTokenFromYourServerAsync("your-software-username");
await client.MassLogin(token);

var desktopSession = await client.Desktop.Login4399ComRandom();
```

### 9.3 token 从哪里来

SDK 里没有“获取 token”的公共接口。示例项目 `Mass.Sdk.Example` 里的 `Server.GetToken(username)` 展示的是“向 Mass 开放平台获取 token”的过程，再把拿到的 token 传给：

```csharp
await client.MassLogin(token);
```

但这个过程在真实项目里应该放在你自己的服务端，而不是直接放在最终客户端里。

原因很简单：

- 请求开放平台时通常会带上敏感凭据
- 这些敏感凭据不应该出现在客户端程序中
- 授权判断应该由调用者自己的服务端完成

也就是说，推荐架构不是：

```text
客户端 -> Mass 开放平台
```

而是：

```text
客户端 -> 你的服务端 -> Mass 开放平台
```

另外要特别注意，`Server.GetToken(username)` 里的 `username` 指的是“调用者自己的软件用户名”，不是网易账号、不是 Mass SDK 的会话用户，也不是 4399 账号。

一个更合理的理解方式是：

- 你的软件先识别出“当前是谁在使用你的软件”
- 你的服务端决定“这个人是否允许申请 Mass token”
- 通过后，你的服务端代表该用户去开放平台申请 token
- 最后把 token 返回给客户端用于 `MassLogin`

---

## 10. `DesktopClient` 用法

`MassClient.Desktop` 提供桌面版相关登录入口。

### 10.1 Cookies 登录

```csharp
var session = await client.Desktop.LoginCookies(cookies);
```

适用场景：

- 你已经有现成的登录态
- 你不想在代码里直接处理明文账号密码

### 10.2 163 邮箱登录

```csharp
var session = await client.Desktop.Login163(email, password);
```

### 10.3 手机号密码登录

```csharp
var session = await client.Desktop.LoginMobile(mobile, password);
```

### 10.4 发送短信验证码

```csharp
await client.Desktop.SendSms(mobile);
```

### 10.5 手机验证码登录

```csharp
var session = await client.Desktop.LoginSms(mobile, code);
```

### 10.6 4399 PC 登录

```csharp
var session = await client.Desktop.Login4399Pc(username, password);
```

### 10.7 4399 网页登录

```csharp
var session = await client.Desktop.Login4399Com(username, password);
```

### 10.8 随机 4399 网页账号登录

```csharp
var session = await client.Desktop.Login4399ComRandom();
```

这个方法非常适合：

- 本地调试
- 快速验证流程是否可通
- 不关心固定账号，只想先跑通功能

---

## 11. `MobileClient` 用法

`MassClient.Mobile` 提供手机版相关登录入口。

它和桌面版的大部分登录方式类似，支持：

- `LoginCookies`
- `Login163`
- `LoginMobile`
- `SendSms`
- `LoginSms`
- `Login4399Com`
- `Login4399ComRandom`

示例：

```csharp
var session = await client.Mobile.Login4399ComRandom();
```

---

## 12. `Session` 对象是什么

账号登录成功后，你拿到的是 `DesktopSession` 或 `MobileSession`。

这两个对象会保存当前登录上下文，常见字段包括：

- `UserId`
- `Cookies`
- `Nickname`
- `Info`

示例：

```csharp
Console.WriteLine(session.UserId);
Console.WriteLine(session.Nickname);
Console.WriteLine(session.Info.Platform);
Console.WriteLine(session.Info.Type);
Console.WriteLine(session.Info.Account);
```

其中 `Info` 里通常包含：

- 登录平台
- 登录方式
- 账号
- 密码

如果你的程序有日志系统，请谨慎打印 `Info.Password`，避免泄露敏感信息。

---

## 13. 查询桌面版网络服

```csharp
var games = await session.GetNetGames();

foreach (var game in games)
{
    Console.WriteLine($"{game.Name} | {game.Id} | 在线 {game.PlayerCount}");
}
```

`DesktopNetGame` 常用字段：

- `Id`
- `Name`
- `PlayerCount`
- `LikeCount`
- `ImageUrl`
- `Summary`
- `DownloadCount`
- `GameVersionId`

适合做这些事情：

- 搜索目标服务器
- 按名称过滤
- 按游戏版本过滤
- 展示在线人数、简介、封面图

例如按名称筛选：

```csharp
var games = await session.GetNetGames();
var target = games.FirstOrDefault(x => x.Name.Contains("布吉岛"));
```

---

## 14. 查询桌面版租赁服

```csharp
var rentalGames = await session.GetRentalGames();

foreach (var game in rentalGames)
{
    Console.WriteLine($"{game.ServerName} | {game.Id} | {game.Status}");
}
```

`DesktopRentalGame` 常用字段：

- `Id`
- `Name`
- `ServerName`
- `PlayerCount`
- `LikeCount`
- `ImageUrl`
- `Visibility`
- `HasPassword`
- `ServerType`
- `Status`
- `Capacity`
- `McVersion`
- `OwnerId`
- `WorldId`
- `MinLevel`
- `IsPvpEnabled`
- `IconIndex`
- `Offset`

如果你的业务需要判断是否能直接进入某个租赁服，通常会重点关注：

- `HasPassword`
- `Status`
- `Visibility`

---

## 15. 查询皮肤列表

### 15.1 查询全部皮肤

```csharp
var page = await session.GetSkins(1);

Console.WriteLine($"总页数: {page.TotalPage}");
foreach (var skin in page.Items)
{
    Console.WriteLine($"{skin.Name} | {skin.Id}");
}
```

### 15.2 查询已拥有皮肤

```csharp
var page = await session.GetOwnedSkins(1);
```

`Page<T>` 结构很简单：

- `Items`
- `TotalPage`

`DesktopSkin` 常用字段：

- `Id`
- `Name`
- `BriefSummary`
- `ImageUrl`

---

## 16. 设置皮肤

```csharp
await session.SetSkin(itemId);
```

通常你会先获取皮肤列表，再把目标皮肤的 `Id` 传给 `SetSkin`：

```csharp
var page = await session.GetOwnedSkins(1);
var targetSkin = page.Items.First();

await session.SetSkin(targetSkin.Id);
```

---

## 17. 查询角色列表

### 17.1 网络服角色

```csharp
var characters = await session.GetNetGameCharacters(gameId);

foreach (var character in characters)
{
    Console.WriteLine($"{character.Name} | {character.CreateTime}");
}
```

### 17.2 租赁服角色

```csharp
var characters = await session.GetRentalGameCharacters(gameId);
```

角色模型共有这些核心字段：

- `GameId`
- `Name`
- `CreateTime`

注意：

- SDK 已经把时间戳转换成了 `DateTime`
- 时间来自时间戳转换器 `TimestampConverter`

---

## 18. 添加角色

### 18.1 给网络服添加角色

```csharp
await session.AddNetGameCharacter(gameId, "MyRoleName");
```

### 18.2 给租赁服添加角色

```csharp
await session.AddRentalGameCharacter(gameId, "MyRoleName");
```

### 18.3 生成随机角色名

SDK 自带了 `RandomHelper`：

```csharp
using Mass.Sdk.Helpers;

var name = RandomHelper.GetString(10);
await session.AddNetGameCharacter(gameId, name);
```

也可以自定义字符集：

```csharp
var name = RandomHelper.GetString(8, "abcdefghijklmnopqrstuvwxyz0123456789");
```

---

## 19. 启动网络服代理

```csharp
var proxy = await session.StartNetGameProxy(gameId, roleName);

Console.WriteLine(proxy.Id);
Console.WriteLine(proxy.Port);
Console.WriteLine(proxy.Type);
Console.WriteLine(proxy.LaunchTime);
```

返回值是 `ProxyInstance`，它继承自 `MassInstance`，额外包含：

- `Port`

最常见的用法是：

```csharp
var proxy = await session.StartNetGameProxy(gameId, roleName);
var address = $"127.0.0.1:{proxy.Port}";

Console.WriteLine($"请连接到 {address}");
```

---

## 20. 启动租赁服代理

```csharp
var proxy = await session.StartRentalGameProxy(gameId, roleName, password);
```

如果目标租赁服没有密码，也可以不传：

```csharp
var proxy = await session.StartRentalGameProxy(gameId, roleName);
```

建议在调用前先检查：

- `game.HasPassword`
- 你是否真的拿到了正确密码

---

## 21. 启动桌面版 Java 游戏

### 21.1 启动网络服 Java 游戏

```csharp
var game = await session.StartNetJavaGame(gameId, roleName);
```

### 21.2 启动租赁服 Java 游戏

```csharp
var game = await session.StartRentalJavaGame(gameId, roleName, password);
```

返回值是 `GameInstance`，继承自 `MassInstance`，并增加：

- `ProcessId`

通常意味着：

- 游戏进程已经被启动
- 你可以拿到实例 ID
- 对于 Java / Cpp 启动型实例，可能还能拿到进程号

---

## 22. 监听启动进度

这是 `Mass.Sdk` 很重要的一部分。

游戏启动不是一个瞬时动作，所以 SDK 使用 SignalR 订阅进度事件。你可以把一个 `IProgress<Progress>` 传给启动方法。

示例：

```csharp
var progress = new Progress<Mass.Sdk.Models.Progress>(p =>
{
    Console.WriteLine($"[{p.Step}/{p.Total}] {p.Percentage}% - {p.Message}");
});

var instance = await session.StartNetJavaGame(gameId, roleName, progress);
```

`Progress` 模型包含：

- `Step`
- `Total`
- `Percentage`
- `Message`

这很适合：

- 控制台实时输出
- 桌面 UI 进度条
- WebSocket 中转给前端
- 任务状态监控

---

## 23. `MobileSession` 的特殊能力

`MobileSession` 继承自 `DesktopSession`，但有两个关键点需要注意：

### 23.1 `GetNetGames()` 返回类型不同

手机版的 `GetNetGames()` 返回的是 `MobileNetGame[]`：

```csharp
var games = await mobileSession.GetNetGames();
```

字段包括：

- `Id`
- `Name`
- `Description`
- `PlayerCount`
- `ImageUrl`

### 23.2 可启动 Cpp 游戏

```csharp
var progress = new Progress<Mass.Sdk.Models.Progress>(p =>
{
    Console.WriteLine($"{p.Percentage}% - {p.Message}");
});

var instance = await mobileSession.StartNetCppGame(gameId, progress);
```

---

## 24. 实例管理 `InstanceClient`

当前版本里，`InstanceClient` 已经挂在 `MassClient` 上，可以直接通过 `client.Instance` 使用。

### 24.1 查询实例列表

```csharp
var instances = await client.Instance.GetList();

foreach (var instance in instances)
{
    Console.WriteLine($"{instance.Id} | {instance.Type} | {instance.LaunchTime}");
}
```

### 24.2 关闭所有实例

```csharp
await client.Instance.CloseAll();
```

### 24.3 按游戏 ID 和角色名关闭实例

```csharp
await client.Instance.Close(gameId, roleName);
```

### 24.4 按实例 ID 关闭实例

```csharp
await client.Instance.Close(instanceId);
```

---

## 25. `MassInstance`、`GameInstance`、`ProxyInstance` 的区别

### 25.1 `MassInstance`

所有实例的基础模型，公共字段：

- `UserId`
- `Type`
- `Id`
- `LaunchTime`

### 25.2 `GameInstance`

表示真正启动了一个游戏实例，额外字段：

- `ProcessId`

### 25.3 `ProxyInstance`

表示启动了一个代理服务实例，额外字段：

- `Port`

### 25.4 SDK 如何自动识别实例类型

SDK 内部有一个 `InstanceConverter`，会根据返回 JSON 里的 `type` 字段自动反序列化：

- `java` 或 `cpp` -> `GameInstance`
- `java_proxy` -> `ProxyInstance`
- 其他 -> `MassInstance`

这意味着你拿到 `MassInstance[]` 时，里面实际可能混有不同子类型。

例如：

```csharp
foreach (var instance in instances)
{
    if (instance is ProxyInstance proxy)
    {
        Console.WriteLine($"代理端口: {proxy.Port}");
    }
    else if (instance is GameInstance game)
    {
        Console.WriteLine($"游戏进程: {game.ProcessId}");
    }
}
```

---

## 26. 错误处理

### 26.1 SDK 的异常行为

`MassClient.Request<T>` 在这些情况下会抛 `HttpRequestException`：

- HTTP 请求失败
- 响应无法反序列化
- 服务端返回的 `code` 不等于 `200`

所以你应该在业务层捕获异常：

```csharp
try
{
    var session = await client.Desktop.Login4399ComRandom();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"请求失败: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"未知错误: {ex}");
}
```

### 26.2 `FindAsync()` 失败

如果扫描不到本地服务，会抛出异常。

常见原因：

- `Mass.LocalServer` 没启动
- 服务端口不是默认范围
- 本地防火墙或环境限制导致无法访问

建议写成：

```csharp
MassClient client;

try
{
    client = await MassClient.FindAsync();
}
catch
{
    client = new MassClient("http://127.0.0.1:25000");
}
```

---

## 27. 资源释放

`MassClient` 实现了 `IDisposable`，建议在使用完成后释放：

```csharp
using var client = await MassClient.FindAsync();
```

或者：

```csharp
var client = await MassClient.FindAsync();
try
{
    // do work
}
finally
{
    client.Dispose();
}
```

---

## 28. 推荐调用顺序

下面是一条比较稳妥的生产化顺序：

1. 启动并确认本地 `Mass.LocalServer` 可用
2. `using var client = await MassClient.FindAsync();`
3. `await client.MassLogin(token);`
4. 调用 `Desktop` / `Mobile` 完成账号登录
5. 检查 `session` 的 `UserId`、`Nickname`、`Info`
6. 查询游戏列表并筛选目标游戏
7. 查询角色列表
8. 如果没有角色，先添加角色
9. 再启动代理或游戏
10. 用 `client.Instance` 做收尾清理

---

## 29. 一个更完整的控制台示例

```csharp
using Mass.Sdk;
using Mass.Sdk.Helpers;
using Mass.Sdk.Instance.Models;

using var client = await MassClient.FindAsync();

var token = await GetMassTokenFromYourServerAsync("demo-user");
await client.MassLogin(token);

var session = await client.Desktop.Login4399ComRandom();

Console.WriteLine($"登录成功: {session.Nickname} ({session.UserId})");

var games = await session.GetNetGames();
var targetGame = games.FirstOrDefault(g => g.Name.Contains("布吉岛")) ?? games.First();

Console.WriteLine($"目标游戏: {targetGame.Name} | {targetGame.Id}");

var characters = await session.GetNetGameCharacters(targetGame.Id);
var role = characters.FirstOrDefault();

if (role is null)
{
    var newName = RandomHelper.GetString(10);
    await session.AddNetGameCharacter(targetGame.Id, newName);
    characters = await session.GetNetGameCharacters(targetGame.Id);
    role = characters.First();
}

Console.WriteLine($"使用角色: {role.Name}");

var progress = new Progress<Mass.Sdk.Models.Progress>(p =>
{
    Console.WriteLine($"[{p.Step}/{p.Total}] {p.Percentage}% {p.Message}");
});

var proxy = await session.StartNetGameProxy(targetGame.Id, role.Name);
Console.WriteLine($"代理已启动: 127.0.0.1:{proxy.Port}");

var instances = await client.Instance.GetList();

foreach (var instance in instances)
{
    Console.WriteLine($"实例: {instance.Id} | {instance.Type} | {instance.LaunchTime}");
}

// 如果要启动 Java 游戏，改用下面这行：
// var gameInstance = await session.StartNetJavaGame(targetGame.Id, role.Name, progress);

static async Task<string> GetMassTokenFromYourServerAsync(string username)
{
    await Task.CompletedTask;
    throw new NotImplementedException("这里应该调用你自己的服务端接口。username 是你自己软件里的用户名，不是网易账号。");
}
```

---

## 30. 常见问题

### 30.1 为什么 `MassLogin` 之后还要 `Desktop.LoginXXX`？

因为它们是两层登录：

- 第一层：登录本地 `Mass` 服务
- 第二层：登录目标游戏账号

### 30.2 为什么 `FindAsync()` 找不到服务？

优先检查：

- `Mass.LocalServer` 是否已经启动
- 端口是不是默认的 `23333`
- 你是否需要手动指定地址

### 30.3 为什么启动游戏时没有立即返回？

因为游戏启动是异步流程，SDK 会通过 SignalR 等待后端把进度和最终实例推回来。

### 30.4 为什么建议复用同一个 `MassClient`？

因为 `MassLogin` 完成后，后续请求依赖同一个客户端上下文。最稳妥的做法是：

- 一个业务流程使用一个 `MassClient`
- 不要在 `MassLogin` 后频繁新建客户端再继续调用

### 30.5 `InstanceClient` 应该怎么用？

当前版本已经可以直接通过：

```csharp
var instances = await client.Instance.GetList();
```

如果你只是做实例查询、关闭、清理，一般不需要再手动创建 `InstanceClient`。

---

## 31. 最佳实践

- 先 `Ping()` 或 `FindAsync()`，再进入主流程
- 把 token 获取逻辑和 SDK 调用逻辑分开
- token 申请放在你自己的服务端，不要把开放平台凭据下发到客户端
- 不要在日志里打印账号密码
- 启动游戏时始终传入 `IProgress<Progress>`，便于排错
- 启动前先检查是否已有角色，避免不必要的重复创建
- 业务结束后用 `client.Instance` 清理无用实例
- 始终释放 `MassClient`

---

## 32. API 速查

### 32.1 `MassClient`

| 方法/成员 | 说明 |
| --- | --- |
| `new MassClient(baseUrl)` | 手动指定服务地址 |
| `MassClient.FindAsync(startPort, tryTimes)` | 自动探测本地服务 |
| `Ping()` | 检查服务可用性 |
| `MassLogin(token)` | 登录本地 Mass 服务 |
| `Desktop` | 桌面版子客户端 |
| `Mobile` | 手机版子客户端 |
| `Instance` | 实例管理子客户端 |

### 32.2 `DesktopClient`

| 方法 | 说明 |
| --- | --- |
| `LoginCookies(cookies)` | Cookies 登录 |
| `Login163(email, password)` | 163 邮箱登录 |
| `LoginMobile(mobile, password)` | 手机号密码登录 |
| `SendSms(mobile)` | 发送短信验证码 |
| `LoginSms(mobile, code)` | 验证码登录 |
| `Login4399Pc(username, password)` | 4399 PC 登录 |
| `Login4399Com(username, password)` | 4399 网页登录 |
| `Login4399ComRandom()` | 随机 4399 网页账号登录 |

### 32.3 `DesktopSession`

| 方法 | 说明 |
| --- | --- |
| `GetNetGames()` | 获取网络服列表 |
| `GetRentalGames()` | 获取租赁服列表 |
| `GetSkins(page)` | 获取皮肤列表 |
| `GetOwnedSkins(page)` | 获取已拥有皮肤 |
| `GetNetGameCharacters(gameId)` | 获取网络服角色 |
| `GetRentalGameCharacters(gameId)` | 获取租赁服角色 |
| `AddNetGameCharacter(gameId, name)` | 添加网络服角色 |
| `AddRentalGameCharacter(gameId, name)` | 添加租赁服角色 |
| `SetSkin(itemId)` | 设置皮肤 |
| `StartNetGameProxy(gameId, name)` | 启动网络服代理 |
| `StartRentalGameProxy(gameId, name, password)` | 启动租赁服代理 |
| `StartNetJavaGame(gameId, name, callback)` | 启动网络服 Java 游戏 |
| `StartRentalJavaGame(gameId, name, password, callback)` | 启动租赁服 Java 游戏 |

### 32.4 `MobileSession`

| 方法 | 说明 |
| --- | --- |
| `GetNetGames()` | 获取手机版网络服列表 |
| `StartNetCppGame(gameId, callback)` | 启动手机版 Cpp 游戏 |

### 32.5 `InstanceClient`

| 方法 | 说明 |
| --- | --- |
| `GetList()` | 获取实例列表 |
| `CloseAll()` | 关闭所有实例 |
| `Close(gameId, roleName)` | 按游戏和角色关闭实例 |
| `Close(instanceId)` | 按实例 ID 关闭实例 |

---

## 33. 总结

如果只记住一条主线，请记住下面这四步：

1. 启动本地 `Mass.LocalServer`
2. `MassClient.FindAsync()` 找到服务
3. `MassLogin(token)` 登录 `Mass`
4. 用 `DesktopSession` / `MobileSession` 完成后续所有业务动作
