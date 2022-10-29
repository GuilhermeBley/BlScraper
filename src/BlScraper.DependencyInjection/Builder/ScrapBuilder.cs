using BlScraper.DependencyInjection.Model;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Builder;

/// <summary>
/// Implement of assembly
/// </summary>
internal class ScrapBuilder : IScrapBuilder
{
    /// <summary>
    /// Service Providier
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    private HashSet<System.Reflection.Assembly> _assemblies = new();

    /// <summary>
    /// Lock object
    /// </summary>
    private object _lock = new();

    /// <summary>
    /// Instance with service provider
    /// </summary>
    /// <param name="serviceProvider"></param>
    public ScrapBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Try add new assemblies to map
    /// </summary>
    /// <param name="assembly">Assemblie to add</param>
    public void AddAssembly(System.Reflection.Assembly assembly)
    {
        lock(_lock)
        {
            _assemblies.Add(assembly);
        }
    }

    public IModelScraper? CreateModelByQuestOrDefault(string name)
    {
        
        throw new NotImplementedException();
    }

    private IModelScraper? Create(Type questType)
    {
        if (!typeof(Quest<>).IsAssignableFrom(questType))
            return null;

        Type? tData = null;
        foreach (var interfaceQuest in questType.GetInterfaces())
        {
            if (interfaceQuest.Equals(typeof(Quest<>)))
            {
                tData = interfaceQuest.GetGenericArguments().FirstOrDefault();
            }
        }

        if (tData is null)
            return null;

        var modelScraperGenericType = typeof(ModelScraperService<,>);

        var modelScraperType = modelScraperGenericType.MakeGenericType(new Type[]{ questType, tData });

        return
            (IModelScraper?)Activator.CreateInstance(
                modelScraperType,
                new object[]
                {
                }
            );
    }
}