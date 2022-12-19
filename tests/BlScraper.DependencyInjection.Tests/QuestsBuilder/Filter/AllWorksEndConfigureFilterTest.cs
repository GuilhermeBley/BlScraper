using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class AllWorksEndConfigureFilterTest : IAllWorksEndConfigureFilter
{
    private readonly IRouteService _routeService;

    public AllWorksEndConfigureFilterTest(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnFinished(EndEnumerableModel results)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }
}