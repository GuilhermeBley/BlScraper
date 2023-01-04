using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Model.Context;
using BlScraper.Model;
using BlScraper.Results;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class AllConfigureQuestsWithContext : Quest<PublicSimpleData>
{
    private bool _shouldHaveException = true;
    private readonly IScrapContextAcessor _contextAcessor;
    private readonly IRouteObjectService _routeService;

    public AllConfigureQuestsWithContext(IScrapContextAcessor contextAcessor, IRouteObjectService route)
    {
        _contextAcessor = contextAcessor;
        _routeService = route;
    }

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        if (_shouldHaveException)
        {
            _shouldHaveException = false;
            throw new Exception();
        }

        _routeService.Add(this.GetType().GetMethod(nameof(Execute)), _contextAcessor.ScrapContext);
        return QuestResult.Ok();
    }
}


#region Required Configure

public class AllConfigureQuestsWithContextRequiredConfigure : RequiredConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private IServiceMocPublicSimpleData _serviceData;
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _scraperInfo;

    public override int initialQuantity => 5;
    public override bool IsRequiredDataFinished => true;
    public override bool IsRequiredAllWorksEnd => true;
    public override bool IsRequiredArgs => true;
    public override bool IsRequiredDataCollected => true;
    public override bool IsRequiredQuestCreated => true;
    public override bool IsRequiredQuestException => true;

    public AllConfigureQuestsWithContextRequiredConfigure(IServiceMocPublicSimpleData serviceData, IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _serviceData = serviceData;
        _routeService = routeService;
        _scraperInfo = scrapContextAcessor.RequiredScrapContext;
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetData)), _scraperInfo);
        return await _serviceData.GetDataSearch();
    }
}

public class AllConfigureQuestsWithContextAllWorkEndConfigure : IAllWorksEndConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextAllWorkEndConfigure(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public void OnFinished(EndEnumerableModel results)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextDataCollectedConfigure : IDataCollectedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextDataCollectedConfigure(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public void OnCollected(IEnumerable<PublicSimpleData> dataCollected)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextDataFinishedConfigure : IDataFinishedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextDataFinishedConfigure(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public void OnDataFinished(ResultBase<PublicSimpleData> resultFinished)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextGetArgsConfigure : IGetArgsConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextGetArgsConfigure(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public object[] GetArgs()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)), _contextAcessor.ScrapContext);
        return new object[0];
    }
}

public class AllConfigureQuestsWithContextQuestCreatedConfigure : IQuestCreatedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextQuestCreatedConfigure(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public void OnCreated(AllConfigureQuestsWithContext questCreated)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextQuestExceptionConfigure : IQuestExceptionConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextQuestExceptionConfigure(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)), _contextAcessor.ScrapContext);
        return QuestResult.Ok();
    }
}

#endregion

#region Filters

public class AllConfigureQuestsWithContextAllWorksEndConfigureFilter
    : IAllWorksEndConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextAllWorksEndConfigureFilter(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public async Task OnFinished(EndEnumerableModel results)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextDataCollectedConfigureFilter
    : IDataCollectedConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextDataCollectedConfigureFilter(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public async Task OnCollected(IEnumerable<object> dataCollected)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextDataFinishedConfigureFilter
    : IDataFinishedConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextDataFinishedConfigureFilter(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public async Task OnDataFinished(ResultBase resultFinished)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextGetArgsConfigureFilter
    : IGetArgsConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextGetArgsConfigureFilter(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public async Task GetArgs(object[] args)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextQuestCreatedConfigureFilter
    : IQuestCreatedConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextQuestCreatedConfigureFilter(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public async Task OnCreated(IQuest questCreated)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)), _contextAcessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextQuestExceptionConfigureFilter
    : IQuestExceptionConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAcessor _contextAcessor;

    public AllConfigureQuestsWithContextQuestExceptionConfigureFilter(IRouteObjectService routeService, IScrapContextAcessor scrapContextAcessor)
    {
        _routeService = routeService;
        _contextAcessor = scrapContextAcessor;
    }

    public async Task OnOccursException(Exception ex, object data, QuestResult result)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)), _contextAcessor.ScrapContext);
    }
}

#endregion