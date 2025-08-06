using System.Diagnostics;
using CJF.Schedules;
using CJF.Schedules.Interfaces;
using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試 SchedulePlan 類別的功能
/// </summary>
public class SchedulePlanTests
{
    /// <summary>
    /// 測試 SchedulePlan 的基本建構函式功能
    /// 驗證使用名稱、表示式和動作參數能正確建立排程物件
    /// </summary>
    [Fact]
    public void Constructor_WithBasicParameters_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var name = "TestPlan";
        var expression = "6"; // Startup type
        Action action = () => { };

        // Act
        var plan = new SchedulePlan(name, expression, action);

        // Assert
        Assert.Equal(name, plan.Name);
        Assert.Equal(PlanTypes.Startup, plan.TimeTable.PlanType);
        Assert.True(plan.Valid);
        Assert.False(plan.IsRunning);
    }

    /// <summary>
    /// 測試 SchedulePlan 使用帶參數的動作建構函式
    /// 驗證使用 Action&lt;ISchedulePlan&gt; 類型的動作能正確建立排程
    /// </summary>
    [Fact]
    public void Constructor_WithActionT_ShouldWork()
    {
        // Arrange
        var name = "TestPlan";
        var expression = "6";
        ISchedulePlan? receivedPlan = null;
        Action<ISchedulePlan> action = (plan) => receivedPlan = plan;

        // Act
        var schedulePlan = new SchedulePlan(name, expression, action);

        // Assert
        Assert.Equal(name, schedulePlan.Name);
        Assert.Equal(PlanTypes.Startup, schedulePlan.TimeTable.PlanType);
        Assert.True(schedulePlan.Valid);
        Assert.False(schedulePlan.IsRunning);
    }

    /// <summary>
    /// 測試使用不同的表示式建立不同類型的排程
    /// 驗證排程表示式能正確解析為對應的排程類型
    /// </summary>
    /// <param name="expression">排程表示式</param>
    /// <param name="expectedType">期望的排程類型</param>
    [Theory]
    [InlineData("6", PlanTypes.Startup)]
    [InlineData("7", PlanTypes.Stoped)]
    [InlineData("1 2023-12-31 23:59:59", PlanTypes.Once)]
    [InlineData("2 2023-01-01 02:00:00 5", PlanTypes.Day)]
    public void Constructor_WithDifferentExpressions_ShouldCreateCorrectPlanTypes(
        string expression, PlanTypes expectedType)
    {
        // Act
        var plan = new SchedulePlan("TestPlan", expression, () => { });

        // Assert
        Assert.Equal(expectedType, plan.TimeTable.PlanType);
    }

    /// <summary>
    /// 測試 SchedulePlan 的 Valid 屬性可以被修改
    /// 驗證可以動態啟用或停用排程的有效狀態
    /// </summary>
    [Fact]
    public void ValidProperty_CanBeModified()
    {
        // Arrange
        var plan = new SchedulePlan("TestPlan", "6", () => { });

        // Assert initial state
        Assert.True(plan.Valid);

        // Act & Assert
        plan.Valid = false;
        Assert.False(plan.Valid);

        plan.Valid = true;
        Assert.True(plan.Valid);
    }

    /// <summary>
    /// 測試使用無效的排程表示式會拋出例外
    /// 驗證當提供無效或不完整的表示式時能正確拋出例外
    /// </summary>
    /// <param name="invalidExpression">無效的排程表示式</param>
    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("1")]
    [InlineData("2 invalid-date")]
    public void Constructor_WithInvalidExpression_ShouldThrow(string invalidExpression)
    {
        // Act & Assert
        Assert.ThrowsAny<Exception>(() => new SchedulePlan("TestPlan", invalidExpression, () => { }));
    }

    /// <summary>
    /// 測試 TimeTable 屬性提供正確的排程資訊
    /// 驗證時間表能正確解析並提供排程的詳細資訊
    /// </summary>
    [Fact]
    public void TimeTable_ShouldProvideCorrectInformation()
    {
        // Arrange
        var expression = "2 2023-01-01 02:00:00 5";
        var plan = new SchedulePlan("TestPlan", expression, () => { });

        // Act
        var timeTable = plan.TimeTable;
        var description = timeTable.GetDescription();
        var resultExpression = timeTable.GetExpression();

        // Assert
        Assert.Equal(PlanTypes.Day, timeTable.PlanType);
        Assert.Equal(new DateTime(2023, 1, 1, 2, 0, 0), timeTable.StartFrom);
        Assert.Equal(5, timeTable.Period);
        Assert.NotNull(description);
        Assert.NotEmpty(description);
        Assert.Contains("2023-01-01", description);
        Assert.Equal(expression, resultExpression);
    }

    /// <summary>
    /// 測試 TimeTable 的複製功能
    /// 驗證可以正確複製時間表物件，且複製物件具有相同的屬性值
    /// </summary>
    [Fact]
    public void TimeTable_Clone_ShouldCreateCopyWithSameProperties()
    {
        // Arrange
        var plan = new SchedulePlan("TestPlan", "2 2023-01-01 02:00:00 5", () => { });
        var original = plan.TimeTable;

        // Act
        var clone = original.Clone() as IPlanTime;

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(original, clone);
        Assert.Equal(original.PlanType, clone.PlanType);
        Assert.Equal(original.StartFrom, clone.StartFrom);
        Assert.Equal(original.Period, clone.Period);
    }


    /// <summary>
    /// 測試 TimeTable 屬性提供正確的排程資訊
    /// 驗證時間表能正確解析並提供排程的詳細資訊
    /// </summary>
    [Fact]
    public void Constructor_WithTimeTable()
    {
        // Arrange
        var expression = "2 2023-01-01 02:00:00 5";
        var time1 = new TimePlan(new DateTime(2023, 1, 1, 2, 0, 0), 5);
        var time2 = TimePlan.CreatePlan(expression);

        Assert.Equal(time1, time2);
        var plan = new SchedulePlan("TestPlan", time1, () => { });

        // Act
        var timeTable = plan.TimeTable;
        var description = timeTable.GetDescription();
        var resultExpression1 = time1.GetExpression();
        var resultExpression2 = time2.GetExpression();

        // Assert
        Assert.Equal(PlanTypes.Day, timeTable.PlanType);
        Assert.Equal(new DateTime(2023, 1, 1, 2, 0, 0), timeTable.StartFrom);
        Assert.Equal(5, timeTable.Period);
        Assert.NotNull(description);
        Assert.NotEmpty(description);
        Assert.Contains("2023-01-01", description);
        Assert.Equal(expression, resultExpression1);
    }


    /// <summary>
    /// 測試 TimePlan 的運算子重載功能
    /// 驗證等於和不等於運算子能正確比較兩個 TimePlan 物件
    /// </summary>
    [Fact]
    public void TimePlan_Operators_ShouldWorkCorrectly()
    {
        // Arrange
        // 以單次執行的時間計畫作為基礎
        var time1 = new TimePlan(new DateTime(2023, 1, 1, 2, 0, 0));
        var time2 = new TimePlan(new DateTime(2023, 1, 1, 2, 0, 0));
        var time3 = new TimePlan(new DateTime(2022, 1, 1, 2, 0, 0));
        var time4 = new TimePlan(new DateTime(2024, 1, 1, 2, 0, 0));

        Console.WriteLine($"time1 nexttime: {time1.NextTime}, time3 nexttime: {time3.NextTime}");

        // Act & Assert
        Assert.True(time1 == time2);
        Assert.False(time1 != time2);
        Assert.NotEqual(time1.GetHashCode(), time2.GetHashCode());

        Assert.False(time1 == time3);
        Assert.True(time1 != time3);
        Assert.False(time1 < time3);
        Assert.True(time1 > time3);
        Assert.False(time1 <= time3);
        Assert.True(time1 >= time3);

        Assert.False(time2 == time4);
        Assert.True(time2 != time4);
        Assert.False(time2 > time4);
        Assert.True(time2 < time4);
        Assert.False(time2 >= time4);
        Assert.True(time2 <= time4);
    }
}