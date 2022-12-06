using System.Collections.Concurrent;
using BlScraper.Model;
using BlScraper.Tests.Mocks;
using BlScraper.Tests.Monitors;
using Xunit.Abstractions;

namespace BlScraper.Tests;

//
// Tests sintaxe: MethodName_ExpectedBehavior_StateUnderTest
// Example: isAdult_AgeLessThan18_False
//

public class ModelDisposeTest
{
    private readonly ITestOutputHelper _output;

    public ModelDisposeTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void Dispose_RunAndDispose_SuccessDisposeOrWaitDipose()
    {
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                100,
                () => new IntegerExecution(),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); }
            );

        using (model)
        {
            Assert.True((await model.Run()).IsSuccess);
        }

        Assert.True(ModelStateEnum.Disposed == model.State || 
            ModelStateEnum.WaitingDispose == model.State);
    }

    [Fact]
    public async void Dispose_RunAndDisposeAsync_SuccessDispose()
    {
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                100,
                () => new IntegerExecution(),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await model.DisposeAsync();

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void Dispose_DisposeAndRun_FailedRun()
    {
        bool searched = false;
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                100,
                () => new IntegerExecution(),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); },
                whenDataFinished: (dataResult) => searched = true
            );

        using (model)
        {
            
        }

        var result = await model.Run();

        Assert.False(searched);
        Assert.False(result.IsSuccess);
        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void Dispose_DisposeAndRunPauseAndUnpause_SuccessDisposeOrWait()
    {
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                100,
                () => new IntegerExecution(),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); }
            );

        using (model)
        {
            Assert.True((await model.Run()).IsSuccess);    
        }

        var resultRun = await model.Run();
        Assert.False(resultRun.IsSuccess);

        var resultPause = await model.PauseAsync(true);
        Assert.False(resultPause.IsSuccess);

        var resultUnPause = await model.PauseAsync(false);
        Assert.False(resultUnPause.IsSuccess);

        Assert.True(ModelStateEnum.Disposed == model.State || 
            ModelStateEnum.WaitingDispose == model.State);
    }

    [Fact]
    public async void Dispose_DisposeAndPauseAndUnpause_SuccessDisposeOrWait()
    {
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                100,
                () => new IntegerExecution(),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); }
            );

        using (model)
        {
            Assert.True((await model.Run()).IsSuccess);    
        }

        var resultPause = await model.PauseAsync(true);
        Assert.False(resultPause.IsSuccess);

        var resultUnPause = await model.PauseAsync(false);
        Assert.False(resultUnPause.IsSuccess);

        Assert.True(ModelStateEnum.Disposed == model.State || 
            ModelStateEnum.WaitingDispose == model.State);
    }

    /// <summary>
    /// Wait to finish the model
    /// </summary>
    /// <returns>async</returns>
    /// <exception cref="OperationCanceledException"/>
    public async Task WaitFinishModel(IModelScraper model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (model.State != ModelStateEnum.Disposed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(250);
        }

        return;
    }

    [Fact]
    public async void Dispose_RunAndDiposeAndDisposeAsync_SuccessDispose()
    {
        const int maxData = 100;
        const int timeWaitExc = 100;

        BlockingCollection<IntegerData> CollectedData = new();
        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData/2,
                () => new WaitingExecution(timeWaitExc),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (resultData) => { if (resultData.IsSuccess) CollectedData.Add(resultData.Result); }
            );

        using (model)
        {
            Assert.True((await model.Run()).IsSuccess);
            new SimpleMonitor().Wait(timeWaitExc / 2);
        }

        await model.DisposeAsync();

        Assert.Equal(maxData / 2, CollectedData.Count);

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void DisposeQuest_RunAndWaitModelCompareDisposeExc_SuccessDisposeQuest()
    {
        ConcurrentDictionary<OnDisposeExecution, int> countDiposedExc = new();
        IModelScraper model =
            new ModelScraper<OnDisposeExecution, IntegerData>
            (
                20,
                () => new OnDisposeExecution((exc) => { 
                    if (countDiposedExc.TryAdd(exc, exc.DisposedCount))
                        countDiposedExc.TryUpdate(exc, exc.DisposedCount, countDiposedExc[exc]); 
                    }),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(1000); }
            );

        using (model)
        {
            Assert.True((await model.Run()).IsSuccess);
            await WaitFinishModel(model);
        }

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.All(countDiposedExc, dispExc => {
            Assert.Equal(1, dispExc.Value);
        });
    }

    [Fact]
    public async void DisposeQuest_RunCheckIfAllWorksEndCompleteBefereDispose_SuccessDisposeAfterEvent()
    {
        bool allWorksEnd = false;
        bool modelFinished = false;
        ConcurrentDictionary<OnDisposeExecution, int> countDiposedExc = new();
        IModelScraper? model = null;
            model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                5,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1000); },
                whenAllWorksEnd: (result) => {
                    allWorksEnd = true;
                    modelFinished = model?.State != ModelStateEnum.Disposed;
                    Thread.Sleep(50);
                }
            );

        using (model)
        {
            Assert.True((await model.Run()).IsSuccess);
            await WaitFinishModel(model);
            Assert.True(allWorksEnd);
            Assert.True(modelFinished);
        }
    }
}