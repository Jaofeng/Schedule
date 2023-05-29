using CJF.Schedules.Attributes;
using CJF.Schedules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CJF.Schedules;

public sealed class SchedulePlanWorker : IPlanWorker
{
    #region Public Events
    public event PlanWorkerEventHandler? Started;
    public event PlanWorkerEventHandler? Stoped;
    public event SchedulePlanEventHandler? PlanStarted;
    public event SchedulePlanEventHandler? PlanFinished;
    public event ExceptionEventHandler? PlanFailed;
    #endregion

    #region Public Properties
    public ISchedulePlanCollection Plans => _Plans;
    #endregion

    #region Private Variables
    private readonly SchedulePlanCollection _Plans;
    private readonly IScheduleWorkerOptions _Options;
    private readonly IServiceProvider _Provider;
    private readonly ILogger<SchedulePlanWorker>? _Logger;
    private bool _IsDisposed = false;
    private Task _ExecuteTask = null!;
    #endregion


    #region Public Constructor : PlanWorker(IServiceProvider provider, IPlanerOptions options)
    /// <summary>建立新的排程執行個體 <see cref="SchedulePlanWorker"/>。</summary>
    /// <param name="provider">依賴注入(DI)的服務提供者 <see cref="IServiceProvider"/>。</param>
    /// <param name="options">排程執行選項 <see cref="IScheduleWorkerOptions"/>。</param>
    public SchedulePlanWorker(IServiceProvider provider, IScheduleWorkerOptions options)
    {
        _Provider = provider;
        _Options = options;
        _Logger = _Provider.GetService<ILogger<SchedulePlanWorker>>();
        _Plans = new SchedulePlanCollection();
        BindAttributes();
    }
    #endregion


    #region IDsposable Support
    ~SchedulePlanWorker() => Dispose(false);
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private void Dispose(bool disposing)
    {
        if (_IsDisposed) return;
        if (disposing)
        {
            _Plans.Clear();
        }
        _IsDisposed = true;
    }
    #endregion

    #region IPlanWorker Support
    public void AppendPlan(ISchedulePlan item)
    {
        item.Started += OnPlanStarted;
        item.Stoped += OnPlanFinished;
        item.Failed += OnPlanFailed;
        _Plans.Add(item);
        var tp = item.TimeTable;
        //Console.WriteLine($"{tp.GetExpression(),-30}: Start From: {tp.StartFrom:yyyy-MM-dd HH:mm:ss}, Next Time: {tp.NextTime:yyyy-MM-dd HH:mm:ss}");
    }
    public void RemovePlan(string name)
    {
        if (!_Plans.Contains(name)) return;
        _Plans[name]!.Started -= OnPlanStarted;
        _Plans[name]!.Stoped -= OnPlanFinished;
        _Plans[name]!.Failed -= OnPlanFailed;
        _Plans.Remove(name);
    }
    #endregion

    #region IHostedService Support
    public Task StartAsync(CancellationToken token)
    {
        _ExecuteTask = WorkerProcess(token);
        if (_ExecuteTask.IsCompleted)
            return _ExecuteTask;
        return Task.CompletedTask;
    }
    public async Task StopAsync(CancellationToken token)
    {
        if (_ExecuteTask == null) return;
        await Task.WhenAny(_ExecuteTask, Task.Delay(10000, token)).ConfigureAwait(false);
        foreach (var plan in _Plans.GetPlans(PlanTypes.Stoped).Cast<SchedulePlan>())
        {
            _Logger?.LogDebug("Executing Plan : {name}", plan.Name);
            plan.ExecPlan();
        }
        Stoped?.Invoke(this);
        _Logger?.LogDebug("PlanerProcess Stoped...");
    }
    #endregion


    #region Internal Method : Task WorkerProcess(CancellationToken cancellationToken)
    internal async Task WorkerProcess(CancellationToken cancellationToken)
    {
        _Logger?.LogDebug("PlanerProcess Starting...");
        Started?.Invoke(this);
        if (_Options.Delay > 0)
            await Task.Delay(_Options.Delay * 1000, cancellationToken).ConfigureAwait(false);
        foreach (var plan in _Plans.GetPlans(PlanTypes.Startup).Cast<SchedulePlan>())
        {
            _Logger?.LogDebug("Executing Plan : {name}", plan.Name);
            plan.ExecPlan();
        }
        while (!cancellationToken.IsCancellationRequested)
        {
            _Logger?.LogDebug("PlanerProcess Running...");
            foreach (var plan in _Plans.GetOnTime().Cast<SchedulePlan>())
            {
                _Logger?.LogDebug("Executing Plan : {name}", plan.Name);
                plan.ExecPlan();
            }
                
            await Task.Delay(TimeSpan.FromSeconds(_Options.Interval), cancellationToken).ConfigureAwait(false);
        }

        foreach (var plan in _Plans.GetPlans(PlanTypes.Stoped).Cast<SchedulePlan>())
        {
            _Logger?.LogDebug("Executing Plan : {name}", plan.Name);
            plan.ExecPlan();
        }
        Stoped?.Invoke(this);
        _Logger?.LogDebug("PlanerProcess Stoped...");
        await Task.CompletedTask;
    }
    #endregion


    #region Private Method : void BindAttributes()
    private void BindAttributes()
    {
        BindingFlags binding = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            foreach (Type t in a.GetTypes().Where(_t => _t.GetRuntimeMethods().Any(_m => _m.GetCustomAttributes<SchedulePlanAttribute>().Any())))
                foreach (MethodInfo mi in t.GetMethods(binding).Where(_m => _m.IsStatic && _m.GetCustomAttributes<SchedulePlanAttribute>().Any()))
                    foreach (SchedulePlanAttribute sa in mi.GetCustomAttributes<SchedulePlanAttribute>())
                        if (_Plans.Find(mi.Name) is null)
                        {
                            try
                            {
                                ITimePlan tp;
                                if (!string.IsNullOrEmpty(sa.Expression))
                                    tp = TimePlan.CreatePlan(sa.Expression);
                                else
                                    tp = TimePlan.CreatePlan(sa);
                                AppendPlan(new SchedulePlan(mi.Name, tp, () => mi.Invoke(null, null)));
                            }
                            catch (Exception ex)
                            {
                                _Logger?.LogError(ex, "{msg}", ex.Message);
                            }
                        }
    }
    #endregion

    #region Plan Event Handler
    private void OnPlanStarted(ISchedulePlan item)
    {
        _Logger?.LogDebug("Plan Started : {name}", item.Name);
        PlanStarted?.Invoke(item);
    }
    private void OnPlanFinished(ISchedulePlan item)
    {
        _Logger?.LogDebug("Plan Finished : {name}", item.Name);
        PlanFinished?.Invoke(item);
    }
    private void OnPlanFailed(ISchedulePlan item, ExceptionEventArgs e)
    {
        _Logger?.LogWarning("Plan Failed : {name}", item.Name);
        PlanFailed?.Invoke(item, e);
    }
    #endregion
}
