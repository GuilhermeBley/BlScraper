using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class QuestCreatedQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class QuestCreatedRequired : RequiredConfigure<QuestCreatedQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public override bool IsRequiredQuestCreated => true;

   public QuestCreatedRequired(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class QuestCreatedConfigure : IOnQuestCreatedConfigure<QuestCreatedQuest, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public QuestCreatedConfigure(IRouteService routeService)
    {
        _routeService = routeService;
    }
    public void OnCreated(QuestCreatedQuest questCreated)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)));
    }
}