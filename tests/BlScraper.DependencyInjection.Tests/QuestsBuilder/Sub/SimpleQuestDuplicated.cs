using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Sub;

public class SimpleQuestDuplicated : Quest<PublicSimpleData>
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

public class SimpleQuestDuplicatedRequiredConfigure : RequiredConfigure<SimpleQuestDuplicated, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public SimpleQuestDuplicatedRequiredConfigure(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public async override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}