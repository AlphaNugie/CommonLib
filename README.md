# CommonLib 解决方案

## 项目概览

| 项目 | 目标框架 | 说明 |
|------|---------|------|
| `CommonLib` | .NET Framework 4.5 | 基础工具类库（网络、数据、UI、扩展等） |
| `CommonLib.Core` | .NET 9.0 | CommonLib 的 .NET 9.0 版本（以 Link 方式共享源码） |
| `OpcLibraryAnyCpu` | .NET Framework 4.5 | OPC DA 读写辅助类库 |
| `OpcLibraryAnyCpu.Ua` | .NET Framework 4.8 | OPC UA 读写辅助类库 |
| `OpcLibraryAnyCpu.UaNet100` | .NET 10.0 | OPC UA 的 .NET 10.0 版本 |
| `CommonLib.DataUtil.SqliteProviderLib` | .NET Framework 4.5 | SQLite 数据库提供程序 |
| 其他辅助项目 | 各异 | Oracle 提供程序、拼音转换、曲线拟合等 |

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
