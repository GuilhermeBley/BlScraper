using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.Model.Context;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    /// <summary>
    /// Add service scrap builder
    /// </summary>
    /// <remarks>
    ///     <para>Add follow services:</para>
    ///     <list type="bullet">
    ///         <item><see cref="IScrapContextAccessor"/></item>
    ///         <item><see cref="IMapQuest"/></item>
    ///         <item><see cref="IScrapBuilder"/></item>
    ///     </list>
    /// </remarks>
    /// <param name="serviceCollection">current service collection</param>
    /// <param name="onAddAssemblies">assemblies to map</param>
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Action<ScrapBuilderConfig> onAddAssemblies)
    {
        var assemblyBuilderAdd = new ScrapBuilderConfig();
        onAddAssemblies?.Invoke(assemblyBuilderAdd);
        return
            serviceCollection
                .AddSingleton<IScrapContextAccessor>((serviceProvider) => new ScrapContextAcessor())
                .AddSingleton(typeof(IMapQuest), (serviceProvidier) => MapQuestFactory.Create(assemblyBuilderAdd.Assemblies.ToArray()))
                .AddSingleton(typeof(IScrapBuilder), 
                    (serviceProvider) => new ScrapBuilder(serviceProvider.CreateScope().ServiceProvider, assemblyBuilderAdd));
    }
}