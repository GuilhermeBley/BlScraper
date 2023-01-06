namespace BlScraper.DependencyInjection.Tests.Services;

public interface ICountScrapService
{
    int CountScrap { get; }
}

internal class CountScrapService : ICountScrapService
{
    private int _countScrap;
    public int CountScrap => _countScrap;

    public CountScrapService(int countScrap)
    {
        _countScrap = countScrap;
    }
}