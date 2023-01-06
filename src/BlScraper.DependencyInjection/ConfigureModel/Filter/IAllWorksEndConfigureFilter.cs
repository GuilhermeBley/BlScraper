using BlScraper.Model;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <inheritdoc cref="IAllWorksEndConfigureFilter" path="*"/>
public  interface IAllWorksEndConfigureFilter<TQuest, TData> : IAllWorksEndConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage finished searchs
/// </summary>
/// <remarks>
///     <para>This interface is instanced when the method <see cref="IAllWorksEndConfigure.OnFinished(EndEnumerableModel)"/> called.</para>
/// </remarks>
public interface IAllWorksEndConfigureFilter
{

    /// <summary>
    /// Called when all data finished
    /// </summary>
    /// <param name="results">results of quests completed</param>
    /// <returns>async</returns>
    Task OnFinished(EndEnumerableModel results);
}