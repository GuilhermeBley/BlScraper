using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want manage event on create quests
/// </summary>
public interface IQuestCreatedConfigureFilter
{
    /// <summary>
    /// Called when quest created
    /// </summary>
    /// <param name="questCreated">quest created</param>
    Task OnCreated(IQuest questCreated);
}