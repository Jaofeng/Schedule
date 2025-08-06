using CJF.Schedules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CJF.Schedules;

public sealed class PlanWorker : BackgroundService
{
    #region Public Events
    public event PlanWorkerEventHandler? Started;
    public event PlanWorkerEventHandler? Stoped;
    public event SchedulePlanEventHandler? PlanStarted;
    public event SchedulePlanEventHandler? PlanFinished;
    public event ExceptionEventHandler? PlanFailed;
    #endregion

    #region Public Properties
    public IPlanCollection Plans => _Plans;
    #endregion

    #region Private Variables
    private readonly PlanCollection _Plans;
    private readonly PlanWorkerOptions _Options;
    private readonly ILogger<PlanWorker> _Logger;
    #endregion


    /// <summary>建立新的排程執行個體 <see cref="PlanWorker"/>。</summary>
    /// <param name="options">排程執行選項 <see cref="PlanWorkerOptions"/>。</param>
    /// <param name="logger">日誌記錄器 <see cref="ILogger{T}"/>。</param>
    /// <remarks>此建構函式會自動綁定所有具有 <see cref="PlanAttribute"/> 的靜態方法。</remarks>
    public PlanWorker(PlanWorkerOptions options, ILogger<PlanWorker> logger)
    {
        _Options = options;
        _Logger = logger;
        _Plans = [];
        BindAttributes();
    }

    /// <summary>開始執行排程工作器。</summary>
    /// <param name="cancellationToken">提供用來監控要求停止的訊號。</param>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _Logger?.LogDebug("PlanerProcess Starting...");
        Started?.Invoke(this);
        if (_Options.Delay > 0)
            await Task.Delay(_Options.Delay * 1000, cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    /// <summary>停止執行排程工作器。</summary>
    /// <param name="cancellationToken">提供用來監控要求停止的訊號。</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _Logger?.LogDebug("PlanerProcess Stopping...");
        await base.StopAsync(cancellationToken);
    }

    /// <summary>執行排程工作器的主要邏輯。</summary>
    /// <param name="stoppingToken">提供用來監控要求停止的訊號。</param>
    /// <remarks>此方法會在工作器啟動後持續執行，直到收到停止請求。</remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 在啟動時執行所有啟動計畫
        foreach (var plan in _Plans.GetPlans(PlanTypes.Startup).Cast<SchedulePlan>())
        {
            _Logger?.LogTrace("Executing Plan : {name}", plan.Name);
            plan.ExecutePlan();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                // 執行所有符合時間的計畫
                foreach (var plan in _Plans.GetOnTime().Cast<SchedulePlan>())
                {
                    try
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;
                        _Logger?.LogTrace("Executing Plan : {name}", plan.Name);
                        plan.ExecutePlan();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(_Options.Interval), stoppingToken);
            }
            catch (Exception ex)
            {
                _Logger?.LogError(ex, "An error occurred while executing plans: {message}", ex.Message);
            }
        }

        foreach (var plan in _Plans.GetPlans(PlanTypes.Stoped).Cast<SchedulePlan>())
        {
            _Logger?.LogTrace("Executing Plan : {name}", plan.Name);
            plan.ExecutePlan();
        }
        Stoped?.Invoke(this);
        _Logger?.LogDebug("PlanerProcess Stoped...");
        await Task.CompletedTask;
    }

    /// <summary>新增一個排程。</summary>
    /// <param name="item">新增的排程執行個體。</param>
    /// <remarks>此方法會將排程加入到工作器中，並訂閱相關事件。</remarks>
    /// <exception cref="KeyExistsException">指定的排程項目名稱已存在。</exception>
    public void AppendPlan(ISchedulePlan item)
    {
        item.Started += OnPlanStarted;
        item.Stopped += OnPlanFinished;
        item.Failed += OnPlanFailed;
        _Plans.Add(item);
    }

    /// <summary>移除一個排程。</summary>
    /// <param name="name">欲移除的排程名稱。</param>
    public void RemovePlan(string name)
    {
        if (!_Plans.Contains(name)) return;
        _Plans[name]!.Started -= OnPlanStarted;
        _Plans[name]!.Stopped -= OnPlanFinished;
        _Plans[name]!.Failed -= OnPlanFailed;
        _Plans.Remove(name);
    }


    #region Private Method : void BindAttributes()
    private void BindAttributes()
    {
        BindingFlags binding = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in a.GetTypes().Where(_t => _t.GetRuntimeMethods().Any(_m => _m.GetCustomAttributes<PlanAttribute>().Any())))
            {
                foreach (MethodInfo mi in t.GetMethods(binding).Where(_m => _m.IsStatic && _m.GetCustomAttributes<PlanAttribute>().Any()))
                {
                    foreach (PlanAttribute sa in mi.GetCustomAttributes<PlanAttribute>())
                    {
                        if (_Plans.Contains(mi.Name)) continue;
                        try
                        {
                            IPlanTime tp;
                            if (!string.IsNullOrEmpty(sa.Expression))
                                tp = TimePlan.CreatePlan(sa.Expression);
                            else
                                tp = TimePlan.CreatePlan(sa);
                            if (mi.GetParameters().Length == 0)
                                AppendPlan(new SchedulePlan(mi.Name, tp, () => mi.Invoke(null, null)));
                            else if (mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType == typeof(ISchedulePlan))
                                AppendPlan(new SchedulePlan(mi.Name, tp, plan => mi.Invoke(null, [plan])));
                            else
                                throw new InvalidOperationException($"Method '{mi.Name}' has unsupported parameters.");
                        }
                        catch (Exception ex)
                        {
                            _Logger?.LogError(ex, "{msg}", ex.Message);
                        }
                    }
                }
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

/// <summary><see cref="PlanWorker"/> 使用的設定選項 <see cref="PlanWorkerOptions"/>。</summary>
public sealed class PlanWorkerOptions
{
    /// <summary><see cref="PlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    public int Delay { get; set; } = 0;
    /// <summary><see cref="PlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    public int Interval { get; set; } = 30;
}

/// <summary>擴展 <see cref="IHostBuilder"/> 以使用排程工作器服務。</summary>
public static class ScheduleHostServiceExtensions
{

    #region Public Static Method : IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanerOptions> options)
    /// <summary>使用 <see cref="PlanWorker"/> 服務，並使用 <see cref="Action" />&lt;<see cref="PlanWorkerOptions"/>&gt; 進行設定。</summary>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    /// <param name="options">進行設定的 <see cref="Action" />&lt;<see cref="PlanWorkerOptions"/>&gt; 執行函示。</param>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanWorkerOptions>? options = null)
    {
        builder.ConfigureServices((context, services) =>
        {
            var opts = new PlanWorkerOptions();
            options?.Invoke(opts);
            services.AddSingleton(opts);
            services.AddSingleton<PlanWorker>();
            services.AddHostedService(provider => provider.GetRequiredService<PlanWorker>());
        });

        return builder;
    }
    #endregion
}
