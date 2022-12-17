using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Implementation for classes which want manage events on data collected
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IDataCollectedConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// Called when data collected to search
    /// </summary>
    /// <param name="dataCollected">Data to quest</param>
    void OnCollected(IEnumerable<TData> dataCollected);
}