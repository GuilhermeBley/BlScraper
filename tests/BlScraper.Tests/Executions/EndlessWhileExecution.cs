using BlScraper.Model;
using BlScraper.Results.Context;

namespace BlScraper.Tests.Executions;

internal class EndlessWhileExecution : Quest<SimpleData>
{
    public bool InRepeat = true;
    private CancellationToken CancellationToken = default;

    public EndlessWhileExecution(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }

    public override void Dispose()
    {

    }

    public override QuestResult Execute(SimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (InRepeat)
            Thread.Sleep(50);

        return QuestResult.Ok();
    }
}