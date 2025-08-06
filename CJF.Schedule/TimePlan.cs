using CJF.Schedules.Interfaces;

namespace CJF.Schedules;

public sealed class TimePlan : IPlanTime
{
    #region Public Properties
    public DateTime StartFrom { get; set; } = default;
    public DateTime StopEnd { get; set; } = default;
    public PlanTypes PlanType { get; set; } = PlanTypes.None;
    public int Period { get; set; } = 0;
    public WeekDays WeekDay { get; set; } = WeekDays.None;
    public Months Month { get; set; } = Months.None;
    public Days Day { get; set; } = Days.None;
    public WeekNo WeekNo { get; set; } = WeekNo.None;
    public DateTime LastExecuted { get; private set; } = default;
    public DateTime NextTime { get; private set; } = default;
    #endregion


    #region Public Constructor : TimePlan(PlanTypes type)
    /// <summary>建立新的排程週期 <see cref="TimePlan"/>，本方法適用於 <see cref="PlanWorker"/> 開始執行與結束執行的排程。</summary>
    /// <param name="type">排程類型。</param>
    /// <exception cref="ArgumentException">必須為 <see cref="PlanTypes.Startup"/> 或 <see cref="PlanTypes.Stoped"/>。</exception>
    public TimePlan(PlanTypes type)
    {
        if (type != PlanTypes.Startup && type != PlanTypes.Stoped)
            throw new ArgumentException($"必須為 {nameof(PlanTypes.Startup)} 或 {nameof(PlanTypes.Stoped)}。");
        PlanType = type;
    }
    #endregion

    #region Public Constructor : TimePlan(DateTime from)
    /// <summary>建立新的排程週期 <see cref="TimePlan"/>，本方法適用於指定時間執行一次的排程。</summary>
    /// <param name="from">指定執行的時間。</param>
    public TimePlan(DateTime from)
    {
        PlanType = PlanTypes.Once;
        StartFrom = from;
        NextTime = from;
    }
    #endregion

    #region Public Constructor : TimePlan(DateTime from, int period)
    /// <summary>建立新的排程週期 <see cref="TimePlan"/>，本方法適用於指定以日為單位，週期執行的排程。</summary>
    /// <param name="from">指定開始執行的時間。</param>
    /// <param name="period">以日為單位的週期天數。</param>
    public TimePlan(DateTime from, int period)
    {
        PlanType = PlanTypes.Day;
        StartFrom = from;
        Period = period;
        NextTime = GetNextTime(from);
    }
    #endregion

    #region Public Constructor : TimePlan(DateTime from, int period, WeekDays weekDay)
    /// <summary>建立新的排程週期 <see cref="TimePlan"/>，本方法適用於指定以星期為單位，週期執行的排程。</summary>
    /// <param name="from">指定開始執行的時間。</param>
    /// <param name="period">以星期為單位的週數。</param>
    /// <param name="weekDay">排程指定執行的星期名稱。</param>
    public TimePlan(DateTime from, int period, WeekDays weekDay)
    {
        PlanType = PlanTypes.Week;
        StartFrom = from;
        Period = period;
        WeekDay = weekDay;
        NextTime = GetNextTime(from);
    }
    #endregion

    #region Public Constructor : TimePlan(DateTime from, Months month, Days day)
    /// <summary>建立新的排程週期 <see cref="TimePlan"/>，本方法適用於指定以月為單位，週期執行的排程。</summary>
    /// <param name="from">指定開始執行的時間。</param>
    /// <param name="month">排程指定執行的月份。</param>
    /// <param name="day">排程指定執行的日期。</param>
    public TimePlan(DateTime from, Months month, Days day)
    {
        PlanType = PlanTypes.Month;
        StartFrom = from;
        Month = month;
        Day = day;
        NextTime = GetNextTime(from);
    }
    #endregion

    #region Public Constructor : TimePlan(DateTime from, Months month, WeekNo weekNo, WeekDays weekDay)
    /// <summary>建立新的排程週期 <see cref="TimePlan"/>，本方法適用於指定以月週為單位，週期執行的排程。</summary>
    /// <param name="from">指定開始執行的時間。</param>
    /// <param name="month">排程指定執行的月份。</param>
    /// <param name="weekNo">排程指定執行的周別。</param>
    /// <param name="weekDay">排程指定執行的星期名稱。</param>
    public TimePlan(DateTime from, Months month, WeekNo weekNo, WeekDays weekDay)
    {
        PlanType = PlanTypes.MonthWeek;
        StartFrom = from;
        Month = month;
        WeekNo = weekNo;
        WeekDay = weekDay;
        NextTime = GetNextTime(from);
    }
    #endregion


