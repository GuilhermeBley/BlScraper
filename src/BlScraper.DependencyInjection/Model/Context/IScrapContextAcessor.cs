using BlScraper.Model;

namespace BlScraper.DependencyInjection.Model.Context;

/// <summary>
/// Context Scrap
/// </summary>
public interface IScrapContextAcessor
{
    /// <summary>
    /// Current context of thread
    /// </summary>
    IModelScraperInfo? ScrapContext { get; }
}