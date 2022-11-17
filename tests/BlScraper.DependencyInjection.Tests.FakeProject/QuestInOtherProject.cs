using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.FakeProject;

public class QuestInOtherProject : Quest<PublicDataInOtherProject>
{
    public override QuestResult Execute(PublicDataInOtherProject data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}

public class QuestInOtherProjectRequiredConfigure : RequiredConfigure<QuestInOtherProject, PublicDataInOtherProject>
{
    public override int initialQuantity => 1;

    public override async Task<IEnumerable<PublicDataInOtherProject>> GetData()
    {
        await Task.CompletedTask;
        return new List<PublicDataInOtherProject>()
        {
            new PublicDataInOtherProject(),
            new PublicDataInOtherProject(),
            new PublicDataInOtherProject(),
            new PublicDataInOtherProject()
        };
    }
}