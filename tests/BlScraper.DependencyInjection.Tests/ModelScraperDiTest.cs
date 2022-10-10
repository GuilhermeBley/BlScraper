using BlScraper.DependencyInjection.Model;
using BlScraper.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BlScraper.DependencyInjection.Tests;

//
// Tests sintaxe: MethodName_ExpectedBehavior_StateUnderTest
// Example: isAdult_AgeLessThan18_False
//


public class ModelScraperDiTest
{
    [Fact(Timeout = 5000)]
    public async Task ModelServices_InstanceAndRunWithoutServices_Success()
    {
        var servicesBase
            = new ServicesTestBase();
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper model
            = new ModelScraperService<SimpleExecution, SimpleData>(
                1,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); }
            );

        await model.Run();

        await WaitModelFinish(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_InstanceAndRunWithObjUnusedInExc_FailedInstance()
    {
        var servicesBase
            = new ServicesTestBase();
        var serviceProvider = servicesBase.ServiceProvider;
        IEnumerable<Results.ResultBase<Exception?>>? listResultEnd = null;
        IModelScraper model
            = new ModelScraperService<SimpleExecution, SimpleData>(
                1,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); },
                whenAllWorksEnd: (listResultEndFunc) => listResultEnd = listResultEndFunc,
                args: new object[] { new Obj1(), new Obj2() }
            );

        await model.Run();

        await WaitModelFinish(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.All(listResultEnd, item => {
            Assert.False(item.IsSuccess);
        });
    }


    [Fact(Timeout = 5000)]
    public async Task ModelServices_TryExecutionWith100ThreadsWith1ConstructorInExc_Success()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;
        bool hasErrorInExecution = false;

        IModelScraper model
            = new ModelScraperService<SimpleExecutionWithController, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenOccursException: (ex, data) => { hasErrorInExecution = true; return QuestResult.ThrowException(); }
            );

        await model.Run();

        await WaitModelFinish(model);

        Assert.False(hasErrorInExecution);
        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact(Timeout = 5000)]
    public void ModelServices_TryExecutionWith100ThreadsWith2ConstructorInExc_Failed()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        Assert.ThrowsAny<Exception>(() =>
        {
            model
                = new ModelScraperService<SimpleExecutionWith2Controller, SimpleData>(
                    100,
                    serviceProvider,
                    async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                    whenOccursException: (ex, data) => { return QuestResult.ThrowException(); }
                );
        });
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_DiWithObjAndService_Success()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        model
            = new ModelScraperService<SimpleExecutionServiceAndObj, SimpleData>(
                1,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenOccursException: (ex, data) => { return QuestResult.ThrowException(); },
                args: new Obj1()
            );

        await RunAndWaitAsync(model);
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_DiWithoutObjAndWithService_FailedRunExpectInvalidOperation()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionServiceAndObj, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenOccursException: (ex, data) => { return QuestResult.ThrowException(); },
                whenAllWorksEnd: (list) => { excList = list; }
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.IsType<InvalidOperationException>(result.Result));
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_DiWithoutService_FailedRunExpectInvalidOperation()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        var serviceArgs = new SimpleService();
        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionServiceAndObj, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenOccursException: (ex, data) => { return QuestResult.ThrowException(); },
                whenAllWorksEnd: (list) => { excList = list; },
                args: serviceArgs
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.IsType<InvalidOperationException>(result.Result));
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_DiWithoutServiceAndObj_FailedRunExpectInvalidOperation()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionServiceAndObj, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenOccursException: (ex, data) => { return QuestResult.ThrowException(); },
                whenAllWorksEnd: (list) => { excList = list; }
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.IsType<InvalidOperationException>(result.Result));
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_DiWithIServiceInArgs_SuccessArgsPriority()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;
        ISimpleService service = new SimpleService();
        List<ISimpleService> servicesOnCreate = new();
        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionWithController, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesOnCreate.Add(context.Service),
                whenAllWorksEnd: (list) => { excList = list; },
                args: service
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.True(result.IsSuccess));

        Assert.All(servicesOnCreate, result => Assert.Equal(service, result));
    }

    [Fact(Timeout = 5000)]
    public async Task ModelServices_DiWithServiceInArgs_SuccessArgsPriority()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;
        SimpleService service = new SimpleService();
        List<ISimpleService> servicesOnCreate = new();
        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionWithController, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesOnCreate.Add(context.Service),
                whenAllWorksEnd: (list) => { excList = list; },
                args: service
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.True(result.IsSuccess));

        Assert.All(servicesOnCreate, result => Assert.Equal(service, result));
    }

    [Fact(Timeout = 5000)]
    public async Task ServiceProvidier_DiWithTransient_FailedAllDifferent()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddTransient<ISimpleService, SimpleService>();
                services.AddScoped<IServiceLinkedWithOther, ServiceLinkedWithOther>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        List<(ISimpleService SimpleService, IServiceLinkedWithOther LinkedWithOther)> servicesOnCreate = new();
        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionLifeTimeService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesOnCreate.Add((context.Service, context.LinkedService)),
                whenAllWorksEnd: (list) => { excList = list; }
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.True(result.IsSuccess));

        Assert.All(servicesOnCreate, result
            => Assert.NotEqual(result.SimpleService, result.LinkedWithOther.SimpleService));
    }

    [Fact(Timeout = 5000)]
    public async Task ServiceProvidier_DiWithScooped_SuccessAllExcEqual()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
                services.AddScoped<IServiceLinkedWithOther, ServiceLinkedWithOther>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IModelScraper? model = null;

        List<(ISimpleService SimpleService, IServiceLinkedWithOther LinkedWithOther)> servicesOnCreate = new();
        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionLifeTimeService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesOnCreate.Add((context.Service, context.LinkedService)),
                whenAllWorksEnd: (list) => { excList = list; }
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.True(result.IsSuccess));

        Assert.All(servicesOnCreate, result
            => Assert.Equal(result.SimpleService, result.LinkedWithOther.SimpleService));

        var firstService = servicesOnCreate.First().SimpleService;

        Assert.NotEqual(firstService, servicesOnCreate.Last().SimpleService);
    }

    [Fact(Timeout = 5000)]
    public async Task ServiceProvidier_DiWithSingleton_SuccessAllEqual()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddSingleton<ISimpleService, SimpleService>();
                services.AddScoped<IServiceLinkedWithOther, ServiceLinkedWithOther>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        var simpleServiceSingledon = serviceProvider.GetService<ISimpleService>();

        IModelScraper? model = null;

        List<(ISimpleService SimpleService, IServiceLinkedWithOther LinkedWithOther)> servicesOnCreate = new();
        IEnumerable<Results.ResultBase<Exception?>> excList = Enumerable.Empty<Results.ResultBase<Exception?>>();
        model
            = new ModelScraperService<SimpleExecutionLifeTimeService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesOnCreate.Add((context.Service, context.LinkedService)),
                whenAllWorksEnd: (list) => { excList = list; }
            );

        await RunAndWaitAsync(model);

        Assert.All(excList, result => Assert.True(result.IsSuccess));

        Assert.All(servicesOnCreate, result
            => Assert.Equal(simpleServiceSingledon, result.SimpleService));

        Assert.All(servicesOnCreate, result
            => Assert.Equal(simpleServiceSingledon, result.LinkedWithOther.SimpleService));
    }

    [Fact(Timeout = 5000)]
    public async Task ShareService_DiSameServiceWithTransient_Success()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddTransient<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        BlockingCollection<ISimpleService> servicesInExecutionsModel1 = new();
        IEnumerable<Results.ResultBase<Exception?>> excList1 = Enumerable.Empty<Results.ResultBase<Exception?>>();
        var model1
            = new ModelScraperService<SimpleExecutionShareService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesInExecutionsModel1.Add(context.SharedService),
                whenAllWorksEnd: (list) => { excList1 = list; }
            );

        await RunAndWaitAsync(model1);

        Assert.All(excList1, result => Assert.True(result.IsSuccess));

        var serviceModel1 = servicesInExecutionsModel1.FirstOrDefault()
            ?? throw new ArgumentNullException(nameof(servicesInExecutionsModel1));

        Assert.All(servicesInExecutionsModel1, result
            => Assert.Equal(serviceModel1, result));

        BlockingCollection<ISimpleService> servicesInExecutionsModel2 = new();
        IEnumerable<Results.ResultBase<Exception?>> excList2 = Enumerable.Empty<Results.ResultBase<Exception?>>();
        var model2
            = new ModelScraperService<SimpleExecutionShareService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesInExecutionsModel2.Add(context.SharedService),
                whenAllWorksEnd: (list) => { excList1 = list; }
            );

        await RunAndWaitAsync(model2);

        Assert.All(excList2, result => Assert.True(result.IsSuccess));

        Assert.All(servicesInExecutionsModel2, result
            => Assert.NotEqual(serviceModel1, result));
    }

    [Fact(Timeout = 5000)]
    public async Task ShareService_DiSameServiceWithScooped_Success()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        BlockingCollection<ISimpleService> servicesInExecutionsModel1 = new();
        IEnumerable<Results.ResultBase<Exception?>> excList1 = Enumerable.Empty<Results.ResultBase<Exception?>>();
        var model1
            = new ModelScraperService<SimpleExecutionShareService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesInExecutionsModel1.Add(context.SharedService),
                whenAllWorksEnd: (list) => { excList1 = list; }
            );

        await RunAndWaitAsync(model1);

        Assert.All(excList1, result => Assert.True(result.IsSuccess));

        var serviceModel1 = servicesInExecutionsModel1.FirstOrDefault()
            ?? throw new ArgumentNullException(nameof(servicesInExecutionsModel1));

        Assert.All(servicesInExecutionsModel1, result
            => Assert.Equal(serviceModel1, result));

        BlockingCollection<ISimpleService> servicesInExecutionsModel2 = new();
        IEnumerable<Results.ResultBase<Exception?>> excList2 = Enumerable.Empty<Results.ResultBase<Exception?>>();
        var model2
            = new ModelScraperService<SimpleExecutionShareService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesInExecutionsModel2.Add(context.SharedService),
                whenAllWorksEnd: (list) => { excList1 = list; }
            );

        await RunAndWaitAsync(model2);

        Assert.All(excList2, result => Assert.True(result.IsSuccess));

        Assert.All(servicesInExecutionsModel2, result
            => Assert.NotEqual(serviceModel1, result));
    }

    [Fact(Timeout = 5000)]
    public async Task ShareService_DiSameServiceWithSingleton_Success()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddSingleton<ISimpleService, SimpleService>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        var singledonService = serviceProvider.GetRequiredService<ISimpleService>();

        BlockingCollection<ISimpleService> servicesInExecutionsModel1 = new();
        IEnumerable<Results.ResultBase<Exception?>> excList1 = Enumerable.Empty<Results.ResultBase<Exception?>>();
        var model1
            = new ModelScraperService<SimpleExecutionShareService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesInExecutionsModel1.Add(context.SharedService),
                whenAllWorksEnd: (list) => { excList1 = list; }
            );

        await RunAndWaitAsync(model1);

        Assert.All(excList1, result => Assert.True(result.IsSuccess));

        Assert.All(servicesInExecutionsModel1, result
            => Assert.Equal(singledonService, result));

        BlockingCollection<ISimpleService> servicesInExecutionsModel2 = new();
        IEnumerable<Results.ResultBase<Exception?>> excList2 = Enumerable.Empty<Results.ResultBase<Exception?>>();
        var model2
            = new ModelScraperService<SimpleExecutionShareService, SimpleData>(
                100,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenExecutionCreated: (context) => servicesInExecutionsModel2.Add(context.SharedService),
                whenAllWorksEnd: (list) => { excList1 = list; }
            );

        await RunAndWaitAsync(model2);

        Assert.All(excList2, result => Assert.True(result.IsSuccess));

        Assert.All(servicesInExecutionsModel2, result
            => Assert.Equal(singledonService, result));
    }


    [Fact(Timeout = 5000)]
    public async Task ServiceProvidier_ServiceNeedsObj_FailedInjectObj()
    {
        var servicesBase
            = new ServicesTestBase((services) =>
            {
                services.AddScoped<IServiceNeedsObj, ServiceNeedsObj>();
            });
        var serviceProvider = servicesBase.ServiceProvider;

        IEnumerable<Results.ResultBase<Exception?>>? listResultEnd = null;

        IModelScraper? model = null;
        model
            = new ModelScraperService<SimpleExecutionNeedsObjInService, SimpleData>(
                1,
                serviceProvider,
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); },
                whenAllWorksEnd: (listResultEndFunc) => listResultEnd = listResultEndFunc,
                args: new Obj1()
            );

        await RunAndWaitAsync(model);

        Assert.NotNull(listResultEnd);

        Assert.All(listResultEnd, result => Assert.False(result.IsSuccess));
    }
    public static async Task WaitModelFinish(IModelScraper model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (model.State != ModelStateEnum.Disposed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(400);
        }
    }

    /// <summary>
    /// Runs model and expected Success in execute
    /// </summary>
    /// <remarks>
    ///     <para>Wait async with cancellation</para>
    /// </remarks>
    /// <param name="model">model to execute</param>
    /// <param name="cancellationToken">token to cancel wait</param>
    public static async Task RunAndWaitAsync(IModelScraper model, CancellationToken cancellationToken = default)
    {
        var result = await model.Run();

        Assert.True(result.IsSuccess, "Failed to run model.");

        await WaitModelFinish(model, cancellationToken);

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }
}