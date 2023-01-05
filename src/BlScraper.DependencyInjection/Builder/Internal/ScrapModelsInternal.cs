using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.Model;

namespace BlScraper.DependencyInjection.Builder.Internal;

/// <summary>
/// Valdation models
/// </summary>
internal sealed class ScrapModelInternal
{
    /// <summary>
    /// Quest type
    /// </summary>
    private readonly Type _questType;

    /// <summary>
    /// Data Type
    /// </summary>
    private readonly Type _dataType;

    /// <summary>
    /// Filters
    /// </summary>
    public PoolFilter Filters { get; set; } = new();

    /// <inheritdoc cref="_questType" path="*"/>
    public Type QuestType => _questType;

    /// <inheritdoc cref="_dataType" path="*"/>
    public Type DataType => _dataType;

    /// <summary>
    /// Contains instance of type <see cref="IAllWorksEndConfigure{TQuest, TData}}"/> or null
    /// </summary>
    private object? _instanceAllWorksEnd;

    /// <summary>
    /// Contains instance of type <see cref="IGetArgsConfigure{TQuest, TData}"/> or null
    /// </summary>
    private object? _instanceArgs;

    /// <summary>
    /// Contains instance of type <see cref="IDataCollectedConfigure{TQuest, TData}"/> or null
    /// </summary>
    private object? _instanceDataCollected;
    
    /// <summary>
    /// Contains instance of type <see cref="IDataFinishedConfigure{TQuest, TData}"/> or null
    /// </summary>
    private object? _instanceDataFinished;
    
    /// <summary>
    /// Checks if is required type of <see cref="RequiredConfigure{TQuest, TData}{TQuest, TData}"/>
    /// </summary>
    private object? _instaceRequired;
    
    /// <summary>
    /// Contains instance of type <see cref="IQuestExceptionConfigure{TQuest, TData}"/> or null
    /// </summary>
    private object? _instanceQuestException;
    
    /// <summary>
    /// Contains instance of type <see cref="IQuestCreatedConfigure{TQuest, TData}{TQuest, TData}"/> or null
    /// </summary>
    private object? _instanceQuestCreated;
    
