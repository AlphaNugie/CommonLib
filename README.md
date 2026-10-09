# CommonLib 解决方案

## 项目概览

| 项目 | 目标框架 | 说明 |
|------|---------|------|
| `CommonLib` | .NET Framework 4.5 | 基础工具类库（网络、数据、UI、扩展等） |
| `CommonLib.Core` | .NET 9.0 | CommonLib 的 .NET 9.0 版本（以 Link 方式共享源码） |
| `CommonLib.Core-win` | .NET 9.0-windows | WPF 控件的 .NET 9.0 版本（以 Link 方式共享 WPF 源码） |
| `OpcLibraryAnyCpu` | .NET Framework 4.5 | OPC DA 读写辅助类库 |
| `OpcLibraryAnyCpu.Ua` | .NET Framework 4.8 | OPC UA 读写辅助类库 |
| `OpcLibraryAnyCpu.UaNet100` | .NET 10.0 | OPC UA 的 .NET 10.0 版本 |
| `CommonLib.DataUtil.SqliteProviderLib` | .NET Framework 4.5 | SQLite 数据库提供程序 |
| 其他辅助项目 | 各异 | Oracle 提供程序、拼音转换、曲线拟合等 |

---

# CommonLib — 基础工具类库

## FileSystemHelper — 文件系统与应用程序数据目录

**文件**：`CommonLib/Helpers/FileSystemHelper.cs`

**命名空间**：`CommonLib.Helpers`

**功能概述**：提供路径处理、文件/目录操作，以及**应用程序数据目录（AppData）的统一管理**。可将日志、配置、状态等文件存放到用户目录而非程序目录，避免程序目录受保护无法写入、被部署覆盖、多用户互相干扰等问题。

### 1. 属性

| 属性 | 说明 |
|------|------|
| `StartupPath` | 可执行文件的启动目录（不是当前 DLL 的目录） |
| `DiskRoot` | 可执行文件所在盘符根目录 |
| `VolumeSeparator` | 盘符与路径的分隔符（如 `C:\`） |
| `DirSeparatorChar` / `DirSeparator` | 当前平台的目录分隔符（字符 / 字符串） |
| `AppDataRoamingPath` | 当前用户 Roaming 目录（%APPDATA%） |
| `AppDataLocalPath` | 当前用户 Local 目录（%LOCALAPPDATA%） |
| `AppDataCommonPath` | 所有用户共用目录（%PROGRAMDATA%） |
| `DEFAULT_APP_DIRECTORY` | 常量，默认应用目录名（`"AppData"`），无法获取产品名时使用 |

### 2. AppData 目录相关方法

| 方法 | 说明 |
|------|------|
| `GetProductName()` | 获取入口程序集的产品名（AssemblyProduct），失败返回 `null` |
| `GetAppDataDirectory(subDirectory, roaming)` | 获取应用数据目录，不存在时**自动创建** |
| `GetAppDataDirectory(roaming, appName, subDirectory)` | 同上，可指定应用目录名 |
| `GetAppDataFilePath(fileName, roaming, appName)` | 获取应用数据目录下的**文件完整路径**，目录自动创建 |
| `OpenAppDataDirectory(roaming, appName, subDirectory)` | 在资源管理器中打开应用数据目录，返回被打开的目录路径 |

### 3. 调用方式

#### 3.1 获取应用数据目录

```csharp
using CommonLib.Helpers;

// 目录名自动取产品名（AssemblyProduct），例如 ScannerBuilder
// 返回 C:\Users\<用户名>\AppData\Roaming\ScannerBuilder
string dir = FileSystemHelper.GetAppDataDirectory();

// 指定子目录（可多级）
// 返回 ...\Roaming\ScannerBuilder\Logs
string logDir = FileSystemHelper.GetAppDataDirectory("Logs");

// 使用 Local 而非 Roaming
string localDir = FileSystemHelper.GetAppDataDirectory("Cache", roaming: false);

// 自定义应用目录名（不使用产品名）
string dir2 = FileSystemHelper.GetAppDataDirectory(roaming: true, appName: "MyApp", subDirectory: "Config");
```

#### 3.2 获取文件完整路径（推荐）

```csharp
using CommonLib.Helpers;

// 返回 C:\Users\<用户名>\AppData\Roaming\ScannerBuilder\PointCloudScanState.txt
// 所在目录不存在时自动创建
string filePath = FileSystemHelper.GetAppDataFilePath("PointCloudScanState.txt");

// 带子目录
string logPath = FileSystemHelper.GetAppDataFilePath("Logs/app.log");

// 之后可直接读写，无需再判断目录是否存在
File.WriteAllLines(filePath, lines);
```

#### 3.3 在资源管理器中打开目录

```csharp
using CommonLib.Helpers;

// 打开应用数据目录（不存在时先创建），返回被打开的目录路径
string dir = FileSystemHelper.OpenAppDataDirectory();

// 打开指定子目录
FileSystemHelper.OpenAppDataDirectory(roaming: true, appName: null, subDirectory: "Logs");
```

> 该方法不捕获异常，调用方需自行处理（例如 WinForms 中弹 MessageBox）。

#### 3.4 完整示例：状态文件持久化

```csharp
using CommonLib.Helpers;
using System.IO;

public class MyStateManager
{
    private const string STATE_FILE_NAME = "MyState.txt";

    // 状态文件路径（目录自动创建）
    private static string StateFilePath
        => FileSystemHelper.GetAppDataFilePath(STATE_FILE_NAME);

    public static void Save(IEnumerable<string> lines)
    {
        // 先写临时文件再替换，避免写入中途损坏原文件
        string temp = StateFilePath + ".tmp";
        File.WriteAllLines(temp, lines);
        if (File.Exists(StateFilePath))
            File.Delete(StateFilePath);
        File.Move(temp, StateFilePath);
    }

    public static List<string> Load()
        => File.Exists(StateFilePath)
            ? File.ReadAllLines(StateFilePath).ToList()
            : new List<string>();

    // 提供给菜单/按钮调用：打开状态文件所在目录
    public static void OpenDirectory()
    {
        try { FileSystemHelper.OpenAppDataDirectory(); }
        catch (Exception ex) { /* 自行处理，如 MessageBox.Show */ }
    }
}
```

### 4. 关键设计说明

- **目录自动创建**：`GetAppDataDirectory` / `GetAppDataFilePath` 在目录不存在时自动 `Directory.CreateDirectory`，调用方无需预先判断。
- **产品名获取链路**：优先取 `Assembly.GetEntryAssembly()`（宿主 exe）；服务、IIS、单元测试等环境下入口程序集可能为 `null`，此时回退到调用程序集；仍失败则使用 `DEFAULT_APP_DIRECTORY`。
- **Roaming vs Local**：
  - `roaming: true`（默认）→ `%APPDATA%`，随用户配置漫游，适合配置、状态等小文件
  - `roaming: false` → `%LOCALAPPDATA%`，不漫游，适合缓存、大文件
- **路径拼接安全**：内部统一使用 `Path.Combine` 与 `TrimFilePath`，自动去除首尾分隔符，避免拼接出畸形路径。
- **不改现有行为**：本次为**纯新增**，`FileClient.DefaultLogDir`、`LogClient` 等现有类的默认路径行为（相对程序目录）保持不变。如需让它们写入 AppData，请显式传入绝对路径：

  ```csharp
  FileClient.DefaultLogDir = FileSystemHelper.GetAppDataDirectory("Logs");
  ```

  > 注意：`FileClient` 内部无条件在路径前拼接启动目录，传入绝对路径会产生畸形路径。若需统一日志位置，需先调整 `FileClient` 的路径拼接逻辑（`LogClient` 已做了绝对路径判断，可正常传绝对路径）。

### 5. 其他常用方法

| 方法 | 说明 |
|------|------|
| `OpenInExplorer(filePath, ignoreIfNotExist)` | 在资源管理器中打开并选中指定文件 |
| `CheckForDirectory(path, isFilePath)` | 检查目录是否存在，不存在则创建 |
| `CheckForStartupPath(path, ignoreEmptyString)` | 路径不含卷分隔符时，在前面添加启动目录 |
| `AddDateToFileName(fileName, format)` | 在文件名中插入日期 |
| `TrimFilePath(filePath)` | 去除路径首尾的分隔符 |
| `UpdateFilePath(...)` | 根据路径与文件名补全完整路径 |

---

## DerivedUdpClient — UDP 连接客户端

**文件**：`CommonLib/Clients/DerivedUdpClient.cs`

**命名空间**：`CommonLib.Clients`

**功能概述**：封装 `System.Net.Sockets.UdpClient`，提供 UDP 通信的完整解决方案，包括连接管理、自动重连、异步数据收发、超时检测等功能。

### 1. 构造器

| 构造器 | 说明 |
|------|------|
| `DerivedUdpClient(string local_ip = "", int local_port = 0, bool auto_receive = true, bool hold_port = true)` | 主构造器，指定本地IP、端口、是否自动接收、重连时是否保持端口 |
| `DerivedUdpClient(bool autoReceive, bool holdPort = true)` | 简化构造器，不绑定特定本地端口 |

**构造器行为**：
- 初始化 `UdpClient` 对象（`BaseClient`）
- 启动 `TimerEventRaiser` 定时器（每1秒触发一次）
- 注册 `Raiser_ThresholdReached` 超时检测回调
- 若 `AutoReceive=true`，立即调用 `BeginReceive` 开始异步接收

### 2. 属性

#### 2.1 连接配置属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ServerIp` | `string` | `"127.0.0.1"` | 远程服务端 IP 地址 |
| `ServerPort` | `int` | `0` | 远程服务端端口 |
| `LocalIp` | `string` | `""` | 本地绑定 IP |
| `LocalPort` | `int` | `0` | 本地绑定端口（≤0 则随机） |
| `HoldLocalPort` | `bool` | `true` | 重连时是否保持相同本地端口 |

#### 2.2 连接状态属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `IsConnected` | `bool` | 是否已连接（逻辑状态，连接成功后置 true） |
| `IsConnected_Socket` | `bool` | Socket 级别的实际连接状态（通过 `IsSocketConnected()` 扩展方法检测） |
| `IsStartListening` | `bool` | 是否已启动监听（异步接收） |
| `Name` | `string` | 连接名称，格式：`本地端口->服务端IP:端口` |

