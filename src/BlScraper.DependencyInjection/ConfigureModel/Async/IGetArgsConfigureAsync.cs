using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Async;

/// <summary>
/// Implementation for classes which want args to quests
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IGetArgsConfigureAsync<TQuest, TData> : IAsyncEventScrap
    where TQuest : Quest<TData>
    where TData : class
{

    /// <summary>
    /// Args to quest
    /// </summary>
    /// <returns>object array</returns>
    Task GetArgs(object[] args);
}