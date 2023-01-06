using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Implementation for classes which want manage event on create quests
/// </summary>
/// <remarks>
///     <para>This interface is instanced when model created.</para>
/// <remarks>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IQuestCreatedConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// Called when quest created
    /// </summary>
    /// <param name="questCreated">quest created</param>
    void OnCreated(TQuest questCreated);
}