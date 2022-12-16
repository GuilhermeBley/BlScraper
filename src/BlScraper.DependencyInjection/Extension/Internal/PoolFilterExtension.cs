using BlScraper.DependencyInjection.Builder.Internal;
using BlScraper.DependencyInjection.ConfigureModel.Filter;

namespace BlScraper.DependencyInjection.Extension.Internal;

internal static class GetPoolFilterExtension
{
    /// <summary>
    /// <see cref="IAllWorksEndConfigureFilter"/> GetPool
    /// </summary>
    internal static IEnumerable<Type> GetPoolAllWorksEndConfigureFilter(this PoolFilter _poolFilters) =>
        _poolFilters.Where(tuple => typeof(IAllWorksEndConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IDataCollectedConfigureFilter"/> GetPool
    /// </summary>
    internal static IEnumerable<Type> GetPoolDataCollectedConfigureFilter(this PoolFilter _poolFilters) =>
        _poolFilters.Where(tuple => typeof(IDataCollectedConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IDataFinishedConfigureFilter"/> GetPool
    /// </summary>
    internal static IEnumerable<Type> GetPoolDataFinishedConfigureFilter(this PoolFilter _poolFilters) =>
        _poolFilters.Where(tuple => typeof(IDataFinishedConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);
    
    /// <summary>
    /// <see cref="IGetArgsConfigureFilter"/> GetPool
    /// </summary>internal static
    internal static IEnumerable<Type> GetPoolGetArgsConfigureFilter(this PoolFilter _poolFilters) =>
        _poolFilters.Where(tuple => typeof(IGetArgsConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IQuestCreatedConfigureFilter"/> GetPool
    /// </summary>
    internal static IEnumerable<Type> GetPoolQuestCreatedConfigureFilter(this PoolFilter _poolFilters) =>
        _poolFilters.Where(tuple => typeof(IQuestCreatedConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);

    /// <summary>
    /// <see cref="IQuestExceptionConfigureFilter"/> GetPool
    /// </summary>
    internal static IEnumerable<Type> GetPoolQuestExceptionConfigureFilter(this PoolFilter _poolFilters) =>
        _poolFilters.Where(tuple => typeof(IQuestExceptionConfigureFilter).IsAssignableFrom(tuple.FilterInterface)).Select(tuple => tuple.Filter);
}