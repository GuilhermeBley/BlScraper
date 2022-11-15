using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class InheritFromSimpleQuestTwoConfig : SimpleQuest
{
    public InheritFromSimpleQuestTwoConfig(ICounterService counterService) : base(counterService)
    {
    }

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _counterService.Add();

        return QuestResult.Ok();
    }
}

public class InheritFromSimpleQuestOneConfigure : RequiredConfigure<InheritFromSimpleQuestTwoConfig, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public InheritFromSimpleQuestOneConfigure(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public sealed async override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class InheritFromSimpleQuestTwoConfigure : InheritFromSimpleQuestOneConfigure
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;
    
    public InheritFromSimpleQuestTwoConfigure(IServiceMocPublicSimpleData serviceData) : base(serviceData)
    {
        _serviceData = serviceData;
    }

}