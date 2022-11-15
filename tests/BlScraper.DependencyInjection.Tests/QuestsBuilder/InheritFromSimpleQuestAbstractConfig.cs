using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class InheritFromSimpleQuestAbstractConfig : SimpleQuest
{

    public InheritFromSimpleQuestAbstractConfig(ICounterService counterService) : base(counterService)
    {
    }

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _counterService.Add();

        return QuestResult.Ok();
    }
}

public abstract class InheritFromSimpleQuestAbstractConfigure : RequiredConfigure<InheritFromSimpleQuestAbstractConfig, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public InheritFromSimpleQuestAbstractConfigure(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public sealed async override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class InheritFromSimpleQuestTwoAbstractConfigure : InheritFromSimpleQuestAbstractConfigure
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;
    
    public InheritFromSimpleQuestTwoAbstractConfigure(IServiceMocPublicSimpleData serviceData) : base(serviceData)
    {
        _serviceData = serviceData;
    }

}