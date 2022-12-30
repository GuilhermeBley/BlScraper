namespace BlScraper.DependencyInjection.Tests.Services;

public interface IServiceMocPublicSimpleData
{
    Task<IEnumerable<PublicSimpleData>> GetDataSearch();
}

public class ServiceMocPublicSimpleData : IServiceMocPublicSimpleData
{
    private readonly int _countDataAvailable;
    private readonly int _timePerEnumerable;

    public ServiceMocPublicSimpleData(int countDataAvailable, int timePerEnumerable = 0)
    {
        _countDataAvailable = countDataAvailable;
        _timePerEnumerable = timePerEnumerable;
    }

    public async Task<IEnumerable<PublicSimpleData>> GetDataSearch()
    {
        if (_timePerEnumerable == 0)
            await Task.CompletedTask;
        else
            await Task.Delay(_timePerEnumerable);
        return PublicSImpleDataFactory.GetData(_countDataAvailable);
    }
}