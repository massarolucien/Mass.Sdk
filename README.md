# Mass.Sdk
## 项目简介
基于 **RestSharp** 的轻量级客户端封装，提供统一的 API 请求处理、鉴权登录、连通性检测及桌面端接口支持。

## ✨ 特性

- 🚀 轻量级 API Client，基于 RestSharp
- 🔐 Token 鉴权登录
- 🖥️ Desktop 客户端能力封装
- 🎮 网络游戏管理（服务器、角色）
- 🌐 一键启动本地代理服务
- 📦 现代 .NET 异步 API 设计（async / await）

## 使用示例

```csharp
using Mass.Sdk;

// 寻找 Mass 服务
var massClient = await MassClient.FindAsync();

// 登录 Mass
await massClient.MassLogin("TOKEN");

// 随机小号登录
var session = await massClient.Desktop.RandomLogin();

// 获取网络服务器列表
var netGames = await session.GetNetGames();

// 添加角色
await session.AddNetGameCharacter(game.Id, "角色名");

// 获取角色列表
var characters = await session.GetNetGameCharacters(game.Id);

// 启动代理服务
var port = await session.StartNetGameProxy(game.Id, "角色名");
```

## 📦构建步骤

1. **克隆仓库**

```bash
git clone https://github.com/massarolucien/Mass.Sdk.git
cd Mass.Sdk
```

2. **还原 NuGet 包**

```bash
dotnet restore
```

3. **编译项目**

```bash
dotnet build
```

4. **运行示例**

```bash
dotnet run --project Mass.Sdk.Example
```
