using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class InheritFromSimpleQuest : SimpleQuest
{
    public InheritFromSimpleQuest(ICounterService counterService) : base(counterService)
    {
    }

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _counterService.Add();

        return QuestResult.Ok();
    }
}

public class InheritFromSimpleQuestConfigure : RequiredConfigure<InheritFromSimpleQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public InheritFromSimpleQuestConfigure(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public async override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}