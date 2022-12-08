using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.ConfigureBuilder;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Action<AssemblyBuilderAdd> onAddAssemblies)
    {
        var assemblyBuilderAdd = new AssemblyBuilderAdd();
        onAddAssemblies?.Invoke(assemblyBuilderAdd);
        return
            serviceCollection
                .AddSingleton(typeof(IMapQuest), () => new MapQuest(assemblyBuilderAdd))
                .AddSingleton(typeof(IScrapBuilder), 
                    (serviceProvider) => new ScrapBuilder(serviceProvider.CreateScope().ServiceProvider, assemblyBuilderAdd));
    }  
}