#### 2.3 网络终结点属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `BaseClient` | `UdpClient?` | 底层的 `System.Net.Sockets.UdpClient` 对象 |
| `RemoteEndPoint` | `IPEndPoint?` | 远程 IP 终结点（连接成功后设置） |
| `LocalEndPoint` | `IPEndPoint?` | 本地 IP 终结点（初始化时根据 `LocalIp`+`LocalPort` 设置） |

#### 2.4 数据收发控制属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `AutoReceive` | `bool` | 是否自动接收数据（修改时仅做状态标记） |
| `ReceiveRestTime` | `int` | 数据接收停顿时间（毫秒），每次接收后等待这段时间再进行下一次接收 |
| `LastErrorMessage` | `string` | 最近一次操作产生的错误信息 |

#### 2.5 超时与重连控制属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ReconnectWhenReceiveNone` | `bool` | `false` | 持续无数据时是否自动重连 |
| `RaiseThreshold` | `int` | `5000` | 计时阈值（毫秒），计时达到此值触发 `OnNoneReceived` 事件 |
| `RaiseInterval` | `int` | `5000` | 触发间隔（毫秒），两次触发 `OnNoneReceived` 事件间的最短时间 |
| `ReconnTimer` | `int` | `0` | 重连成功次数（连接成功后重置为0，每次重连成功+1） |

### 3. 事件

| 事件 | 委托类型 | 触发条件 |
|------|------|------|
| `Connected` | `ConnectedEventHandler?` | UDP 连接成功时触发 |
| `Disconnected` | `DisconnectedEventHandler?` | UDP 连接关闭时触发 |
| `ReconnTimerChanged` | `ReconnTimerChangedEventHandler?` | 重连成功次数变化时触发（参数：连接名称、重连次数） |
| `DataReceived` | `DataReceivedEventHandler?` | 接收到数据时异步触发（参数：连接名称、接收数据字节数组） |
| `OnNoneReceived` | `NoneReceivedEventHandler?` | 持续一段时间未收到数据时触发（由 `RaiseThreshold`/`RaiseInterval` 控制） |

所有事件均使用 `BeginInvoke` 异步触发，不阻塞主流程。

### 4. 核心方法

#### 4.1 连接管理

**`Connect()` / `Connect(string server, int port)` / `Connect(string server, int port, string localIp, int localPort)`**

```cs
// 使用已配置的 ServerIp/ServerPort 连接
int result = client.Connect();

// 指定远程 IP 和端口连接
int result = client.Connect("192.168.1.100", 8000);

// 指定远程 IP/端口 + 本地 IP/端口连接
int result = client.Connect("192.168.1.100", 8000, "192.168.1.50", 9000);
```

**连接流程**：
1. 保存服务端/本地 IP 和端口到属性
2. 若 `BaseClient` 为 null，调用 `InitBaseClient()` 初始化
3. 调用 `BaseClient.Connect(ServerIp, ServerPort)` 建立 UDP 连接
4. 设置 `RemoteEndPoint`、更新 `Name`
5. 重置 `ReconnTimer` 为 0，触发 `ReconnTimerChanged` 事件
6. 触发 `Connected` 事件
7. 启动后台重连线程 `Thread_Reconnect`（若尚未启动）
8. 若 `AutoReceive=true`，调用 `BeginReceive` 开始异步接收

**返回值**：成功返回 `1`，失败返回 `0`。

---

**`Close()`**

```cs
int result = client.Close();
```

**关闭流程**：
1. 清空 `LastErrorMessage`
2. 终止后台重连线程（.NET Framework 用 `Abort()`，.NET 9 用 `Join()`）
3. 关闭并释放 `BaseClient`
4. 重置 `IsConnected=false`、`ServerIp="127.0.0.1"`、`ServerPort=0`
5. 触发 `Disconnected` 事件

**返回值**：成功返回 `1`，失败返回 `0`。

---

**`Reconnect_TheWholeDeal()`**

```cs
client.Reconnect_TheWholeDeal();
```

若当前处于连接状态，则先 `Close()` 再 `Connect()`，实现完整的断开重连。失败时静默处理（try-catch 包裹）。

---

**`SetName()`**

```cs
client.SetName();
```

通过 `BaseClient.Client.GetName()` 扩展方法获取并设置连接名称（格式：`本地端口->服务端IP:端口`）。

---

#### 4.2 自动重连机制

**`TcpAutoReconnect()`**（私有，后台线程）

每隔 **1 秒**检测一次连接状态：

```
┌─ IsConnected == false ──→ Auto_UdpReconnect.WaitOne() 挂起等待
│
└─ IsConnected == true && 实际 Socket 已断开 ──→ 执行重连：
     1. BaseClient.Close()
     2. 重新 new UdpClient()（若 HoldLocalPort=true 则复用 LocalEndPoint）
     3. BaseClient.Connect(ServerIp, ServerPort)
     4. ReconnTimer++，触发 ReconnTimerChanged 事件
     5. 触发 Connected 事件
     6. 若 AutoReceive=true，重新 BeginReceive
```

**线程同步**：使用 `AutoResetEvent`（`Auto_UdpReconnect`）控制线程挂起/唤醒，`Connect()` 方法中通过 `Set()` 唤醒挂起中的重连线程。

**线程终止**：`ThreadAbort()` 方法，.NET Framework 使用 `Abort()`，.NET 9 使用 `Join()` + 释放 `AutoResetEvent`。

---

#### 4.3 超时检测

**`Raiser_ThresholdReached` 回调**（私有）

由 `TimerEventRaiser`（1秒周期定时器）驱动：
- 持续未收到数据达到 `RaiseThreshold` 毫秒 → 触发 `OnNoneReceived` 事件
- 若 `ReconnectWhenReceiveNone=true` → 自动调用 `Reconnect_TheWholeDeal()`
- 两次触发间隔至少 `RaiseInterval` 毫秒

---

#### 4.4 数据发送

**`SendData(object data_origin, out string errorMessage)`**

```cs
// 发送到已连接的服务端
bool ok = client.SendData(hexBytes, out string error);

// 发送到指定 IPEndPoint
bool ok = client.SendData(hexBytes, new IPEndPoint(IPAddress.Parse("192.168.1.100"), 8000), out error);

// 发送到指定 IP + 端口
bool ok = client.SendData(hexBytes, "192.168.1.100", 8000, out error);
```

**支持的 `data_origin` 类型**：
| 类型 | 处理方式 |
|------|------|
| `byte[]` | 直接发送 |
| `string` | 作为 16 进制格式字符串，通过 `HexHelper.HexString2Bytes()` 转换后发送 |
| 其他 | 返回 false，`errorMessage` 报告格式错误 |

---

**`SendString(string content)`**

```cs
// 发送到已连接的服务端
client.SendString("Hello UDP");

// 发送到指定 IPEndPoint
client.SendString("Hello UDP", new IPEndPoint(IPAddress.Parse("192.168.1.100"), 8000));

// 发送到指定 IP + 端口
client.SendString("Hello UDP", "192.168.1.100", 8000);
```

将字符串以 ASCII 编码转换为 `byte[]` 后发送。发送失败时抛出异常并记录到 `LastErrorMessage`。

---

#### 4.5 数据接收

**`ReceiveCallBack(IAsyncResult ar)`**（私有，异步回调）

```cs
// 由 BeginReceive 触发，运行在后台线程
```

**接收流程**：
1. 调用 `BaseClient.EndReceive(ar, ref point)` 获取数据字节数组
2. 若数据长度 > 0：
   - 异步触发 `DataReceived` 事件
   - 调用 `raiser.Click()` 重置超时计时器
   - 若 `ReceiveRestTime > 0`，等待指定毫秒
   - 若 `AutoReceive=true`，再次调用 `BeginReceive` 继续接收

---

**`Read(out string asc, out string hex)`**

```cs
string ascii, hex;
client.Read(out ascii, out hex);
```

**同步读取**当前可用数据（不阻塞），通过 `BaseClient.Receive()` 获取数据后转换为 ASCII 字符串和 16 进制字符串。

---

### 5. 使用示例

#### 5.1 基本 UDP 客户端

```cs
// 创建 UDP 客户端，绑定本地 9000 端口
var client = new DerivedUdpClient("192.168.1.50", 9000, auto_receive: true, hold_port: true);

// 订阅事件
client.Connected += (name, e) => Console.WriteLine($"已连接：{name}");
client.Disconnected += (name, e) => Console.WriteLine($"已断开：{name}");
client.DataReceived += (name, e) => Console.WriteLine($"收到数据：{e.ReceivedInfo_HexString}");
client.OnNoneReceived += (sender, e) => Console.WriteLine($"警告：{e.Threshold}ms 内未收到数据");
client.ReconnTimerChanged += (name, count) => Console.WriteLine($"重连次数：{count}");

// 连接到服务端
client.Connect("192.168.1.100", 8000);

// 发送数据
client.SendData("AA BB CC DD", out string error);
client.SendString("Hello Server");

// 设置超时检测
client.RaiseThreshold = 5000;      // 5秒未收到数据触发事件
client.ReconnectWhenReceiveNone = true; // 超时后自动重连

// 关闭连接
client.Close();
```

#### 5.2 无连接模式（仅发送，不 Connect）

```cs
var client = new DerivedUdpClient("", 0, auto_receive: false);

// 直接向目标地址发送
client.SendString("Ping", "192.168.1.100", 8000);
client.SendData(new byte[] { 0x01, 0x02, 0x03 }, "192.168.1.100", 8000, out string error);
```

#### 5.3 同步读取可用数据

```cs
var client = new DerivedUdpClient("192.168.1.50", 9000);
client.Connect("192.168.1.100", 8000);

// 阻塞直到有数据到达
string ascii, hex;
client.Read(out ascii, out hex);
Console.WriteLine($"ASCII: {ascii}");
Console.WriteLine($"HEX: {hex}");
```

---

### 6. 关键设计说明

