using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class DataFinishedConfigureFilterTest : IDataFinishedConfigureFilter
{
    private readonly IRouteService _routeService;

    public DataFinishedConfigureFilterTest(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnDataFinished(ResultBase resultFinished)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)));
    }
}