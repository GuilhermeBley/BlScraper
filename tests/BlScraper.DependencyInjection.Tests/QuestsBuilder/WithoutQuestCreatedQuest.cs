using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class WithoutQuestCreatedQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class WithoutQuestCreatedRequired : RequiredConfigure<WithoutQuestCreatedQuest, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public override bool IsRequiredQuestCreated => true;

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return Enumerable.Empty<PublicSimpleData>();
    }
}