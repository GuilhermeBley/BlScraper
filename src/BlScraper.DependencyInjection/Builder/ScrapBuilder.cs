using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.Model;
using BlScraper.Model;
using BlScraper.Results.Models;
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

    /// <inheritdoc cref="IScrapBuilder.CreateModelByQuestName(string)" path="*"/>
    public IModelScraper CreateModelByQuestName(string name)
    {
        var questFound = MapUniqueQuestByName(_assemblies.ToArray(), name);

        return Create(questFound);
    }

    /// <inheritdoc cref="IScrapBuilder.CreateModelByQuestNameOrDefault(string)" path="*"/>
    public IModelScraper? CreateModelByQuestNameOrDefault(string name)
    {
        try
        {
            return CreateModelByQuestName(name);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc cref="IScrapBuilder.CreateModelByQuestType(Type)" path="*"/>
    public IModelScraper CreateModelByQuestType(Type type)
    {
        return Create(type);
    }

    /// <inheritdoc cref="IScrapBuilder.CreateModelByQuestTypeOrDefault(Type)" path="*"/>
    public IModelScraper? CreateModelByQuestTypeOrDefault(Type type)
    {
        try
        {
            return Create(type);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc cref="IScrapBuilder.CreateModelByQuestType{TQuest}" path="*"/>
    public IModelScraper CreateModelByQuestType<TQuest>()
    {
        return Create(typeof(TQuest));
    }

    /// <inheritdoc cref="IScrapBuilder.CreateModelByQuestTypeOrDefault{TQuest}" path="*"/>
    public IModelScraper? CreateModelByQuestTypeOrDefault<TQuest>()
    {
        try
        {
            return Create(typeof(TQuest));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Create model by <paramref name="questType"/>
    /// </summary>
    /// <remarks>
    ///     <para>Makes a validation in type</para>
    /// </remarks>
    /// <param name="questType">Concrete and public class of assignable to <see cref="Quest{TData}"/></param>
    /// <returns>Model scraper</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <inheritdoc cref="SetParametersOnModel(ScrapModelInternal)" path="exception"/>
    private IModelScraper Create(Type questType)
    {
        var model = new ScrapModelInternal(questType);

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
                    TypeUtils.CreateDelegateWithTarget(model.InstanceAllWorksEnd?.GetType().GetMethod("OnFinished", new Type[] { typeof(EndEnumerableModel) }), model.InstanceAllWorksEnd) ?? null!,
                    TypeUtils.CreateDelegateWithTarget(model.InstanceDataCollected?.GetType().GetMethod("OnCollected", new Type[] { typeof(IEnumerable<>).MakeGenericType(model.DataType) }), model.InstanceDataCollected) ?? null!,
                    TypeUtils.CreateDelegateWithTarget(model.InstanceQuestCreated?.GetType().GetMethod("OnCreated", new Type[] { model.QuestType }), model.InstanceQuestCreated) ?? null!,
                    (object[]?) model.InstanceArgs?.GetType().GetMethod("GetArgs")?.Invoke(model.InstanceArgs, null) ?? new object[0]
                }
            ) ?? throw new ArgumentNullException(nameof(IModelScraper));
    }

    /// <summary>
    /// Set parameters in model
    /// </summary>
    /// <param name="model">Model to set parameters</param>
    /// <exception cref="ArgumentNullException"></exception>
    private void SetParametersOnModel(ScrapModelInternal model)
    {
        #region RequiredConfigure
        Type typeInstaceRequired = 
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(RequiredConfigure<,>).MakeGenericType(model.QuestType, model.DataType))
            ?? throw ThrowRequiredTypeNotFound(model, typeof(RequiredConfigure<,>));

        model.InstaceRequired =
            ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceRequired);

        var configure = (IRequiredConfigure)model.InstaceRequired;
        #endregion

        #region IOnAllWorksEndConfigure
        Type? typeInstaceAllWorksEnd =
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IAllWorksEndConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeInstaceAllWorksEnd is null && configure.IsRequiredAllWorksEnd)
            throw ThrowRequiredTypeNotFound(model, typeof(IAllWorksEndConfigure<,>));

        if (typeInstaceAllWorksEnd is not null)
            model.InstanceAllWorksEnd = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceAllWorksEnd);
        #endregion
        
        #region IGetArgsConfigure
        Type? typeInstaceArgs =
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IGetArgsConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeInstaceArgs is null && configure.IsRequiredArgs)
            throw ThrowRequiredTypeNotFound(model, typeof(IGetArgsConfigure<,>));

        if (typeInstaceArgs is not null)
            model.InstanceArgs = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceArgs);
        #endregion
                
        #region IOnDataCollectedConfigure
        Type? typeDataCollected =
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IDataCollectedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeDataCollected is null && configure.IsRequiredDataCollected)
            throw ThrowRequiredTypeNotFound(model, typeof(IDataCollectedConfigure<,>));

        if (typeDataCollected is not null)
            model.InstanceDataCollected = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeDataCollected);
        #endregion
                        
        #region IDataFinishedConfigure
        Type? typeDataFinished =
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IDataFinishedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeDataFinished is null && configure.IsRequiredDataFinished)
            throw ThrowRequiredTypeNotFound(model, typeof(IDataFinishedConfigure<,>));

        if (typeDataFinished is not null)
            model.InstanceDataFinished = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeDataFinished);
        #endregion
        
        #region IOnQuestCreatedConfigure
        Type? typeQuestCreated =
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IQuestCreatedConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

        if (typeQuestCreated is null && configure.IsRequiredQuestCreated)
            throw ThrowRequiredTypeNotFound(model, typeof(IQuestCreatedConfigure<,>));

        if (typeQuestCreated is not null)
            model.InstanceQuestCreated = 
                ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeQuestCreated);
        #endregion

        #region IQuestExceptionConfigure
        Type? typeQuestException =
            TypeUtils.GetUniqueAssignableFrom(_assemblies.ToArray(), typeof(IQuestExceptionConfigure<,>).MakeGenericType(model.QuestType, model.DataType));

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
    private static ArgumentException ThrowRequiredTypeNotFound(ScrapModelInternal model, Type typeNotFound)
    {
        return new ArgumentException($"Is necessary a {typeNotFound.FullName} of quest {model.QuestType.FullName}."+
            " Check if the possible class target is public, non-obsolete and concrete.", $"{typeNotFound.Name}");
    }

    /// <summary>
    /// Find and return quest by name
    /// </summary>
    /// <param name="name">Name to find.</param>
    /// <returns>Type assignable to <see cref="Quest{TData}"/> found.</returns>
    /// <exception cref="ArgumentException"/>
    private static Type MapUniqueQuestByName(System.Reflection.Assembly[] assemblies, string name)
    {
         if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        Type? questTypefound = null;

        foreach (var assembly in assemblies)
        {
            var localValuePair = 
                MapClassByQuestNameAssemblie(assembly)
                    .Where(pair => pair.Name == name.ToUpper());

            if (!localValuePair.Any())
                continue;

            if (localValuePair.Count() != 1)
                throw new ArgumentException($"Duplicate names with value {localValuePair.First().Name} in: {string.Join('\n', localValuePair.Select(pair => pair.Type.FullName))}.");

            var localQuestTypefound = localValuePair.First().Type;

            if (questTypefound is not null &&
                localQuestTypefound is not null)
                throw new ArgumentException($"Duplicate QuestTypes with name {name} was found in {localQuestTypefound.FullName} and {questTypefound.FullName}.", nameof(name));

            questTypefound = localQuestTypefound;
        }

        if (questTypefound is null)
            throw new ArgumentException($"QuestTypes with name {name} wasn't found. Check if the possible class target is public, non-obsolete and concrete.", nameof(name));

        return questTypefound;
    }
}