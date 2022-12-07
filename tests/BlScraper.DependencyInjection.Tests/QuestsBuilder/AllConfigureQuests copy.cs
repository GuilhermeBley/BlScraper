using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;
using BlScraper.Results;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class AllWithOutRequiredConfigureQuests : Quest<PublicSimpleData>
{
    private bool _isFirst = true;
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        if (_isFirst)
        {
            _isFirst = false;
            throw new Exception();
        }

        Thread.Sleep(10);
        return QuestResult.Ok();
    }
}

public class AllWorkEndWithoutRequiredConfigureAll : IAllWorksEndConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public AllWorkEndWithoutRequiredConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnFinished(EndEnumerableModel results)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }
}

public class RequiredWithoutRequiredConfigureAll : RequiredConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private IServiceMocPublicSimpleData _serviceData;
    private readonly IRouteService _routeService;

    public override int initialQuantity => 5;
    public override bool IsRequiredDataFinished => true;
    public override bool IsRequiredAllWorksEnd => true;
    public override bool IsRequiredArgs => true;
    public override bool IsRequiredDataCollected => true;
    public override bool IsRequiredQuestCreated => true;
    public override bool IsRequiredQuestException => true;

    public RequiredWithoutRequiredConfigureAll(IServiceMocPublicSimpleData serviceData, IRouteService routeService)
    {
        _serviceData = serviceData;
        _routeService = routeService;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetData)));
        return await _serviceData.GetDataSearch();
    }
}

public class DataCollectedWithoutRequiredConfigureAll : IDataCollectedConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public DataCollectedWithoutRequiredConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnCollected(IEnumerable<PublicSimpleData> dataCollected)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)));
    }
}

public class DataFinishedWithoutRequiredConfigureAll : IDataFinishedConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public DataFinishedWithoutRequiredConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnDataFinished(ResultBase<PublicSimpleData> resultFinished)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)));
    }
}

public class GetArgsWithoutRequiredConfigureAll : IGetArgsConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public GetArgsWithoutRequiredConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public object[] GetArgs()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)));
        return new object[0];
    }
}

public class QuestCreatedWithoutRequiredConfigureAll : IQuestCreatedConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public QuestCreatedWithoutRequiredConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnCreated(AllWithOutRequiredConfigureQuests questCreated)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)));
    }
}

public class QuestExceptionWithoutRequiredConfigureAll : IQuestExceptionConfigure<AllWithOutRequiredConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public QuestExceptionWithoutRequiredConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
        return QuestResult.Ok();
    }
}
