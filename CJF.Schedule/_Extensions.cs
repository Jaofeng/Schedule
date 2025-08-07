using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CJF.Schedules;

/// <summary>擴展 <see cref="IHostBuilder"/> 以使用排程工作器服務。</summary>
public static class HostServiceExtensions
{
    #region Public Static Method : IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanerOptions> options)
    /// <summary>使用 <see cref="PlanWorker"/> 服務，並使用 <see cref="Action" />&lt;<see cref="PlanWorkerOptions"/>&gt; 進行設定。</summary>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    [Obsolete("建議改用 AddSchedulePlaner 方法。", false)]
    public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<PlanWorkerOptions>();
            services.AddSingleton<PlanWorker>();
            services.AddHostedService(provider => provider.GetRequiredService<PlanWorker>());
        });

        return builder;
    }
    #endregion

    #region Public Static Method : IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanerOptions> options)
    /// <summary>使用 <see cref="PlanWorker"/> 服務，並使用 <see cref="Action" />&lt;<see cref="PlanWorkerOptions"/>&gt; 進行設定。</summary>
    /// <param name="builder"><see cref="IHostBuilder"/> 執行個體。</param>
    /// <param name="options">進行設定的 <see cref="Action" />&lt;<see cref="PlanWorkerOptions"/>&gt; 執行函示。</param>
    [Obsolete("建議改用 AddSchedulePlaner 方法。", false)]
    public static IHostBuilder UseSchedulePlaner(this IHostBuilder builder, Action<PlanWorkerOptions> options)
    {
        builder.ConfigureServices((context, services) =>
        {
            var opts = new PlanWorkerOptions();
            options.Invoke(opts);
            services.AddSingleton(opts);
            services.AddSingleton<PlanWorker>();
            services.AddHostedService(provider => provider.GetRequiredService<PlanWorker>());
        });

        return builder;
    }
    #endregion

    #region Public Static Method : IServiceCollection AddSchedulePlaner(this IServiceCollection services)
    /// <summary>將排程工作器服務添加到 <see cref="IServiceCollection"/> 中。</summary>
    /// <param name="services"><see cref="IServiceCollection"/> 執行個體。</param>
    /// <returns>返回 <see cref="IServiceCollection"/> 執行個體，以便進行鏈式調用。</returns>
    /// <remarks>這個方法會添加 <see cref="PlanWorkerOptions"/> 和 <see cref="PlanWorker"/> 服務，並將 <see cref="PlanWorker"/> 註冊為一個託管服務。</remarks>
    public static IServiceCollection AddSchedulePlaner(this IServiceCollection services)
    {
        services.AddSingleton<PlanWorkerOptions>();
        services.AddSingleton<PlanWorker>();
        services.AddHostedService(provider => provider.GetRequiredService<PlanWorker>());
        return services;
    }
    #endregion

    #region Public Static Method : IServiceCollection AddSchedulePlaner(this IServiceCollection services, Action<PlanWorkerOptions> options)
    /// <summary>將排程工作器服務添加到 <see cref="IServiceCollection"/> 中，並使用指定的選項進行配置。</summary>
    /// <param name="services"><see cref="IServiceCollection"/> 執行個體。</param>
    /// <param name="options">一個 <see cref="Action"/>&lt;<see cref="PlanWorkerOptions"/>&gt; 委託，用於配置排程工作器的選項。</param>
    /// <returns>返回 <see cref="IServiceCollection"/> 執行個體，以便進行鏈式調用。</returns>
    /// <remarks>這個方法會創建一個新的 <see cref="PlanWorkerOptions"/> 實例，並將其添加到服務集合中。然後，它會添加 <see cref="PlanWorker"/> 服務，並將 <see cref="PlanWorker"/> 註冊為一個託管服務。</remarks>
    public static IServiceCollection AddSchedulePlaner(this IServiceCollection services, Action<PlanWorkerOptions> options)
    {
        var opts = new PlanWorkerOptions();
        options.Invoke(opts);
        services.AddSingleton(opts);
        services.AddSingleton<PlanWorker>();
        services.AddHostedService(provider => provider.GetRequiredService<PlanWorker>());
        return services;
    }
    #endregion
}

