using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class AllConfigureQuests : Quest<PublicSimpleData>
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

public class AllWorkEndConfigureAll : IOnAllWorksEndConfigure<AllConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public AllWorkEndConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnFinished(IEnumerable<ResultBase<Exception?>> results)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }
}

public class RequiredConfigureAll : RequiredConfigure<AllConfigureQuests, PublicSimpleData>
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

    public RequiredConfigureAll(IServiceMocPublicSimpleData serviceData, IRouteService routeService)
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

public class DataCollectedConfigureAll : IOnDataCollectedConfigure<AllConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public DataCollectedConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnCollected(IEnumerable<PublicSimpleData> dataCollected)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)));
    }
}

public class DataFinishedConfigureAll : IDataFinishedConfigure<AllConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public DataFinishedConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnDataFinished(ResultBase<PublicSimpleData> resultFinished)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)));
    }
}

public class GetArgsConfigureAll : IGetArgsConfigure<AllConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public GetArgsConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public object[] GetArgs()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)));
        return new object[0];
    }
}

public class QuestCreatedConfigureAll : IOnQuestCreatedConfigure<AllConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public QuestCreatedConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public void OnCreated(AllConfigureQuests questCreated)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)));
    }
}

public class QuestExceptionConfigureAll : IQuestExceptionConfigure<AllConfigureQuests, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public QuestExceptionConfigureAll(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
        return QuestResult.Ok();
    }
}
