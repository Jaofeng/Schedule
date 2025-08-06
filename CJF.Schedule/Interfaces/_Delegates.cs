
namespace CJF.Schedules.Interfaces;

/// <summary>定義排程執行中的事件錯誤處理器。</summary>
/// <param name="item">排程項目。</param>
/// <param name="e">事件參數。</param>
public delegate void ExceptionEventHandler(ISchedulePlan item, ExceptionEventArgs e);
/// <summary>定義排程開始或結束時的事件處理器。</summary>
/// <param name="item">排程項目。</param>
public delegate void SchedulePlanEventHandler(ISchedulePlan item);
/// <summary>定義排程工作器開始或結束時的事件處理器。</summary>
/// <param name="worker">排程工作器。</param>
public delegate void PlanWorkerEventHandler(PlanWorker worker);
