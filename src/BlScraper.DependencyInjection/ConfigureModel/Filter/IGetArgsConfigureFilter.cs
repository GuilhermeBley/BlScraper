using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want args to quests - use in unique quest
/// </summary>
[Obsolete("Filters in args are not used.")]
public interface IGetArgsConfigureFilter<TQuest, TData> : IGetArgsConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{

}

/// <summary>
/// Implementation for classes which want args to quests
/// </summary>
[Obsolete("Filters in args are not used.")]
public interface IGetArgsConfigureFilter
{

    /// <summary>
    /// Args to quest
    /// </summary>
    /// <returns>object array</returns>
    Task GetArgs(object[] args);
}