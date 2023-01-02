using System.Reflection;
using System.Collections.Concurrent;

namespace BlScraper.DependencyInjection.Tests.Services;

public interface IRouteObjectService
{
    /// <summary>
    /// Enumerable which contains a storages of sequence of methods executed
    /// </summary>
    IEnumerable<(MethodInfo Method, object? ObjRoute)> Routes { get; }

    void Add(MethodInfo? methodExcuted, object? objRoute);
}

public class RouteObjectService : IRouteObjectService
{
    private ConcurrentQueue<(MethodInfo, object?)> _queue = new();
    public IEnumerable<(MethodInfo, object?)> Routes => _queue;

    public void Add(MethodInfo? methodExcuted, object? objRoute)
    {
        if (methodExcuted is null)
            return;

        _queue.Enqueue(new (methodExcuted, objRoute));
    }
}