namespace BlScraper.Model;

/// <summary>
/// Execution result
/// </summary>
public class QuestResult
{
    /// <summary>
    /// Action for the next execution data
    /// </summary>
    private ExecutionResultEnum _actionToNextData { get; } = ExecutionResultEnum.Next;

    /// <summary>
    /// Complements message
    /// </summary>
    private object _message { get; } = string.Empty;

    /// <inheritdoc cref="_actionToNextData" path="*"/>
    public ExecutionResultEnum ActionToNextData => _actionToNextData;

    /// <inheritdoc cref="_message" path="*"/>
    public object Message => _message;

    /// <summary>
    /// Instances of <see cref="QuestResult"/>
    /// </summary>
    /// <param name="enum">State</param>
    /// <param name="message">Optional message</param>
    internal QuestResult(ExecutionResultEnum @enum, object? message = null)
    {
        _actionToNextData = @enum;
        if (message is not null)
            _message = message;
    }

    /// <summary>
    /// <see cref="QuestResult"/> with state Ok/Next
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <returns>new instace of <see cref="QuestResult"/> with state equals a <see cref="ExecutionResultEnum.Next"/></returns>
    public static QuestResult Ok(object? message = null)
    {
        return new QuestResult(ExecutionResultEnum.Next, message);
    }

    /// <summary>
    /// <see cref="QuestResult"/> with state Retry Same
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <returns>new instace of <see cref="QuestResult"/> with state equals a <see cref="ExecutionResultEnum.RetrySame"/></returns>
    public static QuestResult RetrySame(object? message = null)
    {
        return new QuestResult(ExecutionResultEnum.RetrySame, message);
    }

    /// <summary>
    /// <see cref="QuestResult"/> with state Retry Other
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <returns>new instace of <see cref="QuestResult"/> with state equals a <see cref="ExecutionResultEnum.RetryOther"/></returns>
    public static QuestResult RetryOther(object? message = null)
    {
        return new QuestResult(ExecutionResultEnum.RetryOther, message);
    }

    /// <summary>
    /// <see cref="QuestResult"/> with state Throw Exception
    /// </summary>
    /// <param name="message">Optional exception</param>
    /// <returns>new instace of <see cref="QuestResult"/> with state equals a <see cref="ExecutionResultEnum.ThrowException"/></returns>
    public static QuestResult ThrowException(Exception? exception = null)
    {
        return new QuestResult(ExecutionResultEnum.ThrowException, exception);
    }

    /// <summary>
    /// Request all quests to dispose, cancel token and dispose <see cref="IModelScraper"/>
    /// </summary>
    /// <param name="args">Args</param>
    /// <returns>new instace of <see cref="QuestResult"/> with state equals a <see cref="ExecutionResultEnum.DisposeAll"/></returns>
    public static QuestResult DisposeAll(object? args = null)
    {
        return new QuestResult(ExecutionResultEnum.DisposeAll, args);
    }
}

/// <summary>
/// Possible results
/// </summary>
public enum ExecutionResultEnum : sbyte
{
    /// <summary>
    /// Ok, Go to next data
    /// </summary>
    Next = 1,

    /// <summary>
    /// Retry same data
    /// </summary>
    RetrySame = 2,

    /// <summary>
    /// Retry other data
    /// </summary>
    RetryOther = 3,

    /// <summary>
    /// Throw exception
    /// </summary>
    ThrowException = 4,

    /// <summary>
    /// Dispose all scrapers
    /// </summary>
    DisposeAll = 5
}