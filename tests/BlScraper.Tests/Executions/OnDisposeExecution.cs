using BlScraper.Model;
using BlScraper.Results.Context;

namespace BlScraper.Tests.Executions;

internal class OnDisposeExecution : Quest<IntegerData>
{
    private int _disposedCount = 0;
    public int DisposedCount => _disposedCount;
    public Action<OnDisposeExecution> OnDisposedContext { get; }

    public OnDisposeExecution(Action<OnDisposeExecution> onDisposedContext)
    {
        OnDisposedContext = onDisposedContext 
            ?? throw new ArgumentNullException(nameof(onDisposedContext));
    }

    public override QuestResult Execute(IntegerData data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }

    public override void Dispose()
    {
        _disposedCount++;
        OnDisposedContext.Invoke(this);
    }
}