using System.Collections.Concurrent;

namespace BlScraper.Internal;

/// <summary>
/// Sync counter
/// </summary>
internal sealed class ConcurrentCounter
{
    private BlockingCollection<byte?> _collection = new();
    private readonly object _stateLock = new();
    public int Count => GetCountConcurrent();

    /// <summary>
    /// Add more to counter
    /// </summary>
    public void Add()
    {
        lock (_stateLock)
            _collection.Add(byte.MinValue);
    }

    /// <summary>
    /// Remove of the counter
    /// </summary>
    /// <returns>True : Removed, False: Didn't remove, <see cref="Count"/> is zero.</returns>
    public bool Remove(out bool isLast)
    {
        isLast = false;
        lock(_stateLock)
        {
            if (!_collection.TryTake(out byte? taked) &&
                taked is null)
                return false;

            if (!_collection.TryTake(out byte? lastTaked) &&
                lastTaked is null)
                isLast = true;

            if (!isLast)
                _collection.Add(lastTaked);

            return true;
        }
    }

    /// <summary>
    /// Get concurrent
    /// </summary>
    private int GetCountConcurrent()
    {
        lock(_stateLock)
            return _collection.Count;
    }
}