using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.Model.Context;
using BlScraper.Model;
using BlScraper.Results;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class AllConfigureQuestsWithContext : Quest<PublicSimpleData>
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

public class AllConfigureQuestsWithContextAllWorkEndConfigure : IAllWorksEndConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public AllConfigureQuestsWithContextAllWorkEndConfigure(IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public void OnFinished(EndEnumerableModel results)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }
}

public class AllConfigureQuestsWithContextRequiredConfigure : RequiredConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private IServiceMocPublicSimpleData _serviceData;
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public override int initialQuantity => 5;
    public override bool IsRequiredDataFinished => true;
    public override bool IsRequiredAllWorksEnd => true;
    public override bool IsRequiredArgs => true;
    public override bool IsRequiredDataCollected => true;
    public override bool IsRequiredQuestCreated => true;
    public override bool IsRequiredQuestException => true;

    public AllConfigureQuestsWithContextRequiredConfigure(IServiceMocPublicSimpleData serviceData, IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _serviceData = serviceData;
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetData)));
        return await _serviceData.GetDataSearch();
    }
}

public class AllConfigureQuestsWithContextDataCollectedConfigure : IDataCollectedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public AllConfigureQuestsWithContextDataCollectedConfigure(IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public void OnCollected(IEnumerable<PublicSimpleData> dataCollected)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)));
    }
}

public class AllConfigureQuestsWithContextDataFinishedConfigure : IDataFinishedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public AllConfigureQuestsWithContextDataFinishedConfigure(IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public void OnDataFinished(ResultBase<PublicSimpleData> resultFinished)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)));
    }
}

public class AllConfigureQuestsWithContextGetArgsConfigure : IGetArgsConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public AllConfigureQuestsWithContextGetArgsConfigure(IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public object[] GetArgs()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)));
        return new object[0];
    }
}

public class AllConfigureQuestsWithContextQuestCreatedConfigure : IQuestCreatedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public AllConfigureQuestsWithContextQuestCreatedConfigure(IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public void OnCreated(AllConfigureQuestsWithContext questCreated)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)));
    }
}

public class AllConfigureQuestsWithContextQuestExceptionConfigure : IQuestExceptionConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteService _routeService;
    private readonly IScrapContextAcessor _scrapContextAcessor;

    public AllConfigureQuestsWithContextQuestExceptionConfigure(IRouteService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _scrapContextAcessor = scrapContextAcessor;
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
        return QuestResult.Ok();
    }
}
