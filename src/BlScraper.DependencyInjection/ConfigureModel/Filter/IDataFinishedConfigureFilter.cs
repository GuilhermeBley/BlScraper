using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.ConfigureModel.Filter;

/// <inheritdoc cref="IDataFinishedConfigureFilter" path="*"/>
public interface IDataFinishedConfigureFilter<TQuest, TData> : IDataFinishedConfigureFilter
    where TQuest : Quest<TData>
    where TData : class
{
    
}

/// <summary>
/// Implementation for classes which want manage events on data finished
/// </summary>
/// <remarks>
///     <para>This interface is instanced when the method <see cref="IDataFinishedConfigureFilter.OnDataFinished(ResultBase)"/> called.</para>
/// </remarks>
public interface IDataFinishedConfigureFilter
{
    /// <summary>
    /// On data finished
    /// </summary>
    /// <param name="resultFinished">Result after search</param>
    Task OnDataFinished(ResultBase resultFinished);
}