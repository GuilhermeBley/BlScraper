namespace BlScraper.DependencyInjection.Tests.Services;

public interface IServiceMocPublicSimpleData
{
    Task<IEnumerable<PublicSimpleData>> GetDataSearch();
}

public class ServiceMocPublicSimpleData : IServiceMocPublicSimpleData
{
    private readonly int _countDataAvailable;

    public ServiceMocPublicSimpleData(int countDataAvailable)
    {
        _countDataAvailable = countDataAvailable;
    }

    public async Task<IEnumerable<PublicSimpleData>> GetDataSearch()
    {
        await Task.CompletedTask;
        return PublicSImpleDataFactory.GetData(_countDataAvailable);
    }
}