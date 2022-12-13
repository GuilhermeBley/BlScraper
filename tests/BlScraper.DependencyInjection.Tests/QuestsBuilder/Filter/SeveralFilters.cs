using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Model;
using BlScraper.Results;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class SeveralFilters : IAllWorksEndConfigureFilter, IDataCollectedConfigureFilter, 
    IDataFinishedConfigureFilter, IGetArgsConfigureFilter, IQuestCreatedConfigureFilter, IQuestExceptionConfigureFilter
{
    private readonly IRouteService _routeService;

    public SeveralFilters(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task GetArgs(object[] args)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(GetArgs)));
    }

    public async Task OnCollected(IEnumerable<object> dataCollected)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnCollected)));
    }

    public async Task OnCreated(IQuest questCreated)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)));
    }

    public async Task OnDataFinished(ResultBase<object> resultFinished)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnDataFinished)));
    }

    public async Task OnFinished(EndEnumerableModel results)
    {   
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnFinished)));
    }

    public async Task OnOccursException(Exception ex, object data, QuestResult result)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
    }
}