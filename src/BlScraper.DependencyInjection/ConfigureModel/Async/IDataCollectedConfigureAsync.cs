using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Async;

/// <summary>
/// Implementation for classes which want manage events on data collected
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IDataCollectedConfigureAsync<TQuest, TData> : IAsyncEventScrap
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// Called when data collected to search
    /// </summary>
    /// <param name="dataCollected">Data to quest</param>
    Task OnCollected(IEnumerable<TData> dataCollected);
}