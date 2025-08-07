# CJF.Schedule - 排程工作器 (Schedule Plan Worker)

[![NuGet version](https://badge.fury.io/nu/CJF.Schedule.svg)](https://badge.fury.io/nu/CJF.Schedule)

一個功能強大且易於使用的 .NET 8.0 排程工作器程式庫，提供完整的排程執行功能。

## 版本資訊

- **版本**: 1.21.360
- **作者**: Chen Jaofeng
- **許可證**: MIT
- **目標框架**: .NET 8.0

## 功能特色

### 多種排程類型支援

- ✅ **一次性執行** (`Once`) - 在指定時間執行一次
- ✅ **每日執行** (`Day`) - 以日為單位的週期執行
- ✅ **每週執行** (`Week`) - 以週為單位的週期執行
- ✅ **每月執行** (`Month`) - 在指定月份和日期執行
- ✅ **每月週執行** (`MonthWeek`) - 在指定月份的某週某日執行
- ✅ **生命週期執行** (`Startup`/`Stoped`) - 程式啟動/停止時執行

### 簡易屬性裝飾器

使用 `PlanAttribute` 裝飾靜態方法，自動註冊為排程任務：

```csharp
[Plan("09:00:00", 1)]  // 每日早上9點執行
public static void DailyTask()
{
    Console.WriteLine("每日任務執行");
}

[Plan("14:30:00", 1, WeekDays.Friday)]  // 每週五下午2:30執行
public static void WeeklyReport()
{
    Console.WriteLine("週報產生");
}

[Plan(PlanTypes.Startup)]  // 程式啟動時執行
public static void InitializeTask()
{
    Console.WriteLine("初始化任務");
}
```

### 彈性的表達式語法

支援簡易表達式格式：

```
一次性執行: "1 yyyy-MM-dd HH:mm:ss"
每日執行:   "2 yyyy-MM-dd HH:mm:ss period"
每週執行:   "3 yyyy-MM-dd HH:mm:ss period weekdays"
每月執行:   "4 yyyy-MM-dd HH:mm:ss months days"
月週執行:   "5 yyyy-MM-dd HH:mm:ss months weeknos weekdays"
程式啟動:   "6"
程式停止:   "7"
```

## 安裝方式

### NuGet 套件管理員

```bash
Install-Package CJF.Schedule
```

### .NET CLI

```bash
dotnet add package CJF.Schedule
```

### PackageReference

```xml
<PackageReference Include="CJF.Schedule" Version="1.20.353" />
```

## 快速開始

### 1. 註冊服務

在 `Program.cs` 中註冊背景服務：

```csharp
using CJF.Schedules;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // 使用 AddSchedulePlaner 方法註冊排程工作器服務
        services.AddSchedulePlaner();
        // 或使用自訂設定
        // services.AddSchedulePlaner(opts =>
        // {
        //     opts.Delay = 2;      // 延遲 2 秒啟動
        //     opts.Interval = 30;  // 每 30 秒檢查一次
        //     opts.AutoBind = true; // 自動綁定 PlanAttribute
        // });
    })
    .Build();

host.Run();
```

### 2. 建立排程任務

```csharp
using CJF.Schedules;

public class ScheduledTasks
{
    // 每日上午8點執行
    [Plan("08:00:00", 1)]
    public static void MorningTask()
    {
        Console.WriteLine($"早晨任務執行: {DateTime.Now}");
    }

    // 每兩天下午3點執行
    [Plan("15:00:00", 2)]
    public static void BidailyTask()
    {
        Console.WriteLine($"隔日任務執行: {DateTime.Now}");
    }

    // 每週二和週四上午10點執行
    [Plan("10:00:00", 1, WeekDays.Tuesday | WeekDays.Thursday)]
    public static void WeeklyTask()
    {
        Console.WriteLine($"週期任務執行: {DateTime.Now}");
    }

    // 每月15號下午5點執行
    [Plan("17:00:00", Months.All, Days.Day15)]
    public static void MonthlyTask()
    {
        Console.WriteLine($"月度任務執行: {DateTime.Now}");
    }

    // 每月第一個週一上午9點執行
    [Plan("09:00:00", Months.All, WeekNo.First, WeekDays.Monday)]
    public static void MonthlyWeekTask()
    {
        Console.WriteLine($"月週任務執行: {DateTime.Now}");
    }

    // 程式啟動時執行
    [Plan(PlanTypes.Startup)]
    public static void StartupTask()
    {
        Console.WriteLine("程式啟動初始化完成");
    }

    // 程式停止時執行
    [Plan(PlanTypes.Stoped)]
    public static void ShutdownTask()
    {
        Console.WriteLine("程式正在關閉，執行清理作業");
    }
}
```

## 核心元件

### PlanWorker
主要的背景服務，繼承自 `BackgroundService`，負責：
- 自動掃描帶有 `PlanAttribute` 的靜態方法
- 定期檢查排程執行時間（預設30秒間隔）
- 管理排程生命週期

### PlanAttribute
裝飾器屬性，用於標記要排程執行的靜態方法：
- 支援多種建構函式以適應不同排程類型
- 自動解析排程參數
- 彈性的時間和週期設定

### 排程集合管理
- `SchedulePlan`: 代表單一排程項目
- `PlanCollection`: 管理多個排程項目
- `TimePlan`: 處理時間計算邏輯

## 進階用法

### 配置選項

#### PlanWorkerOptions
```csharp
public sealed class PlanWorkerOptions
{
    /// <summary>PlanWorker 開始執行後延遲執行排程項目的時間，單位秒</summary>
    public int Delay { get; set; } = 0;
    
    /// <summary>PlanWorker 排程項目檢查週期的間隔時間，單位秒</summary>
    public int Interval { get; set; } = 30;

    /// <summary>是否自動綁定具有 <see cref="PlanAttribute"/> 的靜態方法。</summary>
    public bool AutoBind { get; set; } = true;
}
```

**屬性說明**:
- `Delay`: 程式啟動後延遲多少秒才開始執行排程檢查（預設：0 秒）
- `Interval`: 每次檢查排程的間隔時間（預設：30 秒）
- `AutoBind`: 是否自動綁定帶有 `PlanAttribute` 的靜態方法（預設：`true`）

#### 自訂設定範例
```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // 使用 AddSchedulePlaner 方法註冊服務
        services.AddSchedulePlaner(opts =>
        {
            opts.Delay = 2;      // 延遲 2 秒啟動
            opts.Interval = 30;  // 每 30 秒檢查一次
            opts.AutoBind = false; // 不自動綁定 PlanAttribute
        });
    })
    .Build();
```

### 排程表達式語法

排程表達式使用空白區隔的欄位格式：

| 排程類型 | 格式 | 範例 |
|----------|------|------|
| **一次性** | `1 yyyy-MM-dd HH:mm:ss` | `1 2025-12-31 23:59:59` |
| **每日** | `2 yyyy-MM-dd HH:mm:ss period` | `2 2025-01-01 02:00:00 5` |
| **每週** | `3 yyyy-MM-dd HH:mm:ss period weekdays` | `3 2025-01-01 02:00:00 1 1/3/5` |
| **每月** | `4 yyyy-MM-dd HH:mm:ss months days` | `4 2025-01-01 02:00:00 A 1/15` |
| **月週** | `5 yyyy-MM-dd HH:mm:ss months weeknos weekdays` | `5 2025-01-01 02:00:00 A 2/4 1/5` |
| **啟動時** | `6` | `6` |
| **停止時** | `7` | `7` |

#### 使用表達式建立排程

```csharp
[Plan("2 2024-01-01 09:00:00 1")]  // 從2024年1月1日開始，每日上午9點執行
public static void ExpressionBasedTask()
{
    Console.WriteLine("表達式排程執行");
}

[Plan("4 2025-01-01 12:00:00 A 1/15")]  // 每月1號和15號中午12點執行
public static void BiweeklyTask()
{
    Console.WriteLine("每月雙週任務執行");
}
```

### 程式化管理排程

```csharp
using CJF.Schedules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 取得 PlanWorker 服務
var planWorker = host.Services.GetRequiredService<PlanWorker>();

// 動態新增排程
planWorker.AppendPlan(new SchedulePlan(
    "每 5 天執行", 
    "2 2025-01-01 03:00:00 5", 
    () => Console.WriteLine($"[Every 5 days] {DateTime.Now}")
));

planWorker.AppendPlan(new SchedulePlan(
    "每週一三五", 
    "3 2025-01-01 08:00:00 1 1/3/5", 
    () => Console.WriteLine($"[Mon/Wed/Fri] {DateTime.Now}")
));

planWorker.AppendPlan(new SchedulePlan(
    "每月最後一週的週五", 
    new TimePlan(DateTime.Now, Months.All, WeekNo.Last, WeekDays.Friday), 
    (plan) => Console.WriteLine($"[{plan.Name}] {DateTime.Now}")
));
```

## 支援的列舉類型

### PlanTypes
- `None`: 無排程
- `Once`: 執行一次
- `Day`: 每日執行
- `Week`: 每週執行
- `Month`: 每月執行
- `MonthWeek`: 每月週執行
- `Startup`: 程式啟動時執行
- `Stoped`: 程式停止時執行

### WeekDays (可組合)
- `Sunday`, `Monday`, `Tuesday`, `Wednesday`, `Thursday`, `Friday`, `Saturday`
- `All`: 每一天

### Months (可組合)
- `January` 至 `December`
- `All`: 每個月

### Days (可組合)
- `Day1` 至 `Day31`
- `All`: 每一天

### WeekNo (可組合)
- `First`, `Second`, `Third`, `Fourth`, `Fifth`, `Sixth`
- `Last`: 最後一週
- `All`: 每一週

## 依賴項目

- **Microsoft.Extensions.Hosting** (9.0.8)
- **.NET 8.0**

## 貢獻

歡迎提交 Pull Request 或回報 Issues：
- GitHub: https://github.com/Jaofeng/Schedule

## 許可證

本專案採用 MIT 許可證，詳見 [LICENSE](https://github.com/Jaofeng/Schedule/blob/main/LICENSE) 檔案。

## 測試和開發

## 效能考量

- **輕量級**: 基於 .NET 的 BackgroundService，資源使用極少
- **彈性**: 支援動態調整檢查間隔
- **可擴展**: 易於擴展和客製化
- **執行緒安全**: 各排程任務獨立執行，互不影響

## 注意事項

### 使用限制
- 欲使用自動綁定的排程方法必須是 **靜態 (static)** 方法
- 排程方法可以是 `public` 或 `private`
- 支援兩種方法簽名：`Action` 和 `Action<ISchedulePlan>`
- 排程表達式中的日期時間使用當地時區
- 程式停止時會執行所有 `PlanTypes.Stoped` 排程

### 開發建議
- 使用 `CJF.Schedule.Test` 專案作為測試參考
- 參考 `PlanAttributeReflectionTests.cs` 了解如何測試反射功能
- 查看測試專案的 README.md 獲取詳細的測試指南

## 更新日誌

### 1.21.360(2025-08-07)
- 新增 `AddSchedulePlaner` 方法，取代 `UseSchedulePlaner`
- 增加 `UseSchedulePlaner` 的過時標記

### 1.20.353(2025-08-06)
- 支援 .NET 8.0
- 完整的排程類型支援
- 強化的屬性裝飾器功能
- 優化效能和穩定性
- 新增完整測試套件，包含94個通過測試
- 解決反射測試問題，提供隔離測試方案

### 1.12.235(2023-06-05)
- 支援 .NET 6.0
- 初始版本，首次發布

## 相關連結

- [GitHub Repository](https://github.com/Jaofeng/Schedule)
- [NuGet Package](https://www.nuget.org/packages/CJF.Schedule/)

---

© 2025 Chen Jaofeng. All rights reserved.