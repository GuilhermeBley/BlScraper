using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class WithoutQuestExceptionNonRequired : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new Exception(Guid.NewGuid().ToString());
    }
}

public class WithoutQuestExceptionNonRequiredExRequired : RequiredConfigure<WithoutQuestExceptionNonRequired, PublicSimpleData>
{
    public override int initialQuantity => 1;

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return PublicSImpleDataFactory.GetData(10);
    }
}