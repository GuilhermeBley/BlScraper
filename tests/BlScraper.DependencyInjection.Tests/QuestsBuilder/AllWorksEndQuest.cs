using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class AllWorksEndQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}

public class AllWorksEndRequired : RequiredConfigure<AllWorksEndQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public override bool IsRequiredAllWorksEnd => true;

    public AllWorksEndRequired(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class AllWorksEndConfigure : IOnAllWorksEndConfigure<AllWorksEndQuest, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public AllWorksEndConfigure(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnFinished(IEnumerable<ResultBase<Exception?>> results)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }
}