using CJF.Schedules;
using CJF.Schedules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試 PlanWorker 類別的功能
/// </summary>
public class PlanWorkerTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ILogger<PlanWorker> _logger;

    public PlanWorkerTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger<PlanWorker>>();
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }

    /// <summary>
    /// 測試 PlanWorker 的建構函式正確初始化
    /// 驗證能正確建立工作器物件並初始化排程集合
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var options = new PlanWorkerOptions();
        
        // Act
        var worker = new PlanWorker(options, _logger);
        
        // Assert
        Assert.NotNull(worker);
        Assert.NotNull(worker.Plans);
    }

    /// <summary>
    /// 測試 AppendPlan 方法能正確新增排程到集合中
    /// 驗證新增的排程能被正確儲存並可以透過名稱存取
    /// </summary>
    [Fact]
    public void AppendPlan_ShouldAddPlanToCollection()
    {
        // Arrange
        var options = new PlanWorkerOptions();
        var worker = new PlanWorker(options, _logger);
        var plan = new SchedulePlan("TestPlan", "6", () => { });
        
        // Act
        worker.AppendPlan(plan);
        
        // Assert
        Assert.True(worker.Plans.Contains("TestPlan"));
        Assert.Equal(plan, worker.Plans["TestPlan"]);
    }

    /// <summary>
    /// 測試 RemovePlan 方法能正確從集合中移除排程
    /// 驗證指定的排程能被正確移除且不再存在於集合中
    /// </summary>
    [Fact]
    public void RemovePlan_ShouldRemovePlanFromCollection()
    {
        // Arrange
        var options = new PlanWorkerOptions();
        var worker = new PlanWorker(options, _logger);
        var plan = new SchedulePlan("TestPlan", "6", () => { });
        worker.AppendPlan(plan);
        
        // Act
        worker.RemovePlan("TestPlan");
        
        // Assert
        Assert.False(worker.Plans.Contains("TestPlan"));
    }

    /// <summary>
    /// 測試移除不存在的排程不會拋出例外
    /// 驗證當試圖移除不存在的排程時，操作能安全地完成不會引起例外
    /// </summary>
    [Fact]
    public void RemovePlan_WithNonExistentPlan_ShouldNotThrow()
    {
        // Arrange
        var options = new PlanWorkerOptions();
        var worker = new PlanWorker(options, _logger);
        
        // Act & Assert
        var exception = Record.Exception(() => worker.RemovePlan("NonExistent"));
        Assert.Null(exception);
    }

    /// <summary>
    /// 測試 PlanWorker 的非同步啟動功能
    /// 驗證能正確啟動排程工作器且不會拋出例外
    /// </summary>
    [Fact]
    public async Task StartAsync_ShouldStartCorrectly()
    {
        // Arrange
        var options = new PlanWorkerOptions { Delay = 0 };
        var worker = new PlanWorker(options, _logger);
        using var cts = new CancellationTokenSource();
        
        // Act
        await worker.StartAsync(cts.Token);
        
        // Assert - 如果沒有拋出例外，則表示啟動成功
        Assert.True(true);
        
        // Cleanup
        await worker.StopAsync(cts.Token);
    }

    /// <summary>
    /// 測試 PlanWorker 的非同步停止功能
    /// 驗證能正確停止排程工作器且不會拋出例外
    /// </summary>
    [Fact]
    public async Task StopAsync_ShouldStopCorrectly()
    {
        // Arrange
        var options = new PlanWorkerOptions { Delay = 0 };
        var worker = new PlanWorker(options, _logger);
        using var cts = new CancellationTokenSource();
        
        await worker.StartAsync(cts.Token);
        
        // Act
        await worker.StopAsync(cts.Token);
        
        // Assert - 如果沒有拋出例外，則表示停止成功
        Assert.True(true);
    }
}

/// <summary>
/// 測試 PlanWorkerOptions 類別
/// </summary>
public class PlanWorkerOptionsTests
{
    /// <summary>
    /// 測試 PlanWorkerOptions 的預設值是否正確
    /// 驗證預設延遲為 0 秒，預設檢查間隔為 30 秒
    /// </summary>
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Act
        var options = new PlanWorkerOptions();
        
        // Assert
        Assert.Equal(0, options.Delay);
        Assert.Equal(30, options.Interval);
    }

    /// <summary>
    /// 測試 PlanWorkerOptions 的屬性可以被設定和讀取
    /// 驗證可以正確設定延遲時間和檢查間隔屬性
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndGet()
    {
        // Arrange
        var options = new PlanWorkerOptions();
        
        // Act
        options.Delay = 5;
        options.Interval = 60;
        
        // Assert
        Assert.Equal(5, options.Delay);
        Assert.Equal(60, options.Interval);
    }
}