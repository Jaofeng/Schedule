
namespace CJF.Schedules;

/// <summary><see cref="PlanWorker"/> 使用的設定選項 <see cref="PlanWorkerOptions"/>。</summary>
public sealed class PlanWorkerOptions
{
    /// <summary><see cref="PlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    public int Delay { get; set; } = 0;
    /// <summary><see cref="PlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    public int Interval { get; set; } = 30;
    /// <summary>是否自動綁定具有 <see cref="PlanAttribute"/> 的靜態方法。</summary>
    /// <remarks>如果為 <see langword="true"/>，則會在建構 <see cref="PlanWorker"/> 時自動綁定所有符合條件的方法。</remarks>
    public bool AutoBind { get; set; } = true;
}

