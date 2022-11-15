using BlScraper.Model;

namespace BlScraper.DependencyInjection.Builder;

/// <summary>
/// Builds a <see cref="IModelScraper"/>
/// </summary>
public interface IScrapBuilder
{
    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by name
    /// </summary>
    /// <param name="name">Quest Name</param>
    /// <returns>Model Scraper</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    IModelScraper CreateModelByQuestName(string name);

    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by name or null if don't find
    /// </summary>
    /// <param name="name">Quest Name</param>
    /// <returns>Model Scraper or null</returns>
    IModelScraper? CreateModelByQuestNameOrDefault(string name);

    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by type
    /// </summary>
    /// <param name="type"><see cref="Quest{TData}"/></param>
    /// <returns>Model Scraper</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    IModelScraper CreateModelByQuestType(Type type);
    
    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by type or null if don't find
    /// </summary>
    /// <param name="type"><see cref="Quest{TData}"/></param>
    /// <returns>Model Scraper or null</returns>
    IModelScraper? CreateModelByQuestTypeOrDefault(Type type);

    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by type
    /// </summary>
    /// <typeparam name="TQuest">Quest type assignable to <see cref="Quest{TData}"/></typeparam>
    /// <returns>Model Scraper</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    IModelScraper CreateModelByQuestType<TQuest>();

    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by type or null if don't find
    /// </summary>
    /// <typeparam name="TQuest">Quest type assignable to <see cref="Quest{TData}"/></typeparam>
    /// <returns>Model Scraper or null</returns>
    IModelScraper? CreateModelByQuestTypeOrDefault<TQuest>();
}