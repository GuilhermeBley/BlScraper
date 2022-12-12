using BlScraper.DependencyInjection.ConfigureModel.Filter;
using System.Collections;

namespace BlScraper.DependencyInjection.ConfigureBuilder;

/// <summary>
/// Configuration builder
/// </summary>
public class ScrapBuilderConfig
{
    /// <summary>
    /// Type filters pool
    /// </summary>
    private readonly PoolFilter _poolFilters = new();

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    private HashSet<System.Reflection.Assembly> _assemblies = new();

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    internal IEnumerable<System.Reflection.Assembly> Assemblies => _assemblies;

    /// <summary>
    /// <see cref="IAllWorksEndConfigureFilter"/> pool
    /// </summary>
    internal IEnumerable<Type> PoolAllWorksEndConfigureFilter =>
        _poolFilters.Where(tuple => typeof(IAllWorksEndConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IDataCollectedConfigureFilter"/> pool
    /// </summary>
    internal IEnumerable<Type> PoolDataCollectedConfigureFilter =>
        _poolFilters.Where(tuple => typeof(IDataCollectedConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IDataFinishedConfigureFilter"/> pool
    /// </summary>
    internal IEnumerable<Type> PoolDataFinishedConfigureFilter =>
        _poolFilters.Where(tuple => typeof(IDataFinishedConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);
    
    /// <summary>
    /// <see cref="IGetArgsConfigureFilter"/> pool
    /// </summary>
    internal IEnumerable<Type> PoolGetArgsConfigureFilter =>
        _poolFilters.Where(tuple => typeof(IGetArgsConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IQuestCreatedConfigureFilter"/> pool
    /// </summary>
    internal IEnumerable<Type> PoolQuestCreatedConfigureFilter =>
        _poolFilters.Where(tuple => typeof(IQuestCreatedConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IQuestExceptionConfigureFilter"/> pool
    /// </summary>
    internal IEnumerable<Type> PoolQuestExceptionConfigureFilter =>
        _poolFilters.Where(tuple => typeof(IQuestExceptionConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// Lock object
    /// </summary>
    private object _lock = new();

    /// <summary>
    /// Instance of assembly builder
    /// </summary>
    /// <param name="modelScraperServiceType">Type parameter, it must have assignable from <see cref="ModelScraperService"/></param>
    internal ScrapBuilderConfig() 
    {
    }

    /// <summary>
    /// Try add new assemblies to map
    /// </summary>
    /// <param name="assembly">Assemblie to add</param>
    public ScrapBuilderConfig AddAssembly(params System.Reflection.Assembly[] assemblies)
    {
        lock(_lock)
        {
            foreach (var assembly in assemblies)
                _assemblies.Add(assembly);
        }

        return this;
    }

    /// <summary>
    /// Add filter of type <see cref="IAllWorksEndConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddAllWorksEndConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddAllWorksEndConfigureFilter<TFilter>()
        where TFilter : IAllWorksEndConfigureFilter
    {
        return AddAllWorksEndConfigureFilter(typeof(TFilter));
    }

    /// <summary>
    /// Add filter of type <see cref="IAllWorksEndConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="CheckAndAddToPoll(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddAllWorksEndConfigureFilter(Type filter)
    {
        return CheckAndAddToPoll(typeof(IAllWorksEndConfigureFilter), filter);
    }

    /// <summary>
    /// Add filter of type <see cref="IDataCollectedConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddDataCollectedConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataCollectedConfigureFilter<TFilter>()
        where TFilter : IDataCollectedConfigureFilter
    {
        return AddDataCollectedConfigureFilter(typeof(TFilter));
    }

    /// <summary>
    /// Add filter of type <see cref="IDataCollectedConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="CheckAndAddToPoll(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataCollectedConfigureFilter(Type filter)
    {
        return CheckAndAddToPoll(typeof(IDataCollectedConfigureFilter), filter);
    }

    /// <summary>
    /// Add filter of type <see cref="IDataFinishedConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddDataFinishedConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataFinishedConfigureFilter<TFilter>()
        where TFilter : IDataFinishedConfigureFilter
    {
        return AddDataFinishedConfigureFilter(typeof(TFilter));
    }

    /// <summary>
    /// Add filter of type <see cref="IDataFinishedConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="CheckAndAddToPoll(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataFinishedConfigureFilter(Type filter)
    {
        return CheckAndAddToPoll(typeof(IDataFinishedConfigureFilter), filter);
    }
    
    /// <summary>
    /// Add filter of type <see cref="IGetArgsConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddGetArgsConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddGetArgsConfigureFilter<TFilter>()
        where TFilter : IGetArgsConfigureFilter
    {
        return AddGetArgsConfigureFilter(typeof(TFilter));
    }

    /// <summary>
    /// Add filter of type <see cref="IGetArgsConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="CheckAndAddToPoll(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddGetArgsConfigureFilter(Type filter)
    {
        return CheckAndAddToPoll(typeof(IGetArgsConfigureFilter), filter);
    }

    /// <summary>
    /// Add filter of type <see cref="IQuestCreatedConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddQuestCreatedConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestCreatedConfigureFilter<TFilter>()
        where TFilter : IQuestCreatedConfigureFilter
    {
        return AddQuestCreatedConfigureFilter(typeof(TFilter));
    }

    /// <summary>
    /// Add filter of type <see cref="IQuestCreatedConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="CheckAndAddToPoll(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestCreatedConfigureFilter(Type filter)
    {
        return CheckAndAddToPoll(typeof(IQuestCreatedConfigureFilter), filter);
    }

    /// <summary>
    /// Add filter of type <see cref="IQuestExceptionConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddQuestExceptionConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestExceptionConfigureFilter<TFilter>()
        where TFilter : IQuestExceptionConfigureFilter
    {
        return AddQuestExceptionConfigureFilter(typeof(TFilter));
    }

    /// <summary>
    /// Add filter of type <see cref="IQuestExceptionConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="CheckAndAddToPoll(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestExceptionConfigureFilter(Type filter)
    {
        return CheckAndAddToPoll(typeof(IQuestExceptionConfigureFilter), filter);
    }

    /// <summary>
    /// Private check and add
    /// </summary>
    /// <param name="filterInterface">filter interface</param>
    /// <param name="filter">filter implementation</param>
    /// <inheritdoc cref="CheckAndThrowFilter(Type, Type)" path="/exception"/>
    private ScrapBuilderConfig CheckAndAddToPoll(Type filterInterface, Type filter)
    {
        CheckAndThrowFilter(filter, filterInterface);

        _poolFilters.Add(filterInterface, filter);

        return this;
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
    
    /// <summary>
    /// Sync Pool filter
    /// </summary>
    private class PoolFilter : IEnumerable<(Type FilterInterface, Type Filter)>
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
            if (_poolFilters.Contains((filterInterface, filter)))
                return false;

            _poolFilters.Add((filterInterface, filter));

            return true;
        }

        /// <summary>
        /// Add and check
        /// </summary>
        /// <param name="filterInterface">Filter interface</param>
        /// <param name="filter">Filter</param>
        /// <exception cref="ArgumentException"/>
        public void Add(Type filterInterface, Type filter)
        {
            if (!TryAdd(filterInterface, filter))
                throw new ArgumentException($"List already contains '{filter.FullName}' to '{filterInterface.FullName}'.", filter.FullName);
        }

        public IEnumerator<(Type FilterInterface, Type Filter)> GetEnumerator()
        {
            return _poolFilters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}