#region Internal Static Class : DataTimeExtensions
/// <summary>提供 <see cref="DateTime"/> 的擴充方法。</summary>
/// <remarks>這些方法用於日期和時間的計算和比較。</remarks>
/// <seealso cref="DateTime"/>
/// <seealso cref="CultureInfo"/>
internal static class DataTimeExtensions
{
    private static readonly CultureInfo culture = new("en-US");

    #region Public Static Method : int WeekOfMonth(this DateTime date)
    /// <summary>取得日期於該月份的第幾週。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns>返回該日期在當月的週數。</returns>
    public static int WeekOfMonth(this DateTime date)
    {
        Calendar cal = culture.Calendar;
        int dateWeek = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        DateTime monthFirstDay = new(date.Year, date.Month, 1);
        int firstDayWeek = cal.GetWeekOfYear(monthFirstDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        return dateWeek - firstDayWeek + 1;
    }
    #endregion

    #region Public Static Method : bool LastWeekOfMonth(this DateTime date)
    /// <summary>取得該日期是否為該月份的最後一週。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns>如果是最後一週則返回 <see langword="true"/>，否則返回 <see langword="false"/>。</returns>
    public static bool LastWeekOfMonth(this DateTime date)
    {
        Calendar cal = culture.Calendar;
        int dateWeek = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        DateTime monthLastDay = new(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        int lastDayWeek = cal.GetWeekOfYear(monthLastDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        return dateWeek == lastDayWeek;
    }
    #endregion

    #region Public Static Method : int WeekOfYear(this DateTime date)
    /// <summary>取得日期於該年度的第幾週。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns>返回該日期在當年的週數。</returns>
    public static int WeekOfYear(this DateTime date) => culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    #endregion

    #region Public Static Method : int WeeksOfMonth(this DateTime date)
    /// <summary>取得日期於該月份的週數。</summary>
    /// <param name="date">欲查詢的日期。</param>
    /// <returns>返回該月份的週數。</returns>
    /// <remarks>這個方法計算從該月的第一天到最後一天的週數。</remarks>
    public static int WeeksOfMonth(this DateTime date)
    {
        Calendar cal = culture.Calendar;
        DateTime monthFirstDay = new(date.Year, date.Month, 1);
        int firstDayWeek = cal.GetWeekOfYear(monthFirstDay, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        DateTime monthLastDay = new(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
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
/// <summary>提供列舉型別的擴充方法。</summary>
internal static class EnumExtensions
{
    #region Public Static Method : string GetDescription<T>(this T source) where T : Enum
    /// <summary>來自 <see cref="EnumExtensions"/> 的擴充函示，取得列舉型別值以 DescriptionAttribute 設定的說明文字。</summary>
    /// <typeparam name="T">列舉型別。</typeparam>
    /// <param name="source">欲取得屬性的列舉型別。</param>
    /// <returns>如果有設定說明文字則返回該文字，否則返回列舉值的名稱。</returns>
    /// <remarks>如果列舉型別是 Flags，則會返回所有設定的值的說明文字，並以逗號分隔。</remarks>
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
    /// <returns>如果有設定該屬性則返回該屬性的實體物件，否則返回 <see langword="null"/>。</returns>
    /// <remarks>如果列舉型別是 Flags，則會返回第一個符合條件的屬性。</remarks>
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
/// <summary>提供整數型別的擴充方法。</summary>
internal static class IntExtensions
{
    #region Public Static Method : string ToChinese(this int source)
    /// <summary>轉成中文數字表示字串。</summary>
    /// <param name="source">數字。</param>
    /// <returns>中文型態的數字。</returns>
    public static string ToChinese(this int source)
    {
        var str = Math.Abs(source).ToString();
        string[] num = ["零", "一", "二", "三", "四", "五", "六", "七", "八", "九"];
        string[] unit = ["", "十", "百", "千", "萬", "十", "百", "千", "億", "十", "百", "千", "兆", "十", "百", "千"];
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
        string[] num = ["零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖"];
        string[] unit = ["", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億", "拾", "佰", "仟", "兆", "拾", "佰", "仟"];
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
