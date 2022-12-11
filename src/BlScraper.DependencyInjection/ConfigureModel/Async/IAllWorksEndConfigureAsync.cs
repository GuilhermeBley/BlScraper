using BlScraper.Model;
using BlScraper.Results;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.ConfigureModel.Async;

/// <summary>
/// Implementation for classes which want manage finished searchs
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IAllWorksEndConfigureAsync<TQuest, TData> : IAsyncEventScrap
    where TQuest : Quest<TData>
    where TData : class
{

    /// <summary>
    /// Called when all data finished
    /// </summary>
    /// <param name="results">results of quests completed</param>
    /// <returns>async</returns>
    Task OnFinished(EndEnumerableModel results);
}