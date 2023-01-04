using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

[Obsolete("Args filters are not used.")]
public class GetArgsConfigureFilterTest : IGetArgsConfigureFilter
{
    private readonly IRouteService _routeService;

    public GetArgsConfigureFilterTest(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task GetArgs(object[] args)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)));
    }
}