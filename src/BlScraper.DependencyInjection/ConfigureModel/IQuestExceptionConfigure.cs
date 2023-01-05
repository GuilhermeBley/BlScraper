using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Implementation for classes which want manage exceptions on quests
/// </summary>
/// <remarks>
///     <para>This interface is instanced when model created.</para>
/// <remarks>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IQuestExceptionConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{

    /// <summary>
    /// Called on exception
    /// </summary>
    /// <param name="ex">Exception generated for the quest</param>
    /// <param name="data">Data that occurred exception</param>
    /// <returns>QuestResult for next step</returns>
    QuestResult OnOccursException(Exception ex, TData data);
}