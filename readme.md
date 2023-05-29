# 排程工作器<br>Schedule Plan Worker for Console Hosting Application

## 使用方法
```C#
var host = Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime(opts => opts.SuppressStatusMessages = true)
    // 多載一
    .UseSchedulePlaner()
    // 多載二，加上設定選項
    // .UseSchedulePlaner(opts => opts.Delay = 5)
    .Build();
```

## 設定選項
```C#
/// <summary><see cref="ScheduleHostedService"/> 使用的設定選項 <see cref="ScheduleWorkerOptions"/>。</summary>
public sealed class ScheduleWorkerOptions : IScheduleWorkerOptions
{
    /// <summary><see cref="PlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    public int Delay { get; set; } = 0;
    /// <summary><see cref="PlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    public int Interval { get; set; } = 30;
}

```

## 多載方法
```C#
/// <summary>使用 <see cref="SchedulePlanWorkerHostedService"/> 服務。</summary>
public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
        services.AddSingleton<IScheduleHostedService, ScheduleHostedService>();
        services.AddSingleton<IPlanWorker, PlanWorker>(sp => (PlanWorker)sp.GetRequiredService<IScheduleHostedService>().PlanWorker);
        services.AddHostedService(sp => (ScheduleHostedService)sp.GetRequiredService<IScheduleHostedService>());
    });
    return builder;
}

/// <summary>使用 <see cref="SchedulePlanWorkerHostedService"/> 服務，並使用 <see cref="Action" />&lt;<see cref="ScheduleWorkerOptions"/>&gt; 進行設定。</summary>
public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<ScheduleWorkerOptions> options)
{
    builder.ConfigureServices(services =>
    {
        services.Configure(options);
        services.AddSingleton<IScheduleHostedService, ScheduleHostedService>(sp => new ScheduleHostedService(sp, options));
        services.AddSingleton<IPlanWorker, PlanWorker>(sp => (PlanWorker)sp.GetRequiredService<IScheduleHostedService>().PlanWorker);
        services.AddHostedService(sp => (ScheduleHostedService)sp.GetRequiredService<IScheduleHostedService>());
    });
    return builder;
}
```

## 使用自訂屬性設定排程
### 自訂屬性 `SchedulePlanAttribute`
```C#
/// <summary>以排程週期的簡易表示式建立新的 <see cref="SchedulePlanAttribute"/>。</summary>
/// <param name="expression">排程週期的簡易表示式。</param>
public SchedulePlanAttribute(string expression);

/// <summary>建立新的排程週期 <see cref="SchedulePlanAttribute"/>，本建立式適用於 <see cref="IPlanWorker"/> 開始執行(<see cref="PlanTypes.Startup"/>)與結束執行(<see cref="PlanTypes.Stoped"/>)的排程。</summary>
/// <param name="type">排程類型。</param>
/// <exception cref="ArgumentException">必須為 <see cref="PlanTypes.Startup"/> 或 <see cref="PlanTypes.Stoped"/>。</exception>
public SchedulePlanAttribute(PlanTypes type);

/// <summary>建立新的排程週期 <see cref="SchedulePlanAttribute"/>，本建立式適用於指定以日(<see cref="PlanTypes.Day"/>)為單位，週期執行的排程。</summary>
/// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。
/// <para>此參數為排程執行的時間。</para>
/// </param>
/// <param name="period">以日為單位的週期天數。</param>
public SchedulePlanAttribute(string timeString, int period);

/// <summary>建立新的排程週期 <see cref="SchedulePlanAttribute"/>，本建立式適用於指定以星期(<see cref="PlanTypes.Week"/>)為單位，週期執行的排程。</summary>
/// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。
/// <para>此參數為排程執行的時間。</para>
/// </param>
/// <param name="period">以星期為單位的週數。</param>
/// <param name="weekDay">排程指定執行的星期名稱。</param>
public SchedulePlanAttribute(string timeString, int period, WeekDays weekDay);

/// <summary>建立新的排程週期 <see cref="SchedulePlanAttribute"/>，本建立式適用於指定以月(<see cref="PlanTypes.Month"/>)為單位，週期執行的排程。</summary>
/// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。
/// <para>此參數為排程執行的時間。</para>
/// </param>
/// <param name="month">排程指定執行的月份。</param>
/// <param name="day">排程指定執行的日期。</param>
public SchedulePlanAttribute(string timeString, Months month, Days day);

/// <summary>建立新的排程週期 <see cref="SchedulePlanAttribute"/>，本建立式適用於指定以月週(<see cref="PlanTypes.MonthWeek"/>)為單位，週期執行的排程。</summary>
/// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。
/// <para>此參數為排程執行的時間。</para>
/// </param>
/// <param name="month">排程指定執行的月份。</param>
/// <param name="weekNo">排程指定執行的周別。</param>
/// <param name="weekDay">排程指定執行的星期名稱。</param>
public SchedulePlanAttribute(string timeString, Months month, WeekNo weekNo, WeekDays weekDay);
```
排程的簡易表示式 `expression` 以空白區隔每個欄位，每種週期的欄位數目皆不同。其格式依序如下：
1. `PlanTypes.Once`
    * 排程週期值 = 1。
    * 啟動日 = yyyy-MM-dd。
    * 執行時間 = HH:mm:ss。
