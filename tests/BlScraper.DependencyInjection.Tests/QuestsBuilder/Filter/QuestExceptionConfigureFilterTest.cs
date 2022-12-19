using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Model;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class QuestExceptionConfigureFilterTest : IQuestExceptionConfigureFilter
{
    private readonly IRouteService _routeService;

    public QuestExceptionConfigureFilterTest(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnOccursException(Exception ex, object data, QuestResult result)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
    }
}