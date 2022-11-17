using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class SimpleQuest : Quest<PublicSimpleData>
{
    protected readonly ICounterService _counterService;

    public SimpleQuest(ICounterService counterService)
        => _counterService = counterService;

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _counterService.Add();

        return QuestResult.Ok();
    }
}

public class QuestWithDataRequiredConfigure : RequiredConfigure<SimpleQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public QuestWithDataRequiredConfigure(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public async override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}