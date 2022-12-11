using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Async;

/// <summary>
/// Implementation for classes which want manage event on create quests
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IQuestCreatedConfigureAsync<TQuest, TData> : IAsyncEventScrap
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// Called when quest created
    /// </summary>
    /// <param name="questCreated">quest created</param>
    Task OnCreated(TQuest questCreated);
}