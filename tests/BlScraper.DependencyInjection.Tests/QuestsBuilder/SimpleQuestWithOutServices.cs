using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class SimpleQuestWithOutServices : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return QuestResult.Ok();
    }
}

public class QuestWithoutServicesRequiredConfigure : RequiredConfigure<SimpleQuestWithOutServices, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public async override Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return Enumerable.Empty<PublicSimpleData>();
    }
}