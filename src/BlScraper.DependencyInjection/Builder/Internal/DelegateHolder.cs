using System.Reflection;
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
internal sealed class DelageteHolder
{
    private IModelScraperInfo? _currentInfo;
    private readonly PoolFilter _poolFilter;
    private readonly IServiceProvider _serviceProvider;
    private readonly ScrapModelInternal _model;
    private readonly Model.Context.ScrapContextAcessor _contextAcessor = new();

    public IModelScraperInfo? CurrentInfo { 
        get { return _currentInfo; } 
        set { 
            if (value is null) 
                throw new ArgumentNullException(nameof(CurrentInfo)); 
            _currentInfo = value;
        }}

    public DelageteHolder(ScrapModelInternal model, IServiceProvider serviceProvider, ScrapBuilderConfig builderConfig)
    {
        _serviceProvider = serviceProvider;
        _model = model;
        _poolFilter = builderConfig.Filters.Union(model.Filters);
    }

    /// <summary>
    /// Create event 'OnOccursException' and sets <see cref="Model.Context.ScrapContextAcessor"/>
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public Delegate CreateGetData()
    {
        return TypeUtils.CreateDelegateWithTarget(
            this.GetType().GetMethod(nameof(GetDataHolderMethod), BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(_model.DataType) ?? throw new ArgumentNullException("method", $"Failed in {nameof(CreateGetData)}."), 
            this) ?? throw new ArgumentNullException("func", $"Failed in {nameof(CreateGetData)}.");
    }

    /// <summary>
    /// Create event 'OnOccursException'
    /// </summary>
    public Func<Exception, object, QuestResult> CreateOnOccursException()
    {
        IQuestExceptionConfigureFilter[] filters 
            = TypeUtils.CreateInstancesOfType<IQuestExceptionConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestExceptionConfigureFilter()).ToArray();

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
            = TypeUtils.CreateInstancesOfType<IDataFinishedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataFinishedConfigureFilter()).ToArray();

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
            = TypeUtils.CreateInstancesOfType<IAllWorksEndConfigureFilter>(_serviceProvider, _poolFilter.GetPoolAllWorksEndConfigureFilter()).ToArray();

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
    /// Create event 'OnCollected' and sets <see cref="Model.Context.ScrapContextAcessor"/>
    /// </summary>
    public Action<IEnumerable<object>> CreateOnCollected()
    {
        IDataCollectedConfigureFilter[] filters 
            = TypeUtils.CreateInstancesOfType<IDataCollectedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataCollectedConfigureFilter()).ToArray();

        return (collectedList) =>
        {
            try
            {
                _contextAcessor.ScrapContext = _currentInfo;

                var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataCollected?.GetType().GetMethod("OnCollected", 
                    new Type[] { typeof(IEnumerable<>).MakeGenericType(_model.DataType) }), _model.InstanceDataCollected) ?? null;

                if (act is not null)
                    act.DynamicInvoke(collectedList);

                try
                {
                    Task.WaitAll(filters.Select(f => f.OnCollected(collectedList)).ToArray());
                }
                catch { }
            }
            finally
            {
                _contextAcessor.ScrapContext = null;
            }
        };
    }

    /// <summary>
    /// Create event 'GetArgs'
    /// </summary>
    public object[] CreateArgs()
    {
        var args = new object[0];

        IGetArgsConfigureFilter[] filters 
            = TypeUtils.CreateInstancesOfType<IGetArgsConfigureFilter>(_serviceProvider, _poolFilter.GetPoolGetArgsConfigureFilter()).ToArray();

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
    /// Create event 'OnCreated' and sets <see cref="Model.Context.ScrapContextAcessor"/>
    /// </summary>
    public Action<IQuest> CreateOnCreated()
    {
        IQuestCreatedConfigureFilter[] filters 
            = TypeUtils.CreateInstancesOfType<IQuestCreatedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestCreatedConfigureFilter()).ToArray();

        return (excCreated) =>
        {
            _contextAcessor.ScrapContext = CurrentInfo;

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

    private Task<IEnumerable<TData>> GetDataHolderMethod<TData>()
        where TData : class
    {
        var method = _model.InstanceRequired?.GetType().GetMethod("GetData")
            ?? throw new ArgumentNullException("GetData");

        try
        {
            _contextAcessor.ScrapContext = _currentInfo;

            var func = TypeUtils.CreateDelegateWithTarget(method, _model.InstanceRequired) 
                ?? throw new ArgumentNullException("GetData");
            
            return (Task<IEnumerable<TData>>?)func.DynamicInvoke() ?? throw new ArgumentNullException();
        }
        finally
        {
            _contextAcessor.ScrapContext = null;
        }
    }
}