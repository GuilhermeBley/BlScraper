using BlScraper.Results;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <summary>
/// Implementation for classes which want manage events on data finished
/// </summary>
public interface IDataFinishedConfigureFilter
{
    /// <summary>
    /// On data finished
    /// </summary>
    /// <param name="resultFinished">Result after search</param>
    Task OnDataFinished(ResultBase<object> resultFinished);
}