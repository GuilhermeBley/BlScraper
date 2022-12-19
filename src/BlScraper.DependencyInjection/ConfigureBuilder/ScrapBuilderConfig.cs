using BlScraper.DependencyInjection.Builder.Internal;
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

    /// <inheritdoc cref="_poolFilters" path="*"/>
    internal PoolFilter Filters => _poolFilters;

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    private HashSet<System.Reflection.Assembly> _assemblies = new();

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    internal IEnumerable<System.Reflection.Assembly> Assemblies => _assemblies;

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
    /// <inheritdoc cref="PoolFilter.Add(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddAllWorksEndConfigureFilter(Type filter)
    {
        if (TypeUtils.IsObsolete(filter)) return this;

        _poolFilters.Add(typeof(IAllWorksEndConfigureFilter), filter);
        return this;
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
    /// <inheritdoc cref="PoolFilter.Add(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataCollectedConfigureFilter(Type filter)
    {
        if (TypeUtils.IsObsolete(filter)) return this;

        _poolFilters.Add(typeof(IDataCollectedConfigureFilter), filter);
        return this;
    }

    /// <summary>
    /// Add filter of type <see cref="IDataFinishedConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddDataFinishedConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataFinishedConfigureFilter<TFilter>()
        where TFilter : IDataFinishedConfigureFilter
    {
        AddDataFinishedConfigureFilter(typeof(TFilter));
        return this;
    }

    /// <summary>
    /// Add filter of type <see cref="IDataFinishedConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="PoolFilter.Add(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddDataFinishedConfigureFilter(Type filter)
    {
        if (TypeUtils.IsObsolete(filter)) return this;

        _poolFilters.Add(typeof(IDataFinishedConfigureFilter), filter);
        return this;
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
    /// <inheritdoc cref="PoolFilter.Add(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddGetArgsConfigureFilter(Type filter)
    {
        if (TypeUtils.IsObsolete(filter)) return this;

        _poolFilters.Add(typeof(IGetArgsConfigureFilter), filter);
        return this;
    }

    /// <summary>
    /// Add filter of type <see cref="IQuestCreatedConfigureFilter"/>
    /// </summary>
    /// <typeparam name="TFilter">Type of filter</typeparam>
    /// <inheritdoc cref="AddQuestCreatedConfigureFilter(Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestCreatedConfigureFilter<TFilter>()
        where TFilter : IQuestCreatedConfigureFilter
    {
        AddQuestCreatedConfigureFilter(typeof(TFilter));
        return this;
    }

    /// <summary>
    /// Add filter of type <see cref="IQuestCreatedConfigureFilter"/>
    /// </summary>
    /// <param name="filter">filter to add</param>
    /// <inheritdoc cref="PoolFilter.Add(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestCreatedConfigureFilter(Type filter)
    {
        if (TypeUtils.IsObsolete(filter)) return this;

        _poolFilters.Add(typeof(IQuestCreatedConfigureFilter), filter);
        return this;
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
    /// <inheritdoc cref="PoolFilter.Add(Type, Type)" path="/exception"/>
    public ScrapBuilderConfig AddQuestExceptionConfigureFilter(Type filter)
    {
        if (TypeUtils.IsObsolete(filter)) return this;

        _poolFilters.Add(typeof(IQuestExceptionConfigureFilter), filter);
        return this;
    }
}