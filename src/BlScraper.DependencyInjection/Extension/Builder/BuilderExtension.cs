using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.Model.Context;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    private static readonly object _stateLock = new();

    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Action<ScrapBuilderConfig> onAddAssemblies)
    {
        var assemblyBuilderAdd = new ScrapBuilderConfig();
        onAddAssemblies?.Invoke(assemblyBuilderAdd);
        return
            serviceCollection
                .AddSingleton<ScrapContextAcessor>()
                .AddSingleton<IScrapContextAcessor>((serviceProvider) => serviceProvider.GetRequiredService<ScrapContextAcessor>())
                .AddSingleton(typeof(IMapQuest), (serviceProvidier) => MapQuestFactory.Create(assemblyBuilderAdd.Assemblies.ToArray()))
                .AddSingleton(typeof(IScrapBuilder), 
                    (serviceProvider) => new ScrapBuilder(serviceProvider.CreateScope().ServiceProvider, assemblyBuilderAdd));
    }
}