| 设计点 | 说明 |
|------|------|
| **双状态检测** | `IsConnected`（逻辑状态）+ `IsConnected_Socket`（Socket 实际状态），防止逻辑状态与实际状态不一致 |
| **自动重连线程** | 后台线程每 1 秒轮询，`AutoResetEvent` 控制挂起/唤醒，避免空转消耗 CPU |
| **异步事件** | 所有事件通过 `BeginInvoke` 异步触发，不阻塞网络 IO 线程 |
| **超时检测** | 基于 `TimerEventRaiser` 的滑动窗口计时器，收到数据时 `Click()` 重置，超时后触发事件 |
| **收发控制** | `ReceiveRestTime` 控制接收节奏，`AutoReceive` 控制是否持续接收 |
| **条件编译** | .NET Framework 用 `Thread.Abort()`，.NET 9 用 `Thread.Join()`（Abort 在 .NET Core+ 中不支持） |
| **空安全** | .NET 9 下事件委托和引用类型属性使用 `?` 标记可空 |

---

## DerivedHttpClient — HTTP 发送客户端

**文件**：`CommonLib/Clients/DerivedHttpClient.cs`

**命名空间**：`CommonLib.Clients`

**功能概述**：封装 `System.Net.Http.HttpClient`，提供简洁的 HTTP 同步请求方法（GET/POST/PUT/DELETE），支持 Bearer Token 认证、多种 Content-Type、超时控制。

### 1. 构造器

| 构造器 | 说明 |
|------|------|
| `DerivedHttpClient()` | 默认构造器，超时时间 5000ms |
| `DerivedHttpClient(int timeout)` | 指定超时时间（毫秒） |

> .NET 9 下使用主构造器语法：`public class DerivedHttpClient(int timeout)`

### 2. 属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Timeout` | `int` | `5000` | 执行任务时的等待时间（毫秒），超过此时间抛出 `TimeoutException` |

### 3. ContentType 枚举

| 值 | HTTP Content-Type 头 |
|------|------|
| `ContentType.FormUrlEncoded` | `application/x-www-form-urlencoded` |
| `ContentType.Xml` | `application/xml` |
| `ContentType.Json` | `application/json` |
| `ContentType.FormData` | `multipart/form-data` |

### 4. 核心方法

#### 4.1 OAuth — Bearer Token 认证

```cs
client.OAuth("my-access-token");
```

在请求头的 `Authorization` 中设置 `Bearer {token}`。调用一次后，后续所有请求自动携带该认证信息。

#### 4.2 Get — GET 请求

```cs
string response = client.Get("http://api.example.com/users");
```

URL 未包含 `http://` 时会自动补全。

#### 4.3 Post — POST 请求（3 个重载）

```cs
// 方式一：字符串内容，默认 FormUrlEncoded
string response = client.Post("http://api.example.com/data", "key1=value1&key2=value2");

// 方式二：指定 Content-Type
string response = client.Post("http://api.example.com/data", "{\"name\":\"张三\"}", ContentType.Json);

// 方式三：Dictionary 自动编码为 FormUrlEncodedContent
var data = new Dictionary<string, string> { ["key"] = "value" };
string response = client.Post("http://api.example.com/data", data);
```

#### 4.4 Put — PUT 请求（2 个重载）

```cs
// 无内容
string response = client.Put("http://api.example.com/status");

// 带 Dictionary 内容
var data = new Dictionary<string, string> { ["status"] = "active" };
string response = client.Put("http://api.example.com/status", data);
```

#### 4.5 Delete — DELETE 请求

```cs
string response = client.Delete("http://api.example.com/users/1");
```

#### 4.6 FixUrl — URL 自动补全（私有静态）

```cs
// 输入 "api.example.com/data" → 返回 "http://api.example.com/data"
// 输入 "https://api.example.com/data" → 保持不变
```

#### 4.7 RunTask — 任务执行引擎（私有）

内部核心方法，所有 HTTP 方法最终都委托给它执行：
1. 调用 `task.Wait(Timeout)` 同步等待
2. 若超时，抛出 `TimeoutException`
3. 若 `AggregateException`，解包 `InnerException` 并重新抛出（保留原始堆栈）
4. 成功则返回 `response.Content.ReadAsStringAsync().Result`

### 5. 使用示例

```cs
// 创建客户端（超时 10 秒）
var client = new DerivedHttpClient(10000);

// Bearer Token 认证
client.OAuth("eyJhbGciOiJIUzI1NiIs...");

// GET 请求
string users = client.Get("https://api.example.com/users");
Console.WriteLine(users);

// POST JSON
string result = client.Post("https://api.example.com/users",
    "{\"name\":\"李四\",\"age\":30}", ContentType.Json);

// POST 表单
var formData = new Dictionary<string, string>
{
    ["username"] = "admin",
    ["password"] = "123456"
};
string loginResult = client.Post("https://api.example.com/login", formData);

// PUT 更新
var updateData = new Dictionary<string, string> { ["name"] = "王五" };
string putResult = client.Put("https://api.example.com/users/1", updateData);

// DELETE 删除
string deleteResult = client.Delete("https://api.example.com/users/1");
```

---

## DerivedHttpListener — HTTP 监听服务端

**文件**：`CommonLib/Clients/DerivedHttpListener.cs`

**命名空间**：`CommonLib.Clients`

**功能概述**：封装 `System.Net.HttpListener`，提供轻量级 HTTP 服务端，支持异步请求处理、POST 数据接收、可定制响应、GET 浏览器友好消息。

### 1. 构造器

| 构造器 | 说明 |
|------|------|
| `DerivedHttpListener()` | 默认构造器，端口 8080，地址 `127.0.0.1`，后缀 `/` |
| `DerivedHttpListener(Encoding encoding)` | 指定编解码方式 |
| `DerivedHttpListener(string ip, int port, string suffix, Encoding encoding)` | 完整构造器，指定 IP、端口、后缀、编解码 |

> .NET 9 下 `ip` 和 `suffix` 参数标记为可空（`string?`）

### 2. 属性

#### 2.1 监听配置

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `IpAddress` | `string` | `"127.0.0.1"` | 监听地址（实际监听使用 `+` 通配符） |
| `Port` | `int` | `8080` | 监听端口 |
| `Suffix` | `string` | `"/"` | URL 路径后缀 |
| `Name` | `string?` | — | 完整监听地址（格式：`http://+:{Port}{Suffix}`） |
| `BaseListener` | `HttpListener?` | — | 底层的 `HttpListener` 对象 |

#### 2.2 响应配置

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `RESPONSE_OK` | `const string` | `"OK"` | 操作成功的默认响应消息 |
| `ResponseDescription` | `string` | `"OK"` | 定制化返回的 POST 响应状态描述（HTTP Status Description） |
| `WebExplorerMessage` | `string` | `""` | 浏览器 GET 访问时的返回内容 |

#### 2.3 其他

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `GUID` | `string?` | — | 每次接收到请求时生成的全局唯一标识符 |
| `LastErrorMessage` | `string` | `""` | 最近一次错误信息 |
| `ReceiveBufferSize` | `int` | `32768` | 接收缓冲区大小（字节数） |

### 3. 事件

| 事件 | 委托类型 | 触发条件 |
|------|------|------|
| `DataReceived` | `DataReceivedEventHandler?` | 收到 POST 请求数据时触发 |
| `ServiceStateChanged` | `ServiceStateEventHandler?` | 服务启动/停止时触发 |

### 4. 核心方法

#### 4.1 Start — 启动监听

```cs
int result = listener.Start();
```

**流程**：
1. 创建 `HttpListener`，添加前缀 `http://+:{Port}{Suffix}`
2. 调用 `BaseListener.Start()` 启动监听
3. 调用 `BeginGetContext(Result, null)` 开始异步等待请求
4. 启动 `TimerEventRaiser` 超时检测定时器（1 秒周期）
5. 触发 `ServiceStateChanged` 事件（状态：`ServiceState.Started`）

**返回值**：成功 `1`，失败 `0`。

> 注意：监听 `http://+:{Port}/` 格式需要**管理员权限**，否则会抛异常。

#### 4.2 Stop — 停止监听

```cs
int result = listener.Stop();
```

停止 `HttpListener` 和超时检测定时器，触发 `ServiceStateChanged` 事件（状态：`ServiceState.Stopped`）。`ObjectDisposedException` 单独捕获（设为 null 即可，不抛出）。

#### 4.3 Result — 异步回调（私有）

每个 HTTP 请求到来时自动执行：

```
请求到达
  ├── 重新 BeginGetContext（持续监听）
  ├── 生成 GUID
  ├── 获取 HttpListenerContext
  │
  ├── 请求方法 == "POST" 且有输入流？
  │   ├── 是 → HandleRequest() 处理请求
  │   │        ├── 循环读取 InputStream 直到读完
  │   │        ├── 触发 DataReceived 事件（传入字节数组 + 编码）
  │   │        ├── 重置超时检测计时器
  │   │        └── 返回 ResponseDescription（状态码 200）
  │   │
  │   └── 否 → 返回 WebExplorerMessage（浏览器访问提示）
  │
  └── 通过 response.OutputStream 写回客户端
```

**关键设计**：
- **POST vs GET 区分**：POST 请求走数据处理流程，其他方法（GET 等）返回 `WebExplorerMessage`，方便浏览器测试
- **响应编码**：使用构造器传入的 `Encoding`（默认 `Encoding.Default`）
- **Content-Type**：固定 `text/plain;charset=UTF-8`

### 5. 超时检测

内部使用 `TimerEventRaiser`（1 秒周期），默认阈值 10 秒：
- 每次收到数据时调用 `raiser.Click()` 重置计时
- 若 10 秒内无新请求 → 触发 `Raiser_ThresholdReached` 回调（当前为空实现，供扩展）

### 6. 使用示例

#### 6.1 基本监听服务

```cs
// 创建监听器，端口 8080
var listener = new DerivedHttpListener("127.0.0.1", 8080, "/api/", Encoding.UTF8);

// 订阅事件
listener.DataReceived += (sender, e) =>
{
    Console.WriteLine($"收到数据：{e.ReceivedInfo_String}");
};

listener.ServiceStateChanged += (sender, e) =>
{
    Console.WriteLine($"服务状态：{e.State} - {e.Message}");
};

// 设置 GET 浏览器访问时的返回消息
listener.WebExplorerMessage = "<html><body><h1>API 服务运行中</h1></body></html>";

// 启动
listener.Start();

Console.WriteLine($"HTTP 服务已启动：{listener.Name}");
Console.ReadLine();

// 停止
listener.Stop();
```

#### 6.2 客户端发送 POST 请求

