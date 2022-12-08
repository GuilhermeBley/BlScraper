namespace BlScraper.Model;

/// <summary>
/// Context to execute quest
/// </summary>
/// <typeparam name="TData">Works with a type of data.</typeparam>
public interface IQuest : IDisposable
{
    /// <summary>
    /// Thread which execute the class
    /// </summary>
    int Id { get; }
}