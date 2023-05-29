namespace CJF.Schedules.Interfaces;

public interface ISchedulePlanCollection : IEnumerable<ISchedulePlan>
{
    /// <summary>依名稱取得 <see cref="ISchedulePlan"/> 執行個體。</summary>
    /// <param name="name">排程項目名稱。</param>
    /// <returns><see cref="ISchedulePlan"/> 執行個體。</returns>
    ISchedulePlan? this[string name] { get; }
    /// <summary>取得 <see cref="ISchedulePlanCollection"/> 中的項目數。</summary>
    int Count { get; }

    /// <summary>取得指定名稱的排程項目。</summary>
    /// <param name="name">欲取得的排程項目名稱。</param>
    /// <returns><see cref="ISchedulePlan"/> 執行個體。</returns>
    ISchedulePlan? Find(string name);
    /// <summary>判斷指定名稱的排程項目是否存在。</summary>
    bool Contains(string name);
    /// <summary>取得所有排程項目。</summary>
    IEnumerable<ISchedulePlan> GetPlans();
    /// <summary>取得所有排程項目。</summary>
    /// <param name="type">欲取得的排程類型。</param>
    IEnumerable<ISchedulePlan> GetPlans(PlanTypes type);
    /// <summary>取得所有已啟用的排程項目。</summary>
    IEnumerable<ISchedulePlan> GetEnabledPlans();
    /// <summary>取得所有已啟用的排程項目。</summary>
    /// <param name="type">欲取得的排程類型。</param>
    IEnumerable<ISchedulePlan> GetEnabledPlans(PlanTypes type);
    /// <summary>取得所有已啟用且未過期的排程項目。</summary>
    IEnumerable<ISchedulePlan> GetOnTime();
    /// <summary>取得所有已啟用且未過期的排程項目。</summary>
    /// <param name="type">欲取得的排程類型。</param>
    IEnumerable<ISchedulePlan> GetOnTime(PlanTypes type);
}
