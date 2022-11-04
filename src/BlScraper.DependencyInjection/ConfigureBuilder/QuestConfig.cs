using BlScraper.Model;
using BlScraper.Results;

namespace BlScraper.DependencyInjection.ConfigureBuilder;

public abstract class QuestConfig<TQuest, TData> : IQuestConfig
    where TData : class
    where TQuest : Quest<TData>
{
    HashSet<object> _args = new();
    internal object[] Args => _args.ToArray();
    internal Action<TQuest> WhenExecutionCreated => ExecutionCreated;
    internal Action<IEnumerable<TData>> WhenDataWasCollected => DataWasCollected;
    internal Action<IEnumerable<ResultBase<Exception?>>> WhenAllWorksEnd => AllWorksEnd;
    internal Action<ResultBase<TData>> WhenDataFinished => DataFinished;
    internal Func<Exception, TData, QuestResult> WhenOccursException => OccursException;
    internal Func<Task<IEnumerable<TData>>> GetDataScrap => GetData;
    private object _lock { get; } = new();

    protected abstract Task<IEnumerable<TData>> GetData();

    /// <summary>
    /// Add object to args
    /// </summary>
    /// <param name="arg">obj shared</param>
    /// <returns>this</returns>
    protected QuestConfig<TQuest, TData> AddArg(object arg)
    {
        lock(_lock)
        {
            _args.Add(arg);
        }

        return this;
    }

    protected virtual void ExecutionCreated(TQuest questCreated)
    { }

    protected virtual void DataWasCollected(IEnumerable<TData> datas)
    {

    }

    protected virtual void AllWorksEnd (IEnumerable<ResultBase<Exception?>> listFinishers)
    {

    }

    protected virtual void DataFinished(ResultBase<TData> resultFinished)
    {

    }

    protected virtual QuestResult OccursException (Exception ex, TData data)
    {
        return QuestResult.ThrowException(ex);
    }
}