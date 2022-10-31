using BlScraper.DependencyInjection.Builder;
using BlScraper.DependencyInjection.Extension.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlScraper.DependencyInjection.Tests;

//
// Tests sintaxe: MethodName_ExpectedBehavior_StateUnderTest
// Example: isAdult_AgeLessThan18_False
//


public class ModelScraperDiBuilder
{
    [Fact]
    public async Task AddScraperBuilder_AddSameService_SuccessSameTypeLocal()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder()
                    .AddScoped<IScrapBuilder, ScrapBuilderFake>();
            });
        Assert.IsType(
            typeof(ScrapBuilderFake), 
            servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>().GetType());
    }
}