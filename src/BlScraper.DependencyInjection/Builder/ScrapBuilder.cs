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

    public IModelScraper? CreateModelByQuestOrDefault(string name, int initialQuantity)
    {
        Type? questTypeFinded = null;

        lock(_lock)
        foreach (var assembly in _assemblies)
        {
            var localQuestTypeFinded = Type.GetType($"{assembly.FullName}.{name}", throwOnError: false, ignoreCase: true);

            if (questTypeFinded is not null &&
                localQuestTypeFinded is not null)
                throw new ArgumentException($"Duplicate QuestTypes with name {name} was found.");

            if (questTypeFinded is null)
                continue;

            questTypeFinded = localQuestTypeFinded;
        }

        if (questTypeFinded is null)
            throw new ArgumentNullException($"QuestTypes with name {name} wasn't found.");
        
        return Create(questTypeFinded, initialQuantity);
    }

    private IModelScraper? Create(Type questType, int initialQuantity)
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
                    initialQuantity
                }
            );
    }
}