using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.FakeProject;

public class QuestWithoutConfig : Quest<PublicDataInOtherProject>
{
    public override QuestResult Execute(PublicDataInOtherProject data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}