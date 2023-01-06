using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <inheritdoc cref="IDataCollectedConfigureFilter" path="*"/>
public interface IDataCollectedConfigureFilter<TQuest, TData> : IDataCollectedConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage events on data collected
/// </summary>
/// <remarks>
///     <para>This interface is instanced when the method <see cref="IDataCollectedConfigureFilter.OnCollected(IEnumerable{object})"/> called.</para>
/// </remarks>
public interface IDataCollectedConfigureFilter
{
    /// <summary>
    /// Called when data collected to search
    /// </summary>
    /// <param name="dataCollected">Data to quest</param>
    Task OnCollected(IEnumerable<object> dataCollected);
}