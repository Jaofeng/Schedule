using CJF.Schedules.Interfaces;
using System.Collections;

namespace CJF.Schedules;

internal class PlanCollection : IPlanCollection
{
    private readonly Dictionary<string, ISchedulePlan> _Plans;

    public ISchedulePlan? this[string name] => Find(name);
    public int Count => _Plans.Count;

    /// <summary>建立一個新的 <see cref="PlanCollection"/> 執行個體。</summary>
    public PlanCollection()
    {
        _Plans = [];
    }

    /// <summary>新增一個排程項目。</summary>
    /// <param name="item">欲新增的排程項目。</param>
    /// <exception cref="KeyExistsException">指定的排程項目名稱已存在。</exception>
    public void Add(ISchedulePlan item)
    {
        if (_Plans.ContainsKey(item.Name))
            throw new KeyExistsException(item.Name, $"Name '{item.Name}' already exists.");
        _Plans.Add(item.Name, item);
    }
    /// <summary>移除指定名稱的排程項目。</summary>
    /// <param name="name">欲移除的排程項目名稱。</param>
    public void Remove(string name) => _Plans.Remove(name);

    /// <summary>清除所有排程項目。</summary>
    public void Clear() => _Plans.Clear();
    public bool Contains(string name) => _Plans.ContainsKey(name);
    public ISchedulePlan? Find(string name) => _Plans.TryGetValue(name, out ISchedulePlan? value) ? value : null;
    public IEnumerable<ISchedulePlan> GetEnabledPlans() => [.. _Plans.Values.Where(p => p.Valid)];
    public IEnumerable<ISchedulePlan> GetEnabledPlans(PlanTypes type) => [.. _Plans.Values.Where(p => p.Valid && p.TimeTable.PlanType == type)];
    public IEnumerable<ISchedulePlan> GetOnTime() =>
        from _sc in _Plans.Values
        where _sc.Valid && !_sc.IsRunning && _sc.TimeTable.OnTime(DateTime.Now)
        select _sc;
    public IEnumerable<ISchedulePlan> GetOnTime(PlanTypes type) =>
        from _sc in _Plans.Values
        where _sc.Valid && _sc.TimeTable.PlanType == type && !_sc.IsRunning && _sc.TimeTable.OnTime(DateTime.Now)
        select _sc;
    public IEnumerable<ISchedulePlan> GetPlans() => [.. _Plans.Values];
    public IEnumerable<ISchedulePlan> GetPlans(PlanTypes type)=> [.. _Plans.Values.Where(p => p.TimeTable.PlanType == type)];
    public IEnumerator<ISchedulePlan> GetEnumerator() => _Plans.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
