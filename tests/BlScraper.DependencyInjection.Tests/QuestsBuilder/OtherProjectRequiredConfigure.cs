using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.Tests.FakeProject;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class OtherProjectRequiredConfigure : RequiredConfigure<QuestWithoutConfig, PublicDataInOtherProject>
{
    public override int initialQuantity => 1;

    public override async Task<IEnumerable<PublicDataInOtherProject>> GetData()
    {
        await Task.CompletedTask;
        return new List<PublicDataInOtherProject>{
            new PublicDataInOtherProject(),
            new PublicDataInOtherProject(),
            new PublicDataInOtherProject(),
            new PublicDataInOtherProject()
        };
    }
}