    /// <inheritdoc cref="_instanceAllWorksEnd" path="*"/>
    [Obsolete]
    public object? InstanceAllWorksEnd { 
        get => _instanceAllWorksEnd;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceAllWorksEnd));
            var typeInstanceRequired =
                typeof(IAllWorksEndConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instanceAllWorksEnd = value;
            else
                throw new ArgumentException(nameof(InstanceAllWorksEnd));
        }}
    
    /// <inheritdoc cref="_instanceArgs" path="*"/>
    public object? InstanceArgs { 
        get => _instanceArgs;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceArgs));
            var typeInstanceRequired =
                typeof(IGetArgsConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instanceArgs = value;
            else
                throw new ArgumentException(nameof(InstanceArgs));
        }}

    /// <inheritdoc cref="_instanceDataCollected" path="*"/>
    [Obsolete]
    public object? InstanceDataCollected { 
        get => _instanceDataCollected;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceDataCollected));
            var typeInstanceRequired =
                typeof(IDataCollectedConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instanceDataCollected = value;
            else
                throw new ArgumentException(nameof(InstanceDataCollected));
        }}

    /// <inheritdoc cref="_instanceDataFinished" path="*"/>
    [Obsolete]
    public object? InstanceDataFinished { 
        get => _instanceDataFinished;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceDataFinished));
            var typeInstanceRequired =
                typeof(IDataFinishedConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instanceDataFinished = value;
            else
                throw new ArgumentException(nameof(InstanceDataFinished));
        }}
    
    /// <inheritdoc cref="_instaceRequired" path="*"/>
    public object? InstanceRequired { 
        get => _instaceRequired;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceRequired));
            var typeInstanceRequired =
                typeof(RequiredConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instaceRequired = value;
            else
                throw new ArgumentException(nameof(InstanceRequired));
        }}

    /// <inheritdoc cref="_instanceQuestCreated" path="*"/>
    [Obsolete]
    public object? InstanceQuestCreated { 
        get => _instanceQuestCreated;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceQuestCreated));
            var typeInstanceRequired =
                typeof(IQuestCreatedConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instanceQuestCreated = value;
            else
                throw new ArgumentException(nameof(InstanceQuestCreated));
        }}

    /// <inheritdoc cref="_instanceQuestException" path="*"/>
    [Obsolete]
    public object? InstanceQuestException { 
        get => _instanceQuestException;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(InstanceQuestException));
            var typeInstanceRequired =
                typeof(IQuestExceptionConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.GetType()))
                _instanceQuestException = value;
            else
                throw new ArgumentException(nameof(InstanceQuestException));
        }}

    /// <summary>
    /// Contains instance of type <see cref="IAllWorksEndConfigure{TQuest, TData}}"/> or null
    /// </summary>
    private TypeUtils.FactoryType? _factoryAllWorksEnd;

    /// <summary>
    /// Contains factory of type <see cref="IGetArgsConfigure{TQuest, TData}"/> or null
    /// </summary>
    private TypeUtils.FactoryType? _factoryArgs;

    /// <summary>
    /// Contains factory of type <see cref="IDataCollectedConfigure{TQuest, TData}"/> or null
    /// </summary>
    private TypeUtils.FactoryType? _factoryDataCollected;
    
    /// <summary>
    /// Contains factory of type <see cref="IDataFinishedConfigure{TQuest, TData}"/> or null
    /// </summary>
    private TypeUtils.FactoryType? _factoryDataFinished;
    
    /// <summary>
    /// Contains factory of type <see cref="IQuestExceptionConfigure{TQuest, TData}"/> or null
    /// </summary>
    private TypeUtils.FactoryType? _factoryQuestException;
    
    /// <summary>
    /// Contains factory of type <see cref="IQuestCreatedConfigure{TQuest, TData}{TQuest, TData}"/> or null
    /// </summary>
    private TypeUtils.FactoryType? _factoryQuestCreated;
    
    /// <inheritdoc cref="_factoryAllWorksEnd" path="*"/>
    public TypeUtils.FactoryType? FactoryAllWorksEnd { 
        get => _factoryAllWorksEnd;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(FactoryAllWorksEnd));
            var typeInstanceRequired =
                typeof(IAllWorksEndConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.TypeToCreate.GetType()))
                _factoryAllWorksEnd = value;
            else
                throw new ArgumentException(nameof(FactoryAllWorksEnd));
        }}
    
    /// <inheritdoc cref="_factoryArgs" path="*"/>
    public TypeUtils.FactoryType? FactoryArgs { 
        get => _factoryArgs;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(FactoryArgs));
            var typeInstanceRequired =
                typeof(IGetArgsConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.TypeToCreate.GetType()))
                _factoryArgs = value;
            else
                throw new ArgumentException(nameof(FactoryArgs));
        }}

    /// <inheritdoc cref="_factoryDataCollected" path="*"/>
    public TypeUtils.FactoryType? FactoryDataCollected { 
        get => _factoryDataCollected;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(FactoryDataCollected));
            var typeInstanceRequired =
                typeof(IDataCollectedConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.TypeToCreate.GetType()))
                _factoryDataCollected = value;
            else
                throw new ArgumentException(nameof(FactoryDataCollected));
        }}

    /// <inheritdoc cref="_factoryDataFinished" path="*"/>
    public TypeUtils.FactoryType? FactoryDataFinished { 
        get => _factoryDataFinished;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(FactoryDataFinished));
            var typeInstanceRequired =
                typeof(IDataFinishedConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.TypeToCreate.GetType()))
                _factoryDataFinished = value;
            else
                throw new ArgumentException(nameof(FactoryDataFinished));
        }}

    /// <inheritdoc cref="_factoryQuestCreated" path="*"/>
    public TypeUtils.FactoryType? FactoryQuestCreated { 
        get => _factoryQuestCreated;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(FactoryQuestCreated));
            var typeInstanceRequired =
                typeof(IQuestCreatedConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.TypeToCreate.GetType()))
                _factoryQuestCreated = value;
            else
                throw new ArgumentException(nameof(FactoryQuestCreated));
        }}

    /// <inheritdoc cref="_factoryQuestException" path="*"/>
    public TypeUtils.FactoryType? FactoryQuestException { 
        get => _factoryQuestException;
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(FactoryQuestException));
            var typeInstanceRequired =
                typeof(IQuestExceptionConfigure<,>).MakeGenericType(_questType, _dataType);
            if (typeInstanceRequired.IsAssignableFrom(value.TypeToCreate.GetType()))
               _factoryQuestException = value;
            else
                throw new ArgumentException(nameof(FactoryQuestException));
        }}

    /// <summary>
    /// Instance with model
    /// </summary>
    /// <remarks>
    ///     <para>The <paramref name="questType"/> must be a class non static and public.</para>
    /// </remarks>
    /// <param name="questType">Type assingnable from <see cref="Quest{TData}"/></param>
    /// <exception cref="ArgumentException"></exception>
    public ScrapModelInternal(Type questType)
    {
        if (!TypeUtils.IsSubclassOfRawGeneric(typeof(Quest<>), questType, out Type? assignableToGenericFound) 
            || !TypeUtils.IsTypeValidQuest(questType)
            || assignableToGenericFound is null)
            throw new ArgumentException($"{nameof(questType)} is a invalid type.", typeof(Quest<>).Name);

        var genericTypes = assignableToGenericFound.GetGenericArguments();
        if (genericTypes.Length != 1)
            throw new ArgumentException($"The data of quest can't be found.", typeof(Quest<>).Name);

        var dataType = genericTypes[0];
        if (dataType.IsAbstract ||
            !dataType.IsClass ||
            dataType.GetGenericArguments().Any())
            throw new ArgumentException($"{dataType.FullName} is a invalid type.", typeof(Quest<>).Name);

        _dataType = dataType;
        _questType = questType;
    }
}