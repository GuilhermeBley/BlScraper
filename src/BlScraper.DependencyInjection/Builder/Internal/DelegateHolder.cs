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
    private PoolFilter _poolFilter;
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

    /// <summary>
    /// Instances delegate holder
    /// </summary>
    public DelageteHolder(ScrapModelInternal model, IServiceProvider serviceProvider, ScrapBuilderConfig builderConfig)
    {
        _serviceProvider = serviceProvider;
        _model = model;
        _poolFilter = builderConfig.Filters;
    }

    /// <summary>
    /// Create event 'OnOccursException' and sets <see cref="Model.Context.ScrapContextAcessor"/>
    /// </summary>
    /// <remarks>
    ///     <para>Called by the Thread which creates the model.</para>
    /// </remarks>
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
    /// <remarks>
    ///     <para>The thread created when the model runs, calls this method when occurs exception in search.</para>
    /// </remarks>
    public Func<Exception, object, QuestResult> CreateOnOccursException()
    {
        return (exc, data) =>
        {
            QuestResult result = QuestResult.ThrowException(exc);

            var func = TypeUtils.CreateDelegateWithTarget(_model.InstanceQuestException?.GetType().GetMethod(nameof(IQuestExceptionConfigure<HolderQuest, dynamic>.OnOccursException)
                , new Type[] { typeof(Exception), _model.DataType })
                , _model.InstanceQuestException) ?? null;
            
            if (func is not null)
                result = (QuestResult?)func.DynamicInvoke(exc, data) ?? throw new ArgumentNullException();

            try
            {
                IQuestExceptionConfigureFilter[] filters 
                    = TypeUtils.CreateInstancesOfType<IQuestExceptionConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestExceptionConfigureFilter()).ToArray();

                Task.WaitAll(filters.Select(t => t.OnOccursException(exc, data, result)).ToArray());
            }
            catch { }

            return result;
        };
    }

    /// <summary>
    /// Create event 'OnOccursException'
    /// </summary>
    /// <remarks>
    ///     <para>The thread created when the model runs, calls this method when data is collected.</para>
    /// </remarks>
    public Action<Results.ResultBase> CreateOnDataFinished()
    {
        return (result) =>
        {
            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataFinished?.GetType().GetMethod(nameof(IDataFinishedConfigure<HolderQuest, dynamic>.OnDataFinished), 
                new Type[] { typeof(Results.ResultBase<>).MakeGenericType(_model.DataType) }), _model.InstanceDataFinished) ?? null;

            if (act is not null)
                act.DynamicInvoke(result);

            try
            {
                IDataFinishedConfigureFilter[] filters 
                    = TypeUtils.CreateInstancesOfType<IDataFinishedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataFinishedConfigureFilter()).ToArray();

                Task.WaitAll(filters.Select(f => f.OnDataFinished(result)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Create event 'OnFinished'
    /// </summary>
    /// <remarks>
    ///     <para>The thread created when the model runs, calls this method in the end of search.</para>
    /// </remarks>
    public Action<Results.Models.EndEnumerableModel> CreateOnAllWorksEnd()
    {
        return (endModelEnumerable) =>
        {
            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceAllWorksEnd?.GetType().GetMethod(nameof(IAllWorksEndConfigure<HolderQuest, dynamic>.OnFinished),
                new Type[] { typeof(Results.Models.EndEnumerableModel) }), _model.InstanceAllWorksEnd) ?? null;

            if (act is not null)
                act.DynamicInvoke(endModelEnumerable);

            try
            {
                IAllWorksEndConfigureFilter[] filters 
                    = TypeUtils.CreateInstancesOfType<IAllWorksEndConfigureFilter>(_serviceProvider, _poolFilter.GetPoolAllWorksEndConfigureFilter()).ToArray();

                Task.WaitAll(filters.Select(f => f.OnFinished(endModelEnumerable)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Create event 'OnCollected' and sets <see cref="Model.Context.ScrapContextAcessor"/>
    /// </summary>
    /// <remarks>
    ///     <para>Called by the Thread which creates the model.</para>
    /// </remarks>
    public Action<IEnumerable<object>> CreateOnDataToSearchCollected()
    {
        return (collectedList) =>
        {
            try
            {
                SetCurrentContextThread(Context);

                var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceDataCollected?.GetType().GetMethod(nameof(IDataCollectedConfigure<HolderQuest, dynamic>.OnCollected), 
                    new Type[] { typeof(IEnumerable<>).MakeGenericType(_model.DataType) }), _model.InstanceDataCollected) ?? null;

                if (act is not null)
                    act.DynamicInvoke(collectedList);

                try
                {
                    IDataCollectedConfigureFilter[] filters 
                        = TypeUtils.CreateInstancesOfType<IDataCollectedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolDataCollectedConfigureFilter()).ToArray();

                    Task.WaitAll(filters.Select(f => f.OnCollected(collectedList)).ToArray());
                }
                catch { }
            }
            finally
            {
                SetCurrentContextThread();
            }
        };
    }

    /// <summary>
    /// Create event 'GetArgs'
    /// </summary>
    /// <remarks>
    ///     <para>Called by the Thread which creates the model.</para>
    /// </remarks>
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
    /// <remarks>
    ///     <para>The thread created when the model runs, calls this method first</para>
    /// </remarks>
    public Action<IQuest> CreateOnCreatedQuest()
    {
        return (excCreated) =>
        {
            SetCurrentContextThread(_currentInfo);

            var act = TypeUtils.CreateDelegateWithTarget(_model.InstanceQuestCreated?.GetType().GetMethod(nameof(IQuestCreatedConfigure<HolderQuest, dynamic>.OnCreated),
                new Type[] { _model.QuestType }), _model.InstanceQuestCreated) ?? null;

            if (act is not null)
                act.DynamicInvoke(excCreated);

            try
            {
                IQuestCreatedConfigureFilter[] filters 
                    = TypeUtils.CreateInstancesOfType<IQuestCreatedConfigureFilter>(_serviceProvider, _poolFilter.GetPoolQuestCreatedConfigureFilter()).ToArray();

                Task.WaitAll(filters.Select(f => f.OnCreated(excCreated)).ToArray());
            }
            catch { }
        };
    }

    /// <summary>
    /// Holder method to get data
    /// </summary>
    /// <remarks>
    ///     <para>Called by the Thread which creates the model.</para>
    /// </remarks>
    private Task<IEnumerable<TData>> GetDataHolderMethod<TData>()
        where TData : class
    {
        try
        {
            SetCurrentContextThread(Context);
            
            var method = _model.InstanceRequired?.GetType().GetMethod(nameof(RequiredConfigure<HolderQuest, dynamic>.GetData))
                ?? throw new ArgumentNullException(nameof(RequiredConfigure<HolderQuest, dynamic>.GetData));

            var func = TypeUtils.CreateDelegateWithTarget(method, _model.InstanceRequired) 
                ?? throw new ArgumentNullException(nameof(RequiredConfigure<HolderQuest, dynamic>.GetData));
            
            return (Task<IEnumerable<TData>>?)func.DynamicInvoke() ?? throw new ArgumentNullException();
        }
        finally
        {
            SetCurrentContextThread();
        }
    }

    /// <summary>
    /// Sets context on current thread which called it
    /// </summary>
    /// <param name="infoContext">context or null to refresh</param>
    public void SetCurrentContextThread(IModelScraperInfo? infoContext = null)
    {
        var context = new Model.Context.ScrapContextAcessor();
        if (context.ScrapContext != infoContext)
            context.ScrapContext = infoContext;
    }
    
    /// <summary>
    /// Union filters
    /// </summary>
    /// <param name="filter">filters to add</param>
    public void AddFilters(PoolFilter filter)
    {
        _poolFilter = _poolFilter.Union(filter);
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