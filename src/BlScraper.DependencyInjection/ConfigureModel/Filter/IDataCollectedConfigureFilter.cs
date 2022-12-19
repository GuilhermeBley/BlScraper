using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want manage events on data collected - use in unique quest
/// </summary>
public interface IDataCollectedConfigureFilter<TQuest, TData> : IDataCollectedConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage events on data collected
/// </summary>
public interface IDataCollectedConfigureFilter
{
    /// <summary>
    /// Called when data collected to search
    /// </summary>
    /// <param name="dataCollected">Data to quest</param>
    Task OnCollected(IEnumerable<object> dataCollected);
}