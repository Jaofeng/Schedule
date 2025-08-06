using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試 HostBuilder 擴充方法
/// </summary>
public class HostBuilderExtensionsTests
{
    /// <summary>
    /// 測試不使用自訂選項的 UseSchedulePlaner 擴充方法
    /// 驗證能正確註冊排程相關的服務並使用預設選項
    /// </summary>
    [Fact]
    public void UseSchedulePlaner_WithoutOptions_ShouldRegisterServices()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        
        // Act
        hostBuilder.UseSchedulePlaner();
        var host = hostBuilder.Build();
        
        // Assert
        using (host)
        {
            var planWorkerOptions = host.Services.GetService<PlanWorkerOptions>();
            var planWorker = host.Services.GetService<PlanWorker>();
            
            Assert.NotNull(planWorkerOptions);
            Assert.NotNull(planWorker);
            
            // 驗證預設選項值
            Assert.Equal(0, planWorkerOptions.Delay);
            Assert.Equal(30, planWorkerOptions.Interval);
        }
    }

    /// <summary>
    /// 測試使用自訂選項的 UseSchedulePlaner 擴充方法
    /// 驗證能正確註冊服務並套用使用者提供的自訂選項
    /// </summary>
    [Fact]
    public void UseSchedulePlaner_WithOptions_ShouldConfigureServices()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        var delay = 5;
        var interval = 60;
        
        // Act
        hostBuilder.UseSchedulePlaner(opts =>
        {
            opts.Delay = delay;
            opts.Interval = interval;
        });
        var host = hostBuilder.Build();
        
        // Assert
        using (host)
        {
            var planWorkerOptions = host.Services.GetRequiredService<PlanWorkerOptions>();
            var planWorker = host.Services.GetRequiredService<PlanWorker>();
            
            Assert.NotNull(planWorkerOptions);
            Assert.NotNull(planWorker);
            
            // 驗證自訂選項值
            Assert.Equal(delay, planWorkerOptions.Delay);
            Assert.Equal(interval, planWorkerOptions.Interval);
        }
    }

    /// <summary>
    /// 測試 UseSchedulePlaner 能正確註冊託管服務
    /// 驗證 PlanWorker 被正確註冊為 IHostedService 服務
    /// </summary>
    [Fact]
    public void UseSchedulePlaner_ShouldRegisterHostedService()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        
        // Act
        hostBuilder.UseSchedulePlaner(null);
        var host = hostBuilder.Build();
        
        // Assert
        using (host)
        {
            var hostedServices = host.Services.GetServices<IHostedService>();
            Assert.Contains(hostedServices, service => service is PlanWorker);
        }
    }

    /// <summary>
    /// 測試 UseSchedulePlaner 擴充方法傳回相同的 HostBuilder 實例
    /// 驗證擴充方法遵循流暢 API 設計，支援方法鏈式呼叫
    /// </summary>
    [Fact]
    public void UseSchedulePlaner_ShouldReturnSameHostBuilder()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        
        // Act
        var result = hostBuilder.UseSchedulePlaner(null);
        
        // Assert
        Assert.Same(hostBuilder, result);
    }
}