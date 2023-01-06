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
    private readonly IScrapContextAccessor _contextAccessor;
    private readonly IRouteObjectService _routeService;

    public AllConfigureQuestsWithContext(IScrapContextAccessor contextAccessor, IRouteObjectService route)
    {
        _contextAccessor = contextAccessor;
        _routeService = route;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), _contextAccessor.ScrapContext);
    }

    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        if (_shouldHaveException)
        {
            _shouldHaveException = false;
            throw new Exception();
        }

        _routeService.Add(this.GetType().GetMethod(nameof(Execute)), _contextAccessor.ScrapContext);
        return QuestResult.Ok();
    }
}


#region Required Configure

public class AllConfigureQuestsWithContextRequiredConfigure : RequiredConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private IServiceMocPublicSimpleData _serviceData;
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAccessor _contextAccessor;

    public override int initialQuantity => 5;
    public override bool IsRequiredDataFinished => true;
    public override bool IsRequiredAllWorksEnd => true;
    public override bool IsRequiredArgs => true;
    public override bool IsRequiredDataCollected => true;
    public override bool IsRequiredQuestCreated => true;
    public override bool IsRequiredQuestException => true;

    public AllConfigureQuestsWithContextRequiredConfigure(IServiceMocPublicSimpleData serviceData, IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _serviceData = serviceData;
        _routeService = routeService;
        _contextAccessor = scrapcontextAccessor;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), _contextAccessor.ScrapContext);
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetData)), _contextAccessor.ScrapContext);
        return await _serviceData.GetDataSearch();
    }
}

public class AllConfigureQuestsWithContextAllWorkEndConfigure : IAllWorksEndConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextAllWorkEndConfigure(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public void OnFinished(EndEnumerableModel results)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)), _context);
    }
}

public class AllConfigureQuestsWithContextDataCollectedConfigure : IDataCollectedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextDataCollectedConfigure(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public void OnCollected(IEnumerable<PublicSimpleData> dataCollected)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)), _context);
    }
}

public class AllConfigureQuestsWithContextDataFinishedConfigure : IDataFinishedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextDataFinishedConfigure(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public void OnDataFinished(ResultBase<PublicSimpleData> resultFinished)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)), _context);
    }
}

public class AllConfigureQuestsWithContextGetArgsConfigure : IGetArgsConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo? _context;

    public AllConfigureQuestsWithContextGetArgsConfigure(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.ScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public object[] GetArgs()
    {
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)), _context);
        return new object[0];
    }
}

public class AllConfigureQuestsWithContextQuestCreatedConfigure : IQuestCreatedConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextQuestCreatedConfigure(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public void OnCreated(AllConfigureQuestsWithContext questCreated)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)), _context);
    }
}

public class AllConfigureQuestsWithContextQuestExceptionConfigure : IQuestExceptionConfigure<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextQuestExceptionConfigure(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)), _context);
        return QuestResult.Ok();
    }
}

#endregion

#region Filters

public class AllConfigureQuestsWithContextAllWorksEndConfigureFilter
    : IAllWorksEndConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextAllWorksEndConfigureFilter(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public async Task OnFinished(EndEnumerableModel results)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)), _context);
    }
}

public class AllConfigureQuestsWithContextDataCollectedConfigureFilter
    : IDataCollectedConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextDataCollectedConfigureFilter(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public async Task OnCollected(IEnumerable<object> dataCollected)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)), _context);
    }
}

public class AllConfigureQuestsWithContextDataFinishedConfigureFilter
    : IDataFinishedConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextDataFinishedConfigureFilter(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public async Task OnDataFinished(ResultBase resultFinished)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)), _context);
    }
}

[Obsolete("Args filters are not used.")]
public class AllConfigureQuestsWithContextGetArgsConfigureFilter
    : IGetArgsConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IScrapContextAccessor _contextAccessor;

    public AllConfigureQuestsWithContextGetArgsConfigureFilter(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _contextAccessor = scrapcontextAccessor;
    }

    public async Task GetArgs(object[] args)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)), _contextAccessor.ScrapContext);
    }
}

public class AllConfigureQuestsWithContextQuestCreatedConfigureFilter
    : IQuestCreatedConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextQuestCreatedConfigureFilter(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public async Task OnCreated(IQuest questCreated)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)), _context);
    }
}

public class AllConfigureQuestsWithContextQuestExceptionConfigureFilter
    : IQuestExceptionConfigureFilter<AllConfigureQuestsWithContext, PublicSimpleData>
{
    private readonly IRouteObjectService _routeService;
    private readonly IModelScraperInfo _context;

    public AllConfigureQuestsWithContextQuestExceptionConfigureFilter(IRouteObjectService routeService, IScrapContextAccessor scrapcontextAccessor)
    {
        _routeService = routeService;
        _context = scrapcontextAccessor.RequiredScrapContext;
        _routeService.Add(GetType().GetConstructors().FirstOrDefault(), scrapcontextAccessor.ScrapContext);
    }

    public async Task OnOccursException(Exception ex, object data, QuestResult result)
    {
        await Task.CompletedTask;
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)), _context);
    }
}

#endregion