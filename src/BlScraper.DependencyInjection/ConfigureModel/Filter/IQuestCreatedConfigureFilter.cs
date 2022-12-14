using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <inheritdoc cref="IQuestCreatedConfigureFilter" path="*"/>
public interface IQuestCreatedConfigureFilter<TQuest, TData> : IQuestCreatedConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage event on create quests
/// </summary>
/// <remarks>
///     <para>This interface is instanced when the method <see cref="IQuestCreatedConfigureFilter.OnCreated(IQuest)"/> called.</para>
/// </remarks>
public interface IQuestCreatedConfigureFilter
{
    /// <summary>
    /// Called when quest created
    /// </summary>
    /// <param name="questCreated">quest created</param>
    Task OnCreated(IQuest questCreated);
}