using BlScraper.Model;

namespace BlScraper.DependencyInjection.Builder;

/// <summary>
/// Builds a <see cref="IModelScraper"/>
/// </summary>
public interface IScrapBuilder
{
    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by name or null if don't find
    /// </summary>
    /// <param name="name">Quest Name</param>
    /// <returns>Model Scraper or null</returns>
    IModelScraper? CreateModelByQuestOrDefault(string name);

    
    /// <summary>
    /// Builds a <see cref="IModelScraper"/> and return by name or null if don't find
    /// </summary>
    /// <param name="name">Quest Name</param>
    /// <returns>Model Scraper</returns>
    IModelScraper CreateModelByQuest(string name);
}