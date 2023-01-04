using BlScraper.Model;

namespace BlScraper.DependencyInjection.ConfigureModel;

/// <summary>
/// Required implementation for quests, it that defines the data to search and initial quantity of scrapers
/// </summary>
/// <typeparam name="TQuest">Identifier quest</typeparam>
/// <typeparam name="TData">Data type</typeparam>
public abstract class RequiredConfigure<TQuest, TData> : IRequiredConfigure, IRequiredConfigureFilters
    where TQuest : Quest<TData>
    where TData : class
{
    /// <summary>
    /// Required filters
    /// </summary>
    protected virtual Type[] _requiredFilters { get; } = new Type[0];

    /// <inheritdoc cref="IRequiredConfigure.IsRequiredAllWorksEnd" path="*"/>
    public virtual bool IsRequiredAllWorksEnd => false;

    /// <inheritdoc cref="IRequiredConfigure.IsRequiredArgs" path="*"/>
    public virtual bool IsRequiredArgs => false;

    /// <inheritdoc cref="IRequiredConfigure.IsRequiredDataCollected" path="*"/>
    public virtual bool IsRequiredDataCollected => false;

    /// <inheritdoc cref="IRequiredConfigure.IsRequiredDataFinished" path="*"/>
    public virtual bool IsRequiredDataFinished => false;

    /// <inheritdoc cref="IRequiredConfigure.IsRequiredQuestCreated" path="*"/>
    public virtual bool IsRequiredQuestCreated => false;

    /// <inheritdoc cref="IRequiredConfigure.IsRequiredQuestException" path="*"/>
    public virtual bool IsRequiredQuestException => false;

    /// <inheritdoc cref="IRequiredConfigure.initialQuantity" path="*"/>
    public abstract int initialQuantity { get; }

    /// <summary>
    /// Required filters
    /// </summary>
    public Type[] RequiredFilters => _requiredFilters;

    /// <summary>
    /// Instance
    /// </summary>
    public RequiredConfigure() { }

    /// <summary>
    /// Instance with filters
    /// </summary>
    /// <param name="filter">filters</param>
    public RequiredConfigure(params Type[] filter)
    {
        _requiredFilters = filter;
    }

    /// <summary>
    /// Called to collect data to search
    /// </summary>
    /// <returns>Async list of data</returns>
    public abstract Task<IEnumerable<TData>> GetData();
}

/// <summary>
/// Required events
/// </summary>
internal interface IRequiredConfigure
{
    /// <summary>
    /// Checks if is required type of <see cref="IAllWorksEndConfigure{TQuest, TData}"/>
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
    [Obsolete("Required args is used on creation of model.")]
    bool IsRequiredArgs { get; }

    /// <summary>
    /// Checks if is required type of <see cref="IDataCollectedConfigure{TQuest, TData}"/>
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
    /// Checks if is required type of <see cref="IQuestCreatedConfigure{TQuest, TData}"/>
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
}

/// <summary>
/// Required filters
/// </summary>
internal interface IRequiredConfigureFilters
{
    /// <summary>
    /// Required filters
    /// </summary>
    Type[] RequiredFilters { get; }
}