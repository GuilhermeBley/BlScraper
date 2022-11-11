using System.Reflection;
using System.Collections.Concurrent;

namespace BlScraper.DependencyInjection.Tests.Services;

public interface IRouteService
{
    /// <summary>
    /// Enumerable which contains a storages of sequence of methods executed
    /// </summary>
    IEnumerable<MethodInfo> Routes { get; }

    void Add(MethodInfo? methodExcuted);
}

public class RouteService : IRouteService
{
    private ConcurrentQueue<MethodInfo> _queue = new();
    public IEnumerable<MethodInfo> Routes => _queue;

    public void Add(MethodInfo? methodExcuted)
    {
        if (methodExcuted is null)
            return;

        _queue.Enqueue(methodExcuted);
    }
}