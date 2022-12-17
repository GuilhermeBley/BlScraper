using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;
using BlScraper.Model;
using BlScraper.Results.Models;

namespace BlScraper.DependencyInjection.Tests.QuestsBuilder;

public class QuestWithFilterNotImplemented : Quest<PublicSimpleData>
{
    public override QuestResult Execute(PublicSimpleData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class QuestWithFilterNotImplementedRequired : RequiredConfigure<QuestWithFilterNotImplemented, PublicSimpleData>
{
    public override int initialQuantity => 1;
    public QuestWithFilterNotImplementedRequired()
        : base(typeof(AllWorksEndConfigureFilterTest), typeof(AllWorksEndConfigureFilterUnique))
    {
        
    }

    public override async Task<IEnumerable<PublicSimpleData>> GetData()
    {
        await Task.CompletedTask;
        return PublicSImpleDataFactory.GetData(10);
    }
}

public class AllWorksEndConfigureFilterUnique : IAllWorksEndConfigureFilter<QuestWithFilterNotImplemented, PublicSimpleData>
{
    public Task OnFinished(EndEnumerableModel results)
    {
        throw new NotImplementedException();
    }
}