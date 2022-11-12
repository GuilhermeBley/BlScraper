using BlScraper.DependencyInjection.Builder;
using BlScraper.DependencyInjection.Extension.Builder;
using BlScraper.DependencyInjection.Tests.QuestsBuilder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.Tests.Extension;
using BlScraper.DependencyInjection.ConfigureModel;

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
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
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
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly))
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
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly));
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

    [Fact]
    public async Task CreateModel_TryInstanceModel_FailedBecauseQuestIsObsolete()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        var model = service.CreateModelByQuestOrDefault("ObsoleteQuest");
        Assert.Null(model);
    }

    [Fact]
    public async Task CreateModel_TryInstanceModelTwoWithSameName_FailedBecauseBothAsSameName()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        var model = service.CreateModelByQuestOrDefault(nameof(SimpleQuestDuplicated));
        Assert.Null(model);
    }
    
    [Fact]
    public async Task CreateModel_TryInstanceModelTwoWithSameName_SuccessBecauseOneIsObsolete()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        var model = service.CreateModelByQuestOrDefault(nameof(SimpleQuestDuplicatedObsolete));
        Assert.IsType<Model.ModelScraperService<SimpleQuestDuplicatedObsolete, PublicSimpleData>>(model);
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConifg_FailedAllWorksEnd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        
        Assert.Throws<ArgumentException>(
            typeof(IOnAllWorksEndConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuest(nameof(WithoutAllWorksEndQuest));        
            });
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConifg_FailedGetArgs()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        
        Assert.Throws<ArgumentException>(
            typeof(IGetArgsConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuest(nameof(WithoutGetArgsQuest));        
            });
    }

    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConifg_FailedDataCollected()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        
        Assert.Throws<ArgumentException>(
            typeof(IOnDataCollectedConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuest(nameof(WithoutDataCollectedQuest));        
            });
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConifg_FailedDataFinished()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        
        Assert.Throws<ArgumentException>(
            typeof(IDataFinishedConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuest(nameof(WithoutDataFinishedQuest));        
            });
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConifg_FailedQuestCreated()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        
        Assert.Throws<ArgumentException>(
            typeof(IOnQuestCreatedConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuest(nameof(WithoutQuestCreatedQuest));        
            });
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConifg_FailedQuestException()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        
        Assert.Throws<ArgumentException>(
            typeof(IQuestExceptionConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuest(nameof(WithoutQuestExceptionQuest));        
            });
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConifg_SuccessDataCollected()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));;
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var serviceRoute = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();
        
        var model = service.CreateModelByQuest(nameof(DataCollectedQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(DataCollectedConfigure).GetMethod(nameof(DataCollectedConfigure.OnCollected)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConifg_SuccessAllWorksEnd()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));;
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var serviceRoute = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();
        
        var model = service.CreateModelByQuest(nameof(AllWorksEndQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(AllWorksEndConfigure).GetMethod(nameof(AllWorksEndConfigure.OnFinished)),
            serviceRoute.Routes);
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConifg_SuccessDataFinished()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));;
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var serviceRoute = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();
        
        var model = service.CreateModelByQuest(nameof(DataFinishedQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(DataFinishedConfigure).GetMethod(nameof(DataFinishedConfigure.OnDataFinished)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConifg_SuccessGetArgs()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));;
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var serviceRoute = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();
        
        var model = service.CreateModelByQuest(nameof(GetArgsQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(GetArgsConfigure).GetMethod(nameof(GetArgsConfigure.GetArgs)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConifg_SuccessQuestCreated()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));;
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var serviceRoute = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();
        
        var model = service.CreateModelByQuest(nameof(QuestCreatedQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(QuestCreatedConfigure).GetMethod(nameof(QuestCreatedConfigure.OnCreated)),
            serviceRoute.Routes);
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConifg_SuccessQuestException()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData));;
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var serviceRoute = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();
        
        var model = service.CreateModelByQuest(nameof(QuestExceptionQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(QuestExceptionConfigure).GetMethod(nameof(QuestExceptionConfigure.OnOccursException)),
            serviceRoute.Routes);
    }
}