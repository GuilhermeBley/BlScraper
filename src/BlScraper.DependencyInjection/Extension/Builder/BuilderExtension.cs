using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.ConfigureBuilder;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection)
    {
        return 
            serviceCollection
                .AddSingleton(typeof(IScrapBuilder), typeof(ScrapBuilder));
    }

    
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection, Action<AssemblyBuilderAdd> onAddAssemblies)
    {
        return 
            serviceCollection
                .AddSingleton(typeof(IScrapBuilder), typeof(ScrapBuilder))
                .AddSingleton<AssemblyBuilderAdd>((provider)=>{
                    var AssemblyBuilderAdd = new AssemblyBuilderAdd();
                    onAddAssemblies?.Invoke(AssemblyBuilderAdd);
                    return AssemblyBuilderAdd;
                });
    }  
}