2. `PlanTypes.Day`
    * 排程週期值 = 2。
    * 啟動日 = yyyy-MM-dd。
    * 執行時間 = HH:mm:ss。
3. `PlanTypes.Week`
    * 排程週期值 = 3。
    * 啟動日 = yyyy-MM-dd。
    * 執行時間 = HH:mm:ss。
    * 週期間隔 = int。
    * 星期幾 = 1~7 代表星期一至星期日，以/符號區隔，如 1/3/5 即每週一、三、五。
4. `PlanTypes.Month`
    * 排程週期值 = 4。
    * 啟動日 = yyyy-MM-dd。
    * 執行時間 = HH:mm:ss。
    * 月份，1~12 代表 1 ~ 12 月，以/符號區隔，如 3/6/9/12 即每年3, 6, 9 及 12 月。<br>可用 A 代表每一個月。
    * 日期，1 ~ 31 代表 1 ~ 31 號，以/符號區隔，如 1/15 即當月的 1 和 15 號。<br>可用 A 代表每一天。
5. `PlanTypes.MonthWeek`
    * 排程週期值 = 5。
    * 啟動日 = yyyy-MM-dd。
    * 執行時間 = HH:mm:ss。
    * 月份，1~12 代表 1 ~ 12 月，以/符號區隔，如 3/6/9/12 即每年 3, 6, 9 及 12 月。<br>可用 A 代表每一個月。
    * 當月週別，當月的 1 號即為第 1 週，以/符號區隔，如 2/4 即當月的第 2 和 4 週。<br>可用 A 代表每一週。
    * 星期幾 = 1~7 代表星期一至星期日，以/符號區隔，如 1/3/5 即每週一、三、五。

其 `PlanTypes`  列舉值如下：
```C#
/// <summary>排程週期類型列舉</summary>
public enum PlanTypes : int
{
    /// <summary>無</summary>
    None = 0,
    /// <summary>執行一次</summary>
    Once = 1,
    /// <summary>每隔 x 天</summary>
    Day = 2,
    /// <summary>每隔 x 週</summary>
    Week = 3,
    /// <summary>每月</summary>
    Month = 4,
    /// <summary>每月週</summary>
    MonthWeek = 5,
    /// <summary>程式執行時</summary>
    Startup = 6,
    /// <summary>程式停止時</summary>
    Stoped = 7,
}
```


## 自訂屬性 `SchedulwPlanAttribute` 範例
```C#
    // 程式執行時
    [SchedulePlan("6")]
    static void SP_Start()
    {
        Console.WriteLine($"[*] {nameof(SP_Start)}: {DateTime.Now}");
    }
    // 關閉程式時
    [SchedulePlan("7")]
    static void SP_Stop()
    {
        Console.WriteLine($"[*] {nameof(SP_Stop)}: {DateTime.Now}");
    }
    // 於 2023-05-23 17:30:00 時執行一次
    [SchedulePlan("1 2023-05-23 17:30:00")]
    static void SP_Once()
    {
        Console.WriteLine($"[*] {nameof(SP_Once)}: {DateTime.Now}");
    }
    // 從程式啟動後每 2 週逢 周二, 周五 的 12:00:00 時執行
    [SchedulePlan("12:00:00", 2, WeekDays.Tuesday | WeekDays.Friday)]
    static void SP_EveryWeek2_5()
    {
        Console.WriteLine($"[*] {nameof(SP_EveryWeek2_5)}: {DateTime.Now}");
    }
```

## 使用程式新增排程
```C#
```