    #region Public Static Method : ITimePlan CreatePlan(string expression)
    /// <summary>以簡易表示式建立排程週期。</summary>
    /// <param name="expression">簡易表示式。</param>
    /// <returns>建立完成的 <see cref="IPlanTime"/> 執行個體。</returns>
    /// <exception cref="ArgumentException">排程表達式格式錯誤。</exception>
    /// <exception cref="FormatException">排程表達式內容欄位格式錯誤。</exception>
    public static IPlanTime CreatePlan(string expression)
    {
        var fds = expression.Split(' ');
        string[] arr;
        var w = WeekDays.None;
        var m = Months.None;
        var d = Days.None;
        var wn = WeekNo.None;
        try
        {
            var pt = (PlanTypes)Convert.ToInt32(fds[0]);
            TimePlan tp;
            switch (pt)
            {
                case PlanTypes.Startup:
                case PlanTypes.Stoped:
                    tp = new TimePlan(pt);
                    break;
                case PlanTypes.Once:
                    if (fds.Length < 3)
                        throw new ArgumentException($"排程表達式 \"{expression}\" 格式錯誤。");
                    tp = new TimePlan(Convert.ToDateTime($"{fds[1]} {fds[2]}"));
                    break;
                case PlanTypes.Day:
                    if (fds.Length < 4)
                        throw new ArgumentException($"排程表達式 \"{expression}\" 格式錯誤。");
                    tp = new TimePlan(Convert.ToDateTime($"{fds[1]} {fds[2]}"), Convert.ToInt32(fds[3]));
                    if (fds.Length >= 6)
                        tp.StopEnd = Convert.ToDateTime($"{fds[4]} {fds[5]}");
                    break;
                case PlanTypes.Week:
                    if (fds.Length < 5)
                        throw new ArgumentException($"排程表達式 \"{expression}\" 格式錯誤。");
                    if (fds[4] == "A")
                        w = WeekDays.All;
                    else
                    {
                        arr = fds[4].Split("/");
                        foreach (var s in arr)
                            w |= (WeekDays)(1 << Convert.ToInt32(s));
                    }
                    tp = new TimePlan(Convert.ToDateTime($"{fds[1]} {fds[2]}"), Convert.ToInt32(fds[3]), w);
                    if (fds.Length >= 7)
                        tp.StopEnd = Convert.ToDateTime($"{fds[5]} {fds[6]}");
                    break;
                case PlanTypes.Month:
                    if (fds.Length < 5)
                        throw new ArgumentException($"排程表達式 \"{expression}\" 格式錯誤。");
                    if (fds[3] == "A")
                        m = Months.All;
                    else
                    {
                        arr = fds[3].Split("/");
                        foreach (var s in arr)
                            m |= (Months)(1 << (Convert.ToInt32(s) - 1));
                    }
                    if (fds[4] == "A")
                        d = Days.All;
                    else
                    {
                        arr = fds[4].Split("/");
                        foreach (var s in arr)
                            d |= (Days)(1 << (Convert.ToInt32(s) - 1));
                    }
                    tp = new TimePlan(Convert.ToDateTime($"{fds[1]} {fds[2]}"), m, d);
                    if (fds.Length >= 7)
                        tp.StopEnd = Convert.ToDateTime($"{fds[5]} {fds[6]}");
                    break;
                case PlanTypes.MonthWeek:
                    if (fds.Length < 6)
                        throw new ArgumentException($"排程表達式 \"{expression}\" 格式錯誤。");
                    if (fds[3] == "A")
                        m = Months.All;
                    else
                    {
                        arr = fds[3].Split("/");
                        foreach (var s in arr)
                            m |= (Months)(1 << (Convert.ToInt32(s) - 1));
                    }
                    if (fds[4] == "A")
                        wn = WeekNo.All;
                    else if (fds[4] == "L")
                        wn = WeekNo.Last;
                    else
                    {
                        arr = fds[4].Split("/");
                        foreach (var s in arr)
                            wn |= (WeekNo)(1 << (Convert.ToInt32(s) - 1));
                    }
                    if (fds[5] == "A")
                        w = WeekDays.All;
                    else
                    {
                        arr = fds[5].Split("/");
                        foreach (var s in arr)
                            w |= (WeekDays)(1 << Convert.ToInt32(s));
                    }
                    tp = new TimePlan(Convert.ToDateTime($"{fds[1]} {fds[2]}"), m, wn, w);
                    if (fds.Length >= 8)
                        tp.StopEnd = Convert.ToDateTime($"{fds[6]} {fds[7]}");
                    break;
                default:
                    tp = new TimePlan(PlanTypes.None);
                    break;
            }
            return tp;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new FormatException($"排程表達式內容欄位格式錯誤 \"{expression}\"。", ex);
        }
    }
    #endregion

