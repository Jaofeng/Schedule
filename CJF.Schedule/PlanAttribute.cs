
namespace CJF.Schedules;

/// <summary>定義方法綁定的排程自訂屬性類別。</summary>
/// <remarks>使用此自訂屬性有以下限制或預設值：
/// <list type="number">
/// <item>排程名稱將會使用繫結的方法名稱。</item>
/// <item>無設定終止執行排程的時間。</item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PlanAttribute : Attribute
{
    #region Public Properties
    /// <summary>排程週期的簡易表示式。</summary>
    public string Expression { get; private set; } = string.Empty;
    /// <summary>開始執行排程的時間。</summary>
    public DateTime StartFrom { get; private set; } = DateTime.Now;
    /// <summary>排程執行週期類型。</summary>
    public PlanTypes PlanType { get; private set; } = PlanTypes.None;
    /// <summary>排程執行週期，此值需參照 <see cref="PlanType"/>。當 <see cref="PlanType"/> 屬性值不為 <see cref="PlanTypes.Day"/> 與 <see cref="PlanTypes.Week"/> 時無效。</summary>
    public int Period { get; private set; } = 0;
    /// <summary>一週內設定的星期名稱。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.Week"/> 與 <see cref="PlanTypes.MonthWeek"/> 時無效。</summary>
    public WeekDays WeekDay { get; private set; } = WeekDays.None;
    /// <summary>排程訂定的月份。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.Month"/> 時無效。</summary>
    public Months Month { get; private set; } = Months.None;
    /// <summary>排程訂定的日期。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.Month"/> 時無效。</summary>
    public Days Day { get; private set; } = Days.None;
    /// <summary>排定的周別。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.MonthWeek"/> 時無效。</summary>
    public WeekNo WeekNo { get; private set; } = WeekNo.None;
    #endregion


    #region Public Constructor : PlanAttribute(string expression)
    /// <summary>以排程週期的簡易表示式建立新的 <see cref="PlanAttribute"/>。</summary>
    /// <param name="expression">排程週期的簡易表示式。</param>
    public PlanAttribute(string expression)
    {
        Expression = expression;
    }
    #endregion

    #region Public Constructor : PlanAttribute(PlanTypes type)
    /// <summary>建立新的排程週期 <see cref="PlanAttribute"/>，本建立式適用於 <see cref="PlanWorker"/> 開始執行(<see cref="PlanTypes.Startup"/>)與結束執行(<see cref="PlanTypes.Stoped"/>)的排程。</summary>
    /// <param name="type">排程類型。</param>
    /// <exception cref="ArgumentException">必須為 <see cref="PlanTypes.Startup"/> 或 <see cref="PlanTypes.Stoped"/>。</exception>
    public PlanAttribute(PlanTypes type)
    {
        if (type != PlanTypes.Startup && type != PlanTypes.Stoped)
            throw new ArgumentException($"必須為 {nameof(PlanTypes.Startup)} 或 {nameof(PlanTypes.Stoped)}。");
        PlanType = type;
    }
    #endregion

    #region Public Constructor : PlanAttribute(string timeString, int period)
    /// <summary>建立新的排程週期 <see cref="PlanAttribute"/>，本建立式適用於指定以日(<see cref="PlanTypes.Day"/>)為單位，週期執行的排程。</summary>
    /// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。
    /// <para>此參數為排程執行的時間。</para>
    /// </param>
    /// <param name="period">以日為單位的週期天數。</param>
    public PlanAttribute(string timeString, int period)
    {
        PlanType = PlanTypes.Day;
        StartFrom = DateTime.Today + TimeSpan.Parse(timeString);
        Period = period;
    }
    #endregion

    #region Public Constructor : PlanAttribute(string timeString, int period, WeekDays weekDay)
    /// <summary>建立新的排程週期 <see cref="PlanAttribute"/>，本建立式適用於指定以星期(<see cref="PlanTypes.Week"/>)為單位，週期執行的排程。</summary>
    /// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。此參數為排程執行的時間。</param>
    /// <param name="period">以星期為單位的週數。</param>
    /// <param name="weekDay">排程指定執行的星期名稱。</param>
    public PlanAttribute(string timeString, int period, WeekDays weekDay)
    {
        PlanType = PlanTypes.Week;
        StartFrom = DateTime.Today + TimeSpan.Parse(timeString);
        Period = period;
        WeekDay = weekDay;
    }
    #endregion

    #region Public Constructor : PlanAttribute(string timeString, Months month, Days day)
    /// <summary>建立新的排程週期 <see cref="PlanAttribute"/>，本建立式適用於指定以月(<see cref="PlanTypes.Month"/>)為單位，週期執行的排程。</summary>
    /// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。此參數為排程執行的時間。</param>
    /// <param name="month">排程指定執行的月份。</param>
    /// <param name="day">排程指定執行的日期。</param>
    public PlanAttribute(string timeString, Months month, Days day)
    {
        PlanType = PlanTypes.Month;
        StartFrom = DateTime.Today + TimeSpan.Parse(timeString);
        Month = month;
        Day = day;
    }
    #endregion

    #region Public Constructor : PlanAttribute(string timeString, Months month, WeekNo weekNo, WeekDays weekDay)
    /// <summary>建立新的排程週期 <see cref="PlanAttribute"/>，本建立式適用於指定以月週(<see cref="PlanTypes.MonthWeek"/>)為單位，週期執行的排程。</summary>
    /// <param name="timeString">以 HH:mm:ss 為格式的時間表示字串。此參數為排程執行的時間。</param>
    /// <param name="month">排程指定執行的月份。</param>
    /// <param name="weekNo">排程指定執行的周別。</param>
    /// <param name="weekDay">排程指定執行的星期名稱。</param>
    public PlanAttribute(string timeString, Months month, WeekNo weekNo, WeekDays weekDay)
    {
        PlanType = PlanTypes.MonthWeek;
        StartFrom = DateTime.Today + TimeSpan.Parse(timeString);
        Month = month;
        WeekNo = weekNo;
        WeekDay = weekDay;
    }
    #endregion

}