```cs
// 配合 DerivedHttpClient 使用
var client = new DerivedHttpClient();
string response = client.Post("http://127.0.0.1:8080/api/", "Hello Server");
Console.WriteLine($"服务端响应：{response}"); // 输出：OK
```

#### 6.3 定制 POST 响应

```cs
listener.ResponseDescription = "Data received successfully";
// 客户端 POST 后将收到 "Data received successfully" 而非默认的 "OK"
```

---

## SetClauseBuilder — SQL SET 子句链式构建器

**文件**：`CommonLib/DataUtil/SetClauseBuilder.cs`

**命名空间**：`CommonLib.DataUtil`

**功能概述**：用于动态拼接 UPDATE 语句的 SET 部分，支持链式调用，减少重复的 `string.Format` 代码。

### 1. 构造器

| 构造器 | 说明 |
|------|------|
| `SetClauseBuilder()` | 无参构造，内部自动创建列表 |
| `SetClauseBuilder(List<string> list)` | 传入外部列表（兼容已有列表的场景） |

### 2. API

| 方法 | 说明 | 示例输出 |
|------|------|----------|
| `.Add(col, val)` | 普通数值字段 | `walking = 123.456` |
| `.AddFormatted(col, val, fmt)` | 带格式化数值字段 | `stack_height = 10.500` |
| `.AddQuoted(col, val)` | 字符串字段（自动加单引号） | `quality = 'good'` |
| `.AddRaw(raw)` | 原始子句，不做任何处理 | 用于复杂表达式 |
| `.Build()` | 用 `,\n  ` 拼接所有子句并返回 | 完整 SET 子句字符串 |

### 3. 使用示例

#### 3.1 基本用法

```cs
// 链式构建，最终调用 Build() 输出
var builder = new SetClauseBuilder();
builder.Add("col1", 123)
       .AddFormatted("col2", 3.14159, "f3")      // col2 = 3.142
       .AddQuoted("col3", "hello")                // col3 = 'hello'
       .AddRaw("ms0 = 100, ms1 = 200");           // 原始内容直接追加

string setClause = builder.Build();
// 结果：
// col1 = 123,
//   col2 = 3.142,
//   col3 = 'hello',
//   ms0 = 100, ms1 = 200
```

#### 3.2 结合条件判断

```cs
var builder2 = new SetClauseBuilder();

// 始终更新的字段
builder2.Add("walking_left_plc", opcSource.WalkingPositionLeft_Plc)
        .Add("pitch_plc", opcSource.PitchAngle_Plc);

// 条件更新的字段
if (gnssInfo != null && gnssInfo.ProperlyInitialized)
{
    builder2.Add("t.walking", gnssInfo.WalkingPosition)
            .Add("t.pitch", gnssInfo.PitchAngle)
            .AddQuoted("quality", gnssInfo.PositionQuality);
}

string sql = $"update t_table set\n  {builder2.Build()},\n  t.time = sysdate where t.name = 'xxx'";
```

### 4. 关键设计说明

| 设计点 | 说明 |
|------|------|
| **链式调用** | 所有 `Add` 方法返回 `this`，支持 `.Add().Add().Build()` 的流式写法 |
| **类型兼容** | `Add` / `AddFormatted` 参数类型为 `object`，接受 `double`、`int`、`bool` 等任意类型 |
| **字符串自动加引号** | `AddQuoted` 自动包裹单引号，用于字符串字段值 |
| **原始内容透传** | `AddRaw` 不做任何处理直接追加，适用于预构建的子句或复杂表达式 |
| **输出不含尾逗号** | `Build()` 返回拼接后的字符串，不包含末尾逗号，方便与 SQL 语句拼接 |

---

## NumericParamEditor — 数值参数编辑器

**文件位置**：`CommonLib/UIControlUtil/NumericParamEditor/`（`NumericParam.cs`、`NumericParamTable.cs`）

**命名空间**：`CommonLib.UIControlUtil`

**功能概述**：将「数值参数的声明式描述 + 表驱动读写逻辑」抽象为可复用的纯逻辑库，与具体业务实体和 UI 框架解耦，供 WPF、WinForms、控制台等任意宿主项目共用。

### 1. 设计目标与分层架构

**设计目标**：纯逻辑库不依赖任何 UI 框架（`DispatcherTimer`、`TextBox` 等 UI 元素留在各宿主项目），保证可被 .NET Framework 4.8 与 .NET 9 项目同时引用。

```
┌─ 类库：NumericParamEditor（纯逻辑，无 UI/业务依赖）────────┐
│  NumericParam<T>          ← 单个参数的元数据 + 读写/钳制/格式化 │
│  NumericParamTable<T>     ← 参数集合 + 按名/Tag 查找 + 方向解析 │
└───────────────────────────────────────────────────────────────┘
              ↑ 引用
┌─ 各宿主项目（WPF/WinForms/控制台） ────────────────────────┐
│  - 构建 NumericParam<TSource>[]                             │
│  - UI 控件绑定（TextBox/Button 映射）                       │
│  - 长按重复机制（由宿主自行实现，如 DispatcherTimer）        │
└───────────────────────────────────────────────────────────────┘
```

### 2. 核心类

#### 2.1 `NumericParam<T>` — 单个数值参数描述

描述一个可编辑的数值参数，通过 getter/setter 委托读写宿主对象 `T` 上的某个 `double` 字段/属性。

**属性**：

| 成员 | 类型 | 说明 |
|---|---|---|
| `Name` | `string` | 参数名（唯一标识），也用于控件 Tag 匹配 |
| `Step` | `double` | 单次步进量（正数） |
| `Min` | `double` | 取值范围下限（含边界） |
| `Max` | `double` | 取值范围上限（含边界） |
| `DecimalPlaces` | `int` | 显示小数位数（**显式字段**，替代旧实现的 `Step==0.05` 隐式判断） |
| `Getter` | `Func<T, double>` | 读取宿主对象上的值 |
| `Setter` | `Action<T, double>` | 写入宿主对象 |

**方法**：

| 方法 | 说明 |
|---|---|
| `Get(T host)` | 读取当前值 |
| `Set(T host, double value)` | 写入值 |
| `Clamp(double value)` | 钳制到 `[Min, Max]` 区间 |
| `StepValue(T host, double direction)` | 步进一次并返回钳制后新值（不写回） |
| `Format(double value)` | 按 `DecimalPlaces` 格式化为文本 |

**构造校验**：
- `name` / `getter` / `setter` 为空 → 抛 `ArgumentNullException`
- `step` 非正数 → 抛 `ArgumentOutOfRangeException`
- `min > max` → 抛 `ArgumentOutOfRangeException`

#### 2.2 `NumericParamTable<T>` — 数值参数集合

一组 `NumericParam<T>` 的集合，提供查找与批量操作。

**成员**：

| 成员 | 说明 |
|---|---|
| `Count` | 参数数量 |
| `this[int index]` | 按索引获取参数 |
| `Add(param)` | 添加参数，重名抛 `ArgumentException` |
| `Find(name)` | 按参数名精确查找，找不到返回 `null` |
| `FindByTag(object tag)` | 按控件 Tag 查找（支持 `"Roll"` 或 `"Roll:1"` 两种格式） |
| `ParseDirection(tag, positiveDirection)` | 从 Tag 解析方向（冒号后部分为 `"1"` 或正数返回 `+positiveDirection`，否则返回 `-positiveDirection`） |
| `GetEnumerator()` | 支持 `foreach` 遍历 |

> 注意：`ParseDirection` 是静态方法，签名为 `ParseDirection(object tag, double positiveDirection = 1.0)`。

### 3. 使用示例

#### 3.1 泛用用法（任意宿主类型）

`NumericParamEditor` 与具体业务实体无关，宿主类型 `T` 可以是任意包含若干 `double` 字段/属性的类：

```cs
using CommonLib.UIControlUtil;

// 1. 定义宿主对象：任意含 double 成员的类
public class RobotConfig
{
    public double MaxSpeed   { get; set; } = 2.0;
    public double Accel      { get; set; } = 0.5;
    public double PidKp      { get; set; } = 1.2;
    public double OffsetX    { get; set; } = 0.0;
}

// 2. 创建实例 + 元数据表
var config = new RobotConfig();
var table = new NumericParamTable<RobotConfig>();

// 3. 声明参数（名字、步长、范围、小数位、读写委托）
table.Add(new NumericParam<RobotConfig>("MaxSpeed", 0.1, 0,    10,  1, c => c.MaxSpeed, (c, v) => c.MaxSpeed = v));
table.Add(new NumericParam<RobotConfig>("Accel",    0.05, 0,   5,   2, c => c.Accel,    (c, v) => c.Accel = v));
table.Add(new NumericParam<RobotConfig>("PidKp",    0.01, 0,   100, 3, c => c.PidKp,    (c, v) => c.PidKp = v));
table.Add(new NumericParam<RobotConfig>("OffsetX",  0.01, -10, 10,  2, c => c.OffsetX,  (c, v) => c.OffsetX = v));

// 4. 纯逻辑操作（无 UI，可独立测试）
var p = table.Find("MaxSpeed");
double newVal = p.StepValue(config, +1);   // 步进 +0.1，自动钳制
p.Set(config, newVal);

string text = p.Format(p.Get(config));      // "2.1"
double clamped = p.Clamp(999);              // 钳制到上限 10

// 5. 与 UI 绑定（由各项目自行实现控件映射）
//    按 Tag 查找（支持 "MaxSpeed" 或 "MaxSpeed:1"）：
var param = table.FindByTag(button.Tag);
double dir = NumericParamTable<RobotConfig>.ParseDirection(button.Tag);
```

#### 3.2 泛用要点

| 要点 | 说明 |
|---|---|
| 宿主类型任意 | `T` 可以是 POCO、实体类、配置类等，仅需提供 `double` 的 getter/setter |
| 纯逻辑可独立测试 | 建表、步进、钳制、格式化均不依赖 UI，可在单元测试或控制台直接调用 |
| UI 绑定解耦 | 库不持有控件引用，由宿主自行维护「参数名 → 控件」映射（如 `Dictionary<string, TextBox>`） |
| 方向解析内置 | `FindByTag` + `ParseDirection` 已封装「`名字:方向`」Tag 格式的解析，UI 层无需手写 split |
| 多目标框架 | 可被 .NET Framework 4.8 与 .NET 9 项目同时引用，跨框架复用 |

