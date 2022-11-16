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

    public IModelScraper CreateModelByQuestType(Type type)
    {
        throw new NotImplementedException();
    }

    public IModelScraper CreateModelByQuestType<TQuest>()
    {
        throw new NotImplementedException();
    }

    public IModelScraper? CreateModelByQuestTypeOrDefault(Type type)
    {
        throw new NotImplementedException();
    }

    public IModelScraper? CreateModelByQuestTypeOrDefault<TQuest>()
    {
        throw new NotImplementedException();
    }
}