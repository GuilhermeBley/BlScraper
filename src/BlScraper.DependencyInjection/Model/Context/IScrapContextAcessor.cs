using BlScraper.Model;

namespace BlScraper.DependencyInjection.Model.Context;

/// <summary>
/// Context Scrap
/// </summary>
public interface IScrapContextAccessor
{
    /// <summary>
    /// Current context of thread
    /// </summary>
    IModelScraperInfo? ScrapContext { get; }
    
    /// <summary>
    /// Required current context of thread
    /// </summary>
    IModelScraperInfo RequiredScrapContext { get; }
}