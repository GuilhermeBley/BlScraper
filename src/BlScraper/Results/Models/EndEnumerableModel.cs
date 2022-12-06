using System.Collections;

namespace BlScraper.Results.Models;

/// <summary>
/// Enumerable of <see cref="ResultBase"/> with exception, if contains
/// </summary>
public sealed class EndEnumerableModel : IEnumerable<ResultBase<Exception?>>
{
    private readonly IEnumerable<ResultBase<Exception?>> _results;

    /// <summary>
    /// If all data was searched, will be true
    /// </summary>
    public readonly bool AllSearched;

    /// <summary>
    /// Check if has exception in execution
    /// </summary>
    public bool ContainsError
        => this.Any(r => r.Result is not null);

    /// <summary>
    /// Internal instance
    /// </summary>
    internal EndEnumerableModel(IEnumerable<ResultBase<Exception?>> results, bool allSearched)
    {
        _results = results;
        AllSearched = allSearched;
    }

    public IEnumerator<ResultBase<Exception?>> GetEnumerator()
    {
        return _results.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _results.GetEnumerator();
    }
}