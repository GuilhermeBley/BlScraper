using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class WithoutDataCollectedQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class WithoutDataCollectedRequired : RequiredConfigure<WithoutDataCollectedQuest, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public override bool IsRequiredDataCollected => true;

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return Enumerable.Empty<PublicSimpleData>();
    }
}