### 4. 复用价值

任何「一堆 `double` 属性需要 UI 编辑」的场景均可复用该库：
- 阈值表、标定参数、PID 参数、坐标变换参数等
- 新增参数只需在元数据表中增删一行，无需修改任何逻辑代码
- 确认按钮重置后恢复为用户设定值

---

# OpcLibraryAnyCpu 与 OpcLibraryAnyCpu.Ua

## 一、项目架构设计

### 1.1 代码共享机制

两个项目**基本共享代码**，通过以下两层机制区分 OPC DA 和 OPC UA：

| 机制 | 说明 |
|------|------|
| **Link 文件引用** | `OpcLibraryAnyCpu.Ua` 通过 `<Compile Include="..\OpcLibraryAnyCpu\xxx.cs" Link="xxx.cs" />` 方式共享 `OpcLibraryAnyCpu` 的源文件 |
| **编译符号条件编译** | 源码中使用 `#if DA` / `#elif UA` 区分两种架构的行为、命名空间和 API 调用 |

### 1.2 项目文件构成

**OpcLibraryAnyCpu（OPC DA 基础项目）**：

```
OpcLibraryAnyCpu/
├── Core/
│   └── OpcConst.cs              # OPC 全局配置（服务器IP、端口、日志等）
├── DataUtil/
│   ├── DataService_Opc.cs       # OPC 主数据服务（联合查询组+项）
│   ├── DataService_OpcGroup.cs  # OPC 组表的数据服务
│   └── DataService_OpcItem.cs   # OPC 项表的数据服务（CRUD）
├── Forms/
│   ├── FormOpcConfig.cs         # OPC DA 配置窗体（桌面界面）
│   ├── FormOpcConfig.designer.cs
│   └── FormOpcConfig.resx
├── Model/
│   ├── OpcItem.cs               # OPC 项实体（含导入/导出/等值判断逻辑）
│   └── OpInfoSource.cs          # 操作结果实体（成功/消息/超时）
├── Task/
│   └── OpcTryTimeoutTask.cs     # OPC 连接超时检测任务（支持自动重启）
├── Properties/
│   └── AssemblyInfo.cs
├── OpcGroupInfo.cs              # OPC 组信息实体（含读取/写入/同步/异步）
├── OpcItemInfo.cs               # OPC 项信息实体（含反射赋值/类型转换/系数偏移）
├── OpcTaskBase.cs               # OPC 任务基类（循环读取→写入或写入→读取）
└── OpcUtilHelper.cs             # OPC 核心包装类（连接/断开/重连/读写）
```

**OpcLibraryAnyCpu.Ua（OPC UA 项目）独有文件**：

```
OpcLibraryAnyCpu.Ua/
├── Forms/
│   ├── FormOpcUaConfig.cs       # 继承 FormOpcConfig 的 UA 配置窗口（修改了UI布局）
│   ├── FormOpcUaConfig.Designer.cs
│   └── FormOpcUaConfig.resx
├── Properties/
│   └── AssemblyInfo.cs
├── app.config                   # .NET Framework 绑定重定向
└── packages.config              # NuGet 包引用（OPC UA SDK 等）
```

### 1.3 命名空间映射

| 源码文件（共享） | DA 命名空间 (`#if DA`) | UA 命名空间 (`#elif UA`) |
|------|------|------|
| `OpcConst.cs` | `OpcLibrary.Core` | `OpcLibrary.Ua.Core` |
| `OpcUtilHelper.cs` | `OpcLibrary` | `OpcLibrary.Ua` |
| `OpcGroupInfo.cs` | `OpcLibrary` | `OpcLibrary.Ua` |
| `OpcItemInfo.cs` | `OpcLibrary` | `OpcLibrary.Ua` |
| `OpcTaskBase.cs` | `OpcLibrary` | `OpcLibrary.Ua.Task` |
| `OpcTryTimeoutTask.cs` | `OpcLibrary.Task` | `OpcLibrary.Ua.Task` |
| `DataService_*.cs` | `OpcLibrary.DataUtil` | `OpcLibrary.Ua.DataUtil` |
| `OpcItem.cs` | `OpcLibrary.Model` | `OpcLibrary.Ua.Model` |
| `OpInfoSource.cs` | `OpcLibrary.Model` | `OpcLibrary.Ua.Model` |

### 1.4 外部依赖

| 依赖库 | DA 项目 | UA 项目 | 用途 |
|------|:---:|:---:|------|
| `CommonLib` | ✅ | ✅ | 基础工具类（日志、反射、数据访问等） |
| `CommonLib.DataUtil.SqliteProviderLibAnyCpu` | ✅ | ✅ | SQLite 数据存储 |
| `Interop.OPCAutomation` | ✅ | ❌ | OPC DA COM 互操作 |
| `OpcUaHelper` | ❌ | ✅ | OPC UA 客户端封装 |
| `OPCFoundation.NetStandard.Opc.Ua.*` | ❌ | ✅ | OPC UA 官方 SDK |
| `System.Data.SQLite` | ✅ | ✅ | SQLite ADO.NET 提供程序 |

---

## 二、核心类功能详解

### 2.1 OpcConst — OPC 全局配置

**文件**：`Core/OpcConst.cs`

**功能**：静态全局配置类，提供 OPC 连接所需的所有基础参数。

**主要属性**：

| 属性 | 说明 | DA/UA |
|------|------|:---:|
| `OpcEnabled` | OPC 功能总开关 | 共用 |
| `OpcConstructureType` | 架构类型（OpcDa/OpcUa） | 共用 |
| `OpcServerIp` | OPC 服务器 IP 地址 | 共用 |
| `OpcServerName` | OPC 服务器名称 | 共用 |
| `OpcServerPort` | OPC 服务器端口（KepServer 默认 49320） | 仅UA |
| `OpcServerUrl` | 完整 UA 服务地址 `opc.tcp://IP:Port[/Name]` | 仅UA |
| `UserName` / `Password` | UA 认证凭据 | 仅UA |
| `Write2Plc` | 是否向 PLC 写入 | 共用 |
| `OpcLoopInterval` | 读写循环间隔（毫秒） | 共用 |
| `SqliteFileDir` / `SqliteFileName` | SQLite 数据库路径/文件名 | 共用 |
| `SchemaFile` | 数据源 JSON Schema 文件路径 | 共用 |
| `Log` | 日志客户端（LogClient） | 共用 |

**使用示例**：
```cs
// 初始化全局 OPC 配置
OpcConst.OpcEnabled = true;
OpcConst.OpcServerIp = "192.168.1.100";
OpcConst.OpcServerName = "Kepware.KEPServerEX.V6";

// UA 专属配置
OpcConst.OpcServerPort = 49320;
OpcConst.UserName = "admin";
OpcConst.Password = "123456";

// SQLite 数据库配置
OpcConst.SqliteFileDir = AppDomain.CurrentDomain.BaseDirectory;
OpcConst.SqliteFileName = "opc_config.db3";

// 循环间隔（毫秒）
OpcConst.OpcLoopInterval = 500;

// 是否向 PLC 写入
OpcConst.Write2Plc = true;
```

---

### 2.2 OpcUtilHelper — OPC 核心操作类

**文件**：`OpcUtilHelper.cs`

**功能**：封装所有 OPC 核心操作，包括连接、断开、重连、创建组、添加项、读取、写入。

#### 2.2.1 服务枚举与连接

**DA 模式**：
```cs
// 创建 OPC 辅助类
var opcHelper = new OpcUtilHelper(reconn_enabled: true);

// 枚举指定 IP 上的 OPC 服务器
string[] servers = opcHelper.ServerEnum("192.168.1.100", out string message);
if (servers != null && servers.Length > 0)
    Console.WriteLine($"找到 {servers.Length} 个 OPC 服务器");

// 同步连接
if (opcHelper.ConnectRemoteServer("192.168.1.100", "Kepware.KEPServerEX.V6", out message))
    Console.WriteLine($"连接成功，服务器状态：{opcHelper.ServerStateStr}");

// 异步连接（带超时）
var result = await opcHelper.ConnectRemoteServerAsync(
    "192.168.1.100", "Kepware.KEPServerEX.V6", timeoutMilliseconds: 5000);
if (result.Success)
    Console.WriteLine("异步连接成功");
else if (result.OperationCancelled)
    Console.WriteLine($"连接超时：{result.Message}");
```

**UA 模式**：
```cs
var opcHelper = new OpcUtilHelper(reconn_enabled: true);

// UA 异步连接（带用户名密码）
var result = await opcHelper.ConnectRemoteServerAsync(
    "192.168.1.100",           // 服务器 IP
    49320,                      // 端口
    "",                         // 服务器名称（可选，作为 URL 路径后缀）
    "admin",                    // 用户名（为空则匿名连接）
    "123456");                  // 密码

if (result.Success)
    Console.WriteLine("UA 连接成功");
else
    Console.WriteLine($"连接失败：{result.Message}");

// UA 服务地址生成
string url = OpcUtilHelper.GetOpcServerUrl("192.168.1.100", 49320, "Kepware");
// 输出：opc.tcp://192.168.1.100:49320/Kepware
```

#### 2.2.2 断开连接

```cs
opcHelper.DisconnectRemoteServer();
```

#### 2.2.3 创建 OPC 组

```cs
// 方式一：通过 OpcGroupInfo 创建组
var groupInfos = new List<OpcGroupInfo>
{
    new OpcGroupInfo(null, "Group_Read", GroupType.READ),
    new OpcGroupInfo(null, "Group_Write", GroupType.WRITE),
};
opcHelper.CreateGroups(groupInfos, out string message);

// 方式二：通过组名称创建组
var groupNames = new[] { "Group_Temperature", "Group_Pressure" };
opcHelper.CreateGroups(groupNames, out message);
```

#### 2.2.4 添加 OPC 项并读写

**DA 模式**：
```cs
// 设置 OPC 项（同时读取值）
bool result = opcHelper.ReadOpc(
    "通道1.设备1.温度",     // 标签 ID
    1,                       // 客户端句柄
    out string value,        // 读取到的值
    out string message);

// 写入值
result = opcHelper.WriteOpc(
    "通道1.设备1.温度",     // 标签 ID
    2,                       // 客户端句柄
    "25.5",                  // 待写入的值
    out message);
```

