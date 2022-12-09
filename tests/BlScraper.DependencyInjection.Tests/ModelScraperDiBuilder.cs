using BlScraper.DependencyInjection.Builder;
using BlScraper.DependencyInjection.Extension.Builder;
using BlScraper.DependencyInjection.Tests.QuestsBuilder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.Tests.Extension;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.Tests.FakeProject;
using BlScraper.DependencyInjection.Model;

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

        Assert.ThrowsAny<ArgumentException>(() => service.CreateModelByQuestName(nameof(SimpleExecution)));
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

        Assert.NotNull(service.CreateModelByQuestName(nameof(PublicQuest)));
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

        Assert.IsType<Model.ModelScraperService<PublicQuest, PublicSimpleData>>(service.CreateModelByQuestName(nameof(PublicQuest)));
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

        Assert.ThrowsAny<InvalidOperationException>(()=>service.CreateModelByQuestName(nameof(SimpleQuest)));
    }

    [Fact]
    public async Task CreateModel_TryInstanceModel_SuccessWithDataExecute()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config=>config.AddAssembly(this.GetType().Assembly))
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData))
                    .AddSingleton<ICounterService, CounterService>();
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var countService = servicesBase.ServiceProvider.GetRequiredService<ICounterService>();

        var model = service.CreateModelByQuestName(nameof(SimpleQuest));
        Assert.NotNull(model);

        if (model is null)
            throw new ArgumentNullException(nameof(model));

        await model.Run();
        
        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(5000).Token));
        Assert.Equal(countData, countService.Count);
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

        var model = service.CreateModelByQuestNameOrDefault("ObsoleteQuest");
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

        var model = service.CreateModelByQuestNameOrDefault(nameof(SimpleQuestDuplicated));
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

        var model = service.CreateModelByQuestNameOrDefault(nameof(SimpleQuestDuplicatedObsolete));
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
            typeof(IAllWorksEndConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuestName(nameof(WithoutAllWorksEndQuest));        
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
                service.CreateModelByQuestName(nameof(WithoutGetArgsQuest));        
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
            typeof(IDataCollectedConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuestName(nameof(WithoutDataCollectedQuest));        
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
                service.CreateModelByQuestName(nameof(WithoutDataFinishedQuest));        
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
            typeof(IQuestCreatedConfigure<,>).Name, ()=>{ 
                service.CreateModelByQuestName(nameof(WithoutQuestCreatedQuest));        
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
                service.CreateModelByQuestName(nameof(WithoutQuestExceptionQuest));        
            });
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfig_SuccessDataCollected()
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
        
        var model = service.CreateModelByQuestName(nameof(DataCollectedQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(DataCollectedConfigure).GetMethod(nameof(DataCollectedConfigure.OnCollected)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfig_SuccessAllWorksEnd()
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
        
        var model = service.CreateModelByQuestName(nameof(AllWorksEndQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(AllWorksEndConfigure).GetMethod(nameof(AllWorksEndConfigure.OnFinished)),
            serviceRoute.Routes);
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfig_SuccessDataFinished()
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
        
        var model = service.CreateModelByQuestName(nameof(DataFinishedQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(DataFinishedConfigure).GetMethod(nameof(DataFinishedConfigure.OnDataFinished)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfig_SuccessGetArgs()
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
        
        var model = service.CreateModelByQuestName(nameof(GetArgsQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(GetArgsConfigure).GetMethod(nameof(GetArgsConfigure.GetArgs)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfig_SuccessQuestCreated()
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
        
        var model = service.CreateModelByQuestName(nameof(QuestCreatedQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(QuestCreatedConfigure).GetMethod(nameof(QuestCreatedConfigure.OnCreated)),
            serviceRoute.Routes);
    }

    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfig_SuccessQuestException()
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
        
        var model = service.CreateModelByQuestName(nameof(QuestExceptionQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(QuestExceptionConfigure).GetMethod(nameof(QuestExceptionConfigure.OnOccursException)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithRequiredConfigAll_SuccessConfigureAll()
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
        
        var model = service.CreateModelByQuestName(nameof(AllConfigureQuests));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(AllWorkEndConfigureAll).GetMethod(nameof(AllWorkEndConfigureAll.OnFinished)),
            serviceRoute.Routes);

        Assert.Contains(typeof(DataCollectedConfigureAll).GetMethod(nameof(DataCollectedConfigureAll.OnCollected)),
            serviceRoute.Routes);

        Assert.Contains(typeof(DataFinishedConfigureAll).GetMethod(nameof(DataFinishedConfigureAll.OnDataFinished)),
            serviceRoute.Routes);

        Assert.Contains(typeof(GetArgsConfigureAll).GetMethod(nameof(GetArgsConfigureAll.GetArgs)),
            serviceRoute.Routes);

        Assert.Contains(typeof(QuestCreatedConfigureAll).GetMethod(nameof(QuestCreatedConfigureAll.OnCreated)),
            serviceRoute.Routes);

        Assert.Contains(typeof(RequiredConfigureAll).GetMethod(nameof(RequiredConfigureAll.GetData)),
            serviceRoute.Routes);

        Assert.Contains(typeof(QuestExceptionConfigureAll).GetMethod(nameof(QuestExceptionConfigureAll.OnOccursException)),
            serviceRoute.Routes);
    }
    
    [Fact]
    public async Task RequiredConfig_TryInstanceWithoutRequiredConfigAll_SuccessConfigureAll()
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
        
        var model = service.CreateModelByQuestName(nameof(AllWithOutRequiredConfigureQuests));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Contains(typeof(AllWorkEndWithoutRequiredConfigureAll).GetMethod(nameof(AllWorkEndWithoutRequiredConfigureAll.OnFinished)),
            serviceRoute.Routes);

        Assert.Contains(typeof(DataCollectedWithoutRequiredConfigureAll).GetMethod(nameof(DataCollectedWithoutRequiredConfigureAll.OnCollected)),
            serviceRoute.Routes);

        Assert.Contains(typeof(DataFinishedWithoutRequiredConfigureAll).GetMethod(nameof(DataFinishedWithoutRequiredConfigureAll.OnDataFinished)),
            serviceRoute.Routes);

        Assert.Contains(typeof(GetArgsWithoutRequiredConfigureAll).GetMethod(nameof(GetArgsWithoutRequiredConfigureAll.GetArgs)),
            serviceRoute.Routes);

        Assert.Contains(typeof(QuestCreatedWithoutRequiredConfigureAll).GetMethod(nameof(QuestCreatedWithoutRequiredConfigureAll.OnCreated)),
            serviceRoute.Routes);

        Assert.Contains(typeof(RequiredWithoutRequiredConfigureAll).GetMethod(nameof(RequiredWithoutRequiredConfigureAll.GetData)),
            serviceRoute.Routes);

        Assert.Contains(typeof(QuestExceptionWithoutRequiredConfigureAll).GetMethod(nameof(QuestExceptionWithoutRequiredConfigureAll.OnOccursException)),
            serviceRoute.Routes);
    }

    [Fact]
    public async Task Inherit_TryInstanceWithRequiredConfig_SuccessQuest()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData))
                    .AddSingleton<ICounterService, CounterService>();
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var countService = servicesBase.ServiceProvider.GetRequiredService<ICounterService>();
        
        var model = service.CreateModelByQuestName(nameof(InheritFromSimpleQuest));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Equal(countData, countService.Count);
    }

    [Fact]
    public async Task Inherit_TryInstanceWithInheritRequiredConfig_FailedConfigure()
    {
        await Task.CompletedTask;
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData))
                    .AddSingleton<ICounterService, CounterService>();
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var countService = servicesBase.ServiceProvider.GetRequiredService<ICounterService>();

        Assert.Throws<ArgumentException>(
            typeof(RequiredConfigure<,>).Name,
            () => service.CreateModelByQuestName(nameof(InheritFromSimpleQuestTwoConfig)));
    }

    [Fact]
    public async Task Inherit_TryInstanceWithAbstractRequiredConfig_SuccessQuest()
    {
        const int countData = 10;
        var servicesBase
            = new ServicesTestBase(services => {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>()
                    .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(countData))
                    .AddSingleton<ICounterService, CounterService>();
            });
        
        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var countService = servicesBase.ServiceProvider.GetRequiredService<ICounterService>();
        
        var model = service.CreateModelByQuestName(nameof(InheritFromSimpleQuestAbstractConfig));
        await model.Run();

        Assert.True(await model.WaitModelDispose(new CancellationTokenSource(3000).Token));

        Assert.Equal(countData, countService.Count);
    }

    [Fact]
    public async Task CreateModelByQuestName_TryInstance_SuccessInstance()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestName(nameof(SimpleQuestWithOutServices)));
    }

    [Fact]
    public async Task CreateModelByQuestName_TryInstance_FailedInstanceBecauseWithoutRequiredConfig()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.ThrowsAny<Exception>(()=>service.CreateModelByQuestName(nameof(WithoutConfigQuest)));
    }
    
    [Fact]
    public async Task CreateModelByQuestNameOrDefault_TryInstance_SuccessInstance()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestNameOrDefault(nameof(SimpleQuestWithOutServices)));
    }

    [Fact]
    public async Task CreateModelByQuestNameOrDefault_TryInstance_FailedInstanceBecauseWithoutRequiredConfig()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestNameOrDefault(nameof(WithoutConfigQuest)));
    }
    
    [Fact]
    public async Task CreateModelByQuestType_TryInstance_SuccessInstance()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestType(typeof(SimpleQuestWithOutServices)));
    }

    [Fact]
    public async Task CreateModelByQuestType_TryInstance_FailedInstanceBecauseWithoutRequiredConfig()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.ThrowsAny<Exception>(()=>service.CreateModelByQuestType(typeof(WithoutConfigQuest)));
    }

    [Fact]
    public async Task CreateModelByQuestType_TryInstance_FailedInstanceBecauseTypeIsInvalid()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Throws<ArgumentException>(typeof(BlScraper.Model.Quest<>).Name, 
            ()=>service.CreateModelByQuestType(typeof(object)));
        Assert.Throws<ArgumentException>(typeof(BlScraper.Model.Quest<>).Name, 
            ()=>service.CreateModelByQuestType(typeof(BlScraper.Model.Quest<>)));
        Assert.Throws<ArgumentException>(typeof(BlScraper.Model.Quest<>).Name, 
            ()=>service.CreateModelByQuestType(typeof(BlScraper.Model.Quest<PublicSimpleData>)));
    }
        
    [Fact]
    public async Task CreateModelByQuestTypeOrDefault_TryInstance_SuccessInstance()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestTypeOrDefault(typeof(SimpleQuestWithOutServices)));
    }

    [Fact]
    public async Task CreateModelByQuestTypeOrDefault_TryInstance_FailedInstanceBecauseWithoutRequiredConfig()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestTypeOrDefault(typeof(WithoutConfigQuest)));
    }

    [Fact]
    public async Task CreateModelByQuestTypeOrDefault_TryInstance_FailedInstanceBecauseTypeIsInvalid()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestTypeOrDefault(typeof(object)));
        Assert.Null(service.CreateModelByQuestTypeOrDefault(typeof(BlScraper.Model.Quest<>)));
        Assert.Null(service.CreateModelByQuestTypeOrDefault(typeof(BlScraper.Model.Quest<PublicSimpleData>)));
    }
    
    [Fact]
    public async Task CreateModelByQuestTypeGeneric_TryInstance_SuccessInstance()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestType<SimpleQuestWithOutServices>());
    }

    [Fact]
    public async Task CreateModelByQuestTypeGeneric_TryInstance_FailedInstanceBecauseWithoutRequiredConfig()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.ThrowsAny<Exception>(()=>service.CreateModelByQuestType<WithoutConfigQuest>());
    }

    [Fact]
    public async Task CreateModelByQuestTypeGeneric_TryInstance_FailedInstanceBecauseTypeIsInvalid()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Throws<ArgumentException>(typeof(BlScraper.Model.Quest<>).Name, 
            ()=>service.CreateModelByQuestType<object>());
        Assert.Throws<ArgumentException>(typeof(BlScraper.Model.Quest<>).Name, 
            ()=>service.CreateModelByQuestType<BlScraper.Model.Quest<PublicSimpleData>>());
    }
        
    [Fact]
    public async Task CreateModelByQuestTypeOrDefaultGeneric_TryInstance_SuccessInstance()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestTypeOrDefault<SimpleQuestWithOutServices>());
    }

    [Fact]
    public async Task CreateModelByQuestTypeOrDefaultGeneric_TryInstance_FailedInstanceBecauseWithoutRequiredConfig()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestTypeOrDefault<WithoutConfigQuest>());
    }

    [Fact]
    public async Task CreateModelByQuestTypeOrDefaultGeneric_TryInstance_FailedInstanceBecauseTypeIsInvalid()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestTypeOrDefault<object>());
        Assert.Null(service.CreateModelByQuestTypeOrDefault<BlScraper.Model.Quest<PublicSimpleData>>());
    }

    [Fact]
    public async Task CreateModelInOtherProject_InstaceQuestOfOtherProject_SuccessAssemblyAdded()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly).AddAssembly(typeof(QuestInOtherProject).Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestTypeOrDefault<QuestInOtherProject>());
    }

    [Fact]
    public async Task CreateModelInOtherProject_InstaceQuestOfOtherProject_FailedBecauseAssemblyDontAdded()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestTypeOrDefault<QuestInOtherProject>());
    }

    [Fact]
    public async Task CreateModelInOtherProject_InstaceQuestWithotherConfigure_SuccessQuestInstanced()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.NotNull(service.CreateModelByQuestTypeOrDefault<QuestWithoutConfig>());
    }

    [Fact]
    public async Task CreateModelInOtherProject_InstaceQuestWithoutConfigure_FailedQuestInstanced()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var service = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();

        Assert.Null(service.CreateModelByQuestTypeOrDefault<QuestWithoutConfig2>());
    }

    [Fact]
    public async Task GetAvailableQuestsAndData_GetTypes_SuccessNotEmpty()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var mapQuest = servicesBase.ServiceProvider.GetRequiredService<ConfigureBuilder.IMapQuest>();

        Assert.NotEmpty(mapQuest.GetAvailableQuestsAndData());
    }

    [Fact]
    public async Task GetAvailableQuests_GetTypes_SuccessNotEmpty()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var mapQuest = servicesBase.ServiceProvider.GetRequiredService<ConfigureBuilder.IMapQuest>();

        Assert.NotEmpty(mapQuest.GetAvailableQuests());
    }

    [Fact]
    public async Task GetAvailableQuestsAndData_CheckAllTypes_SuccessNotAllAreQuest()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var mapQuest = servicesBase.ServiceProvider.GetRequiredService<ConfigureBuilder.IMapQuest>();

        var tuples = mapQuest.GetAvailableQuestsAndData();
        Assert.NotEmpty(tuples);
        Assert.All(tuples, (tuple) =>
        {
            Assert.True(typeof(BlScraper.Model.IQuest).IsAssignableFrom(tuple.Quest));
        });
    }

    [Fact]
    public async Task GetAvailableQuests_CheckAllTypes_SuccessNotAllAreQuest()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly));
            });

        var mapQuest = servicesBase.ServiceProvider.GetRequiredService<ConfigureBuilder.IMapQuest>();

        var quests = mapQuest.GetAvailableQuests();
        Assert.NotEmpty(quests);
        Assert.All(quests, (quest) =>
        {
            Assert.True(typeof(BlScraper.Model.IQuest).IsAssignableFrom(quest));
        });
    }

    [Fact]
    public async Task AddScraperBuilderTyped_TryAddMoreThanOneTime_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<InvalidOperationException>(()=>{
                    services
                        .AddScraperBuilder(config => config.AddAssembly(this.GetType().Assembly))
                        .AddScraperBuilder(typeof(ModelScraperService<,>), (builder) => { });
                });
            });
    }
}