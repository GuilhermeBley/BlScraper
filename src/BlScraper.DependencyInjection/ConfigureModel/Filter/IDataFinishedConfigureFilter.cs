using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want manage events on data finished - use in unique quest
/// </summary>
public interface IDataFinishedConfigureFilter<TQuest, TData> : IDataFinishedConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage events on data finished
/// </summary>
public interface IDataFinishedConfigureFilter
{
    /// <summary>
    /// On data finished
    /// </summary>
    /// <param name="resultFinished">Result after search</param>
    Task OnDataFinished(ResultBase resultFinished);
}