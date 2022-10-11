using BlScraper.Model;
using BlScraper.Results.Context;

namespace BlScraper.Tests.Executions;

internal class EndlessWhileExecution : Quest<SimpleData>
{
    public bool InRepeat = true;
    public int _maxTimeSleepExecute { get; }
    private CancellationToken CancellationToken = default;

    public EndlessWhileExecution(CancellationToken cancellationToken, int maxTimeSleepExecute = 50)
    {
        CancellationToken = cancellationToken;
        _maxTimeSleepExecute = maxTimeSleepExecute;
    }

    public override void Dispose()
    {

    }

    public override QuestResult Execute(SimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        CancellationToken.ThrowIfCancellationRequested();

        while (InRepeat)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CancellationToken.ThrowIfCancellationRequested();
            Thread.Sleep(_maxTimeSleepExecute);
        }

        return QuestResult.Ok();
    }
}