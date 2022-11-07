using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Required implementation for quests, it that defines the data to search and initial quantity of scrapers
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public interface IRequiredConfigure<TQuest, TData>
    where TQuest : Quest<TData>
    where TData : class
{   
    /// <summary>
    /// Checks if is required type of <see cref="IOnAllWorksEndConfigure{TQuest, TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>True : Is necessary to the execution, false: don't is</para>
    /// </remarks>
    bool IsRequiredAllWorksEnd { get; }

    /// <summary>
    /// Checks if is required type of <see cref="IGetArgsConfigure{TQuest, TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>True : Is necessary to the execution, false: don't is</para>
    /// </remarks>
    bool IsRequiredArgs { get; }

    /// <summary>
    /// Checks if is required type of <see cref="IOnDataCollectedConfigure{TQuest, TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>True : Is necessary to the execution, false: don't is</para>
    /// </remarks>
    bool IsRequiredDataCollected { get; }
    
    /// <summary>
    /// Checks if is required type of <see cref="IDataFinishedConfigure{TQuest, TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>True : Is necessary to the execution, false: don't is</para>
    /// </remarks>
    bool IsRequiredDataFinished { get; }

    /// <summary>
    /// Checks if is required type of <see cref="IOnQuestCreatedConfigure{TQuest, TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>True : Is necessary to the execution, false: don't is</para>
    /// </remarks>
    bool IsRequiredQuestCreated { get; }

    /// <summary>
    /// Checks if is required type of <see cref="IQuestExceptionConfigure{TQuest, TData}"/>
    /// </summary>
    /// <remarks>
    ///     <para>True : Is necessary to the execution, false: don't is</para>
    /// </remarks>
    bool IsRequiredQuestException { get; }

    /// <summary>
    /// Initial quantity to execute
    /// </summary>
    int initialQuantity { get; }

    /// <summary>
    /// Called to collect data to search
    /// </summary>
    /// <returns>Async list of data</returns>
    Task<IEnumerable<TData>> GetData();
}