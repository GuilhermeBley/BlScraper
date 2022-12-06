using System.Collections;

namespace BlScraper.Results.Models;

public class EndEnumerableModel : IEnumerable<ResultBase<Exception?>>
{
    private readonly IEnumerable<ResultBase<Exception?>> _results;

    public readonly bool AllSearched;

    public bool ContainsError
        => this.Any(r => r.Result is not null);

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