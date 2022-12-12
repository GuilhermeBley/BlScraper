using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;

public class QuestExceptionConfigureFilterTest : IQuestExceptionConfigureFilter
{
    private readonly IRouteService _routeService;

    public QuestExceptionConfigureFilterTest(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task OnOccursException(Exception ex, object data)
    {
        await Task.CompletedTask;

        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
    }
}