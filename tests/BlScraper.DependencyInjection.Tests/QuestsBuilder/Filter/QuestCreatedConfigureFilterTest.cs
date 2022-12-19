using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Model;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class QuestCreatedConfigureFilterTest : IQuestCreatedConfigureFilter
{
    private readonly IRouteService _routeService;

    public QuestCreatedConfigureFilterTest(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnCreated(IQuest questCreated)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnCreated)));
    }
}