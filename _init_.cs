using CJF.Schedules.Interfaces;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

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


#region Public Delegates
public delegate void ExceptionEventHandler(ISchedulePlan item, ExceptionEventArgs e);
public delegate void SchedulePlanEventHandler(ISchedulePlan item);
public delegate void PlanWorkerEventHandler(IPlanWorker worker);
#endregion


#region Public Class : KeyExistsException
public class KeyExistsException : Exception
{
    public string Key { get; private set; }
    public KeyExistsException(string key, string msg) : base(msg)
    {
        Key = key;
    }
}
#endregion

#region Public Class : ExceptionEventArgs
public class ExceptionEventArgs : EventArgs
{
    public Exception Exception { get; private set; }
    public ExceptionEventArgs(Exception ex) : base()
    {
        Exception = ex;
    }
}
#endregion


#region Internal Static Class : DataTimeExtensions
internal static class DataTimeExtensions
{
    private static readonly CultureInfo culture = new("en-US");

    #region Public Static Method : int WeekOfMonth(this DateTime date)
    /// <summary>取得日期於該月份的第幾週。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns></returns>
    public static int WeekOfMonth(this DateTime date)
    {
        Calendar cal = culture.Calendar;
        int dateWeek = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        DateTime monthFirstDay = new DateTime(date.Year, date.Month, 1);
        int firstDayWeek = cal.GetWeekOfYear(monthFirstDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        return dateWeek - firstDayWeek + 1;
    }
    #endregion

    #region Public Static Method : bool LastWeekOfMonth(this DateTime date)
    /// <summary>取得該日期是否為該月份的最後一週。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns></returns>
    public static bool LastWeekOfMonth(this DateTime date)
    {
        Calendar cal = culture.Calendar;
        int dateWeek = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        DateTime monthLastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        int lastDayWeek = cal.GetWeekOfYear(monthLastDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        return dateWeek == lastDayWeek;
    }
    #endregion

    #region Public Static Method : int WeekOfYear(this DateTime date)
    /// <summary>取得日期於該年度的第幾週。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns></returns>
    public static int WeekOfYear(this DateTime date) => culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    #endregion

    #region Public Static Method : int WeeksOfMonth(this DateTime date)
    /// <summary>取得日期於該月份的週數。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns></returns>
    public static int WeeksOfMonth(this DateTime date)
    {
        Calendar cal = culture.Calendar;
        DateTime monthFirstDay = new DateTime(date.Year, date.Month, 1);
        int firstDayWeek = cal.GetWeekOfYear(monthFirstDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        DateTime monthLastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        int lastDayWeek = cal.GetWeekOfYear(monthLastDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        return lastDayWeek - firstDayWeek + 1;
    }
    #endregion

    #region Public Static Method : int IsSameWeek(this DateTime date)
    /// <summary>檢查是否為同一週的日期。</summary>
    /// <param name="date">檢查的日期。</param>
    /// <param name="target">被檢查的日期。</param>
    /// <returns>同一週為 <see langword="true"/>，否則為 <see langword="false"/>。</returns>
    public static bool IsSameWeek(this DateTime date, DateTime target) => date.Year == target.Year && date.WeekOfYear() == target.WeekOfYear();
    #endregion

    #region Public Static Method : bool IsSameMonth(this DateTime date, DateTime target)
    /// <summary>檢查是否為同一個月的日期。</summary>
    /// <param name="date">檢查的日期。</param>
    /// <param name="target">被檢查的日期。</param>
    /// <returns>同一個月為 <see langword="true"/>，否則為 <see langword="false"/>。</returns>
    public static bool IsSameMonth(this DateTime date, DateTime target) => date.Year == target.Year && date.Month == target.Month;
    #endregion

    #region Public Static Method : bool IsSameYear(this DateTime date, DateTime target)
    /// <summary>檢查是否為同一年的日期。</summary>
    /// <param name="date">檢查的日期。</param>
    /// <param name="target">被檢查的日期。</param>
    /// <returns>同一年為 <see langword="true"/>，否則為 <see langword="false"/>。</returns>
    public static bool IsSameYear(this DateTime date, DateTime target) => date.Year == target.Year;
    #endregion

    #region Public Static Method : DateTime AddWeeks(this DateTime date, int weeks)
    /// <summary>回傳一個 <see cref="DateTime"/> 結構，用來表示 <paramref name="date"/> 日期加上 <paramref name="weeks"/> 週數的日期。</summary>
    /// <param name="date">原始日期。</param>
    /// <param name="weeks">增減的週數。</param>
    /// <returns><paramref name="date"/> 日期加上 <paramref name="weeks"/> 週數的日期。</returns>
    public static DateTime AddWeeks(this DateTime date, int weeks) => date.AddDays(weeks * 7);
    #endregion
}
#endregion

#region Internal Static Class : EnumExtensions
internal static class EnumExtensions
{
    #region Public Static Method : string GetDescription<T>(this T source) where T : Enum
    /// <summary>來自 <see cref="EnumExtensions"/> 的擴充函示，取得列舉型別值以 DescriptionAttribute 設定的說明文字。</summary>
    /// <typeparam name="T">列舉型別。</typeparam>
    /// <param name="source">欲取得屬性的列舉型別。</param>
    /// <returns></returns>
    public static string GetDescription<T>(this T source) where T : Enum
    {
        Type tt = typeof(T);
        if (tt.GetCustomAttribute(typeof(FlagsAttribute)) is not null && !Enum.IsDefined(tt, source))
        {
            // Flag 型列舉
            string res = "";
            foreach (FieldInfo fi in tt.GetFields().Where(_fi => _fi.FieldType.Name == tt.Name))
            {
                if (fi.GetValue(source) is not T fo) continue;
                if ((int)Convert.ChangeType(fo, typeof(int)) != 0 && source.HasFlag(fo))
                {
                    if (fi.GetCustomAttribute(typeof(DescriptionAttribute), true) is Attribute att)
                        res += ((DescriptionAttribute)att).Description + ", ";
                    else
                        res += $"{fo}, ";
                }
            }
            return res.TrimEnd(' ', ',');
        }
        else
        {
            if (tt.GetField(source.ToString()) is not FieldInfo fi) return source.ToString();
            if (fi.GetCustomAttribute(typeof(DescriptionAttribute), true) is not Attribute att) return source.ToString();
            return ((DescriptionAttribute)att).Description;
        }
    }
    #endregion

    #region Public Static Method : Attribute? GetCustAttr<T>(this T source, Type attrType) where T : Enum
    /// <summary>來自 <see cref="EnumExtensions"/> 的擴充函示，取得列舉型別的特定屬性實體物件。</summary>
    /// <typeparam name="T">列舉型別。</typeparam>
    /// <param name="source">欲取得屬性的列舉型別。</param>
    /// <param name="attrType">屬性型別。</param>
    /// <returns></returns>
    public static Attribute? GetCustAttr<T>(this T source, Type attrType) where T : Enum
    {
        //object[] os = typeof(T).GetField(source.ToString()).GetCustomAttributes(attrType, true);
        if (typeof(T).GetField(source.ToString()) is FieldInfo fi)
        {
            object[] os = fi.GetCustomAttributes(attrType, true);
            if (os.Length == 0) return null;
            else return (Attribute)os[0];
        }
        else
            return null;
    }
    #endregion
}
#endregion

#region Internal Static Class : IntExtensions
internal static class IntExtensions
{
    #region Public Static Method : string ToChinese(this int source)
    /// <summary>轉成中文數字表示字串。</summary>
    /// <param name="source">數字。</param>
    /// <returns>中文數字。</returns>
    public static string ToChinese(this int source)
    {
        var str = Math.Abs(source).ToString();
        string[] num = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        string[] unit = new string[] { "", "十", "百", "千", "萬", "十", "百", "千", "億", "十", "百", "千", "兆", "十", "百", "千" };
        string res = "";
        for (int i = str.Length - 1; i >= 0; i--)
        {
            int n = int.Parse(str[i].ToString());
            res = $"{num[n]}{unit[str.Length - i - 1]}{res}";
        }
        if (source < 0)
            res = $"負{res}";
        if (res.Length > 1 && res[0] == '一' && res[1] == '十')
            res = res[1..]; // 去掉「一十」
        res = res.TrimEnd('零');

        return res;
    }
    #endregion

    #region Public Static Method : string ToChinese(this int source, bool isMoney)
    /// <summary>轉成中文數字表示字串。</summary>
    /// <param name="source">數字。</param>
    /// <param name="isMoney">是否輸出金錢的格式。</param>
    /// <returns>中文金錢數字。</returns>
    public static string ToChinese(this int source, bool isMoney)
    {
        if (!isMoney) return source.ToChinese();
        var str = Math.Abs(source).ToString();
        string[] num = new string[] { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
        string[] unit = new string[] { "", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
        string res = "";
        for (int i = str.Length - 1; i >= 0; i--)
        {
            int n = int.Parse(str[i].ToString());
            res = $"{num[n]}{unit[str.Length - i - 1]}{res}";
        }
        if (source < 0)
            res = $"負{res}";
        return res.TrimEnd('零');
    }
    #endregion
}
#endregion
