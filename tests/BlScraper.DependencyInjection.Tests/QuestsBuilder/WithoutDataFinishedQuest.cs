using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class WithoutDataFinishedQuest : Quest<PublicQuest>
{
    public override QuestResult Execute(PublicQuest data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class WithoutDataFinishedRequired : RequiredConfigure<WithoutDataFinishedQuest, PublicQuest>
{
    public override int initialQuantity => 1;

    public override bool IsRequiredDataFinished => true;

    public override async Task<IEnumerable<PublicQuest>> GetData()
    {
        await Task.CompletedTask;
        return Enumerable.Empty<PublicQuest>();
    }
}