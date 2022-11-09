using BlScraper.DependencyInjection.Builder;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.Mocks;

public class ScrapBuilderFake : IScrapBuilder
{
    public IModelScraper CreateModelByQuest(string name)
    {
        throw new NotImplementedException();
    }

    public IModelScraper? CreateModelByQuestOrDefault(string name)
    {
        throw new NotImplementedException();
    }
}