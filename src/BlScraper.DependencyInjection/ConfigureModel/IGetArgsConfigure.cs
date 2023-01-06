using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Implementation for classes which want args to quests
/// </summary>
/// <remarks>
///     <para>This interface is instanced when model created.</para>
/// <remarks>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IGetArgsConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{

    /// <summary>
    /// Args to quest
    /// </summary>
    /// <returns>object array</returns>
    object[] GetArgs();
}