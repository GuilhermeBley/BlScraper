namespace BlScraper.DependencyInjection.Tests.Services;

public interface IServiceNeedsObj
{
    Obj1 NeededObj { get; }
    Guid Id { get; }
}

public class ServiceNeedsObj : IServiceNeedsObj
{

    private Guid _id { get; } = Guid.NewGuid();
    public Guid Id => _id;
    public Obj1 _neededObj { get; }
    public Obj1 NeededObj => _neededObj;

    public ServiceNeedsObj(Obj1 neededObj)
    {
        _neededObj = neededObj;
    }
}