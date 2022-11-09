using BlScraper.DependencyInjection.Builder;
using BlScraper.DependencyInjection.Extension.Builder;
using BlScraper.DependencyInjection.Tests.QuestsBuilder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.Tests.Extension;

namespace BlScraper.DependencyInjection.Tests;

//
// Tests sintaxe: MethodName_ExpectedBehavior_StateUnderTest
// Example: isAdult_AgeLessThan18_False
//


public class ModelScraperDiBuilder
{
    [Fact]
    public async Task AddScraperBuilder_UseExtensionService_SuccessAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder();
            });
        
        Assert.NotNull(servicesBase.ServiceProvider.GetService<IScrapBuilder>());
    }

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
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.IsType<ScrapBuilderFake>(service);
    }
    
    [Fact]
    public async Task AddScraperBuilder_AddSameServiceSwitchOrder_SuccessSameTypeLocal()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScoped<IScrapBuilder, ScrapBuilderFake>()
                    .AddScraperBuilder();
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.IsNotType<ScrapBuilderFake>(service);
    }

    [Fact]
    public async Task CreateModel_TryInstanceModel_FailedBecauseQuestIsNotPublic()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.ThrowsAny<ArgumentException>(() => service.CreateModelByQuest(nameof(SimpleExecution)));
    }

    [Fact]
    public async Task CreateModel_TryInstanceModel_Success()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuest(nameof(PublicQuest)));
    }

    [Fact]
    public async Task CreateModel_TryInstanceModel_SuccessTypeTested()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.IsType<Model.ModelScraperService<PublicQuest, PublicSimpleData>>(service.CreateModelByQuest(nameof(PublicQuest)));
    }

    [Fact]
    public async Task CreateModel_TryInstanceModel_FailedInstanceRequiredConfigure()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.ThrowsAny<InvalidOperationException>(()=>service.CreateModelByQuest(nameof(SimpleQuest)));
    }

    [Fact]
    public async Task CreateModel_TryInstanceModel_SuccessWithDataExecute()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly))
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        var model = service.CreateModelByQuest(nameof(SimpleQuest));
        Assert.NotNull(model);

        if (model is null)
            throw new ArgumentNullException(nameof(model));

        await model.Run();
        
        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(5000).Token));
        Assert.Equal(countData, SimpleQuest.Counter);
    }
}