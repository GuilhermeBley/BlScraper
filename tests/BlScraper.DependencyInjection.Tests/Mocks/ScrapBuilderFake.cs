using BlScraper.DependencyInjection.Builder;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.Mocks;

public class ScrapBuilderFake : IScrapBuilder
{
    public IModelScraper CreateModelByQuestName(string name)
    {
        throw new NotImplementedException();
    }

    public IModelScraper? CreateModelByQuestNameOrDefault(string name)
    {
        throw new NotImplementedException();
    }
}