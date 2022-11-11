using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class DataFinishedQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}

public class DataFinishedRequired : RequiredConfigure<DataFinishedQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public override bool IsRequiredDataFinished => true;

    public DataFinishedRequired(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class DataFinishedConfigure : IDataFinishedConfigure<DataFinishedQuest, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public DataFinishedConfigure(IRouteService routeService)
    {
        _routeService = routeService;
    }
    public async Task OnDataFinished(ResultBase<PublicSimpleData> resultFinished)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)));
    }
}