    #region Public Static Method : ITimePlan CreatePlan(SchedulePlanAttribute attr)
    public static IPlanTime CreatePlan(PlanAttribute attr)
    {
        var pt = attr.PlanType;
        TimePlan tp = pt switch
        {
            PlanTypes.Startup or PlanTypes.Stoped => new TimePlan(pt),
            //PlanTypes.Once => new TimePlan(attr.StartFrom),   // 無意義, 2023/05/24 Marked
            PlanTypes.Day => new TimePlan(attr.StartFrom, attr.Period),
            PlanTypes.Week => new TimePlan(attr.StartFrom, attr.Period, attr.WeekDay),
            PlanTypes.Month => new TimePlan(attr.StartFrom, attr.Month, attr.Day),
            PlanTypes.MonthWeek => new TimePlan(attr.StartFrom, attr.Month, attr.WeekNo, attr.WeekDay),
            _ => new TimePlan(PlanTypes.None),
        };
        return tp;
    }
    #endregion

    #region ICloneable Support
    public IPlanTime? Clone()
    {
        TimePlan t;
        switch (PlanType)
        {
            case PlanTypes.Startup:
            case PlanTypes.Stoped: t = new TimePlan(PlanType); break;
            case PlanTypes.Once: t = new TimePlan(StartFrom) { }; break;
            case PlanTypes.Day: t = new TimePlan(StartFrom, Period); break;
            case PlanTypes.Week: t = new TimePlan(StartFrom, Period, WeekDay); break;
            case PlanTypes.Month: t = new TimePlan(StartFrom, Month, Day); break;
            case PlanTypes.MonthWeek: t = new TimePlan(StartFrom, Month, WeekNo, WeekDay); break;
            default: return new TimePlan(PlanTypes.None);
        }
        t.StopEnd = StopEnd;
        t.LastExecuted = LastExecuted;
        t.NextTime = NextTime;
        return t;
    }
    object ICloneable.Clone() => Clone()!;
    #endregion


    #region Public Method : bool OnTime(DateTime time)
    public bool OnTime(DateTime time)
    {
        if (StopEnd != default && StopEnd <= time) return false;
        if (NextTime == default) return false;
        double _tm = time.Subtract(NextTime).TotalMinutes;
        return LastExecuted < NextTime && _tm >= 0 && _tm <= 1;
    }
    #endregion

    #region Public Method : string GetDescription()
    public string GetDescription()
    {
        var res = PlanType switch
        {
            PlanTypes.Startup => $"程式執行時",
            PlanTypes.Stoped => $"關閉程式時",
            PlanTypes.Once => $"於 {StartFrom:yyyy-MM-dd HH:mm:ss} 時執行一次",
            PlanTypes.Day => $"從 {StartFrom:yyyy-MM-dd} 起每 {Period} 天的 {StartFrom:HH:mm:ss} 執行",
            PlanTypes.Week => $"從 {StartFrom:yyyy-MM-dd} 起每 {Period.ToChinese()} 週逢 {WeekDay.GetDescription()} 的 {StartFrom:HH:mm:ss} 時執行",
            PlanTypes.Month => $"從 {StartFrom:yyyy-MM-dd} 起逢 {Month.GetDescription()} 的 {GetDaysString(Day)} 號的 {StartFrom:HH:mm:ss} 執行",
            PlanTypes.MonthWeek => $"從 {StartFrom:yyyy-MM-dd} 起逢 {Month.GetDescription()} 的 {WeekNo.GetDescription()} {WeekDay.GetDescription()} 的 {StartFrom:HH:mm:ss} 執行",
            _ => string.Empty,
        };
        if (StopEnd != default)
            res += $"，直到 {StopEnd:yyyy-MM-dd HH:mm:ss} 為止";
        return res.Trim('，', ' ');
    }
    #endregion

