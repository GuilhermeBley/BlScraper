using BlScraper.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlScraper.DependencyInjection.Extension.Builder;

public static class BuilderExtension
{
    public static IServiceCollection AddScraperBuilder(this IServiceCollection serviceCollection)
    {
        return 
            serviceCollection
                .AddScoped(typeof(IScrapBuilder), typeof(ScrapBuilder));
    }   
}