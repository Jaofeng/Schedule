using CJF.Schedules.Interfaces;

namespace CJF.Schedules;

/// <summary>提供排程計畫的基底類別。</summary>
public sealed class SchedulePlan : ISchedulePlan
{
    public event SchedulePlanEventHandler? Started;
    public event SchedulePlanEventHandler? Stopped;
    public event ExceptionEventHandler? Failed;

    public string Name { get; private set; }
    public IPlanTime TimeTable { get; private set; }
    public bool Valid { get; set; } = true;
    public bool IsRunning { get; private set; } = false;

    private readonly Action? _Action;
    private readonly Action<ISchedulePlan>? _ActionArg;

    /// <summary>建立一個新的 <see cref="SchedulePlan"/> 排程項目執行個體。</summary>
    /// <param name="name">排程名稱。</param>
    /// <param name="expression">排程週期的簡易表示式。</param>
    /// <param name="action">綁定執行的方法函式。</param>
    public SchedulePlan(string name, string expression, Action action)
    {
        Name = name;
        TimeTable = TimePlan.CreatePlan(expression);
        _Action = action;
    }

    /// <summary>建立一個新的 <see cref="SchedulePlan"/> 排程項目執行個體。</summary>
    /// <param name="name">排程名稱。</param>
    /// <param name="expression">排程週期的簡易表示式。</param>
    /// <param name="action">綁定執行的方法函式。，執行本函式時會把本 <see cref="SchedulePlan"/> 實體執行個體作為參數傳入。</param>
    public SchedulePlan(string name, string expression, Action<ISchedulePlan> action)
    {
        Name = name;
        TimeTable = TimePlan.CreatePlan(expression);
        _ActionArg = action;
    }

    /// <summary>建立一個新的 <see cref="SchedulePlan"/> 排程項目執行個體。</summary>
    /// <param name="name">排程名稱。</param>
    /// <param name="trigger">排程週期的時間表。</param>
    /// <param name="action">綁定執行的方法函式。</param>
    internal SchedulePlan(string name, IPlanTime trigger, Action action)
    {
        Name = name;
        TimeTable = trigger;
        _Action = action;
    }

    /// <summary>建立一個新的 <see cref="SchedulePlan"/> 排程項目執行個體。</summary>
    /// <param name="name">排程名稱。</param>
    /// <param name="trigger">排程週期的時間表。</param>
    /// <param name="action">綁定執行的方法函式。，執行本函式時會把本 <see cref="SchedulePlan"/> 實體執行個體作為參數傳入。</param>
    internal SchedulePlan(string name, IPlanTime trigger, Action<ISchedulePlan> action)
    {
        Name = name;
        TimeTable = trigger;
        _ActionArg = action;
    }

    #region Private Method : void ExecutePlan()
    /// <summary>執行排程。</summary>
    internal void ExecutePlan()
    {
        if (IsRunning)
            return ;
        IsRunning = true;
        Started?.Invoke(this);
        try
        {
            if (_ActionArg != null)
                _ActionArg.Invoke(this);
            else
                _Action?.Invoke();
            TimeTable.UpdateExecuted(DateTime.Now);
        }
        catch (Exception ex)
        {
            Failed?.Invoke(this, new ExceptionEventArgs(ex));
        }
        finally
        {
            IsRunning = false;
            Stopped?.Invoke(this);
        }
    }
    #endregion
}