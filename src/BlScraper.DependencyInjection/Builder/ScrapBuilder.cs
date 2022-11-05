using BlScraper.DependencyInjection.ConfigureBuilder;
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
        _assemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Instance with service provider
    /// </summary>
    /// <param name="serviceProvider"></param>
    public ScrapBuilder(IServiceProvider serviceProvider, AssemblyBuilderAdd assemblyBuilderAdd)
    {
        _serviceProvider = serviceProvider;
        foreach (var assembly in assemblyBuilderAdd.Assemblies)
        {
            _assemblies.Add(assembly);
        }
    }

    public IModelScraper? CreateModelByQuestOrDefault(string name, int initialQuantity)
    {
        Type? questTypeFinded = null;

        lock (_lock)
            foreach (var assembly in _assemblies)
            {
                var localQuestTypeFinded
                    = MapClassFromAssemblie(assembly).FirstOrDefault(pair => pair.Key == name.ToUpper()).Value;

                if (questTypeFinded is not null &&
                    localQuestTypeFinded is not null)
                    throw new ArgumentException($"Duplicate QuestTypes with name {name} was found.");

                if (localQuestTypeFinded is null)
                    continue;

                questTypeFinded = localQuestTypeFinded;
            }

        if (questTypeFinded is null)
            throw new ArgumentNullException($"QuestTypes with name {name} wasn't found.");

        return Create(questTypeFinded, initialQuantity);
    }

    private IModelScraper? Create(
        Type questType, int initialQuantity)
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

        var modelScraperType = modelScraperGenericType.MakeGenericType(new Type[] { questType, tData });

        return
            (IModelScraper?)Activator.CreateInstance(
                modelScraperType,
                new object[]
                {
                    initialQuantity
                }
            );
    }

    /// <summary>
    /// Map all classes which is a instance of type <see cref="BlScraper.Model.Quest{TData}"/>
    /// </summary>
    /// <param name="assembly">assembly types</param>
    /// <returns>Type list of quests</returns>
    /// <exception cref="ArgumentException"/>
    private static IEnumerable<KeyValuePair<string, Type>> MapClassFromAssemblie(System.Reflection.Assembly assembly)
    {
        Dictionary<string, Type> dictionaryTypeQuests = new();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass &&
                !type.IsAbstract &&
                IsSubclassOfRawGeneric(typeof(BlScraper.Model.Quest<>), type))
            {
                var normalizedName = type.Name.ToUpper();
                if (dictionaryTypeQuests.ContainsKey(normalizedName))
                    throw new ArgumentException($"Duplicate names with value {normalizedName}.");
                dictionaryTypeQuests.Add(normalizedName, type);
            }
        }

        return dictionaryTypeQuests;
    }

    private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType!;
        }
        return false;
    }
}