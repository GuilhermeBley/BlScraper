using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.Model;
using BlScraper.Model;
using Microsoft.Extensions.DependencyInjection;

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

        foreach (var assembly in _assemblies)
        {
            var localQuestTypeFinded
                = MapClassByQuestNameAssemblie(assembly)
                    .FirstOrDefault(pair => pair.Key == name.ToUpper()).Value;

            if (questTypeFinded is not null &&
                localQuestTypeFinded is not null)
                throw new ArgumentException($"Duplicate QuestTypes with name {name} was found.");

            if (localQuestTypeFinded is null)
                continue;

            questTypeFinded = localQuestTypeFinded;
        }

        if (questTypeFinded is null)
            throw new ArgumentNullException($"QuestTypes with name {name} wasn't found.");

        return Create(questTypeFinded);
    }

    private IModelScraper? Create(Type questType)
    {
        var model = new ScrapModelsInternal(questType);

        SetParametersOnModel(model);

        var modelScraperGenericType = typeof(ModelScraperService<,>);

        var modelScraperType = modelScraperGenericType.MakeGenericType(model.QuestType, model.DataType);
        
        return
            (IModelScraper?)Activator.CreateInstance(
                modelScraperType,
                new object[]
                {
                    ((IRequiredConfigure)model.InstaceRequired!).initialQuantity,
                    _serviceProvider

                }
            );
    }

    private void SetParametersOnModel(ScrapModelsInternal model)
    {
        #region RequiredConfigure
        Type typeInstaceRequired =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(RequiredConfigure<,>).MakeGenericType(model.QuestType, model.DataType)) 
            ?? throw new ArgumentException($"Is necessary a {typeof(RequiredConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        model.InstaceRequired =
            ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceRequired);

        var configure = (IRequiredConfigure)model.InstaceRequired;
        #endregion

        #region IOnAllWorksEndConfigure
        Type? typeInstaceAllWorksEnd =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IOnAllWorksEndConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeInstaceAllWorksEnd is null && configure.IsRequiredAllWorksEnd)
            throw new ArgumentException($"Is necessary a {typeof(IOnAllWorksEndConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        if (typeInstaceAllWorksEnd is not null)
            model.InstanceAllWorksEnd = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceAllWorksEnd);
        #endregion
        
        #region IGetArgsConfigure
        Type? typeInstaceArgs =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IGetArgsConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeInstaceArgs is null && configure.IsRequiredArgs)
            throw new ArgumentException($"Is necessary a {typeof(IGetArgsConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        if (typeInstaceArgs is not null)
            model.InstanceArgs = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceArgs);
        #endregion
                
        #region IOnDataCollectedConfigure
        Type? typeDataCollected =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IOnDataCollectedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeDataCollected is null && configure.IsRequiredDataCollected)
            throw new ArgumentException($"Is necessary a {typeof(IOnDataCollectedConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        if (typeDataCollected is not null)
            model.InstanceDataCollected = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeDataCollected);
        #endregion
                        
        #region IDataFinishedConfigure
        Type? typeDataFinished =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IDataFinishedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeDataFinished is null && configure.IsRequiredDataFinished)
            throw new ArgumentException($"Is necessary a {typeof(IDataFinishedConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        if (typeDataFinished is not null)
            model.InstanceDataFinished = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeDataFinished);
        #endregion
        
        #region IOnQuestCreatedConfigure
        Type? typeQuestCreated =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IOnQuestCreatedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeQuestCreated is null && configure.IsRequiredQuestCreated)
            throw new ArgumentException($"Is necessary a {typeof(IOnQuestCreatedConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        if (typeQuestCreated is not null)
            model.InstanceQuestCreated = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeQuestCreated);
        #endregion

        #region IQuestExceptionConfigure
        Type? typeQuestException =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IQuestExceptionConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeQuestException is null && configure.IsRequiredQuestException)
            throw new ArgumentException($"Is necessary a {typeof(IQuestExceptionConfigure<,>).Name} of quest {model.QuestType.FullName}.");

        if (typeQuestException is not null)
            model.InstanceQuestException = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeQuestException);
        #endregion
    }

    /// <summary>
    /// Map all classes which is a instance of type <see cref="BlScraper.Model.Quest{TData}"/>
    /// </summary>
    /// <param name="assembly">assembly types</param>
    /// <returns>Type list of quests</returns>
    /// <exception cref="ArgumentException"/>
    private static IEnumerable<KeyValuePair<string, Type>> MapClassByQuestNameAssemblie(System.Reflection.Assembly assembly)
    {
        Dictionary<string, Type> dictionaryTypeQuests = new();

        foreach (Type type in TypeUtils.MapClassFromAssemblie(assembly, typeof(BlScraper.Model.Quest<>)))
        {
            var normalizedName = type.Name.ToUpper();
            if (dictionaryTypeQuests.ContainsKey(normalizedName))
                throw new ArgumentException($"Duplicate names with value {normalizedName} in {type.FullName} and {dictionaryTypeQuests[normalizedName].FullName}.");
            dictionaryTypeQuests.Add(normalizedName, type);
        }

        return dictionaryTypeQuests;
    }

}