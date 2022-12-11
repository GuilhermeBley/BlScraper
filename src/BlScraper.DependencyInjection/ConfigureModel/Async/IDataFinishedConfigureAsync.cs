using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.ConfigureModel.Async;

/// <summary>
/// Implementation for classes which want manage events on data finished
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IDataFinishedConfigureAsync<TQuest, TData> : IAsyncEventScrap
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// On data finished
    /// </summary>
    /// <param name="resultFinished">Result after search</param>
    Task OnDataFinished(ResultBase<TData> resultFinished);
}