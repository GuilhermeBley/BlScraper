namespace BlScraper.DependencyInjection.Tests.Services;

public interface ICounterService
{
    int Count { get; }
    void Add();
}

public class CounterService : ICounterService
{
    private readonly object _lockObj = new();
    private int _count;
    public int Count { get { lock(_lockObj) return _count; } }

    public void Add()
    {
        lock(_lockObj)
            _count++;
    }
}