    #region Public Method : string GetDescription(ConsoleColor fore, ConsoleColor? back = null)
    public string GetDescription(ConsoleColor fore, ConsoleColor? back = null)
    {
        string c = $"{ConsoleColorToAnsiForeColor(fore)}{(back is not null ? $";{ConsoleColorToAnsiBackColor(back.Value)}" : "")}m";
        string e = $"\x1B[39{(back is not null ? ";49" : "")}m";
        var res = PlanType switch
        {
            PlanTypes.Startup => $"程式執行時",
            PlanTypes.Stoped => $"關閉程式時",
            PlanTypes.Once => $"於 \x1B[{c}{StartFrom:yyyy-MM-dd HH:mm:ss}\x1B[{e} 時執行一次",
            PlanTypes.Day => $"從 \x1B[{c}{StartFrom:yyyy-MM-dd}\x1B[{e} 起每 \x1B[{c}{Period}\x1B[{e} 天的 \x1B[{c}{StartFrom:HH:mm:ss}\x1B[{e} 執行",
            PlanTypes.Week => $"從 \x1B[{c}{StartFrom:yyyy-MM-dd}\x1B[{e} 起每 \x1B[{c}{Period.ToChinese()}\x1B[{e} 週逢 \x1B[{c}{WeekDay.GetDescription()}\x1B[{e} 的 \x1B[{c}{StartFrom:HH:mm:ss}\x1B[{e} 時執行",
            PlanTypes.Month => $"從 \x1B[{c}{StartFrom:yyyy-MM-dd}\x1B[{e} 起逢 \x1B[{c}{Month.GetDescription()}\x1B[{e} 的 \x1B[{c}{GetDaysString(Day)}\x1B[{e} 號的 \x1B[{c}{StartFrom:HH:mm:ss}\x1B[{e} 執行",
            PlanTypes.MonthWeek => $"從 \x1B[{c}{StartFrom:yyyy-MM-dd}\x1B[{e} 起逢 \x1B[{c}{Month.GetDescription()}\x1B[{e} 的 \x1B[{c}{WeekNo.GetDescription()}\x1B[{e} \x1B[{c}{WeekDay.GetDescription()}\x1B[{e} 的 \x1B[{c}{StartFrom:HH:mm:ss}\x1B[{e} 執行",
            _ => string.Empty,
        };
        if (StopEnd != default)
            res += $"，直到 \x1B[{c}{StopEnd:yyyy-MM-dd HH:mm:ss}\x1B[{e} 為止";
        return res.Trim('，', ' ');
    }
    #endregion

