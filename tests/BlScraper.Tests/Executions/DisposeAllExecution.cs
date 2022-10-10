using BlScraper.Model;
using BlScraper.Results.Context;

namespace BlScraper.Tests.Executions;

internal class DisposeAllExecution : Quest<IntegerData>
{
    private string ErrorResponse = "Dispose all {Id}.";
    private int _onDisposeAll;
    private int _countExc = 0;

    public DisposeAllExecution(int onDisposeAll = 1)
    {
        _onDisposeAll = onDisposeAll;
    }

    public override QuestResult Execute(IntegerData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_onDisposeAll == _countExc)
            return QuestResult.DisposeAll(ErrorResponse.Replace("{Id}", Id.ToString()));

        _countExc++;
        return QuestResult.Ok();
    }
}