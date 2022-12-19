using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class WithQuestExceptionQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new Exception(Guid.NewGuid().ToString());
    }
}

public class WithQuestExceptionRequired : RequiredConfigure<WithQuestExceptionQuest, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public override bool IsRequiredQuestException => true;

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return PublicSImpleDataFactory.GetData(10);
    }
}

public class WithQuestExceptionConfigure : IQuestExceptionConfigure<WithQuestExceptionQuest, PublicSimpleData>
{
    private readonly IRouteService _routeService;

    public WithQuestExceptionConfigure(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public QuestResult OnOccursException(Exception ex, PublicSimpleData data)
    {
        _routeService.Add(this.GetType().GetMethod(nameof(OnOccursException)));
        return QuestResult.Ok();
    }
}