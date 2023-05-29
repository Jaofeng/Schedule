namespace CJF.Schedules.Interfaces;

/// <summary>提供排程工作器的介面。</summary>
public interface IPlanWorker : IDisposable
{
    /// <summary><see cref="IPlanWorker"/> 執行個體開始運作時所產生的事件。</summary>
    event PlanWorkerEventHandler? Started;
    /// <summary><see cref="IPlanWorker"/> 執行個體停止運作時所產生的事件。</summary>
    event PlanWorkerEventHandler? Stoped;
    /// <summary><see cref="ISchedulePlan"/> 執行個體開始執行時所產生的事件。</summary>
    event SchedulePlanEventHandler? PlanStarted;
    /// <summary><see cref="ISchedulePlan"/> 執行個體開始執行完畢時所產生的事件。</summary>
    event SchedulePlanEventHandler? PlanFinished;
    /// <summary><see cref="ISchedulePlan"/> 執行個體中斷執行時所產生的事件。</summary>
    event ExceptionEventHandler? PlanFailed;

    /// <summary>控管的所有排程。</summary>
    ISchedulePlanCollection Plans { get; }

    /// <summary>新增一個排程。</summary>
    /// <param name="item">新增的排程執行個體。</param>
    void AppendPlan(ISchedulePlan item);
    /// <summary>刪除排程。</summary>
    /// <param name="name">欲刪除排程的名稱。</param>
    void RemovePlan(string name);
    /// <summary>開始執行所有排程。</summary>
    /// <param name="token">提供用來監控要求停止的訊號。</param>
    Task StartAsync(CancellationToken token);
    /// <summary>停止執行所有排程。</summary>
    /// <param name="token">提供用來監控要求停止的訊號。</param>
    Task StopAsync(CancellationToken token);
}