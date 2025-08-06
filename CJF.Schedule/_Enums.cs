using System.ComponentModel;

namespace CJF.Schedules;

#region Public Enum : PlanTypes
/// <summary>排程週期類型列舉</summary>
public enum PlanTypes : int
{
    /// <summary>無</summary>
    None = 0,
    /// <summary>執行一次</summary>
    Once = 1,
    /// <summary>每天</summary>
    Day = 2,
    /// <summary>每週</summary>
    Week = 3,
    /// <summary>每月</summary>
    Month = 4,
    /// <summary>每月週</summary>
    MonthWeek = 5,
    /// <summary>程式執行時</summary>
    Startup = 6,
    /// <summary>程式停止時</summary>
    Stoped = 7,
}
#endregion

#region Public Enum : WeekDays
/// <summary>週期類型為每週時，指定執行的星期。</summary>
[Flags]
public enum WeekDays
{
    /// <summary>無</summary>
    [Description("未指定")]
    None = 0,
    /// <summary>周日</summary>
    [Description("周日")]
    Sunday = 0x01,
    /// <summary>周一</summary>
    [Description("周一")]
    Monday = 0x02,
    /// <summary>周二</summary>
    [Description("周二")]
    Tuesday = 0x04,
    /// <summary>周三</summary>
    [Description("周三")]
    Wednesday = 0x08,
    /// <summary>周四</summary>
    [Description("周四")]
    Thursday = 0x10,
    /// <summary>周五</summary>
    [Description("周五")]
    Friday = 0x20,
    /// <summary>周六</summary>
    [Description("周六")]
    Saturday = 0x40,
    /// <summary>單週每一天</summary>
    [Description("每一天")]
    All = 0x7F
}
#endregion

#region Public Enum : Months
/// <summary>用來表示月份的列舉。</summary>
[Flags]
public enum Months
{
    /// <summary>無</summary>
    [Description("未指定")]
    None = 0,
    /// <summary>一月</summary>
    [Description("一月")]
    January = 0x01,
    /// <summary>二月</summary>
    [Description("二月")]
    February = 0x02,
    /// <summary>三月</summary>
    [Description("三月")]
    March = 0x04,
    /// <summary>四月</summary>
    [Description("四月")]
    April = 0x08,
    /// <summary>五月</summary>
    [Description("五月")]
    May = 0x10,
    /// <summary>六月</summary>
    [Description("六月")]
    June = 0x20,
    /// <summary>七月</summary>
    [Description("七月")]
    July = 0x40,
    /// <summary>八月</summary>
    [Description("八月")]
    August = 0x80,
    /// <summary>九月</summary>
    [Description("九月")]
    September = 0x0100,
    /// <summary>十月</summary>
    [Description("十月")]
    October = 0x0200,
    /// <summary>十一月</summary>
    [Description("十一月")]
    November = 0x0400,
    /// <summary>十二月</summary>
    [Description("十二月")]
    December = 0x0800,
    /// <summary>所有月份</summary>
    [Description("每個月")]
    All = 0x0FFF
}
#endregion

#region Public Enum : WeekNo
/// <summary>用來表示週別的列舉。</summary>
[Flags]
public enum WeekNo
{
    /// <summary>無</summary>
    [Description("未指定")]
    None = 0,
    /// <summary>第一週</summary>
    [Description("第一週")]
    First = 0x01,
    /// <summary>第二週</summary>
    [Description("第二週")]
    Second = 0x02,
    /// <summary>第三週</summary>
    [Description("第三週")]
    Third = 0x04,
    /// <summary>第四週</summary>
    [Description("第四週")]
    Fourth = 0x08,
    /// <summary>第五週</summary>
    [Description("第五週")]
    Fifth = 0x10,
    /// <summary>第六週</summary>
    [Description("第六週")]
    Sixth = 0x20,
    /// <summary>最後一週</summary>
    [Description("最後一週")]
    Last = 0x80,
    /// <summary>每一週</summary>
    [Description("每一週")]
    All = 0xBF
}
#endregion

#region Public Enum : Days
/// <summary>用來表示每月的第幾天的列舉。</summary>
[Flags]
public enum Days : uint
{
    [Description("未指定")]
    None = 0,
    Day1 = 0x00000001,
    Day2 = 0x00000002,
    Day3 = 0x00000004,
    Day4 = 0x00000008,
    Day5 = 0x00000010,
    Day6 = 0x00000020,
    Day7 = 0x00000040,
    Day8 = 0x00000080,
    Day9 = 0x00000100,
    Day10 = 0x00000200,
    Day11 = 0x00000400,
    Day12 = 0x00000800,
    Day13 = 0x00001000,
    Day14 = 0x00002000,
    Day15 = 0x00004000,
    Day16 = 0x00008000,
    Day17 = 0x00010000,
    Day18 = 0x00020000,
    Day19 = 0x00040000,
    Day20 = 0x00080000,
    Day21 = 0x00100000,
    Day22 = 0x00200000,
    Day23 = 0x00400000,
    Day24 = 0x00800000,
    Day25 = 0x01000000,
    Day26 = 0x02000000,
    Day27 = 0x04000000,
    Day28 = 0x08000000,
    Day29 = 0x10000000,
    Day30 = 0x20000000,
    Day31 = 0x40000000,
    [Description("每一天")]
    All = 0x7FFFFFFF
}
#endregion
