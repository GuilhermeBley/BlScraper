namespace BlScraper.Internal;

/// <summary>
/// Sync counter
/// </summary>
internal sealed class ConcurrentCounter
{
    private int _count = 0;
    private readonly object _stateLock = new();
    public int Count => GetCountConcurrent();

    /// <summary>
    /// Add more to counter
    /// </summary>
    public void Add()
    {
        lock (_stateLock)
            _count++;
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
            if (_count == 0)
                return false;

            _count--;

            if (_count == 0)
                isLast = true;

            return true;
        }
    }

    /// <summary>
    /// Get concurrent
    /// </summary>
    private int GetCountConcurrent()
    {
        lock(_stateLock)
            return _count;
    }
}