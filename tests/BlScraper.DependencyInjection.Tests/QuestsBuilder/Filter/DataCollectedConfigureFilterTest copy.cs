using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public abstract class DataCollectedConfigureFilterAbstract : IDataCollectedConfigureFilter
{
    private readonly IRouteService _routeService;

    public DataCollectedConfigureFilterAbstract(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnCollected(IEnumerable<object> dataCollected)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)));
    }
}