using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;
using BlScraper.Model;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class QuestWithFilterImplemented : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}

public class QuestWithFilterImplementedRequired : RequiredConfigure<QuestWithFilterImplemented, PublicSimpleData>
{
    public override int initialQuantity => 1;
    public QuestWithFilterImplementedRequired()
        : base(typeof(AllWorksEndConfigureFilterTest), typeof(AllWorksEndConfigureFilterUniqueImplemented))
    {
        
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return PublicSImpleDataFactory.GetData(10);
    }
}

public class AllWorksEndConfigureFilterUniqueImplemented : IAllWorksEndConfigureFilter<QuestWithFilterImplemented, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public AllWorksEndConfigureFilterUniqueImplemented(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnFinished(EndEnumerableModel results)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }
}