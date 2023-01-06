using BlScraper.Model;
using BlScraper.Results;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Implementation for classes which want manage finished searchs
/// </summary>
/// <remarks>
///     <para>This interface is instanced when model created.</para>
/// <remarks>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IAllWorksEndConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{

    /// <summary>
    /// Called when all data finished
    /// </summary>
    /// <param name="results">results of quests completed</param>
    /// <returns>async</returns>
    void OnFinished(EndEnumerableModel results);
}