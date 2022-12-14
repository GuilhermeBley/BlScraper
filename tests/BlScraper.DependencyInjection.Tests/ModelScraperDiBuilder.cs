using BlScraper.DependencyInjection.Builder;
using BlScraper.DependencyInjection.Extension.Builder;
using BlScraper.DependencyInjection.Tests.QuestsBuilder;
using Microsoft.Extensions.DependencyInjection;
using BlScraper.DependencyInjection.Tests.Extension;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.Tests.FakeProject;
using BlScraper.DependencyInjection.Tests.QuestsBuilder.Filter;
using BlScraper.DependencyInjection.ConfigureModel.Filter;

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
    public async Task AddAllWorksEndConfigureFilter_DuplicateType_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(AllWorksEndConfigureFilterTest).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddAllWorksEndConfigureFilter<AllWorksEndConfigureFilterTest>()
                            .AddAllWorksEndConfigureFilter<AllWorksEndConfigureFilterTest>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Fact]
    public async Task AddDataCollectedConfigureFilter_DuplicateType_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(DataCollectedConfigureFilterTest).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddDataCollectedConfigureFilter<DataCollectedConfigureFilterTest>()
                            .AddDataCollectedConfigureFilter<DataCollectedConfigureFilterTest>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Fact]
    public async Task AddDataFinishedConfigureFilter_DuplicateType_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(DataFinishedConfigureFilterTest).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddDataFinishedConfigureFilter<DataFinishedConfigureFilterTest>()
                            .AddDataFinishedConfigureFilter<DataFinishedConfigureFilterTest>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Obsolete("Args filters are not used.")]
    public async Task AddGetArgsConfigureFilter_DuplicateType_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(GetArgsConfigureFilterTest).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddGetArgsConfigureFilter<GetArgsConfigureFilterTest>()
                            .AddGetArgsConfigureFilter<GetArgsConfigureFilterTest>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Fact]
    public async Task AddQuestCreatedConfigureFilter_DuplicateType_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(QuestCreatedConfigureFilterTest).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddQuestCreatedConfigureFilter<QuestCreatedConfigureFilterTest>()
                            .AddQuestCreatedConfigureFilter<QuestCreatedConfigureFilterTest>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Fact]
    public async Task AddQuestExceptionConfigureFilter_DuplicateType_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(QuestExceptionConfigureFilterTest).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>()
                            .AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Fact]
    public async Task AllFilters_DuplicateType_Success()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                    {
                        config.AddAllWorksEndConfigureFilter<AllWorksEndConfigureFilterTest>()
                        .AddDataCollectedConfigureFilter<DataCollectedConfigureFilterTest>()
                        .AddDataFinishedConfigureFilter<DataFinishedConfigureFilterTest>()
                        .AddQuestCreatedConfigureFilter<QuestCreatedConfigureFilterTest>()
                        .AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>()
                        .AddAssembly(this.GetType().Assembly);
                    })
                    .AddSingleton<IRouteService, RouteService>();
            });

        Assert.NotNull(servicesBase.ServiceProvider.GetService<IScrapBuilder>());
    }

    [Fact]
    public async Task AddDataCollectedConfigureFilter_TryAddAbstract_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(DataCollectedConfigureFilterAbstract).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddDataCollectedConfigureFilter<DataCollectedConfigureFilterAbstract>();
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Fact]
    public async Task AddDataFinishedConfigureFilter_TryAddObject_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(object).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddDataFinishedConfigureFilter(typeof(object));
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }

    [Obsolete("Args filters are not used.")]
    public async Task AddGetArgsConfigureFilter_TryAddGenericClass_FailedAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                Assert.Throws<ArgumentException>(typeof(GetArgsConfigureFilterGeneric<>).FullName, () =>
                {
                    services
                        .AddScraperBuilder(config =>
                        {
                            config.AddGetArgsConfigureFilter(typeof(GetArgsConfigureFilterGeneric<>));
                        })
                        .AddSingleton<IRouteService, RouteService>();
                });
            });
    }
    
    [Fact]
    public async Task SeveralFilters_TryAddGenericClass_SuccessAdd()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                    {
                        config.AddAllWorksEndConfigureFilter<SeveralFilters>()
                        .AddDataCollectedConfigureFilter<SeveralFilters>()
                        .AddDataFinishedConfigureFilter<SeveralFilters>()
                        .AddQuestCreatedConfigureFilter<SeveralFilters>()
                        .AddQuestExceptionConfigureFilter<SeveralFilters>()
                        .AddAssembly(this.GetType().Assembly);
                    })
                    .AddSingleton<IRouteService, RouteService>();
            });
            
        Assert.NotNull(servicesBase.ServiceProvider.GetService<IScrapBuilder>());
    }
    
    [Fact]
    public async Task AddQuestExceptionConfigureFilter_TryRunAndExecuteMethod_SuccessExecuted()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        var model = scrapBuilder.CreateModelByQuestType<WithQuestExceptionQuest>();

        await model.RunAndWaitModelFinish(new CancellationTokenSource(5000).Token);

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(WithQuestExceptionConfigure)
            .GetMethod(nameof(IQuestExceptionConfigure<WithQuestExceptionQuest,PublicSimpleData>.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(QuestExceptionConfigureFilterTest)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));
    }
    
    [Fact]
    public async Task AddQuestExceptionConfigureFilter_TryRunAndExecuteTwoExceptionFilters_SuccessExecuted()
    {
        await Task.CompletedTask;
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>()
                        .AddQuestExceptionConfigureFilter<SeveralFilters>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        var model = scrapBuilder.CreateModelByQuestType<WithQuestExceptionQuest>();

        await model.RunAndWaitModelFinish(new CancellationTokenSource(5000).Token);

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(WithQuestExceptionConfigure)
            .GetMethod(nameof(IQuestExceptionConfigure<WithQuestExceptionQuest,PublicSimpleData>.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(QuestExceptionConfigureFilterTest)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));
    }
    
    [Fact]
    public async Task AddQuestExceptionConfigureFilter_TryRunAndExecuteTwoExceptionFiltersWithoutRequiredException_SuccessExecuted()
    {
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>()
                        .AddQuestExceptionConfigureFilter<SeveralFilters>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        var model = scrapBuilder.CreateModelByQuestType<WithoutQuestExceptionNonRequired>();

        await model.Run();

        await Task.Delay(200);

        await model.StopAsync(new CancellationTokenSource(5000).Token);

        Assert.DoesNotContain(routeService.Routes, r => r.Equals(typeof(WithQuestExceptionConfigure)
            .GetMethod(nameof(IQuestExceptionConfigure<WithQuestExceptionQuest,PublicSimpleData>.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(QuestExceptionConfigureFilterTest)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));
    }
    
    [Fact]
    public async Task TestAllFilters_TryRunAndExecuteAllFilters_SuccessExecuted()
    {
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddAllWorksEndConfigureFilter<AllWorksEndConfigureFilterTest>()
                        .AddDataCollectedConfigureFilter<DataCollectedConfigureFilterTest>()
                        .AddDataFinishedConfigureFilter<DataFinishedConfigureFilterTest>()
                        .AddQuestCreatedConfigureFilter<QuestCreatedConfigureFilterTest>()
                        .AddQuestExceptionConfigureFilter<QuestExceptionConfigureFilterTest>()
                        
                        .AddAllWorksEndConfigureFilter<SeveralFilters>()
                        .AddDataCollectedConfigureFilter<SeveralFilters>()
                        .AddDataFinishedConfigureFilter<SeveralFilters>()
                        .AddQuestCreatedConfigureFilter<SeveralFilters>()
                        .AddQuestExceptionConfigureFilter<SeveralFilters>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        var model = scrapBuilder.CreateModelByQuestType<WithQuestExceptionQuest>();

        await model.Run();

        await Task.Delay(200);

        await model.StopAsync(new CancellationTokenSource(5000).Token);

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(WithQuestExceptionConfigure)
            .GetMethod(nameof(IQuestExceptionConfigure<WithQuestExceptionQuest,PublicSimpleData>.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(AllWorksEndConfigureFilterTest)
            .GetMethod(nameof(AllWorksEndConfigureFilterTest.OnFinished))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(SeveralFilters.OnFinished))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(DataCollectedConfigureFilterTest)
            .GetMethod(nameof(DataCollectedConfigureFilterTest.OnCollected))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(SeveralFilters.OnCollected))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(DataFinishedConfigureFilterTest)
            .GetMethod(nameof(DataFinishedConfigureFilterTest.OnDataFinished))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(SeveralFilters.OnDataFinished))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(QuestCreatedConfigureFilterTest)
            .GetMethod(nameof(QuestCreatedConfigureFilterTest.OnCreated))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(SeveralFilters.OnCreated))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(QuestExceptionConfigureFilterTest)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(SeveralFilters)
            .GetMethod(nameof(IQuestExceptionConfigureFilter.OnOccursException))));
    }

    [Fact]
    public async Task ObsoleteFilter_TryRunAndExecuteObsoleteFilter_FailedExecution()
    {
        #pragma warning disable 612, 618 
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddAllWorksEndConfigureFilter<AllWorksEndConfigureFilterTest>()
                        .AddAllWorksEndConfigureFilter<ObsoleteFilter>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        var model = scrapBuilder.CreateModelByQuestType<WithQuestExceptionQuest>();

        await model.Run();

        await Task.Delay(200);

        await model.StopAsync(new CancellationTokenSource(5000).Token);

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(WithQuestExceptionConfigure)
            .GetMethod(nameof(IQuestExceptionConfigure<WithQuestExceptionQuest,PublicSimpleData>.OnOccursException))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(AllWorksEndConfigureFilterTest)
            .GetMethod(nameof(AllWorksEndConfigureFilterTest.OnFinished))));

        Assert.DoesNotContain(routeService.Routes, r => r.Equals(typeof(ObsoleteFilter)
            .GetMethod(nameof(ObsoleteFilter.OnFinished))));
        #pragma warning restore 612, 618
    }

    [Fact]
    public async Task UniqueFilter_TryUniqueAndGlobalFilter_FailedExecution()
    {
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddAllWorksEndConfigureFilter<UniqueAndGlobalFilter>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        Assert.Throws<ArgumentException>(()=> scrapBuilder.CreateModelByQuestType<UniqueAndGlobalFilterQuest>());

        await Task.CompletedTask;
    }

    [Fact]
    public async Task InvalidFilter_TryInstanceModelWithInvalidFilter_FailedExecution()
    {
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        Assert.Throws<ArgumentException>($"{typeof(object).Name}", ()=> scrapBuilder.CreateModelByQuestType<QuestWithInvalidFilter>());

        await Task.CompletedTask;
    }

    [Fact]
    public async Task RequiredFilter_TryInstanceModelWithRequiredFilter_SuccessExecution()
    {
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddAllWorksEndConfigureFilter<AllWorksEndConfigureFilterTest>()
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        var model = scrapBuilder.CreateModelByQuestType<QuestWithFilterImplemented>();

        await model.RunAndWaitModelFinish();

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(AllWorksEndConfigureFilterTest)
            .GetMethod(nameof(AllWorksEndConfigureFilterTest.OnFinished))));

        Assert.Contains(routeService.Routes, r => r.Equals(typeof(AllWorksEndConfigureFilterUniqueImplemented)
            .GetMethod(nameof(AllWorksEndConfigureFilterUniqueImplemented.OnFinished))));

        await Task.CompletedTask;
    }

    [Fact]
    public async Task RequiredFilter_TryInstanceModelWithRequiredFilterNonImplemented_FailedExecution()
    {
        var servicesBase
            = new ServicesTestBase(services =>
            {
                services
                    .AddScraperBuilder(config =>
                        config
                        .AddAssembly(this.GetType().Assembly))
                    .AddSingleton<IRouteService, RouteService>();
            });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var routeService = servicesBase.ServiceProvider.GetRequiredService<IRouteService>();

        Assert.Throws<ArgumentException>(nameof(AllWorksEndConfigureFilterTest), ()=> scrapBuilder.CreateModelByQuestType<QuestWithFilterImplemented>());

        await Task.CompletedTask;
    }

    [Fact]
    public async Task RequiredConfigure_CheckContext_SuccessGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextRequiredConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextRequiredConfigure.GetData))
            ?? throw new ArgumentNullException("expected method");
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)));
    }

    [Fact]
    public async Task RequiredConfigure_CheckContextInConstructor_FailedGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextRequiredConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, null)));
    }

    [Fact]
    public async Task RequiredConfigure_CheckContextInAsync_SuccessToGetAllContexts()
    {
        var servicesBase
                = new ServicesTestBase(services =>
                {
                    services
                        .AddScraperBuilder(config =>
                            config
                            .AddAssembly(this.GetType().Assembly))
                        .AddSingleton<IRouteObjectService, RouteObjectService>()
                        .AddScoped<ICountScrapService>((serviceProvider) => 
                            new CountScrapService(AllConfigureQuestsWithContextRequiredConfigure.InitialQuantityScrap))
                        .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(10, 100));
                });

        var taskTupleModel1 = CreateContextModelAndRun(servicesBase: servicesBase);
        var taskTupleModel2 = CreateContextModelAndRun(servicesBase: servicesBase);
        var taskTupleModel3 = CreateContextModelAndRun(servicesBase: servicesBase);
        await Task.WhenAll(taskTupleModel1, taskTupleModel2, taskTupleModel3);
        var tupleModel1 = taskTupleModel1.Result;
        var tupleModel2 = taskTupleModel2.Result;
        var tupleModel3 = taskTupleModel3.Result;
        var route = servicesBase.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextRequiredConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextRequiredConfigure.GetData))
            ?? throw new ArgumentNullException("expected method");
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tupleModel1.modelScrapCreated)));
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tupleModel2.modelScrapCreated)));
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tupleModel3.modelScrapCreated)));
    }

    [Fact(Timeout = 5000)]
    public async Task RequiredConfigure_CheckContextInVariousThreads_SuccessToGetAllContexts()
    {
        var servicesBase
                = new ServicesTestBase(services =>
                {
                    services
                        .AddScraperBuilder(config =>
                            config
                            .AddAssembly(this.GetType().Assembly))
                        .AddSingleton<IRouteObjectService, RouteObjectService>()
                        .AddScoped<ICountScrapService>((serviceProvider) => 
                            new CountScrapService(AllConfigureQuestsWithContextRequiredConfigure.InitialQuantityScrap))
                        .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(10));
                });

        var mrse = new ManualResetEvent(false);
        mrse.Reset();

        var threadList = new List<Thread>();

        (IServiceProvider ServiceProvider, BlScraper.Model.IModelScraper modelScrapCreated) tupleModel1 = (null!,null!);
        (IServiceProvider ServiceProvider, BlScraper.Model.IModelScraper modelScrapCreated) tupleModel2 = (null!,null!);
        (IServiceProvider ServiceProvider, BlScraper.Model.IModelScraper modelScrapCreated) tupleModel3 = (null!,null!);
        threadList.Add(new Thread(new ThreadStart(() => { mrse.WaitOne(); tupleModel1 = CreateContextModelAndRun(servicesBase: servicesBase).GetAwaiter().GetResult(); })));
        threadList.Add(new Thread(new ThreadStart(() => { mrse.WaitOne(); tupleModel2 = CreateContextModelAndRun(servicesBase: servicesBase).GetAwaiter().GetResult(); })));
        threadList.Add(new Thread(new ThreadStart(() => { mrse.WaitOne(); tupleModel3 = CreateContextModelAndRun(servicesBase: servicesBase).GetAwaiter().GetResult(); })));

        RunAndWaitThreads(threadList, () => mrse.Set());

        var route = servicesBase.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextRequiredConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextRequiredConfigure.GetData))
            ?? throw new ArgumentNullException("expected method");
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tupleModel1.modelScrapCreated)));
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tupleModel2.modelScrapCreated)));
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, tupleModel3.modelScrapCreated)));

        await Task.CompletedTask;
    }

    [Fact]
    public async Task AllWorksEndConfigure_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextAllWorkEndConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextAllWorkEndConfigure.OnFinished))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task AllWorksEndConfigure_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextAllWorkEndConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataCollectedConfigure_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataCollectedConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextDataCollectedConfigure.OnCollected))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }
    
    [Fact]
    public async Task DataCollectedConfigure_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataCollectedConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataFinishedConfigure_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataFinishedConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextDataFinishedConfigure.OnDataFinished))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataFinishedConfigure_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataFinishedConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task GetArgsConfigure_CheckContext_FailedToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextGetArgsConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextGetArgsConfigure.GetArgs))
            ?? throw new ArgumentNullException("expected method");
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, null)));
    }

    [Fact]
    public async Task GetArgsConfigure_CheckContextInConstructor_FailedToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextGetArgsConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Single(route.Routes.Where(r => r == (expectedMethod, null)));
    }

    [Fact]
    public async Task QuestCreatedConfigure_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestCreatedConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextQuestCreatedConfigure.OnCreated))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestCreatedConfigure_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestCreatedConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestExceptionConfigure_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestExceptionConfigure)
            .GetMethod(nameof(AllConfigureQuestsWithContextQuestExceptionConfigure.OnOccursException))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestExceptionConfigure_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestExceptionConfigure)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task AllWorksEndConfigureFilter_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextAllWorksEndConfigureFilter)
            .GetMethod(nameof(AllConfigureQuestsWithContextAllWorksEndConfigureFilter.OnFinished))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task AllWorksEndConfigureFilter_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextAllWorksEndConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }
    
    [Fact]
    public async Task AllWorksEndConfigureFilter_CheckCountInstancesConstructor_SuccessToGetTheContext()
    {
        const int countInstancesExpected = 1;
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextAllWorksEndConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        int countInstancesActual = route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)).Count();
        Assert.Equal(countInstancesExpected, countInstancesActual);
    }

    [Fact]
    public async Task DataCollectedConfigureFilter_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataCollectedConfigureFilter)
            .GetMethod(nameof(AllConfigureQuestsWithContextDataCollectedConfigureFilter.OnCollected))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataCollectedConfigureFilter_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataCollectedConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataCollectedConfigureFilter_CheckCountInstancesConstructor_SuccessToGetTheContext()
    {
        const int countInstancesExpected = 1;
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataCollectedConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        int countInstancesActual = route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)).Count();
        Assert.Equal(countInstancesExpected, countInstancesActual);
    }

    [Fact]
    public async Task DataFinishedConfigureFilter_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataFinishedConfigureFilter)
            .GetMethod(nameof(AllConfigureQuestsWithContextDataFinishedConfigureFilter.OnDataFinished))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataFinishedConfigureFilter_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataFinishedConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task DataFinishedConfigureFilter_CheckCountInstancesConstructor_SuccessToGetTheContext()
    {
        const int countInstancesExpected = 10;
        var tuple = await CreateContextModelAndRun(maxData: countInstancesExpected);
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextDataFinishedConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        int countInstancesActual = route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)).Count();
        Assert.Equal(countInstancesExpected, countInstancesActual);
    }

    [Obsolete("Args filters are not used.")]
    public async Task GetArgsConfigureFilter_CheckContext_FailedToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextGetArgsConfigureFilter)
            .GetMethod(nameof(AllConfigureQuestsWithContextGetArgsConfigureFilter.GetArgs))
            ?? throw new ArgumentNullException("expected method");
        Assert.DoesNotContain((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestCreatedConfigureFilter_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestCreatedConfigureFilter)
            .GetMethod(nameof(AllConfigureQuestsWithContextQuestCreatedConfigureFilter.OnCreated))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestCreatedConfigureFilter_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestCreatedConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestCreatedConfigureFilter_CheckCountInstancesConstructor_SuccessToGetTheContext()
    {
        const int countInstancesExpected = AllConfigureQuestsWithContextRequiredConfigure.InitialQuantityScrap;
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestCreatedConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        int countInstancesActual = route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)).Count();
        Assert.Equal(countInstancesExpected, countInstancesActual);
    }

    [Fact]
    public async Task QuestExceptionConfigureFilter_CheckContext_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestExceptionConfigureFilter)
            .GetMethod(nameof(AllConfigureQuestsWithContextQuestExceptionConfigureFilter.OnOccursException))
            ?? throw new ArgumentNullException("expected method");
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestExceptionConfigureFilter_CheckContextInConstructor_SuccessToGetTheContext()
    {
        var tuple = await CreateContextModelAndRun();
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestExceptionConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        int countInstancesActual = route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)).Count();
        Assert.Contains((expectedMethod, tuple.modelScrapCreated), 
            route.Routes);
    }

    [Fact]
    public async Task QuestExceptionConfigureFilter_CheckCountInstancesConstructor_SuccessToGetTheContext()
    {
        const int countInstancesExpected = 1;
        var tuple = await CreateContextModelAndRun(countScrap: countInstancesExpected);
        var route = tuple.ServiceProvider.GetRequiredService<IRouteObjectService>();
        var expectedMethod = typeof(AllConfigureQuestsWithContextQuestExceptionConfigureFilter)
            .GetConstructors().FirstOrDefault()
            ?? throw new ArgumentNullException("expected method");
        int countInstancesActual = route.Routes.Where(r => r == (expectedMethod, tuple.modelScrapCreated)).Count();
        Assert.Equal(countInstancesExpected, countInstancesActual);
    }

    private async Task<(IServiceProvider ServiceProvider, BlScraper.Model.IModelScraper modelScrapCreated)> CreateContextModelAndRun(
        int maxData = 10, 
        int countScrap = AllConfigureQuestsWithContextRequiredConfigure.InitialQuantityScrap, 
        ServicesTestBase? servicesBase = null)
    {
        if (servicesBase is null)
            servicesBase
                = new ServicesTestBase(services =>
                {
                    services
                        .AddScraperBuilder(config =>
                            config
                            .AddAssembly(this.GetType().Assembly))
                        .AddSingleton<IRouteObjectService, RouteObjectService>()
                        .AddScoped<ICountScrapService>((serviceProvider) => new CountScrapService(countScrap))
                        .AddScoped<IServiceMocPublicSimpleData>((serviceProvider)=> new ServiceMocPublicSimpleData(maxData));
                });
        
        var scrapBuilder = servicesBase.ServiceProvider.GetRequiredService<IScrapBuilder>();
        var model = scrapBuilder.CreateModelByQuestType<AllConfigureQuestsWithContext>();
        await model.RunAndWaitModelFinish(new CancellationTokenSource(5000).Token);
        return (servicesBase.ServiceProvider.CreateScope().ServiceProvider, model);
    }

    private void RunAndWaitThreads(IEnumerable<Thread> threads, Action? onAllStarted = null)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }

        onAllStarted?.Invoke();

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}