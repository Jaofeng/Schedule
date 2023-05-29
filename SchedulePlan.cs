using CJF.Schedules.Interfaces;

namespace CJF.Schedules;

/// <summary>提供排程計畫的基底類別。</summary>
public sealed class SchedulePlan : ISchedulePlan
{
    public event SchedulePlanEventHandler? Started;
    public event SchedulePlanEventHandler? Stoped;
    public event ExceptionEventHandler? Failed;

    public string Name { get; private set; }
    public ITimePlan TimeTable { get; private set; }
    public bool Valid { get; set; } = true;
    public bool IsRunning { get; private set; } = false;

    private readonly Action? _Method;
    private readonly Action<ISchedulePlan>? _MethodT;


    #region Public Constructor : SchedulePlan(string name, string expression, Action action)
    /// <summary>建立一個新的 <see cref="SchedulePlan"/> 排程項目執行個體。</summary>
    /// <param name="name">排程名稱。</param>
    /// <param name="expression">排程週期的簡易表示式。</param>
    /// <param name="action">綁定執行的方法函示。</param>
    public SchedulePlan(string name, string expression, Action action)
    { 
        Name = name;
        TimeTable = TimePlan.CreatePlan(expression);
        _Method = action;
    }
    #endregion

    #region Public Constructor : SchedulePlan(string name, string expression, Action<ISchedulePlan> action)
    /// <summary>建立一個新的 <see cref="SchedulePlan"/> 排程項目執行個體。</summary>
    /// <param name="name">排程名稱。</param>
    /// <param name="expression">排程週期的簡易表示式。</param>
    /// <param name="action">綁定執行的方法函示。，執行本函示時會把本 <see cref="SchedulePlan"/> 實體執行個體作為參數傳入。</param>
    public SchedulePlan(string name, string expression, Action<ISchedulePlan> action)
    {
        Name = name;
        TimeTable = TimePlan.CreatePlan(expression);
        _MethodT = action;
    }
    #endregion

    #region Internal Constructor : SchedulePlan(string name, ITimePlan trigger, Action action)
    internal SchedulePlan(string name, ITimePlan trigger, Action action)
    {
        Name = name;
        TimeTable = trigger;
        _Method = action;
    }
    #endregion


    #region Private Method : void ExecPlan()
    /// <summary>執行排程。</summary>
    internal void ExecPlan()
    {
        if (IsRunning)
            return;
        IsRunning = true;
        Started?.Invoke(this);
        try
        {
            if (_MethodT != null)
                _MethodT(this);
            else
                _Method?.Invoke();
        }
        catch (Exception ex)
        {
            Failed?.Invoke(this, new ExceptionEventArgs(ex));
        }
        finally
        {
            IsRunning = false;
            Stoped?.Invoke(this);
        }
    }
    #endregion
}