using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class SimpleQuestDuplicatedObsolete : Quest<PublicSimpleData>
{
    private static object _lockObj { get; } = new();
    private static int _counter = 0;
    public static int Counter { get { lock(_lockObj) return _counter; } }

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock(_lockObj)
        {
            _counter++;
        }

        return QuestResult.Ok();
    }
}

public class SimpleQuestDuplicatedObsoleteRequiredConfigure : RequiredConfigure<SimpleQuestDuplicatedObsolete, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        throw new NotImplementedException();
    }
}