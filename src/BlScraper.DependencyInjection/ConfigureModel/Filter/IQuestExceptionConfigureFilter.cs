using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <inheritdoc cref="IQuestExceptionConfigure" path="*"/>
public interface IQuestExceptionConfigureFilter<TQuest, TData> : IQuestExceptionConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{

}

/// <summary>
/// Implementation for classes which want manage exceptions on quests
/// </summary>
/// <remarks>
///     <para>This interface is instanced when the method <see cref="IQuestExceptionConfigureFilter.OnOccursException(Exception, object, QuestResult)"/> called.</para>
/// </remarks>
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