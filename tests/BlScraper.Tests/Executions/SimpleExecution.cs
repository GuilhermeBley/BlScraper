using BlScraper.Model;
using BlScraper.Results.Context;

namespace BlScraper.Tests.Executions;

internal class SimpleExecution : Quest<SimpleData>
{
    private List<DateTime> _execHours { get; } = new();
    public IEnumerable<DateTime> ExecHours => _execHours;
    public ContextRun Context { get; } = new ContextRun();

    public Action<DateTime>? OnSearch;

    public override void Dispose()
    {
        
    }

    public override QuestResult Execute(SimpleData data, CancellationToken cancellationToken = default)
    {
        var time = DateTime.Now;
        _execHours.Add(time);
        OnSearch?.Invoke(time);

        return QuestResult.Ok();
    }
}