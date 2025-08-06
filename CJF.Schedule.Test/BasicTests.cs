using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 基本功能測試
/// </summary>
public class BasicTests
{
    /// <summary>
    /// 測試 SchedulePlan 的基本建立功能
    /// 驗證建構函式能正確設定排程名稱、有效狀態和執行狀態
    /// </summary>
    [Fact]
    public void SchedulePlan_BasicCreation_ShouldWork()
    {
        // Act
        var plan = new SchedulePlan("TestPlan", "6", () => { });
        
        // Assert
        Assert.Equal("TestPlan", plan.Name);
        Assert.True(plan.Valid);
        Assert.False(plan.IsRunning);
    }

    /// <summary>
    /// 測試 PlanWorkerOptions 的預設值設定
    /// 驗證延遲時間預設為 0 秒，檢查間隔預設為 30 秒
    /// </summary>
    [Fact]
    public void PlanWorkerOptions_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var options = new PlanWorkerOptions();
        
        // Assert
        Assert.Equal(0, options.Delay);
        Assert.Equal(30, options.Interval);
    }

    /// <summary>
    /// 測試 PlanTypes 列舉的數值正確性
    /// 驗證每個排程類型對應的整數值符合設計規格
    /// </summary>
    /// <param name="planType">要測試的排程類型</param>
    /// <param name="expectedValue">期望的整數值</param>
    [Theory]
    [InlineData(PlanTypes.None, 0)]
    [InlineData(PlanTypes.Once, 1)]
    [InlineData(PlanTypes.Day, 2)]
    [InlineData(PlanTypes.Week, 3)]
    [InlineData(PlanTypes.Month, 4)]
    [InlineData(PlanTypes.MonthWeek, 5)]
    [InlineData(PlanTypes.Startup, 6)]
    [InlineData(PlanTypes.Stoped, 7)]
    public void PlanTypes_ShouldHaveCorrectValues(PlanTypes planType, int expectedValue)
    {
        // Assert
        Assert.Equal(expectedValue, (int)planType);
    }

    /// <summary>
    /// 測試 WeekDays 旗標列舉的位元運算功能
    /// 驗證可以正確組合多個星期並檢查特定星期是否包含在組合中
    /// </summary>
    [Fact]
    public void WeekDays_FlagEnum_ShouldWork()
    {
        // Arrange
        var combined = WeekDays.Monday | WeekDays.Friday;
        
        // Assert
        Assert.True(combined.HasFlag(WeekDays.Monday));
        Assert.True(combined.HasFlag(WeekDays.Friday));
        Assert.False(combined.HasFlag(WeekDays.Tuesday));
    }

    /// <summary>
    /// 測試 KeyExistsException 自訂例外的基本功能
    /// 驗證例外能正確儲存鍵值和訊息，並繼承自標準 Exception 類別
    /// </summary>
    [Fact]
    public void KeyExistsException_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var exception = new KeyExistsException("TestKey", "Test message");
        
        // Assert
        Assert.Equal("TestKey", exception.Key);
        Assert.Equal("Test message", exception.Message);
        Assert.IsAssignableFrom<Exception>(exception);
    }

    /// <summary>
    /// 測試 ExceptionEventArgs 事件參數類別的基本功能
    /// 驗證能正確封裝例外物件並透過屬性存取
    /// </summary>
    [Fact]
    public void ExceptionEventArgs_ShouldWorkCorrectly()
    {
        // Arrange
        var innerException = new InvalidOperationException("Test exception");
        
        // Act
        var eventArgs = new ExceptionEventArgs(innerException);
        
        // Assert
        Assert.Equal(innerException, eventArgs.Exception);
    }
}