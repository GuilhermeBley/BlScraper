using BlScraper.DependencyInjection.ConfigureModel.Filter;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class GetArgsConfigureFilterGeneric<T> : IGetArgsConfigureFilter
{
    private readonly IRouteService _routeService;

    public GetArgsConfigureFilterGeneric(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task GetArgs(object[] args)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)));
    }
}