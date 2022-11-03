namespace BlScraper.DependencyInjection.ConfigureBuilder;

public class AssemblyBuilderAdd
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