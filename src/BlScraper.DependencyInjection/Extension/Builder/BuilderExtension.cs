using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.ConfigureBuilder;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    private static readonly object _stateLock = new();

    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Type modelScrapServiceType, Action<AssemblyBuilderAdd> onAddAssemblies)
    {
        lock (_stateLock)
            if (serviceCollection.Any(serviceDescriptor => serviceDescriptor?.ServiceType?.Equals(typeof(IMapQuest)) ?? false))
                throw new InvalidOperationException($"'{nameof(AddScraperBuilder)}' already executed in this collection.");

        var assemblyBuilderAdd = new AssemblyBuilderAdd(modelScrapServiceType);
        onAddAssemblies?.Invoke(assemblyBuilderAdd);
        return
            serviceCollection
                .AddSingleton(typeof(IMapQuest), (serviceProvidier) => new MapQuest(assemblyBuilderAdd))
                .AddSingleton(typeof(IScrapBuilder), 
                    (serviceProvider) => new ScrapBuilder(serviceProvider.CreateScope().ServiceProvider, assemblyBuilderAdd));
    }

    
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Action<AssemblyBuilderAdd> onAddAssemblies, Type? modelScrapServiceType = null)
    {
        return AddScraperBuilder(serviceCollection, typeof(Model.ModelScraperService<,>), onAddAssemblies);
    }
}