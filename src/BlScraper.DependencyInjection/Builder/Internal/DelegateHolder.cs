using System.Reflection;
using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Extension.Internal;
using BlScraper.Model;
using Microsoft.Extensions.DependencyInjection;

namespace BlScraper.DependencyInjection.Builder.Internal;

/// <summary>
/// Create delegates
/// </summary>
internal sealed class DelageteHolder
{
    private IModelScraperInfo? _currentInfo;
    private readonly PoolFilter _poolFilter;
    private readonly IServiceProvider _serviceProvider;
    private readonly ScrapModelInternal _model;

    /// <summary>
    /// Context
    /// </summary>
    public IModelScraperInfo? Context { 
        get { return _currentInfo; } 
        set { 
            if (value is null) 
                throw new ArgumentNullException(nameof(Context)); 
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
        return (exc, data) =>
        {
            IQuestExceptionConfigureFilter[] filters 
                = TypeUtils.CreateInstancesOfType<IQuestExceptionConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestExceptionConfigureFilter()).ToArray();

            QuestResult result = QuestResult.ThrowException(exc);

            var func = TypeUtils.CreateDelegateWithTarget(_model.InstanceQuestException?.GetType().GetMethod(nameof(IQuestExceptionConfigure<HolderQuest, dynamic>.OnOccursException)
                , new Type[] { typeof(Exception), _model.DataType })
                , _model.InstanceQuestException) ?? null;
            
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
        return (result) =>
        {
            IDataFinishedConfigureFilter[] filters 
                = TypeUtils.CreateInstancesOfType<IDataFinishedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataFinishedConfigureFilter()).ToArray();

            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataFinished?.GetType().GetMethod(nameof(IDataFinishedConfigure<HolderQuest, dynamic>.OnDataFinished), 
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
        return (endModelEnumerable) =>
        {
            IAllWorksEndConfigureFilter[] filters 
                = TypeUtils.CreateInstancesOfType<IAllWorksEndConfigureFilter>(_serviceProvider, _poolFilter.GetPoolAllWorksEndConfigureFilter()).ToArray();

            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceAllWorksEnd?.GetType().GetMethod(nameof(IAllWorksEndConfigure<HolderQuest, dynamic>.OnFinished),
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
        return (collectedList) =>
        {
            IDataCollectedConfigureFilter[] filters 
                = TypeUtils.CreateInstancesOfType<IDataCollectedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataCollectedConfigureFilter()).ToArray();

            try
            {
                SetCurrentContextThread(_currentInfo);

                var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataCollected?.GetType().GetMethod(nameof(IDataCollectedConfigure<HolderQuest, dynamic>.OnCollected), 
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
        if (_model.InstanceArgs is null)
            return new object[0];
        
        return
            (object[]?) _model.InstanceArgs.GetType().GetMethod(nameof(IGetArgsConfigure<HolderQuest, dynamic>.GetArgs))?.Invoke(_model.InstanceArgs, null)
                ?? throw new ArgumentException($"Class does not constain the method {nameof(IGetArgsConfigure<HolderQuest, dynamic>.GetArgs)}");
    }

    /// <summary>
    /// Create event 'OnCreated' and sets <see cref="Model.Context.ScrapContextAcessor"/>
    /// </summary>
    public Action<IQuest> CreateOnCreated()
    {
        return (excCreated) =>
        {
            IQuestCreatedConfigureFilter[] filters 
                = TypeUtils.CreateInstancesOfType<IQuestCreatedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestCreatedConfigureFilter()).ToArray();

            _contextAcessor.ScrapContext = Context;

            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceQuestCreated?.GetType().GetMethod(nameof(IQuestCreatedConfigure<HolderQuest, dynamic>.OnCreated),
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
    /// Sets context on current thread which called it
    /// </summary>
    /// <param name="infoContext">context or null to refresh</param>
    public void SetCurrentContextThread(IModelScraperInfo? infoContext)
    {
        new Model.Context.ScrapContextAcessor().ScrapContext = infoContext;
    }

    private Task<IEnumerable<TData>> GetDataHolderMethod<TData>()
        where TData : class
    {
        var method = _model.InstanceRequired?.GetType().GetMethod(nameof(RequiredConfigure<HolderQuest, dynamic>.GetData))
            ?? throw new ArgumentNullException(nameof(RequiredConfigure<HolderQuest, dynamic>.GetData));

        try
        {
            _contextAcessor.ScrapContext = _currentInfo;

            var func = TypeUtils.CreateDelegateWithTarget(method, _model.InstanceRequired) 
                ?? throw new ArgumentNullException(nameof(RequiredConfigure<HolderQuest, dynamic>.GetData));
            
            return (Task<IEnumerable<TData>>?)func.DynamicInvoke() ?? throw new ArgumentNullException();
        }
        finally
        {
            _contextAcessor.ScrapContext = null;
        }
    }

    /// <summary>
    /// Holder class
    /// </summary>
    private class HolderQuest : BlScraper.Model.Quest<dynamic>
    {
        public override QuestResult Execute(dynamic data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}