**UA 模式**：
```cs
// 读取值
bool result = opcHelper.ReadOpc(
    "ns=2;s=温度传感器1",   // UA 节点 ID
    out string value,
    out string message);

// 写入值
result = opcHelper.WriteOpc(
    "ns=2;s=温度传感器1",
    "25.5",
    out message);

// 注：ItemId 自动补全 ns=2;s= 前缀
// 即 "温度传感器1" 会被自动处理为 "ns=2;s=温度传感器1"
```

---

### 2.3 OpcGroupInfo — OPC 组信息实体

**文件**：`OpcGroupInfo.cs`

**功能**：封装 OPC 组及其所有 OPC 项的集合操作，包括批量读取、批量写入（同步/异步）、数据源绑定。

**核心属性**：

| 属性 | DA | UA | 说明 |
|------|:---:|:---:|------|
| `GroupName` | ✅ | ✅ | OPC 组名称 |
| `GroupType` | ✅ | ✅ | 组类型（READ=1 读组，WRITE=2 写组） |
| `ListItemInfo` | ✅ | ✅ | 组内所有 OPC 项信息集合 |
| `DataSource` | ✅ | ✅ | 数据源实体对象（用于反射自动赋值） |
| `ItemCount` | ✅ | ✅ | 组内 OPC 项数量 |

**DA 独有属性**：

| 属性 | 说明 |
|------|------|
| `OpcGroup` | OPCGroup COM 对象 |
| `ItemIds` | 标签 ID 数组 |
| `ServerHandles` | 服务端句柄数组 |
| `ClientHandles` | 客户端句柄数组 |
| `Errors` | 错误信息数组 |

**UA 独有属性**：

| 属性 | 说明 |
|------|------|
| `OpcUaClient` | OpcUaClient 对象 |

#### 使用示例

**批量读取**：
```cs
// 在 OPC 组中添加多个 OPC 项
var items = new List<OpcItemInfo>
{
    new OpcItemInfo("通道1.设备1.温度", clientHandle: 1, fieldName: "Temperature"),
    new OpcItemInfo("通道1.设备1.湿度", clientHandle: 2, fieldName: "Humidity"),
    new OpcItemInfo("通道1.设备1.压力", clientHandle: 3, fieldName: "Pressure"),
};

var group = new OpcGroupInfo(null, "Group_Read", GroupType.READ);
group.SetItems(items, out string message);

// 读取组内所有 OPC 项的值
if (group.ReadValues(out message))
{
    foreach (var item in group.ListItemInfo)
        Console.WriteLine($"{item.ItemId}: {item.Value}");
}
```

**UA 异步批量读取**：
```cs
// 异步读取
var result = await group.ReadValuesAsync();
if (result.Success)
{
    foreach (var item in group.ListItemInfo)
        Console.WriteLine($"{item.ItemId}: {item.Value}, 类型: {item.SystemType}");
}
```

**批量写入**：
```cs
// DA 模式 - 同步写入所有 OPC 项
group.WriteValues(out message);

// DA 模式 - 异步写入
group.WriteValues(using_async: true, out message);

// DA 模式 - 选择性写入（只写入指定服务端句柄的 OPC 项）
int[] handles = { 1, 3 };
group.WriteValues(handles, out message);

// UA 模式 - 写入所有 OPC 项
group.WriteValues(out message);
```

**数据源绑定（自动反射赋值）**：
```cs
// 定义数据源类
public class SensorData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double Pressure { get; set; }
    // 支持集合索引：DockMachines[0].WalkPos
}

// 绑定数据源（读取时自动更新数据源属性，写入时自动从数据源取值）
var sensorData = new SensorData();
group.DataSource = sensorData;

// 读取 OPC 值后，值会自动通过反射赋给 sensorData.Temperature 等属性
group.ReadValues(out message);

// 写入时，自动从 dataSource 中的属性值获取待写入的值
group.WriteValues(out message);
```

**数据源支持属性路径和集合索引**：
```cs
// fieldName 支持深层路径和集合索引
new OpcItemInfo("通道1.设备1.位置", clientHandle: 1, fieldName: "DockMachines[0].WalkPos");
new OpcItemInfo("通道1.设备1.状态", clientHandle: 2, fieldName: "Status.Name");
// 读取/写入时支持：dataSource.DockMachines[0].WalkPos、dataSource.Status.Name
```

---

### 2.4 OpcItemInfo — OPC 项信息实体

**文件**：`OpcItemInfo.cs`

**功能**：封装单个 OPC 项的所有信息，包括值、系数偏移、类型转换、反射赋值。

**核心属性**：

| 属性 | DA | UA | 说明 |
|------|:---:|:---:|------|
| `ItemId` | ✅ | ✅ | OPC 项 ID/名称（UA 自动补全 `ns=2;s=` 前缀） |
| `Value` | ✅ | ✅ | 读取或待写入的值（字符串） |
| `Coeff` | ✅ | ✅ | 值系数（0 时不起作用） |
| `Offset` | ✅ | ✅ | 值偏移量（系数为 0 时不起作用） |
| `FieldName` | ✅ | ✅ | 数据源中对应字段名称 |
| `ClientHandle` | ✅ | ❌ | 客户端句柄 |
| `ServerHandle` | ✅ | ❌ | 服务端句柄 |
| `WrappedValue` | ❌ | ✅ | UA 标签的 Variant 类型值 |
| `TypeInfo` | ❌ | ✅ | UA 类型信息（BuiltInType + ValueRank） |
| `SystemType` | ❌ | ✅ | 根据 UA TypeInfo 推断的 System.Type |
| `ValueConverted2SystemType` | ❌ | ✅ | 转换为系统类型的值（用于 UA 写入） |

**系数与偏移量**：
```cs
// 读取到的原始值会经过计算：Value = 原始值 × Coeff + Offset
// 例：原始值 1000，Coeff=0.001，Offset=0  →  Value = 1.0
var item = new OpcItemInfo("通道1.设备1.电流", clientHandle: 1,
    fieldName: "Current", coeff: 0.001, offset: 0);
```

**数据源自动赋值（SetItemValue）**：
```cs
// SetItemValue 从数据源读取属性值并设置到 OPC 项的 Value 中
// 三种空值处理模式：
item.SetItemValue(dataSource, NullValueHandling.Skip);   // 跳过（默认）
item.SetItemValue(dataSource, NullValueHandling.Ignore);  // 忽略空值
// Skip：属性值为 null 时，Value 设为 null 且不写入
// Ignore：属性值为 null 时，创建默认值（值类型默认值，引用类型 null）
```

---

### 2.5 OpcTaskBase — OPC 任务基类

**文件**：`OpcTaskBase.cs`

**功能**：提供标准的 OPC 循环读取/写入任务框架，继承自 `CommonLib.Clients.Tasks.Task`。该类是一个抽象类，使用时需要实现几个抽象方法。

**循环流程**：
```
Init() → LoopContent() 循环执行：
  1. LoopUrContentBeforeRW()  ← 用户自定义（读写前回调）
  2. OpcReadValues()          ← 读取（先读后写模式）
     OpcWriteValues()         ← 写入（先写后读模式）
  3. LoopUrContentBetweenRW() ← 用户自定义（读写之间回调）
  4. OpcWriteValues()         ← 写入（先读后写模式）
     OpcReadValues()          ← 读取（先写后读模式）
  5. LoopUrContentAfterRW()   ← 用户自定义（读写后回调）
```

**必须实现的抽象方法**：

| 方法 | 说明 |
|------|------|
| `LoopUrContentBeforeRW()` | 每次循环读取/写入之前的自定义操作 |
| `LoopUrContentBetweenRW()` | 读取与写入之间的自定义操作 |
| `LoopUrContentAfterRW()` | 每次循环读取/写入之后的自定义操作 |
| `GetNewOpcInstance()` | 返回新的 OpcTaskBase 实例（用于任务重启） |
| `GetOpcDatasource()` | 返回 OPC 数据源实体对象 |

**使用示例**：
```cs
public class MyOpcTask : OpcTaskBase
{
    private readonly SensorData _data = new SensorData();

    public MyOpcTask(params int[] idsIncl) : base(idsIncl)
    {
        ReadBeforeWrite = true; // 先读后写
    }

    protected override object GetOpcDatasource()
    {
        return _data;
    }

    protected override void LoopUrContentBeforeRW()
    {
        // 读写之前：例如更新数据库中的数据
        Console.WriteLine("准备读写...");
    }

    protected override void LoopUrContentBetweenRW()
    {
        // 读写之间：例如业务逻辑计算
        Console.WriteLine($"温度：{_data.Temperature}, 湿度：{_data.Humidity}");
    }

    protected override void LoopUrContentAfterRW()
    {
        // 读写之后：例如保存日志、报警检测等
        Console.WriteLine("读写完成");
    }

    protected override OpcTaskBase GetNewOpcInstance()
    {
        return new MyOpcTask(_idsIncl);
    }
}

// 启动任务
var task = new MyOpcTask(1); // 只处理 group_id=1 的 OPC 组
task.Start();
```

---

### 2.6 OpcTryTimeoutTask — 连接超时检测

**文件**：`Task/OpcTryTimeoutTask.cs`

**功能**：周期性检测 OPC 连接状态，在连接超时时可自动重启计算机。

**使用示例**：
```cs
var cts = new CancellationTokenSource();

// 启动后台监控，每 10 秒尝试连接一次
// 超时后自动重启计算机
_ = OpcTryTimeoutTask.MonitorOpcConnRecurAsync(
    cts.Token,
    restartComputer: true);

// 停止监控
cts.Cancel();
```

---

### 2.7 数据持久层（DataUtil）

**文件**：`DataUtil/DataService_Opc.cs`、`DataService_OpcGroup.cs`、`DataService_OpcItem.cs`

**功能**：基于 SQLite 的 OPC 配置数据存储，两个表结构如下：

**t_plc_opcgroup（OPC 组表）**：
| 字段 | 类型 | 说明 |
|------|------|------|
| `group_id` | INTEGER PK | 组 ID，自增主键 |
| `group_name` | VARCHAR2(32) | 组名称 |
| `group_type` | INTEGER(1) | 组类型（1=读，2=写） |
| `enabled` | INTEGER | 是否启用 |

