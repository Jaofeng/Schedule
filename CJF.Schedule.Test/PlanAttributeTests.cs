using CJF.Schedules;
using System.ComponentModel;
using System.Reflection;
using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試 PlanAttribute 類別的功能
/// </summary>
public class PlanAttributeTests
{
    /// <summary>
    /// 測試使用表示式字串的 PlanAttribute 建構函式
    /// 驗證能正確設定表示式並將排程類型設為 None
    /// </summary>
    [Fact]
    public void Constructor_WithExpression_ShouldSetExpression()
    {
        // Arrange
        var expression = "2 2023-01-01 02:00:00 5";
        
        // Act
        var attr = new PlanAttribute(expression);
        
        // Assert
        Assert.Equal(expression, attr.Expression);
        Assert.Equal(PlanTypes.None, attr.PlanType);
    }

    /// <summary>
    /// 測試使用 Startup 類型的 PlanAttribute 建構函式
    /// 驗證能正確設定排程類型為 Startup
    /// </summary>
    [Fact]
    public void Constructor_WithStartupType_ShouldSetPlanType()
    {
        // Act
        var attr = new PlanAttribute(PlanTypes.Startup);
        
        // Assert
        Assert.Equal(PlanTypes.Startup, attr.PlanType);
        Assert.Equal(string.Empty, attr.Expression);
    }

    /// <summary>
    /// 測試使用 Stoped 類型的 PlanAttribute 建構函式
    /// 驗證能正確設定排程類型為 Stoped
    /// </summary>
    [Fact]
    public void Constructor_WithStopedType_ShouldSetPlanType()
    {
        // Act
        var attr = new PlanAttribute(PlanTypes.Stoped);
        
        // Assert
        Assert.Equal(PlanTypes.Stoped, attr.PlanType);
        Assert.Equal(string.Empty, attr.Expression);
    }

    /// <summary>
    /// 測試使用無效類型的 PlanAttribute 建構函式會拋出例外
    /// 驗證當使用非 Startup/Stoped 類型時會拋出 ArgumentException
    /// </summary>
    /// <param name="invalidType">無效的排程類型</param>
    [Theory]
    [InlineData(PlanTypes.Once)]
    [InlineData(PlanTypes.Day)]
    [InlineData(PlanTypes.Week)]
    [InlineData(PlanTypes.Month)]
    [InlineData(PlanTypes.MonthWeek)]
    [InlineData(PlanTypes.None)]
    public void Constructor_WithInvalidTypeForStartupStoped_ShouldThrowException(PlanTypes invalidType)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new PlanAttribute(invalidType));
        
        Assert.Contains("必須為", exception.Message);
        Assert.Contains("Startup", exception.Message);
        Assert.Contains("Stoped", exception.Message);
    }

    /// <summary>
    /// 測試日排程的 PlanAttribute 建構函式
    /// 驗證能正確設定每日排程的相關屬性（時間、週期等）
    /// </summary>
    [Fact]
    public void Constructor_DaySchedule_ShouldSetCorrectProperties()
    {
        // Arrange
        var timeString = "14:30:00";
        var period = 5;
        
        // Act
        var attr = new PlanAttribute(timeString, period);
        
        // Assert
        Assert.Equal(PlanTypes.Day, attr.PlanType);
        Assert.Equal(period, attr.Period);
        Assert.Equal(new TimeOnly(14, 30, 0), TimeOnly.FromDateTime(attr.StartFrom));
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(attr.StartFrom));
    }

    /// <summary>
    /// 測試週排程的 PlanAttribute 建構函式
    /// 驗證能正確設定每週排程的相關屬性（時間、週期、星期幾等）
    /// </summary>
    [Fact]
    public void Constructor_WeekSchedule_ShouldSetCorrectProperties()
    {
        // Arrange
        var timeString = "09:15:30";
        var period = 2;
        var weekDay = WeekDays.Monday | WeekDays.Friday;
        
        // Act
        var attr = new PlanAttribute(timeString, period, weekDay);
        
        // Assert
        Assert.Equal(PlanTypes.Week, attr.PlanType);
        Assert.Equal(period, attr.Period);
        Assert.Equal(weekDay, attr.WeekDay);
        Assert.Equal(new TimeOnly(9, 15, 30), TimeOnly.FromDateTime(attr.StartFrom));
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(attr.StartFrom));
    }

    /// <summary>
    /// 測試使用無效時間字串的 PlanAttribute 建構函式會拋出例外
    /// 驗證當提供無法解析的時間格式時會拋出例外
    /// </summary>
    /// <param name="invalidTimeString">無效的時間字串</param>
    [Theory]
    [InlineData("invalid-time")]
    public void Constructor_WithInvalidTimeString_ShouldThrowException(string invalidTimeString)
    {
        // Act & Assert
        Assert.ThrowsAny<Exception>(() => new PlanAttribute(invalidTimeString, 1));
    }

    /// <summary>
    /// 測試使用超出範圍的時間值會拋出 OverflowException
    /// 驗證當分鐘或秒數超出有效範圍時會拋出特定類型的例外
    /// </summary>
    /// <param name="invalidTimeString">超出範圍的時間字串</param>
    [Theory]
    [InlineData("12:60:00")] // 分鐘超出範圍
    [InlineData("12:30:60")] // 秒鐘超出範圍
    public void Constructor_WithOutOfRangeTime_ShouldThrowOverflowException(string invalidTimeString)
    {
        // Act & Assert
        Assert.Throws<OverflowException>(() => new PlanAttribute(invalidTimeString, 1));
    }

    /// <summary>
    /// 測試 PlanAttribute 的 AttributeUsage 設定是否正確
    /// 驗證屬性的使用範圍、是否允許多重使用和繼承設定
    /// </summary>
    [Fact]
    public void AttributeUsage_ShouldBeCorrectlyConfigured()
    {
        // Act
        var attributeUsage = typeof(PlanAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .Cast<AttributeUsageAttribute>()
            .FirstOrDefault();
        
        // Assert
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);
        Assert.False(attributeUsage.Inherited);
    }
}