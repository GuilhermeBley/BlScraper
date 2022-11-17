using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.ConfigureBuilder;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Action<AssemblyBuilderAdd> onAddAssemblies)
    {
        var AssemblyBuilderAdd = new AssemblyBuilderAdd();
        onAddAssemblies?.Invoke(AssemblyBuilderAdd);
        return
            serviceCollection
                .AddSingleton(typeof(IScrapBuilder), 
                    (serviceProvider) => new ScrapBuilder(serviceProvider.CreateScope().ServiceProvider, AssemblyBuilderAdd));
    }  
}