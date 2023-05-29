using CJF.Schedules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CJF.Schedules;

#region Public Interface : IPlanerOptions
public interface IPlanerOptions
{
    /// <summary><see cref="PlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    int Delay { get; set; }
    /// <summary><see cref="PlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    int Interval { get; set; }
}
#endregion

#region Public Sealed Class : PlanerOptions
public sealed class PlanerOptions : IPlanerOptions
{
    /// <summary><see cref="PlanWorker"/> 開始執行後延遲執行排程項目的時間，單位秒。</summary>
    public int Delay { get; set; } = 0;
    /// <summary><see cref="PlanWorker"/> 排程項目檢查週期的間隔時間，單位秒。</summary>
    public int Interval { get; set; } = 30;
}
#endregion

#region Public Interface : IPlanWorker
public interface IScheduleHostedService : IHostedService, IDisposable
{
    IPlanWorker PlanWorker { get; }
}
#endregion

#region Public Sealed Class : ScheduleHostedService
public sealed class ScheduleHostedService : IScheduleHostedService
{
    /// <summary>排程常駐服務的工作執行個體。</summary>
    public IPlanWorker PlanWorker { get; private set; }

    private bool _IsDisposed = false;


    #region Public Constructor : ScheduleHostedService(...)
    /// <summary>建立排程常駐服務。</summary>
    /// <param name="provider">注入 (DI) 的 <see cref="IServiceProvider"/>。</param>
    public ScheduleHostedService(IServiceProvider provider) => PlanWorker = new PlanWorker(provider, new PlanerOptions());
    /// <summary>建立排程常駐服務。</summary>
    /// <param name="provider">注入 (DI) 的 <see cref="IServiceProvider"/>。</param>
    /// <param name="options">設定選項。</param>
    public ScheduleHostedService(IServiceProvider provider, Action<PlanerOptions> options)
    {
        var _opts = new PlanerOptions();
        options(_opts);
        PlanWorker = new PlanWorker(provider, _opts);
    }
    #endregion

    #region IDsposable Support
    ~ScheduleHostedService() => Dispose(false);
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
    #endregion

    #region Public Static Method : IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanerOptions> options)
    public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanerOptions> options)
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
    #endregion
}
#endregion
