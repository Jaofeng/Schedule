using CJF.Schedules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CJF.Schedules;

#region Public Interface : IScheduleWorkerOptions
public interface IScheduleWorkerOptions
{
    /// <summary><see cref="SchedulePlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    int Delay { get; set; }
    /// <summary><see cref="SchedulePlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    int Interval { get; set; }
}
#endregion

#region Public Sealed Class : ScheduleWorkerOptions
/// <summary><see cref="SchedulePlanWorkerHostedService"/> 使用的設定選項 <see cref="ScheduleWorkerOptions"/>。</summary>
public sealed class ScheduleWorkerOptions : IScheduleWorkerOptions
{
    /// <summary><see cref="SchedulePlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    public int Delay { get; set; } = 0;
    /// <summary><see cref="SchedulePlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    public int Interval { get; set; } = 30;
}
#endregion

#region Public Interface : IScheduleWorkerHostedService
public interface IScheduleWorkerHostedService : IHostedService, IDisposable
{
    IPlanWorker PlanWorker { get; }
}
#endregion

#region Public Sealed Class : SchedulePlanWorkerHostedService
public sealed class SchedulePlanWorkerHostedService : IScheduleWorkerHostedService
{
    /// <summary>排程常駐服務的工作執行個體。</summary>
    public IPlanWorker PlanWorker { get; private set; }

    private bool _IsDisposed = false;


    #region Public Constructor : ScheduleHostedService(...)
    /// <summary>建立排程常駐服務。</summary>
    /// <param name="provider">注入 (DI) 的 <see cref="IServiceProvider"/>。</param>
    public SchedulePlanWorkerHostedService(IServiceProvider provider) => PlanWorker = new SchedulePlanWorker(provider, new ScheduleWorkerOptions());
    /// <summary>建立排程常駐服務。</summary>
    /// <param name="provider">注入 (DI) 的 <see cref="IServiceProvider"/>。</param>
    /// <param name="options">設定選項。</param>
    public SchedulePlanWorkerHostedService(IServiceProvider provider, Action<ScheduleWorkerOptions> options)
    {
        var _opts = new ScheduleWorkerOptions();
        options(_opts);
        PlanWorker = new SchedulePlanWorker(provider, _opts);
    }
    #endregion

    #region IDsposable Support
    ~SchedulePlanWorkerHostedService() => Dispose(false);
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
            PlanWorker.Dispose();
        }
        _IsDisposed = true;
    }
    #endregion

    #region IHostedService Support
    public Task StartAsync(CancellationToken cancellationToken) => PlanWorker.StartAsync(cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken) => PlanWorker.StopAsync(cancellationToken);
    #endregion
}
#endregion

#region Public Static Class : ScheduleHostServiceExtensions
public static class ScheduleHostServiceExtensions
{
    #region Public Static Method : IHostBuilder UseSchedulePlaner(this IHostBuilder builder)
    /// <summary>使用 <see cref="SchedulePlanWorkerHostedService"/> 服務。</summary>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    /// <returns><see cref="IHostBuilder"/> 執行個體。</returns>
    public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IScheduleWorkerHostedService, SchedulePlanWorkerHostedService>();
            services.AddSingleton<IPlanWorker, SchedulePlanWorker>(sp => (SchedulePlanWorker)sp.GetRequiredService<IScheduleWorkerHostedService>().PlanWorker);
            services.AddHostedService(sp => (SchedulePlanWorkerHostedService)sp.GetRequiredService<IScheduleWorkerHostedService>());
        });
        return builder;
    }
    #endregion

    #region Public Static Method : IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanerOptions> options)
    /// <summary>使用 <see cref="SchedulePlanWorkerHostedService"/> 服務，並使用 <see cref="Action" />&lt;<see cref="ScheduleWorkerOptions"/>&gt; 進行設定。</summary>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    /// <param name="options">進行設定的 <see cref="Action" />&lt;<see cref="ScheduleWorkerOptions"/>&gt; 執行函示。</param>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<ScheduleWorkerOptions> options)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure(options);
            services.AddSingleton<IScheduleWorkerHostedService, SchedulePlanWorkerHostedService>(sp => new SchedulePlanWorkerHostedService(sp, options));
            services.AddSingleton<IPlanWorker, SchedulePlanWorker>(sp => (SchedulePlanWorker)sp.GetRequiredService<IScheduleWorkerHostedService>().PlanWorker);
            services.AddHostedService(sp => (SchedulePlanWorkerHostedService)sp.GetRequiredService<IScheduleWorkerHostedService>());
        });
        return builder;
    }
    #endregion
}
#endregion
