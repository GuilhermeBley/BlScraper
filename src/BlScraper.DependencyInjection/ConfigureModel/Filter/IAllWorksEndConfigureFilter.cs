using BlScraper.Model;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want manage finished searchs - use in unique quest
/// </summary>
public  interface IAllWorksEndConfigureFilter<TQuest, TData> : IAllWorksEndConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage finished searchs
/// </summary>
public interface IAllWorksEndConfigureFilter
{

    /// <summary>
    /// Called when all data finished
    /// </summary>
    /// <param name="results">results of quests completed</param>
    /// <returns>async</returns>
    Task OnFinished(EndEnumerableModel results);
}