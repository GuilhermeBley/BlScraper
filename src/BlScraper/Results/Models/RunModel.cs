namespace BlScraper.Results.Models;

public enum RunModelEnum : sbyte
{
    FailedRequest = 0,
    OkRequest = 1,
    AlreadyExecuted = 2,
    Disposed = 3
}

public class RunModel
{
    public RunModelEnum Status { get; }
    public int CountRunWorkers { get; }
    public IEnumerable<string> Messages { get; }
    public IEnumerable<object> Searches { get; }

    public RunModel(RunModelEnum status, int countRunWorkers, IEnumerable<object>? searches = null, params string[] messages)
    {
        Status = status;
        Messages = messages;
        CountRunWorkers = countRunWorkers;
        Searches = searches ?? Enumerable.Empty<object>();
    }

    public RunModel(RunModelEnum status, int countRunWorkers, IEnumerable<string> messages, IEnumerable<object>? searches = null)
        : this (status, countRunWorkers, searches, messages.ToArray())
    { }
}