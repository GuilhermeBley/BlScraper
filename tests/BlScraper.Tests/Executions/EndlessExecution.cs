using BlScraper.Model;
using BlScraper.Results.Context;

namespace BlScraper.Tests.Executions;

internal class EndlessExecution : Quest<SimpleData>
{
    public bool IsActiveError = true;
    public bool _hasError = false;

    public override void Dispose()
    {

    }

    public override QuestResult Execute(SimpleData data, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (_hasError)
                return QuestResult.Ok();
            
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch
            {
                if (IsActiveError)
                    _hasError = true;
                throw;
            }
            
        }
    }
}