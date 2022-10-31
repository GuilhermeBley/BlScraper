using BlScraper.DependencyInjection.Builder;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.Mocks;

public class ScrapBuilderFake : IScrapBuilder
{
    public IModelScraper? CreateModelByQuestOrDefault(string name, int initialQuantity)
    {
        throw new NotImplementedException();
    }
}