namespace BlScraper.Model;

/// <summary>
/// Basics functions for process
/// </summary>
public interface IModelScraperInfo
{
    /// <summary>
    /// Identifier
    /// </summary>
    Guid IdScraper { get; }

    /// <summary>
    /// Type of scrap to execute in model
    /// </summary>
    Type TypeScrap { get; }

    /// <summary>
    /// Date of initializes run
    /// </summary>
    DateTime? DtRun { get; }

    /// <summary>
    /// Date when all threads end and end event was executed
    /// </summary>
    DateTime? DtEnd { get; }

    /// <summary>
    /// Number of scraper to execute your context
    /// </summary>
    int CountScraper { get; }

    /// <summary>
    /// Count searchs ended
    /// </summary>
    int CountSearched { get; }

    /// <summary>
    /// Count scrapers in progress
    /// </summary>
    int CountProgress { get; }

    /// <summary>
    /// Total of data to search
    /// </summary>
    int TotalSearch { get; }

    /// <summary>
    /// Get current state
    /// </summary>
    ModelStateEnum State { get; }
}