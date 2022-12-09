using BlScraper.DependencyInjection.Model;

namespace BlScraper.DependencyInjection.ConfigureBuilder;

/// <summary>
/// Configuration builder
/// </summary>
public class AssemblyBuilderAdd
{
    /// <summary>
    /// Type of Model to instance
    /// </summary>
    private readonly Type _modelScraperServiceType;

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    private HashSet<System.Reflection.Assembly> _assemblies = new();

    /// <summary>
    /// Assemblies to map models
    /// </summary>
    internal IEnumerable<System.Reflection.Assembly> Assemblies => _assemblies;

    /// <summary>
    /// Lock object
    /// </summary>
    private object _lock = new();

    /// <inheritdoc cref="_modelScraperServiceType" path="*"/>
    public Type ModelScraperServiceType => _modelScraperServiceType;

    /// <summary>
    /// Instance of assembly builder
    /// </summary>
    /// <param name="modelScraperServiceType">Type parameter, it must have assignable from <see cref="ModelScraperService"/></param>
    internal AssemblyBuilderAdd(Type? modelScraperServiceType = null) 
    {
        if (modelScraperServiceType is not null &&
            (!Builder.TypeUtils.IsSubclassOfRawGeneric(typeof(ModelScraperService<,>), modelScraperServiceType) ||
            !typeof(ModelScraperService<,>).Equals(modelScraperServiceType)))
            throw new ArgumentException($"parameter '{nameof(modelScraperServiceType)}' must have assignable or equals from '{typeof(ModelScraperService<,>).FullName}'");
        
        if (modelScraperServiceType is null)
            modelScraperServiceType = typeof(ModelScraperService<,>);

        _modelScraperServiceType = modelScraperServiceType;
    }

    /// <summary>
    /// Try add new assemblies to map
    /// </summary>
    /// <param name="assembly">Assemblie to add</param>
    public AssemblyBuilderAdd AddAssembly(System.Reflection.Assembly assembly)
    {
        lock(_lock)
        {
            _assemblies.Add(assembly);
        }

        return this;
    }
}