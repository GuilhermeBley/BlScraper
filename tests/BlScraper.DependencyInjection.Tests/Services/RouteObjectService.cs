using System.Reflection;
using System.Collections.Concurrent;

namespace BlScraper.DependencyInjection.Tests.Services;

public interface IRouteObjectService
{
    /// <summary>
    /// Enumerable which contains a storages of sequence of methods executed
    /// </summary>
    IEnumerable<(MethodBase Method, object? ObjRoute)> Routes { get; }

    void Add(MethodBase? methodExcuted, object? objRoute);
}

public class RouteObjectService : IRouteObjectService
{
    private ConcurrentQueue<(MethodBase, object?)> _queue = new();
    public IEnumerable<(MethodBase, object?)> Routes => _queue;

    public void Add(MethodBase? methodExcuted, object? objRoute)
    {
        if (methodExcuted is null)
            return;

        _queue.Enqueue(new (methodExcuted, objRoute));
    }
}