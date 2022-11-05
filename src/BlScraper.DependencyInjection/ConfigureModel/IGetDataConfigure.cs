using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Required implementation for quests, it that defines the data to search
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IGetDataConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{

    /// <summary>
    /// Called to collect data to search
    /// </summary>
    /// <returns>Async list of data</returns>
    Task<IEnumerable<TData>> GetData();
}