    #region Public Method : string GetExpression()
    public string GetExpression()
    {
        var res = $"{(int)PlanType}";
        switch (PlanType)
        {
            // "1 2023-12-31 02:00:00"  於 2023-12-31 02:00:00 時執行一次
            case PlanTypes.Once: res += $" {StartFrom:yyyy-MM-dd HH:mm:ss}"; break;
            // "2 2023-01-01 02:00:00 1" 從 2023-01-01 起每 1 天的 02:00:00 執行
            // "2 2023-01-01 02:00:00 10" 從 2023-01-01 起每 10 天的 02:00:00 執行
            case PlanTypes.Day: res += $" {StartFrom:yyyy-MM-dd HH:mm:ss} {Period}"; break;
            // "3 2023-01-01 02:00:00 1 1"      從 2023-01-01 起每週一的 02:00:00 執行
            // "3 2023-01-01 02:00:00 1 1/3/5"  從 2023-01-01 起每週一/三/五的 02:00:00 執行
            case PlanTypes.Week:
                res += $" {StartFrom:yyyy-MM-dd HH:mm:ss} {Period} ";
                if (WeekDay == WeekDays.All)
                    res += "A ";
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (WeekDay.HasFlag((WeekDays)(1 << i)))
                            res += $"{i}/";
                    }
                }
                res = res.TrimEnd('/', ' ');
                break;
            // "4 2023-01-01 02:00:00 A 1/15"           從 2023-01-01 起每月 1/15 號的 02:00:00 執行
            // "4 2023-01-01 02:00:00 2/4/6/8/10/12 1"  從 2023-01-01 起每月 2/4/6/8/10/12 號的 02:00:00 執行
            case PlanTypes.Month:
                res += $" {StartFrom:yyyy-MM-dd HH:mm:ss} ";
                if (Month == Months.All)
                    res += "A ";
                else
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (Month.HasFlag((Months)(1 << i)))
                            res += $"{i + 1}/";
                    }
                    res = res.TrimEnd('/', ' ') + ' ';
                }
                if (Day == Days.All)
                    res += "A ";
                else
                {
                    for (int i = 1; i <= 31; i++)
                    {
                        if (Day.HasFlag((Days)(1 << (i - 1))))
                            res += $"{i}/";
                    }
                    res = res.TrimEnd('/', ' ');
                }
                break;
            // "5 2023-01-01 02:00:00 A 2 6"                從 2023-01-01 起每月第二週的週六的 02:00:00 執行
            // "5 2023-01-01 02:00:00 2/4/6/8/10/12 1 1"    從 2023-01-01 起每月第二週的週一的 02:00:00 執行
            case PlanTypes.MonthWeek:
                res += $" {StartFrom:yyyy-MM-dd HH:mm:ss} ";
                if (Month == Months.All)
                    res += "A ";
                else
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (Month.HasFlag((Months)(1 << i)))
                            res += $"{i + 1}/";
                    }
                    res = res.TrimEnd('/', ' ') + ' ';
                }
                if (WeekNo == WeekNo.All)
                    res += "A ";
                else if (WeekNo == WeekNo.Last)
                    res += "L ";
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (WeekNo.HasFlag((WeekNo)(1 << i)))
                            res += $"{i + 1}/";
                    }
                    res = res.TrimEnd('/', ' ') + ' ';
                }
                if (WeekDay == WeekDays.All)
                    res += "A ";
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (WeekDay.HasFlag((WeekDays)(1 << i)))
                            res += $"{i}/";
                    }
                    res = res.TrimEnd('/', ' ');
                }
                break;
            default: break;
        }
        if (StopEnd != default)
            res += $" {StopEnd:yyyy-MM-dd HH:mm:ss}";
        return res;
    }
    #endregion

    #region Public Method : void UpdateExecuted(DateTime time)
    /// <summary>更新排程的執行時間與下一次執行時間。</summary>
    /// <param name="time">執行時間。</param>
    /// <remarks>此方法會更新 <see cref="LastExecuted"/> 與 <see cref="NextTime"/> 屬性。</remarks>
    public void UpdateExecuted(DateTime time)
    {
        LastExecuted = time;
        NextTime = GetNextTime(time);
    }
    #endregion

    /// <summary>判斷當前排程是否與指定的排程相等。</summary>
    /// <param name="other">要比較的排程。</param>
    public bool Equals(IPlanTime? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (PlanType != other.PlanType) return false;
        if (StartFrom != other.StartFrom) return false;
        if (StopEnd != other.StopEnd) return false;
        if (Period != other.Period) return false;
        if (WeekDay != other.WeekDay) return false;
        if (Month != other.Month) return false;
        if (Day != other.Day) return false;
        if (WeekNo != other.WeekNo) return false;
        if (LastExecuted != other.LastExecuted) return false;
        if (NextTime != other.NextTime) return false;
        return true;
    }

    /// <summary>判斷當前排程是否與指定的物件相等。</summary>
    /// <param name="obj">要比較的物件。</param>
    /// <returns>如果指定的物件與當前排程相等，則為 <see langword="true"/>，否則為 <see langword="false"/>。</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IPlanTime other) return false;
        return Equals(other);
    }
    /// <summary>取得當前排程的哈希碼。</summary>
    /// <returns>當前排程的哈希碼。</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>判斷兩個 <see cref="TimePlan"/> 物件是否相等。</summary>
    public static bool operator ==(TimePlan? left, TimePlan? right)
    {
        if (left is null) return right is null;
        if (right is null) return false;
        return left.Equals(right);
    }
    /// <summary>判斷兩個 <see cref="TimePlan"/> 物件是否不相等。</summary>
    public static bool operator !=(TimePlan? left, TimePlan? right) => !(left == right);
    /// <summary>判斷左側的 <see cref="TimePlan"/> 是否在右側之前。</summary>
    public static bool operator >(TimePlan? left, TimePlan? right)
    {
        if (left is null) return false;
        if (right is null) return true;
        return left.NextTime > right.NextTime;
    }
    /// <summary>判斷左側的 <see cref="TimePlan"/> 是否在右側之後。</summary>
    public static bool operator <(TimePlan? left, TimePlan? right)
    {
        if (left is null) return true;
        if (right is null) return false;
        return left.NextTime < right.NextTime;
    }
    /// <summary>判斷左側的 <see cref="TimePlan"/> 是否在或等於右側。</summary>
    public static bool operator >=(TimePlan? left, TimePlan? right) => !(left < right);
    /// <summary>判斷左側的 <see cref="TimePlan"/> 是否在或等於右側。</summary>
    public static bool operator <=(TimePlan? left, TimePlan? right) => !(left > right);


    #region Internal Method : DateTime GetNextTime(DateTime time)
    private DateTime GetNextTime(DateTime time)
    {
        // LastExecuted.Equals(default) 表示未執行過
        if (StopEnd != default && StopEnd <= time) return default;
        DateTime dt = time, now = DateTime.Now, ndt;
        switch (PlanType)
        {
            case PlanTypes.Once: return time;
            case PlanTypes.Day:     // Verified @ 2023-05-24 14:02
                do
                {
                    ndt = dt.AddDays(Period);
                    if (StopEnd != default && StopEnd <= ndt) return default;
                    dt = ndt;
                }
                while (dt.Subtract(now).TotalDays < 0);
                return dt;
            case PlanTypes.Week:    // Verified @ 2023-05-24 14:02
                                    // 檢查當週
                for (int i = (int)dt.DayOfWeek; i < 7; i++)
                {
                    if (!WeekDay.HasFlag((WeekDays)(1 << i)))
                        continue;
                    if (!LastExecuted.Equals(default) && i == (int)dt.DayOfWeek && LastExecuted > dt)
                        continue;   // 前次執行日與檢查日為同一天, 且時間已過 By pass
                    ndt = dt.AddDays(i - (int)dt.DayOfWeek);
                    if (ndt < now)
                        continue;   // 檢查日為今天，但執行時間已過
                    else
                        return ndt;
                }
                // 檢查下一週期
                dt = dt.AddDays(-(int)dt.DayOfWeek).AddWeeks(Period);
                while (dt.Subtract(now).TotalDays < 0)
                    dt = dt.AddWeeks(Period);
                dt = dt.AddWeeks(-Period);
                //Console.WriteLine("".PadLeft(30) + $"    dt  = {dt:yyyy-MM-dd HH:mm:ss}({dt.DayOfWeek})");
                ndt = dt;
                do
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (!WeekDay.HasFlag((WeekDays)(1 << i)))
                            continue;
                        ndt = ndt.AddDays(i);
                        //Console.WriteLine("".PadLeft(30) + $"   ndt  = {ndt:yyyy-MM-dd HH:mm:ss}({ndt.DayOfWeek})");
                        if (ndt > now)
                            return ndt;
                        ndt = ndt.AddDays(-i);
                    }
                    ndt = dt.AddWeeks(Period); // 當週已過，找下一次週期
                }
                while (true);
            case PlanTypes.Month:   // Verified @ 2023-05-24 15:07
                                    // 當月份週期, 檢查其餘日期是否在排程內
                dt = dt.AddDays(-dt.Day + 1);   // 當月第一天
                while (Month.HasFlag((Months)(1 << dt.AddMonths(1).Month - 1)) && !dt.IsSameMonth(now) && dt < now)
                    dt = dt.AddMonths(1);
                do
                {
                    //Console.WriteLine("".PadLeft(30) + $"  A: dt  = {dt:yyyy-MM-dd HH:mm:ss}");
                    for (int i = 1; i <= DateTime.DaysInMonth(dt.Year, dt.Month); i++)
                    {
                        if (!Day.HasFlag((Days)(1 << (i - 1))))
                            continue;
                        if (!LastExecuted.Equals(default) && i == (int)dt.DayOfWeek)    // 前次執行日與檢查日為同一天 => By pass
                            continue;
                        //Console.WriteLine("".PadLeft(30) + $"  B: dt  = {dt.AddDays(i - dt.Day):yyyy-MM-dd HH:mm:ss}");
                        if (dt.AddDays(i - dt.Day) < DateTime.Now)                      // 下次執行時間已過 => By pass
                            continue;
                        return dt.AddDays(i - dt.Day);
                    }
                    do { dt = dt.AddMonths(1); }
                    while (!Month.HasFlag((Months)(1 << dt.Month - 1)));
                }
                while (true);
            case PlanTypes.MonthWeek:   // Verified @ 2023-05-24 17:29
                                        // 當月份週次, 檢查其餘日期是否在排程內
                while (Month.HasFlag((Months)(1 << dt.AddMonths(1).Month - 1)) && !dt.IsSameMonth(now) && dt < now)
                    dt = dt.AddMonths(1);
                do
                {
                    int mw = dt.WeekOfMonth(), ws = dt.WeeksOfMonth();
                    dt = dt.AddDays(-dt.Day + 1);   // 當月第一天
                    for (int j = mw; j <= ws; j++)
                    {
                        if (!(WeekNo.HasFlag(WeekNo.Last) && j == ws))
                        {
                            if (!WeekNo.HasFlag((WeekNo)(1 << j - 1)))
                                continue;
                        }
                        for (int i = 0; i <= 6; i++)
                        {
                            if (!WeekDay.HasFlag((WeekDays)(1 << i)))
                                continue;
                            if (!LastExecuted.Equals(default) && i == (int)dt.DayOfWeek)    // 前次執行日與檢查日為同一天, By pass
                                continue;
                            ndt = dt.AddDays(i + (j - mw) * 7 - (int)dt.DayOfWeek);
                            //Console.WriteLine("".PadLeft(30) + $"  ndt  = {ndt:yyyy-MM-dd HH:mm:ss}");
                            if (ndt < DateTime.Now)           // 下次執行時間已過 => By pass
                                continue;
                            return ndt;
                        }
                    }
                    do { dt = dt.AddMonths(1); }
                    while (!Month.HasFlag((Months)(1 << dt.Month - 1)));
                }
                while (true);
            default: return time;
        }
    }
    #endregion


    #region Private Static Method : string GetDaysString(Days days)
    private static string GetDaysString(Days days)
    {
        if (days == Days.All || days == Days.None)
            return days.GetDescription();
        else
        {
            var res = "";
            for (int i = 1; i <= 31; i++)
            {
                if (days.HasFlag((Days)(1 << (i - 1))))
                    res += $"{i}, ";
            }
            return res.TrimEnd(',', ' ');
        }
    }
    #endregion

    #region Private Static Method : int ConsoleColorToAnsiForeColor(ConsoleColor foreColor)
    private static int ConsoleColorToAnsiForeColor(ConsoleColor foreColor)
    {
        return foreColor switch
        {
            ConsoleColor.Black => 30,
            ConsoleColor.DarkRed => 31,
            ConsoleColor.DarkGreen => 32,
            ConsoleColor.DarkYellow => 33,
            ConsoleColor.DarkBlue => 34,
            ConsoleColor.DarkMagenta => 35,
            ConsoleColor.DarkCyan => 36,
            ConsoleColor.Gray => 37,
            ConsoleColor.DarkGray => 90,
            ConsoleColor.Red => 91,
            ConsoleColor.Green => 92,
            ConsoleColor.Yellow => 93,
            ConsoleColor.Blue => 94,
            ConsoleColor.Magenta => 95,
            ConsoleColor.Cyan => 96,
            ConsoleColor.White => 97,
            _ => 97,
        };
    }
    #endregion

    #region Private Static Method : int ConsoleColorToAnsiBackColor(ConsoleColor foreColor)
    private static int ConsoleColorToAnsiBackColor(ConsoleColor foreColor)
    {
        return foreColor switch
        {
            ConsoleColor.Black => 40,
            ConsoleColor.DarkRed => 41,
            ConsoleColor.DarkGreen => 42,
            ConsoleColor.DarkYellow => 43,
            ConsoleColor.DarkBlue => 44,
            ConsoleColor.DarkMagenta => 45,
            ConsoleColor.DarkCyan => 46,
            ConsoleColor.Gray => 47,
            ConsoleColor.DarkGray => 100,
            ConsoleColor.Red => 101,
            ConsoleColor.Green => 102,
            ConsoleColor.Yellow => 103,
            ConsoleColor.Blue => 104,
            ConsoleColor.Magenta => 105,
            ConsoleColor.Cyan => 106,
            ConsoleColor.White => 107,
            _ => 107,
        };
    }
    #endregion
}
