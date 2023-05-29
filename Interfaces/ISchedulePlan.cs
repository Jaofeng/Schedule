namespace CJF.Schedules.Interfaces;

/// <summary>提供排程項目的介面。</summary>
public interface ISchedulePlan
{
    /// <summary>排程開始執行時所產生的事件。</summary>
    event SchedulePlanEventHandler? Started;
    /// <summary>排程執行完畢時所產生的事件。</summary>
    event SchedulePlanEventHandler? Stoped;
    /// <summary>排程執行過程中發生錯誤時所產生的事件。</summary>
    event ExceptionEventHandler? Failed;

    /// <summary>排程名稱。</summary>
    string Name { get; }
    /// <summary>觸發的時間表。</summary>
    ITimePlan TimeTable { get; }
    /// <summary>是否啟用此排程。</summary>
    bool Valid { get; set; }
    /// <summary>排程是否正在執行中。</summary>
    bool IsRunning { get; }
}

