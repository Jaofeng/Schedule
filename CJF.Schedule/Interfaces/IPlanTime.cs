namespace CJF.Schedules.Interfaces;

/// <summary>提供排程計畫的介面。</summary>
public interface IPlanTime : ICloneable
{
    /// <summary>開始執行排程的時間。</summary>
    DateTime StartFrom { get; }
    /// <summary>終止執行排程的時間。</summary>
    DateTime StopEnd { get; }
    /// <summary>排程執行週期類型。</summary>
    PlanTypes PlanType { get; }
    /// <summary>排程執行週期，此值需參照 <see cref="PlanType"/>。當 <see cref="PlanType"/> 屬性值不為 <see cref="PlanTypes.Day"/> 與 <see cref="PlanTypes.Week"/> 時無效。</summary>
    int Period { get; }
    /// <summary>一週內設定的星期名稱。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.Week"/> 與 <see cref="PlanTypes.MonthWeek"/> 時無效。</summary>
    WeekDays WeekDay { get; }
    /// <summary>排程訂定的月份。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.Month"/> 時無效。</summary>
    Months Month { get; }
    /// <summary>排程訂定的日期。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.Month"/> 時無效。</summary>
    Days Day { get; }
    /// <summary>排定的周別。此屬性當 <see cref="PlanType"/> 不為 <see cref="PlanTypes.MonthWeek"/> 時無效。</summary>
    WeekNo WeekNo { get; }
    /// <summary>前次執行時間。</summary>
    DateTime LastExecuted { get; }
    /// <summary>下次執行的時間。</summary>
    DateTime NextTime { get; }

    /// <summary>檢查時間是否處於可執行的排程時間內。</summary>
    /// <param name="time">欲檢查的時間。</param>
    /// <returns>true 該時間處於排程時間內; false 不在時間內。</returns>
    bool OnTime(DateTime time);
    /// <summary>取得詳細的中文說明。</summary>
    /// <returns>詳細說明。</returns>
    string GetDescription();
    /// <summary>取得詳細的中文說明。</summary>
    /// <param name="fore">文字前景顏色。</param>
    /// <param name="back">[可選]文字背景顏色。</param>
    /// <returns>詳細說明。</returns>
    string GetDescription(ConsoleColor fore, ConsoleColor? back = null);
    /// <summary>取得簡易字串表示式。</summary>
    /// <returns>簡易表示式。</returns>
    string GetExpression();
    /// <summary>更新排程的執行時間與下一次執行時間。</summary>
    /// <param name="time">執行時間。</param>
    /// <remarks>此方法會更新 <see cref="LastExecuted"/> 與 <see cref="NextTime"/> 屬性。</remarks>
    void UpdateExecuted(DateTime time);
}
