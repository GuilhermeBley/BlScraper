using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Extension.Internal;
using BlScraper.Model;
using Microsoft.Extensions.DependencyInjection;

namespace BlScraper.DependencyInjection.Builder.Internal;

/// <summary>
/// Create Filters
/// </summary>
internal sealed class CreateFilters
{
    private readonly PoolFilter _poolFilter;
    private readonly IServiceProvider _serviceProvider;
    private readonly ScrapModelInternal _model;

    public CreateFilters(ScrapModelInternal model, IServiceProvider serviceProvider, ScrapBuilderConfig builderConfig)
    {
        _serviceProvider = serviceProvider;
        _model = model;
        _poolFilter = builderConfig.Filters.Union(model.Filters);
    }

    
    /// <summary>
    /// Create event 'OnOccursException'
    /// </summary>
    public Func<Exception, object, QuestResult> CreateOnOccursException()
    {
        IQuestExceptionConfigureFilter[] filters 
            = CreateInstancesOfType<IQuestExceptionConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestExceptionConfigureFilter()).ToArray();
            

        return (exc, data) =>
        {
            QuestResult result = QuestResult.ThrowException(exc);

            var func = TypeUtils.CreateDelegateWithTarget(_model.InstanceQuestException?.GetType().GetMethod("OnOccursException", new Type[] { typeof(Exception), _model.DataType }), _model.InstanceQuestException) ?? null;

            if (func is not null)
                result = (QuestResult?)func.DynamicInvoke(exc, data) ?? throw new ArgumentNullException();

            try
            {
                Task.WaitAll(filters.Select(t => t.OnOccursException(exc, data, result)).ToArray());
            }
            catch { }

            return result;
        };
    }

    /// <summary>
    /// Create event 'OnOccursException'
    /// </summary>
    public Action<Results.ResultBase> CreateOnDataFinished()
    {
        IDataFinishedConfigureFilter[] filters 
            = CreateInstancesOfType<IDataFinishedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataFinishedConfigureFilter()).ToArray();

        return (result) =>
        {
            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataFinished?.GetType().GetMethod("OnDataFinished", 
                new Type[] { typeof(Results.ResultBase<>).MakeGenericType(_model.DataType) }), _model.InstanceDataFinished) ?? null;

            if (act is not null)
                act.DynamicInvoke(result);

            try
            {
                Task.WaitAll(filters.Select(f => f.OnDataFinished(result)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Create event 'OnFinished'
    /// </summary>
    public Action<Results.Models.EndEnumerableModel> CreateOnAllWorksEnd()
    {
        IAllWorksEndConfigureFilter[] filters 
            = CreateInstancesOfType<IAllWorksEndConfigureFilter>(_serviceProvider, _poolFilter.GetPoolAllWorksEndConfigureFilter()).ToArray();

        return (endModelEnumerable) =>
        {
            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceAllWorksEnd?.GetType().GetMethod("OnFinished",
                new Type[] { typeof(Results.Models.EndEnumerableModel) }), _model.InstanceAllWorksEnd) ?? null;

            if (act is not null)
                act.DynamicInvoke(endModelEnumerable);

            try
            {
                Task.WaitAll(filters.Select(f => f.OnFinished(endModelEnumerable)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Create event 'OnCollected'
    /// </summary>
    public Action<IEnumerable<object>> CreateOnCollected()
    {
        IDataCollectedConfigureFilter[] filters 
            = CreateInstancesOfType<IDataCollectedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataCollectedConfigureFilter()).ToArray();

        return (collectedList) =>
        {
            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataCollected?.GetType().GetMethod("OnCollected", 
                new Type[] { typeof(IEnumerable<>).MakeGenericType(_model.DataType) }), _model.InstanceDataCollected) ?? null;

            if (act is not null)
                act.DynamicInvoke(collectedList);

            try
            {
                Task.WaitAll(filters.Select(f => f.OnCollected(collectedList)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Create event 'GetArgs'
    /// </summary>
    public object[] CreateArgs()
    {

        var args = new object[0];

        IGetArgsConfigureFilter[] filters 
            = CreateInstancesOfType<IGetArgsConfigureFilter>(_serviceProvider, _poolFilter.GetPoolGetArgsConfigureFilter()).ToArray();

        args = 
            (object[]?) _model.InstanceArgs?.GetType().GetMethod("GetArgs")?.Invoke(_model.InstanceArgs, null) ?? new object[0];

        try
        {
            Task.WaitAll(filters.Select(f => f.GetArgs(args)).ToArray());
        }
        catch { }

        return args;
    }

    /// <summary>
    /// Create event 'OnCreated'
    /// </summary>
    public Action<IQuest> CreateOnCreated()
    {
        IQuestCreatedConfigureFilter[] filters 
            = CreateInstancesOfType<IQuestCreatedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestCreatedConfigureFilter()).ToArray();

        return (excCreated) =>
        {
            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceQuestCreated?.GetType().GetMethod("OnCreated",
                new Type[] { _model.QuestType }), _model.InstanceQuestCreated) ?? null;

            if (act is not null)
                act.DynamicInvoke(excCreated);

            try
            {
                Task.WaitAll(filters.Select(f => f.OnCreated(excCreated)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Create instances with new scope of <paramref name="types"/> and convert to <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Type to convert</typeparam>
    /// <param name="serviceProvider">providier</param>
    /// <param name="types">classes types</param>
    /// <returns>List of instaced types</returns>
    private static IEnumerable<T> CreateInstancesOfType<T>(IServiceProvider serviceProvider, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            yield return (T)ActivatorUtilities.CreateInstance(serviceProvider.CreateScope().ServiceProvider, type);
        }
    }
}