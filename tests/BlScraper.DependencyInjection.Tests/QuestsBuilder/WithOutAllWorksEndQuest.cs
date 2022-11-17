using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class WithoutAllWorksEndQuest : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class WithOutAllWorksEndRequired : RequiredConfigure<WithoutAllWorksEndQuest, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public override bool IsRequiredAllWorksEnd => true;

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return Enumerable.Empty<PublicSimpleData>();
    }
}