**t_plc_opcitem（OPC 项表）**：
| 字段 | 类型 | 说明 |
|------|------|------|
| `record_id` | INTEGER PK | 记录 ID，自增主键 |
| `item_id` | VARCHAR2(64) | OPC 标签 ID |
| `opcgroup_id` | INTEGER FK | 所属组 ID |
| `field_name` | VARCHAR2(64) | 对应数据源字段名称 |
| `enabled` | INTEGER | 是否启用 |
| `coeff` | DOUBLE | 值系数 |
| `offset` | DOUBLE | 值偏移量 |

**使用示例**：
```cs
// 查询所有 OPC 项
var service = new DataService_OpcItem();
var table = service.GetAllOpcItemsOrderbyId();

// 新增 OPC 项
var item = new OpcItem
{
    ItemId = "通道1.设备1.温度",
    OpcGroupId = 1,
    FieldName = "Temperature",
    Enabled = true,
    Coeff = 0.01,
    Offset = 0
};
service.SaveOpcItem(item);

// 批量保存
var items = new List<OpcItem> { item1, item2, item3 };
service.SaveOpcItems(items);

// 批量删除
service.DeleteOpcItemsByIds(new[] { 1, 2, 3 });
```

---

### 2.8 FormOpcConfig — OPC 配置窗体

**文件**：`Forms/FormOpcConfig.cs`（DA） / `OpcLibraryAnyCpu.Ua/Forms/FormOpcUaConfig.cs`（UA）

**功能**：Windows Forms 桌面界面，提供：
- OPC 服务器枚举与连接
- OPC 项表格管理（增删改查）
- CSV 文件导入/导出
- 单个 OPC 项的值读取/写入

**UA 版本**：`FormOpcUaConfig` 继承自 `FormOpcConfig`，修改了界面布局（隐藏 DA 服务器枚举相关控件，将 "IP地址" 标签改为 "服务地址"，URL 输入框默认值为 `opc.tcp://` 格式）。

---

### 2.9 OpcItem 实体与 OpInfoSource

**OpcItem**（`Model/OpcItem.cs`）：
- OPC 项的业务实体，支持从 DataRow 构造、从 CSV 导入字符串解析、导出为 CSV 格式
- 实现 `IEquatable<OpcItem>`，判断两个 OPC 项是否相同（读取组按 FieldName 判等，写入组按 ItemId 判等）
- `Copy()` 方法实现从另一个实例复制属性值

**OpInfoSource**（`Model/OpInfoSource.cs`）：
- 操作结果实体，包含 `Success`（成功标志）、`Message`（消息）、`OperationCancelled`（超时标志）

---

## 三、DA vs UA 主要差异对比

| 特性 | DA | UA |
|------|:---:|:---:|
| 通信协议 | COM/DCOM | TCP（opc.tcp://） |
| 服务地址 | IP + 服务名 | 完整 URL |
| 认证方式 | Windows 集成认证 | 匿名 / 用户名密码 |
| 客户端句柄 | 需要手动分配 | 不需要 |
| 服务端句柄 | 由 COM 分配 | 不需要 |
| 异步连接 | `ConnectRemoteServerAsync`（内部 Task.Run） | `ConnectRemoteServerAsync`（原生 async） |
| 批量读取 | `SyncRead`（COM 同步调用） | `ReadNodes` + `ReadNodesAsync` |
| 批量写入 | `SyncWrite` / `AsyncWrite` | `WriteNodes`（仅同步） |
| 类型转换 | 字符串转换 | 基于 UA TypeInfo 的 SystemType 转换 |
| 重连机制 | 后台轮询线程 | OpcUaClient.ReconnectPeriod |
| 所需 SDK | Interop.OPCAutomation | OPCFoundation.NetStandard + OpcUaHelper |
| 默认端口 | 无 | 49320（KepServer V6） |

---

## 四、典型使用场景

### 4.1 场景一：从 SQLite 数据库读取配置，建立 OPC 连接并循环读写

```cs
// 1. 初始化全局配置
OpcConst.OpcEnabled = true;
OpcConst.Write2Plc = true;
OpcConst.OpcLoopInterval = 500;
OpcConst.SqliteFileDir = AppDomain.CurrentDomain.BaseDirectory;
OpcConst.SqliteFileName = "opc_config.db3";

// DA 场景
OpcConst.OpcServerIp = "192.168.1.100";
OpcConst.OpcServerName = "Kepware.KEPServerEX.V6";

// UA 场景（替代上述 IP/Name 配置）
// OpcConst.OpcServerIp = "192.168.1.100";
// OpcConst.OpcServerPort = 49320;
// OpcConst.UserName = "admin";
// OpcConst.Password = "123456";

// 2. 自定义数据源类
public class ProductionData
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double Pressure { get; set; }
    public int MachineStatus { get; set; }
}

// 3. 创建任务类
public class ProductionOpcTask : OpcTaskBase
{
    private readonly ProductionData _data = new ProductionData();
    private int _cycleCount = 0;

    public ProductionOpcTask(params int[] idsIncl) : base(idsIncl) { }

    protected override object GetOpcDatasource() => _data;

    protected override void LoopUrContentBeforeRW()
    {
        _cycleCount++;
        OpcConst.WriteConsoleLog($"--- 第 {_cycleCount} 次循环 ---");
    }

    protected override void LoopUrContentBetweenRW()
    {
        // 读取后检查报警
        if (_data.Temperature > 80)
            OpcConst.WriteConsoleLog($"温度过高: {_data.Temperature}°C");
    }

    protected override void LoopUrContentAfterRW()
    {
        OpcConst.WriteConsoleLog($"温度={_data.Temperature}, 湿度={_data.Humidity}, " +
            $"压力={_data.Pressure}, 状态={_data.MachineStatus}");
    }

    protected override OpcTaskBase GetNewOpcInstance()
    {
        return new ProductionOpcTask(_idsIncl);
    }
}

// 4. 启动
var task = new ProductionOpcTask();
task.Start();
```

### 4.2 场景二：手动创建 OPC 组和项，不依赖数据库

```cs
var opcHelper = new OpcUtilHelper(reconn_enabled: true);

// DA 连接
if (!opcHelper.ConnectRemoteServer("192.168.1.100", "Kepware.KEPServerEX.V6", out string msg))
{
    Console.WriteLine($"连接失败: {msg}");
    return;
}

// 创建读组和写组
var readGroup = new OpcGroupInfo(null, "MyReadGroup", GroupType.READ);
var writeGroup = new OpcGroupInfo(null, "MyWriteGroup", GroupType.WRITE);

// 添加数据源
var data = new SensorData();
readGroup.DataSource = data;
writeGroup.DataSource = data;

// 为读组添加 OPC 项
var readItems = new List<OpcItemInfo>
{
    new OpcItemInfo("通道1.设备1.温度", clientHandle: 1, fieldName: "Temperature"),
    new OpcItemInfo("通道1.设备1.湿度", clientHandle: 2, fieldName: "Humidity"),
};
readGroup.SetItems(readItems, out msg);

// 为写组添加 OPC 项
var writeItems = new List<OpcItemInfo>
{
    new OpcItemInfo("通道1.设备1.目标温度", clientHandle: 10, fieldName: "TargetTemp"),
};
writeGroup.SetItems(writeItems, out msg);

// 定时循环
while (true)
{
    // 设置待写入值
    data.TargetTemp = 37.5;

    // 从数据源赋值到 OPC 项
    writeGroup.ListItemInfo.ForEach(i => i.SetItemValue(data));

    // 写入
    writeGroup.WriteValues(out msg);

    // 读取
    readGroup.ReadValues(out msg);

    Console.WriteLine($"当前温度: {data.Temperature}, 目标: {data.TargetTemp}");

    Thread.Sleep(500);
}
```

### 4.3 场景三：UA 异步读取并处理强类型数据

```cs
var opcHelper = new OpcUtilHelper(reconn_enabled: true);

// UA 连接
var result = await opcHelper.ConnectRemoteServerAsync(
    "192.168.1.100", 49320, "", "admin", "123456");

if (!result.Success)
{
    Console.WriteLine($"连接失败: {result.Message}");
    return;
}

// 创建组并添加 OPC 项
var group = new OpcGroupInfo(null, "Group_Read", GroupType.READ);
var items = new List<OpcItemInfo>
{
    new OpcItemInfo("温度传感器1"),
    new OpcItemInfo("压力传感器1"),
};
group.SetItems(items, out string msg);

// 异步批量读取
var readResult = await group.ReadValuesAsync();
if (readResult.Success)
{
    foreach (var item in group.ListItemInfo)
    {
        // UA 支持类型识别
        Console.WriteLine($"{item.ItemId}: {item.Value} (类型: {item.SystemType?.Name})");

        // 获取强类型值用于计算
        if (item.SystemType == typeof(double) && double.TryParse(item.Value, out double d))
            Console.WriteLine($"  值 + 10 = {d + 10}");
    }
}
```

### 4.4 场景四：使用配置窗体管理 OPC 项

```cs
// DA 配置窗口
var formDa = new OpcLibrary.Controls.Forms.FormOpcConfig();
formDa.OpcServerConnected += (sender, e) =>
{
    Console.WriteLine($"DA 连接成功: {e.OpcServerName} @ {e.OpcServerIp}");
};
formDa.ShowDialog();

// UA 配置窗口（继承自 DA 窗口，UI 已调整）
var formUa = new OpcLibrary.Ua.Forms.FormOpcUaConfig();
formUa.ShowDialog();
```

---

## 五、关键设计说明

### 5.1 条件编译模式

代码中通过 `#if DA` / `#elif UA` 在编译期决定使用 OPC DA 还是 OPC UA 的 API 和行为：

```cs
// 示例：同一个方法，两种实现
#if DA
    public bool ConnectRemoteServer(string ip, string name, out string message)
    {
        OpcServer = new OPCServer();
        OpcServer.Connect(name, ip);
        // DA 特定的连接逻辑...
    }
#elif UA
    public async Task<OpInfoSource> ConnectRemoteServerAsync(
        string ip, int port, string name, string user, string pwd)
    {
        OpcUaClient = new OpcUaClient();
        await OpcUaClient.ConnectServer(url);
        // UA 特定的连接逻辑...
    }
#endif
```

### 5.2 数据源反射机制

