using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試 PlanWorkerOptions 類別
/// </summary>
public class PlanWorkerOptionsTests
{
    /// <summary>
    /// 測試 PlanWorkerOptions 的預設值是否正確
    /// 驗證預設延遲為 0 秒，預設檢查間隔為 30 秒，預設自動綁定為 true
    /// </summary>
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Act
        var options = new PlanWorkerOptions();
        
        // Assert
        Assert.Equal(0, options.Delay);
        Assert.Equal(30, options.Interval);
        Assert.True(options.AutoBind);
    }

    /// <summary>
    /// 測試 PlanWorkerOptions 的屬性可以被設定和讀取
    /// 驗證可以正確設定延遲時間、檢查間隔和自動綁定屬性
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndGet()
    {
        // Arrange
        var options = new PlanWorkerOptions();
        
        // Act
        options.Delay = 5;
        options.Interval = 60;
        options.AutoBind = false;
        
        // Assert
        Assert.Equal(5, options.Delay);
        Assert.Equal(60, options.Interval);
        Assert.False(options.AutoBind);
    }

    /// <summary>
    /// 測試 AutoBind 屬性的預設行為
    /// 驗證 AutoBind 預設為 true，可以被正確設定為 false
    /// </summary>
    [Fact]
    public void AutoBind_DefaultValue_ShouldBeTrue()
    {
        // Act
        var options = new PlanWorkerOptions();
        
        // Assert
        Assert.True(options.AutoBind);
    }

    /// <summary>
    /// 測試 AutoBind 屬性可以被設定為不同的值
    /// 驗證 AutoBind 屬性的設定和讀取功能
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AutoBind_CanBeSetAndGet(bool autoBind)
    {
        // Arrange
        var options = new PlanWorkerOptions();
        
        // Act
        options.AutoBind = autoBind;
        
        // Assert
        Assert.Equal(autoBind, options.AutoBind);
    }
}