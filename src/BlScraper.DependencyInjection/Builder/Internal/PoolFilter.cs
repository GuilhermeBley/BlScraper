using System.Collections;
using System.Collections.Generic;

namespace BlScraper.DependencyInjection.Builder.Internal;

/// <summary>
/// Sync Pool filter
/// </summary>
internal class PoolFilter : IEnumerable<(Type FilterInterface, Type Filter)>
{
    /// <summary>
    /// Type filters pool
    /// </summary>
    private System.Collections.Concurrent.ConcurrentBag<(Type filterInterface, Type Filter)> _poolFilters = new();

    /// <summary>
    /// Try add
    /// </summary>
    /// <param name="filterInterface">Filter interface</param>
    /// <param name="filter">Filter</param>
    /// <returns>true : item added, otherwise already contains</returns>
    public bool TryAdd(Type filterInterface, Type filter)
    {
        try
        {
            Add(filterInterface, filter);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Add and check
    /// </summary>
    /// <param name="filterInterface">Filter interface</param>
    /// <param name="filter">Filter</param>
    /// <exception cref="ArgumentException"/>
    public void Add(Type filterInterface, Type filter)
    {
        if (_poolFilters.Contains((filterInterface, filter)))
            throw new ArgumentException($"List already contains '{filter.FullName}' to '{filterInterface.FullName}'.", filter.FullName);

        CheckAndThrowFilter(filter, filterInterface);

        _poolFilters.Add((filterInterface, filter));
    }

    /// <summary>
    /// union
    /// </summary>
    public PoolFilter Union(PoolFilter poolFilter)
    {
        var unionFilter = new PoolFilter();
        
        foreach (var filterCurr in this)
        {
            unionFilter.Add(filterCurr.FilterInterface, filterCurr.Filter);
        }

        foreach(var filterCurr in poolFilter)
        {
            unionFilter.Add(filterCurr.FilterInterface, filterCurr.Filter);
        }

        return unionFilter;
    }

    public IEnumerator<(Type FilterInterface, Type Filter)> GetEnumerator()
    {
        return _poolFilters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Check options of filter and if is assgnable to <paramref name="assignable"/>
    /// </summary>
    /// <param name="filter">filter</param>
    /// <param name="assignable">filter assignable</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private static void CheckAndThrowFilter(Type filter, Type assignable)
    {
        if (filter is null)
            throw new ArgumentNullException($"filter");

        if (assignable is null)
            throw new ArgumentNullException($"assignable");

        if (!assignable.IsAssignableFrom(filter))
            throw new ArgumentException($"Filter: '{filter.FullName}' isn't assignable to '{assignable.FullName}'.", $"{filter.FullName}");

        if (!filter.IsClass)
            throw new ArgumentException($"Filter: '{filter.FullName}' must be a class.", $"{filter.FullName}");

        if (filter.IsAbstract)
            throw new ArgumentException($"Filter: '{filter.FullName}' must not be abstract.", $"{filter.FullName}");

        if (!filter.IsPublic)
            throw new ArgumentException($"Filter: '{filter.FullName}' must be public.", $"{filter.FullName}");

        if (filter.ContainsGenericParameters)
            throw new ArgumentException($"Filter: '{filter.FullName}' must not have generic parameters.", $"{filter.FullName}");
    }
}