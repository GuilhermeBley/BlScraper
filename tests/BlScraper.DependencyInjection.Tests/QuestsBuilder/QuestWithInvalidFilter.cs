using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class QuestWithInvalidFilter : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class QuestWithInvalidFilterRequired : RequiredConfigure<QuestWithInvalidFilter, PublicSimpleData>
{
    public override int initialQuantity => 1;
    public QuestWithInvalidFilterRequired()
        : base(typeof(object))
    {
        
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return PublicSImpleDataFactory.GetData(10);
    }
}