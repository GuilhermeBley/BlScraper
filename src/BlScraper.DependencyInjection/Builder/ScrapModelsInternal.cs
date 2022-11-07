using BlScraper.DependencyInjection.ConfigureModel;

namespace BlScraper.DependencyInjection.Builder;

internal class ScrapModelsInternal
{
    private readonly Type _questType;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Contains instance of type <see cref="IOnAllWorksEndConfigure{TQuest, TData}}"/> or null
    /// </summary>
    public object? InstanceAllWorksEnd { get; set; }
    
    /// <summary>
    /// Contains instance of type <see cref="IGetArgsConfigure{TQuest, TData}"/> or null
    /// </summary>
    public object? InstanceArgs { get; set; }

    /// <summary>
    /// Contains instance of type <see cref="IOnDataCollectedConfigure{TQuest, TData}"/> or null
    /// </summary>
    public object? InstanceDataCollected { get; set; }

    /// <summary>
    /// Contains instance of type <see cref="IDataFinishedConfigure{TQuest, TData}"/> or null
    /// </summary>
    public object? InstanceDataFinished { get; set; }
    
    /// <summary>
    /// Checks if is required type of <see cref="IOnAllWorksEndConfigure{TQuest, TData}"/>
    /// </summary>
    public object? InstaceRequired { get; set; }

    /// <summary>
    /// Contains instance of type <see cref="IOnQuestCreatedConfigure{TQuest, TData}"/> or null
    /// </summary>
    public object? InstanceQuestCreated { get; set; }

    /// <summary>
    /// Contains instance of type <see cref="IQuestExceptionConfigure{TQuest, TData}"/> or null
    /// </summary>
    public object? InstanceQuestException { get; set; }

    public ScrapModelsInternal(IServiceProvider serviceProvider, Type questType)
    {
        _questType = questType;
        _serviceProvider = serviceProvider;
    }
}