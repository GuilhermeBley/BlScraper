using System.Collections;
using System.Collections.Concurrent;

namespace BlScraper.Internal;

/// <summary>
/// Sync blocking list with max lenght
/// </summary>
internal sealed class MaxBlockingList<T> : IEnumerable<T>
{
    private BlockingCollection<T>? _collection;

    public MaxBlockingList(int boundingCapacity)
    {
        if (boundingCapacity > 0)
            _collection = new(boundingCapacity);
    }

    /// <summary>
    /// Try add more item
    /// </summary>
    /// <returns>
    /// True : Item added, False : non added
    /// </returns>
    public bool TryAdd(T item)
    {
        if (_collection is null)
            return false;
        return _collection.TryAdd(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_collection is null)
            return Enumerable.Empty<T>().GetEnumerator();
        return _collection.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}