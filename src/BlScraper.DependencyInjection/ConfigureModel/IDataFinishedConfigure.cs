using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Implementation for classes which want manage events on data finished
/// </summary>
/// <remarks>
///     <para>This interface is instanced when model created.</para>
/// <remarks>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IDataFinishedConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// On data finished
    /// </summary>
    /// <param name="resultFinished">Result after search</param>
    void OnDataFinished(ResultBase<TData> resultFinished);
}