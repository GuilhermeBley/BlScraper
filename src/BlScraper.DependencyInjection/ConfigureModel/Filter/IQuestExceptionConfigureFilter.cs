using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want manage exceptions on quests
/// </summary>
public interface IQuestExceptionConfigureFilter<TQuest, TData> : IQuestExceptionConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{

}

/// <summary>
/// Implementation for classes which want manage exceptions on quests
/// </summary>
public interface IQuestExceptionConfigureFilter
{
    /// <summary>
    /// Called on exception
    /// </summary>
    /// <param name="ex">Exception generated for the quest</param>
    /// <param name="data">Data that occurred exception</param>
    /// <returns>QuestResult for next step</returns>
    Task OnOccursException(Exception ex, object data, QuestResult result);
}