using BlScraper.DependencyInjection.Model;

namespace BlScraper.DependencyInjection.ConfigureBuilder;

/// <summary>
/// Configuration builder
/// </summary>
public class ScrapBuilderConfig
{
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

    /// <summary>
    /// Instance of assembly builder
    /// </summary>
    /// <param name="modelScraperServiceType">Type parameter, it must have assignable from <see cref="ModelScraperService"/></param>
    internal ScrapBuilderConfig() 
    {
    }

    /// <summary>
    /// Try add new assemblies to map
    /// </summary>
    /// <param name="assembly">Assemblie to add</param>
    public ScrapBuilderConfig AddAssembly(System.Reflection.Assembly assembly)
    {
        lock(_lock)
        {
            _assemblies.Add(assembly);
        }

        return this;
    }
}