using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class QuestExceptionQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new Exception();
    }
}

public class QuestExceptionQuestRequired : RequiredConfigure<QuestExceptionQuest, PublicSimpleData>
{
    private readonly IServiceMocPublicSimpleData _serviceData;

    public override int initialQuantity => 1;

    public override bool IsRequiredQuestException => true;

    public QuestExceptionQuestRequired(IServiceMocPublicSimpleData serviceData)
    {
        _serviceData = serviceData;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        return await _serviceData.GetDataSearch();
    }
}

public class QuestExceptionConfigure : IQuestExceptionConfigure<QuestExceptionQuest, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public QuestExceptionConfigure(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
        return QuestResult.Ok();
    }
}