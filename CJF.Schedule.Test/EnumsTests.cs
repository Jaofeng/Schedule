using System.ComponentModel;
using System.Reflection;
using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試各種列舉類型的功能
/// </summary>
public class EnumsTests
{
    /// <summary>
    /// 測試 PlanTypes 列舉的數值正確性
    /// 驗證每個排程類型對應的整數值符合設計規格
    /// </summary>
    [Fact]
    public void PlanTypes_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)PlanTypes.None);
        Assert.Equal(1, (int)PlanTypes.Once);
        Assert.Equal(2, (int)PlanTypes.Day);
        Assert.Equal(3, (int)PlanTypes.Week);
        Assert.Equal(4, (int)PlanTypes.Month);
        Assert.Equal(5, (int)PlanTypes.MonthWeek);
        Assert.Equal(6, (int)PlanTypes.Startup);
        Assert.Equal(7, (int)PlanTypes.Stoped);
    }

    /// <summary>
    /// 測試 WeekDays 旗標列舉的位元值正確性
    /// 驗證每個星期對應的位元值符合位元運算設計
    /// </summary>
    [Fact]
    public void WeekDays_ShouldHaveCorrectFlagValues()
    {
        // Assert
        Assert.Equal(0, (int)WeekDays.None);
        Assert.Equal(0x01, (int)WeekDays.Sunday);
        Assert.Equal(0x02, (int)WeekDays.Monday);
        Assert.Equal(0x04, (int)WeekDays.Tuesday);
        Assert.Equal(0x08, (int)WeekDays.Wednesday);
        Assert.Equal(0x10, (int)WeekDays.Thursday);
        Assert.Equal(0x20, (int)WeekDays.Friday);
        Assert.Equal(0x40, (int)WeekDays.Saturday);
        Assert.Equal(0x7F, (int)WeekDays.All);
    }

    /// <summary>
    /// 測試 WeekDays 旗標列舉的位元運算功能
    /// 驗證可以正確組合多個星期並檢查是否包含特定星期
    /// </summary>
    [Fact]
    public void WeekDays_ShouldSupportFlags()
    {
        // Arrange
        var combined = WeekDays.Monday | WeekDays.Friday;
        
        // Assert
        Assert.True(combined.HasFlag(WeekDays.Monday));
        Assert.True(combined.HasFlag(WeekDays.Friday));
        Assert.False(combined.HasFlag(WeekDays.Tuesday));
    }

    /// <summary>
    /// 測試 Months 旗標列舉的位元值正確性
    /// 驗證每個月份對應的位元值符合位元運算設計
    /// </summary>
    [Fact]
    public void Months_ShouldHaveCorrectFlagValues()
    {
        // Assert
        Assert.Equal(0, (int)Months.None);
        Assert.Equal(0x01, (int)Months.January);
        Assert.Equal(0x02, (int)Months.February);
        Assert.Equal(0x04, (int)Months.March);
        Assert.Equal(0x0800, (int)Months.December);
        Assert.Equal(0x0FFF, (int)Months.All);
    }

    /// <summary>
    /// 測試 Months 旗標列舉的位元運算功能
    /// 驗證可以正確組合多個月份並檢查是否包含特定月份
    /// </summary>
    [Fact]
    public void Months_ShouldSupportFlags()
    {
        // Arrange
        var combined = Months.January | Months.December;
        
        // Assert
        Assert.True(combined.HasFlag(Months.January));
        Assert.True(combined.HasFlag(Months.December));
        Assert.False(combined.HasFlag(Months.June));
    }

    /// <summary>
    /// 測試 WeekNo 旗標列舉的位元值正確性
    /// 驗證每個週別對應的位元值符合位元運算設計
    /// </summary>
    [Fact]
    public void WeekNo_ShouldHaveCorrectFlagValues()
    {
        // Assert
        Assert.Equal(0, (int)WeekNo.None);
        Assert.Equal(0x01, (int)WeekNo.First);
        Assert.Equal(0x02, (int)WeekNo.Second);
        Assert.Equal(0x04, (int)WeekNo.Third);
        Assert.Equal(0x08, (int)WeekNo.Fourth);
        Assert.Equal(0x10, (int)WeekNo.Fifth);
        Assert.Equal(0x20, (int)WeekNo.Sixth);
        Assert.Equal(0x80, (int)WeekNo.Last);
        Assert.Equal(0xBF, (int)WeekNo.All);
    }

    /// <summary>
    /// 測試 WeekNo 旗標列舉的位元運算功能
    /// 驗證可以正確組合多個週別並檢查是否包含特定週別
    /// </summary>
    [Fact]
    public void WeekNo_ShouldSupportFlags()
    {
        // Arrange
        var combined = WeekNo.First | WeekNo.Third;
        
        // Assert
        Assert.True(combined.HasFlag(WeekNo.First));
        Assert.True(combined.HasFlag(WeekNo.Third));
        Assert.False(combined.HasFlag(WeekNo.Second));
    }

    /// <summary>
    /// 測試 Days 旗標列舉的位元值正確性
    /// 驗證每日期對應的位元值符合 32 位元旗標設計
    /// </summary>
    [Fact]
    public void Days_ShouldHaveCorrectFlagValues()
    {
        // Assert
        Assert.Equal(0u, (uint)Days.None);
        Assert.Equal(0x00000001u, (uint)Days.Day1);
        Assert.Equal(0x00000002u, (uint)Days.Day2);
        Assert.Equal(0x00000004u, (uint)Days.Day3);
        Assert.Equal(0x40000000u, (uint)Days.Day31);
        Assert.Equal(0x7FFFFFFFu, (uint)Days.All);
    }

    /// <summary>
    /// 測試 Days 旗標列舉的位元運算功能
    /// 驗證可以正確組合多個日期並檢查是否包含特定日期
    /// </summary>
    [Fact]
    public void Days_ShouldSupportFlags()
    {
        // Arrange
        var combined = Days.Day1 | Days.Day15;
        
        // Assert
        Assert.True(combined.HasFlag(Days.Day1));
        Assert.True(combined.HasFlag(Days.Day15));
        Assert.False(combined.HasFlag(Days.Day31));
    }

    private static string GetDescription<T>(T enumValue) where T : Enum
    {
        var type = typeof(T);
        
        if (type.GetCustomAttribute(typeof(FlagsAttribute)) is not null && !Enum.IsDefined(type, enumValue))
        {
            var result = "";
            foreach (var fi in type.GetFields().Where(f => f.FieldType.Name == type.Name))
            {
                if (fi.GetValue(enumValue) is not T fieldValue) continue;
                if ((int)Convert.ChangeType(fieldValue, typeof(int)) != 0 && enumValue.HasFlag(fieldValue))
                {
                    if (fi.GetCustomAttribute(typeof(DescriptionAttribute), true) is DescriptionAttribute att)
                        result += att.Description + ", ";
                    else
                        result += $"{fieldValue}, ";
                }
            }
            return result.TrimEnd(' ', ',');
        }
        else
        {
            if (type.GetField(enumValue.ToString()) is not FieldInfo fi) return enumValue.ToString();
            if (fi.GetCustomAttribute(typeof(DescriptionAttribute), true) is not DescriptionAttribute att) return enumValue.ToString();
            return att.Description;
        }
    }

    /// <summary>
    /// 測試 WeekDays 列舉的中文說明文字
    /// 驗證每個星期值能正確顯示對應的中文說明
    /// </summary>
    /// <param name="weekDay">要測試的星期列舉值</param>
    /// <param name="expectedDescription">期望的中文說明</param>
    [Theory]
    [InlineData(WeekDays.Sunday, "周日")]
    [InlineData(WeekDays.Monday, "周一")]
    [InlineData(WeekDays.All, "每一天")]
    [InlineData(WeekDays.None, "未指定")]
    public void WeekDays_ShouldHaveCorrectDescriptions(WeekDays weekDay, string expectedDescription)
    {
        // Act
        var description = GetDescription(weekDay);
        
        // Assert
        Assert.Equal(expectedDescription, description);
    }

    /// <summary>
    /// 測試 Months 列舉的中文說明文字
    /// 驗證每個月份值能正確顯示對應的中文說明
    /// </summary>
    /// <param name="month">要測試的月份列舉值</param>
    /// <param name="expectedDescription">期望的中文說明</param>
    [Theory]
    [InlineData(Months.January, "一月")]
    [InlineData(Months.February, "二月")]
    [InlineData(Months.December, "十二月")]
    [InlineData(Months.All, "每個月")]
    [InlineData(Months.None, "未指定")]
    public void Months_ShouldHaveCorrectDescriptions(Months month, string expectedDescription)
    {
        // Act
        var description = GetDescription(month);
        
        // Assert
        Assert.Equal(expectedDescription, description);
    }

    /// <summary>
    /// 測試 WeekNo 列舉的中文說明文字
    /// 驗證每個週別值能正確顯示對應的中文說明
    /// </summary>
    /// <param name="weekNo">要測試的週別列舉值</param>
    /// <param name="expectedDescription">期望的中文說明</param>
    [Theory]
    [InlineData(WeekNo.First, "第一週")]
    [InlineData(WeekNo.Last, "最後一週")]
    [InlineData(WeekNo.All, "每一週")]
    [InlineData(WeekNo.None, "未指定")]
    public void WeekNo_ShouldHaveCorrectDescriptions(WeekNo weekNo, string expectedDescription)
    {
        // Act
        var description = GetDescription(weekNo);
        
        // Assert
        Assert.Equal(expectedDescription, description);
    }

    /// <summary>
    /// 測試 Days.All 的中文說明文字
    /// 驗證 Days.All 值能正確顯示「每一天」的中文說明
    /// </summary>
    [Fact]
    public void Days_All_ShouldHaveCorrectDescription()
    {
        // Act
        var description = GetDescription(Days.All);
        
        // Assert
        Assert.Equal("每一天", description);
    }
}