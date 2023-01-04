using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Model;
using BlScraper.DependencyInjection.Builder.Internal;
using BlScraper.Model;
using BlScraper.Results.Models;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.Model.Context;

namespace BlScraper.DependencyInjection.Builder;

/// <summary>
/// Implement of assembly
/// </summary>
internal class ScrapBuilder : IScrapBuilder
{

    /// <summary>
    /// Type of <see cref="ModelScraperService{TQuest, TData}" to instance./>
    /// </summary>
    private static readonly Type _modelType = typeof(ModelScraperService<,>);

    /// <summary>
    /// Service Providier
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Builder config
    /// </summary>
    private readonly ScrapBuilderConfig _builderConfig;

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    private readonly HashSet<System.Reflection.Assembly> _assemblies;

    /// <summary>
    /// Instance with service provider
    /// </summary>
    /// <param name="serviceProvider"></param>
    public ScrapBuilder(IServiceProvider serviceProvider, ScrapBuilderConfig builderConfig)
    {
        _serviceProvider = serviceProvider;
        _assemblies = builderConfig.Assemblies.ToHashSet();
        _builderConfig = builderConfig;
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

        SetFiltersOnModel(model);

        var modelScraperType = TypeUtils.SetGenericParameters(_modelType, model.QuestType, model.DataType);

        var delegateEvents = ActivatorUtilities.CreateInstance<DelageteHolder>(_serviceProvider, model, _builderConfig);
        
        IModelScraper? modelScraper = null;
        modelScraper =
            (IModelScraper?)Activator.CreateInstance(
                modelScraperType,
                ((IRequiredConfigure)model.InstanceRequired!).initialQuantity,
                (IServiceProvider)_serviceProvider,
                delegateEvents.CreateGetData(),
                delegateEvents.CreateOnOccursException(),
                delegateEvents.CreateOnDataFinished(),
                delegateEvents.CreateOnAllWorksEnd(),
                delegateEvents.CreateOnCollected(),
                delegateEvents.CreateOnCreated(),
                delegateEvents.CreateArgs()
            ) ?? throw new ArgumentNullException(nameof(IModelScraper));

        delegateEvents.CurrentInfo = modelScraper;

        return modelScraper;
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
    private IModelScraper CreateWithContext(Type questType)
    {
        try
        {

            return Create(questType);
        }
        finally
        {

        }
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

        model.InstanceRequired =
            ActivatorUtilities.CreateInstance(_serviceProvider.CreateScope().ServiceProvider, typeInstaceRequired);

        var configure = (IRequiredConfigure)model.InstanceRequired;
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
    /// Set filters in model
    /// </summary>
    /// <param name="model">Model to set parameters</param>
    private void SetFiltersOnModel(ScrapModelInternal model)
    {
        #region IAllWorksEndConfigureFilter

        foreach (var filterType in TypeUtils.GetAssignableFrom(_assemblies.ToArray(), typeof(IAllWorksEndConfigureFilter<,>).MakeGenericType(model.QuestType, model.DataType)))
            model.Filters.Add(typeof(IAllWorksEndConfigureFilter), filterType);

        #endregion

        #region IDataCollectedConfigureFilter

        foreach (var filterType in TypeUtils.GetAssignableFrom(_assemblies.ToArray(), typeof(IDataCollectedConfigureFilter<,>).MakeGenericType(model.QuestType, model.DataType)))
            model.Filters.Add(typeof(IDataCollectedConfigureFilter), filterType);

        #endregion

        #region IDataFinishedConfigureFilter

        foreach (var filterType in TypeUtils.GetAssignableFrom(_assemblies.ToArray(), typeof(IDataFinishedConfigureFilter<,>).MakeGenericType(model.QuestType, model.DataType)))
            model.Filters.Add(typeof(IDataFinishedConfigureFilter), filterType);

        #endregion

        #region IQuestCreatedConfigureFilter

        foreach (var filterType in TypeUtils.GetAssignableFrom(_assemblies.ToArray(), typeof(IQuestCreatedConfigureFilter<,>).MakeGenericType(model.QuestType, model.DataType)))
            model.Filters.Add(typeof(IQuestCreatedConfigureFilter), filterType);

        #endregion

        #region IQuestExceptionConfigureFilter

        foreach (var filterType in TypeUtils.GetAssignableFrom(_assemblies.ToArray(), typeof(IQuestExceptionConfigureFilter<,>).MakeGenericType(model.QuestType, model.DataType)))
            model.Filters.Add(typeof(IQuestExceptionConfigureFilter), filterType);

        #endregion

        var requiredFilters = ((IRequiredConfigureFilters?)model.InstanceRequired)?.RequiredFilters
            ?? throw new ArgumentNullException(nameof(IRequiredConfigureFilters));

        var invalidFilters = requiredFilters.Where(f => !TypeUtils.IsFilter(f));

        if (invalidFilters.Any())
            throw new ArgumentException($"These required types aren't filters, config '{model.InstanceRequired.GetType().FullName}':\n" +
                $"{string.Join('\n', invalidFilters.Select(f => f.FullName))}.", $"{string.Join('|', invalidFilters.Select(f => f.Name))}");

        var requiredNonSetted = requiredFilters.Except(_builderConfig.Filters.Union(model.Filters).Select(f => f.Filter));

        if (requiredNonSetted.Any())
            throw new ArgumentException($"Required filters not implemented in config '{model.InstanceRequired.GetType().FullName}':\n " +
                $"{string.Join('\n', requiredNonSetted.Select(r => r.FullName))}.", 
                $"{string.Join('|', requiredNonSetted.Select(r => r.Name))}");
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

    /// <summary>
    /// Create instance of type <typeparamref name="T"/> with constructor parameters <paramref name="args"/>
    /// </summary>
    /// <typeparam name="T">type of model</typeparam>
    /// <param name="args">constructor arguments</param>
    /// <returns>instanced <see cref="IModelScraper"/></returns>
    /// <inheritdoc cref="CreateModel(Type, object[])" path="/exception"/>
    [Obsolete]
    private static IModelScraper? CreateModel<T>(params object[] args)
        where T : IModelScraper
    {
        return CreateModel(typeof(T), args);
    }

    /// <summary>
    /// Create instance of type <paramref name="model"/> with constructor parameters <paramref name="args"/>
    /// </summary>
    /// <param name="model">type of model</param>
    /// <param name="args">constructor arguments</param>
    /// <returns>instanced <see cref="IModelScraper"/></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    [Obsolete]
    private static IModelScraper? CreateModel(Type model, params object[] args)
    {
        if (!typeof(IModelScraper).IsAssignableFrom(model))
            throw new ArgumentException($"'{model.FullName}' isn't assignable to '{typeof(IModelScraper).FullName}'.");

        if (model.ContainsGenericParameters)
            throw new ArgumentException($"'{model.FullName}' cannot be create. The class contains not setted generic parameters.");

        if (!model.IsClass ||
            model.IsAbstract ||
            !model.IsPublic || 
            !model.GetConstructors().Where(c => c.IsPublic || !c.IsStatic).Any())
            throw new ArgumentException($"'{model.FullName}' cannot be create. Check if the class is public, contains a public constructor and non-abstract.");

        foreach (var constructor in model.GetConstructors().Where(c => c.IsPublic || !c.IsStatic).OrderByDescending(o => o.GetParameters().Count()))
        {
            var parsedArgs = TypeUtils.TryParseConstructorParameters(constructor, args);
            if (parsedArgs is null)
                continue;

            return (IModelScraper?)Activator.CreateInstance(
                model,
                parsedArgs
            ) ?? throw new ArgumentNullException(nameof(IModelScraper));
        }

        return null;
    }
}