`OpcItemInfo` 利用 `PropertyMapperExtension.GetEntityProperty_InConstruction()` 方法，支持通过 `FieldName` 属性路径字符串（如 `"DockMachines[0].WalkPos"`）深层访问数据源对象的属性，并能处理集合索引（`[0]`）。在 `DataSource` 绑定后：

- **读取方向**（OPC → 数据源）：`ReadValues()` 后自动调用 `SetPropertyValue()`，通过反射将 OPC 值写入数据源的对应属性
- **写入方向**（数据源 → OPC）：`WriteValues()` 前自动调用 `SetItemValue()`，通过反射从数据源读取属性值并赋值给 OPC 项的 `Value`

### 5.3 组类型隔离

同一个数据源可以同时绑定到**读组**（`GroupType.READ`）和**写组**（`GroupType.WRITE`）：
- 读组在读取后更新数据源中的值
- 写组在写入前从数据源中取值
- `OpcTaskBase` 循环中自动按类型过滤组

### 5.4 错误处理策略

- 连接失败不会抛出异常，而是通过 `out message` 或 `OpInfoSource.Message` 返回错误信息
- 资源清理在 `DisposeOpcResources()` 中使用 try-catch 包裹，确保释放过程不抛出异常
- `OpcTryTimeoutTask` 在检测到连接超时时，可选择自动重启计算机

---

# NumericParamEditor — 数值参数编辑器

## 概述

`NumericParamEditor` 是一套「数值参数的声明式描述 + 表驱动读写」工具，将「一堆 `double` 属性需要 UI 编辑」的通用场景抽象为可复用的纯逻辑库与 WPF 控件。

**设计目标**：与具体业务实体和 UI 框架解耦，供所有项目（WPF、WinForms、控制台、.NET Framework / .NET 9）共用。

### 分层架构

```
┌─ 纯逻辑层（无 UI 依赖）─────────────────────────────────────┐
│  NumericParam<T>          ← 单个参数的元数据 + 读写/钳制/格式化 │
│  NumericParamTable<T>     ← 参数集合 + 按名/Tag 查找 + 方向解析 │
└───────────────────────────────────────────────────────────────┘
              ↑ 引用
┌─ WPF 控件层（有 UI 依赖）────────────────────────────────────┐
│  NumericParamPanel        ← 自动生成控件 + 长按 + 重置 + 提交  │
└───────────────────────────────────────────────────────────────┘
```

### 项目分布

| 文件 | 所属项目 | 目标框架 |
|------|---------|---------|
| `NumericParam.cs` / `NumericParamTable.cs` | `CommonLib`（纯逻辑） | net45 + net9.0 |
| `NumericParamPanel.cs` | `CommonLib`（WPF） | net45 + net9.0-windows |

---

## 一、纯逻辑层

**文件位置**：`CommonLib/UIControlUtil/NumericParamEditor/`

**命名空间**：`CommonLib.UIControlUtil.NumericParamEditor`

### 1. `NumericParam<T>`

描述一个可编辑的数值参数，与具体业务实体解耦：

| 成员 | 类型 | 说明 |
|------|------|------|
| `Name` | `string` | 参数名（唯一标识），也用于控件 Tag 匹配 |
| `Step` | `double` | 单次步进量（正数） |
| `Min` / `Max` | `double` | 取值范围（含边界） |
| `DecimalPlaces` | `int` | 显示小数位数（显式字段） |
| `Getter` | `Func<T, double>` | 读取宿主对象上的值 |
| `Setter` | `Action<T, double>` | 写入宿主对象 |

**方法**：

| 方法 | 说明 |
|------|------|
| `Get(T host)` | 读取当前值 |
| `Set(T host, double value)` | 写入值 |
| `Clamp(double value)` | 钳制到 `[Min, Max]` 区间 |
| `StepValue(T host, double direction)` | 步进一次并返回钳制后新值（不写回） |
| `Format(double value)` | 按 `DecimalPlaces` 格式化为文本 |

**构造校验**：`name`/`getter`/`setter` 为空抛 `ArgumentNullException`；`step` 非正数或 `min > max` 抛 `ArgumentOutOfRangeException`。

### 2. `NumericParamTable<T>`

一组 `NumericParam<T>` 的集合，提供查找与批量操作：

| 成员 | 说明 |
|------|------|
| `Add(param)` | 添加参数，重名抛异常 |
| `Find(name)` | 按参数名精确查找 |
| `FindByTag(object tag)` | 按控件 Tag 查找（支持 `"Roll"` 或 `"Roll:1"`） |
| `ParseDirection(tag)` | 从 Tag 解析方向（`"1"` 返回 +1，否则 -1） |
| `IEnumerable<NumericParam<T>>` | 支持 `foreach` 遍历 |

### 3. 纯逻辑使用示例

```csharp
using CommonLib.UIControlUtil.NumericParamEditor;

// 1. 定义宿主对象：任意含 double 成员的类
public class RobotConfig
{
    public double MaxSpeed { get; set; } = 2.0;
    public double Accel    { get; set; } = 0.5;
    public double PidKp    { get; set; } = 1.2;
}

// 2. 创建实例 + 元数据表
var config = new RobotConfig();
var table = new NumericParamTable<RobotConfig>();

// 3. 声明参数（名字、步长、范围、小数位、读写委托）
table.Add(new NumericParam<RobotConfig>("MaxSpeed", 0.1, 0, 10,  1, c => c.MaxSpeed, (c, v) => c.MaxSpeed = v));
table.Add(new NumericParam<RobotConfig>("Accel",    0.05, 0, 5,  2, c => c.Accel,    (c, v) => c.Accel = v));
table.Add(new NumericParam<RobotConfig>("PidKp",    0.01, 0, 100, 3, c => c.PidKp,    (c, v) => c.PidKp = v));

// 4. 纯逻辑操作（无 UI，可独立测试）
var p = table.Find("MaxSpeed");
double newVal = p.StepValue(config, +1);   // 步进 +0.1，自动钳制
p.Set(config, newVal);

string text = p.Format(p.Get(config));      // "2.1"
double clamped = p.Clamp(999);              // 钳制到上限 10

// 5. 与 UI 绑定（按 Tag 查找，支持 "MaxSpeed" 或 "MaxSpeed:1"）
var param = table.FindByTag(button.Tag);
double dir = NumericParamTable<RobotConfig>.ParseDirection(button.Tag);
```

---

## 二、WPF 控件层

**文件位置**：`CommonLib/UIControlUtil/WPF/Controls/NumericParamPanel.cs`

**命名空间**：`CommonLib.UIControlUtil.WPF.Controls`

### 1. 概述

`NumericParamPanel` 是一个纯代码构建的 WPF `UserControl`（无 XAML），传入参数表和宿主对象后，自动生成 N 组「标签 + 输入框 + −/+ 按钮」，支持单击步进、长按连续调整、手动输入、一键重置。

### 2. 对外接口

| 成员 | 类型 | 说明 |
|------|------|------|
| `Title` | `string` | 面板标题（显示在左侧） |
| `Columns` | `int` | 参数网格列数（默认 4） |
| `RepeatIntervalMs` | `int` | 长按重复间隔（毫秒，默认 50） |
| `ValueChanged` | `event Action<string, double>` | 参数值修改后触发（参数名 + 新值） |
| `Initialize<T>(...)` | 方法 | 传入参数表 + 宿主 + 初始值字典，生成控件 |
| `Clear()` | 方法 | 清空所有参数（供重复 Initialize） |

### 3. 使用方法

```csharp
using CommonLib.UIControlUtil.NumericParamEditor;
using CommonLib.UIControlUtil.WPF.Controls;

// 1. 构建参数表（纯逻辑）
var table = new NumericParamTable<RobotConfig>();
table.Add(new NumericParam<RobotConfig>("MaxSpeed", 0.1, 0, 10,  1, c => c.MaxSpeed, (c, v) => c.MaxSpeed = v));
table.Add(new NumericParam<RobotConfig>("Accel",    0.05, 0, 5,  2, c => c.Accel,    (c, v) => c.Accel = v));
table.Add(new NumericParam<RobotConfig>("PidKp",    0.01, 0, 100, 3, c => c.PidKp,    (c, v) => c.PidKp = v));

// 2. 创建面板（可在 XAML 中声明，也可代码创建）
var panel = new NumericParamPanel
{
    Title = "机器人参数",
    Columns = 3
};

// 3. 初始化：传入参数表 + 宿主 + 初始值字典（供重置）
var config = new RobotConfig();
panel.Initialize(
    table,
    config,
    new Dictionary<string, double>
    {
        ["MaxSpeed"] = 2.0,
        ["Accel"] = 0.5,
        ["PidKp"] = 1.2
    });

// 4. 订阅参数变化事件（可选）
panel.ValueChanged += (name, val) =>
{
    Console.WriteLine($"参数 {name} 变更为 {val}");
};

// 5. 将面板添加到界面
rootGrid.Children.Add(panel);
```

### 4. 交互行为

| 交互 | 行为 |
|------|------|
| **单击 `+`/`-`** | 按步长增减一次 |
| **长按 `+`/`-`** | 立即步进一次 + 以 `RepeatIntervalMs` 间隔持续增减，松开停止 |
| **手动输入** | 失焦或回车提交，解析失败恢复原值，超出范围自动截断 |
| **重置按钮** | 一键恢复到 `initialValues` 传入的初始值 |

### 5. 泛型隔离设计

`NumericParamPanel` 本身是非泛型 `UserControl`（WPF 限制），通过泛型 `Initialize<T>` 方法接收任意宿主类型 `T`。内部将 `NumericParam<T>` 的读写/钳制/格式化/步进全部包装成**非泛型闭包**存入字典，后续事件处理走非泛型路径，从而规避「控件无法泛型」的限制。

### 6. 框架兼容性

`NumericParamPanel` 只使用 net45 与 net9.0-windows 共有的 WPF API（`UserControl`、`DispatcherTimer`、`IReadOnlyDictionary` 等），因此**无需条件编译**。框架差异仅在项目文件层处理：

| 项目 | 框架标识 | 差异 |
|------|---------|------|
| `CommonLib` | `TargetFrameworkVersion v4.5` | 旧式 csproj，直接 `<Compile>` 包含 |
| `CommonLib.Core-win` | `net9.0-windows` + `UseWPF=true` | SDK 风格，`<Compile Include=... Link=...>` 共享 |
