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
    public ScrapBuilder(IServiceProvider serviceProvider, AssemblyBuilderAdd assemblyBuilderAdd)
    {
        _serviceProvider = serviceProvider;
        foreach (var assembly in assemblyBuilderAdd.Assemblies)
        {
            _assemblies.Add(assembly);
        }
    }

    public IModelScraper? CreateModelByQuestOrDefault(string name)
    {
        try
        {
            return CreateModelByQuest(name);
        }
        catch
        {
            return null;
        }
    }

    public IModelScraper CreateModelByQuest(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        Type? questTypeFinded = null;

        foreach (var assembly in _assemblies)
        {
            var localValuePair = 
                MapClassByQuestNameAssemblie(assembly)
                    .Where(pair => pair.Name == name.ToUpper());

            if (!localValuePair.Any())
                continue;

            if (localValuePair.Count() != 1)
                throw new ArgumentException($"Duplicate names with value {localValuePair.First().Name} in: {string.Join('\n', localValuePair.Select(pair => pair.Type.FullName))}.");

            var localQuestTypeFinded = localValuePair.First().Type;

            if (questTypeFinded is not null &&
                localQuestTypeFinded is not null)
                throw new ArgumentException($"Duplicate QuestTypes with name {name} was found in {localQuestTypeFinded.FullName} and {questTypeFinded.FullName}.", nameof(name));

            questTypeFinded = localQuestTypeFinded;
        }

        if (questTypeFinded is null)
            throw new ArgumentException($"QuestTypes with name {name} wasn't found. Check if the possible class target is public, non-obsolete and concrete.", nameof(name));

        return Create(questTypeFinded);
    }

    private IModelScraper Create(Type questType)
    {
        var model = new ScrapModelsInternal(questType);

        SetParametersOnModel(model);

        var modelScraperType = typeof(ModelScraperService<,>).MakeGenericType(model.QuestType, model.DataType);

        return
            (IModelScraper?)Activator.CreateInstance(
                modelScraperType,
                new object[]
                {
                    ((IRequiredConfigure)model.InstaceRequired!).initialQuantity,
                    _serviceProvider,
                    TypeUtils.CreateDelegateWithTarget(model.InstaceRequired.GetType().GetMethod("GetData"), model.InstaceRequired) ?? throw new ArgumentNullException("GetData"),
                    TypeUtils.CreateDelegateWithTarget(model.InstanceQuestException?.GetType().GetMethod("OnOccursException", new Type[] { typeof(Exception), model.DataType }), model.InstanceQuestException) ?? null!,
                    TypeUtils.CreateDelegateWithTarget(model.InstanceDataFinished?.GetType().GetMethod("OnDataFinished", new Type[] { typeof(Results.ResultBase<>).MakeGenericType(model.DataType) }), model.InstanceDataFinished) ?? null!,
                    TypeUtils.CreateDelegateWithTarget(model.InstanceAllWorksEnd?.GetType().GetMethod("OnFinished", new Type[] { typeof(IEnumerable<Results.ResultBase<Exception?>>) }), model.InstanceAllWorksEnd) ?? null!,
                    TypeUtils.CreateDelegateWithTarget(model.InstanceDataCollected?.GetType().GetMethod("OnCollected", new Type[] { typeof(IEnumerable<>).MakeGenericType(model.DataType) }), model.InstanceDataCollected) ?? null!,
                    TypeUtils.CreateDelegateWithTarget(model.InstanceQuestCreated?.GetType().GetMethod("OnCreated", new Type[] { model.QuestType }), model.InstanceQuestCreated) ?? null!,
                    (object[]?) model.InstanceArgs?.GetType().GetMethod("GetArgs")?.Invoke(model.InstanceArgs, null) ?? new object[0]
                }
            ) ?? throw new ArgumentNullException(nameof(IModelScraper));
    }

    private void SetParametersOnModel(ScrapModelsInternal model)
    {
        #region RequiredConfigure
        Type typeInstaceRequired = 
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(RequiredConfigure<,>).MakeGenericType(model.QuestType, model.DataType))
            ?? throw ThrowRequiredTypeNotFound(model, typeof(RequiredConfigure<,>));

        model.InstaceRequired =
            ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceRequired);

        var configure = (IRequiredConfigure)model.InstaceRequired;
        #endregion

        #region IOnAllWorksEndConfigure
        Type? typeInstaceAllWorksEnd =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IOnAllWorksEndConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeInstaceAllWorksEnd is null && configure.IsRequiredAllWorksEnd)
            throw ThrowRequiredTypeNotFound(model, typeof(IOnAllWorksEndConfigure<,>));

        if (typeInstaceAllWorksEnd is not null)
            model.InstanceAllWorksEnd = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceAllWorksEnd);
        #endregion
        
        #region IGetArgsConfigure
        Type? typeInstaceArgs =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IGetArgsConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeInstaceArgs is null && configure.IsRequiredArgs)
            throw ThrowRequiredTypeNotFound(model, typeof(IGetArgsConfigure<,>));

        if (typeInstaceArgs is not null)
            model.InstanceArgs = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceArgs);
        #endregion
                
        #region IOnDataCollectedConfigure
        Type? typeDataCollected =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IOnDataCollectedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeDataCollected is null && configure.IsRequiredDataCollected)
            throw ThrowRequiredTypeNotFound(model, typeof(IOnDataCollectedConfigure<,>));

        if (typeDataCollected is not null)
            model.InstanceDataCollected = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeDataCollected);
        #endregion
                        
        #region IDataFinishedConfigure
        Type? typeDataFinished =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IDataFinishedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeDataFinished is null && configure.IsRequiredDataFinished)
            throw ThrowRequiredTypeNotFound(model, typeof(IDataFinishedConfigure<,>));

        if (typeDataFinished is not null)
            model.InstanceDataFinished = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeDataFinished);
        #endregion
        
        #region IOnQuestCreatedConfigure
        Type? typeQuestCreated =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IOnQuestCreatedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeQuestCreated is null && configure.IsRequiredQuestCreated)
            throw ThrowRequiredTypeNotFound(model, typeof(IOnQuestCreatedConfigure<,>));

        if (typeQuestCreated is not null)
            model.InstanceQuestCreated = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeQuestCreated);
        #endregion

        #region IQuestExceptionConfigure
        Type? typeQuestException =
            TypeUtils.TryGetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IQuestExceptionConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeQuestException is null && configure.IsRequiredQuestException)
            throw ThrowRequiredTypeNotFound(model, typeof(IQuestExceptionConfigure<,>));

        if (typeQuestException is not null)
            model.InstanceQuestException = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeQuestException);
        #endregion
    }

    /// <summary>
    /// Map all classes which is a instance of type <see cref="BlScraper.Model.Quest{TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>If found a class with same name, the value going to be null</para>
    /// </remarks>
    /// <param name="assembly">assembly types</param>
    /// <returns>Type list of quests</returns>
    private static IEnumerable<(string Name, Type Type)> MapClassByQuestNameAssemblie(System.Reflection.Assembly assembly)
    {
        List<(string Name, Type Type)> listTypeQuests = new();

        foreach (Type type in TypeUtils.MapClassFromAssemblie(assembly, typeof(BlScraper.Model.Quest<>)))
        {
            if (TypeUtils.IsObsolete(type))
                continue;
                
            listTypeQuests.Add((type.Name.ToUpper(), type));
        }

        return listTypeQuests;
    }

    /// <summary>
    /// Return a exception
    /// </summary>
    private static ArgumentException ThrowRequiredTypeNotFound(ScrapModelsInternal model, Type typeNotFound)
    {
        return new ArgumentException($"Is necessary a {typeNotFound.FullName} of quest {model.QuestType.FullName}."+
            " Check if the possible class target is public, non-obsolete and concrete.", $"{typeNotFound.Name}");
    }
}