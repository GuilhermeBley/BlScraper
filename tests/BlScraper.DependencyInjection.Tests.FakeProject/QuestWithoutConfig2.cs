using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.FakeProject;

public class QuestWithoutConfig2 : Quest<PublicDataInOtherProject>
{
    public override QuestResult Execute(PublicDataInOtherProject data, CancellationToken cancellationToken = default)
    {
        return QuestResult.Ok();
    }
}