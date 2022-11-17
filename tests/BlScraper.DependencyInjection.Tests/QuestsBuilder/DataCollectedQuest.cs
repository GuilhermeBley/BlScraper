using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class DataCollectedQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}

public class DataCollectedRequired : RequiredConfigure<DataCollectedQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public override bool IsRequiredDataCollected => true;

    public DataCollectedRequired(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class DataCollectedConfigure : IDataCollectedConfigure<DataCollectedQuest, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public DataCollectedConfigure(IRouteService routeService)
    {
        _routeService = routeService;
    }
    public void OnCollected(IEnumerable<PublicSimpleData> dataCollected)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)));
    }
}