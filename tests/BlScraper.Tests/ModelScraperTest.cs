using System.Collections.Concurrent;
using BlScraper.Model;
using BlScraper.Tests.Mocks;
using BlScraper.Tests.Monitors;
using Xunit.Abstractions;

namespace BlScraper.Tests;

public class ModelScraperTest
{
    private readonly ITestOutputHelper _output;

    public ModelScraperTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void ExecuteModel_Exec_Add10ItemsToListWith1Thread()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_Add10ItemsToListWith1Thread));
        BlockingCollection<DateTime> blockList = new();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                1,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(10); },
                whenAllWorksEnd: (finishList) => { isFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(isFinished);

        Assert.True(blockList.Count == 10);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Exec_Add20ItemsToListWith2Thread()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_Add20ItemsToListWith2Thread));
        BlockingCollection<DateTime> blockList = new();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                2,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(20); },
                whenAllWorksEnd: (finishList) => { isFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(blockList.Count == 20 && isFinished);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Exec_Add1000ItemsToListWith10Thread()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_Add1000ItemsToListWith10Thread));
        BlockingCollection<DateTime> blockList = new();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                10,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1000); },
                whenAllWorksEnd: (finishList) => { isFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(blockList.Count == 1000 && isFinished);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Exec_Add1000ItemsToListWith10ThreadWithConfirmationData()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_Add1000ItemsToListWith10ThreadWithConfirmationData));
        BlockingCollection<int> blockList = new();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                10,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(Thread.CurrentThread.ManagedThreadId); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1000); },
                whenAllWorksEnd: (finishList) => { isFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(isFinished);

        Assert.True(blockList.Distinct().Any());

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Oredered_ChecksOrderFromData()
    {
        _output.WriteLine(nameof(ExecuteModel_Oredered_ChecksOrderFromData));
        BlockingCollection<int> blockList = new();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                1,
                () => new IntegerExecution() { OnSearch = (data) => { blockList.Add(data.Id); } },
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); },
                whenAllWorksEnd: (finishList) => { isFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        IReadOnlyList<int> readOnlyOrdered = blockList.ToList();
        for (int i = 0; i < blockList.Count; i++)
        {
            if (i == 0)
                continue;

            Assert.True(readOnlyOrdered[i - 1] < readOnlyOrdered[i]);
        }

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed && isFinished);
    }

    [Fact]
    public async void ExecuteModel_Oredered_ChecksOrderFromDataWithException()
    {
        _output.WriteLine(nameof(ExecuteModel_Oredered_ChecksOrderFromDataWithException));
        const int onError = 32;
        BlockingCollection<int> blockList = new();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<ThrowExcIntegerExecution, IntegerData>
            (
                1,
                () => new ThrowExcIntegerExecution(onError) { OnSearch = (data) => { blockList.Add(data.Id); } },
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(100); },
                whenAllWorksEnd: (finishList) => { isFinished = true; },
                whenOccursException: (exception, data) => { return QuestResult.RetryOther(); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(isFinished);

        Assert.Equal(onError, blockList.Last());

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void PauseModel_Pause_Pause()
    {
        _output.WriteLine(nameof(PauseModel_Pause_Pause));
        IEnumerable<Results.ResultBase<Exception?>>? finishedList = null;
        var monitor = new SimpleMonitor();
        var isFinishedData = false;
        IModelScraper model =
            new ModelScraper<EndlessExecution, SimpleData>
            (
                1,
                () => new EndlessExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); },
                whenAllWorksEnd: (finishList) => { finishedList = finishList; },
                whenDataFinished: (resultData) =>
                {
                    _output.WriteLine($"Is data searched:{resultData.IsSuccess}");
                    if (resultData.IsSuccess) isFinishedData = true;
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(150);

        var pause = await model.PauseAsync(true);

        Assert.True(pause.IsSuccess);

        monitor.Wait(150);

        Assert.Null(finishedList);

        Assert.Equal(ModelStateEnum.Paused, model.State);

        var resultUnpause = model.PauseAsync(false).GetAwaiter().GetResult();

        _output.WriteLine($"State:{Enum.GetName(resultUnpause.Result.Status)}, Message:{resultUnpause.Result.Message}");

        Assert.True(model.State == ModelStateEnum.Running || model.State == ModelStateEnum.Disposed);

        Assert.True(resultUnpause.IsSuccess);

        monitor.Wait(100);

        await WaitFinishModel(model);

        monitor.Wait(200);

        Assert.NotNull(finishedList);
        Assert.True(isFinishedData);
        Assert.True(model.State == ModelStateEnum.Disposed);

        await model.DisposeAsync();
    }

    [Fact]
    public async void StopModel_Dispose_Stop()
    {
        _output.WriteLine(nameof(StopModel_Dispose_Stop));
        var monitor = new SimpleMonitor();
        var isFinished = false;
        IModelScraper model =
            new ModelScraper<EndlessExecution, SimpleData>
            (
                1,
                () => new EndlessExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); },
                whenAllWorksEnd: (finishList) => { isFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        var cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => model.StopAsync(cancellationTokenSource.Token));

        await Task.Delay(100);

        await WaitFinishModel(model);
        
        Assert.True(isFinished);
        Assert.True(model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void PauseModel_Pause_CancelPause()
    {
        _output.WriteLine(nameof(PauseModel_Pause_CancelPause));
        CancellationTokenSource cts = new();
        IModelScraper model =
            new ModelScraper<EndlessWhileExecution, SimpleData>
            (
                1,
                () => new EndlessWhileExecution(cts.Token),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        var cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => model.PauseAsync(true, cancellationToken: cancellationTokenSource.Token));

        Assert.True(model.State == ModelStateEnum.WaitingPause 
            || model.State == ModelStateEnum.Paused);

        cts.Cancel();
        cts.Dispose();
        await model.DisposeAsync();
    }

    [Fact]
    public async void StopModel_Dispose_CancelStop()
    {
        _output.WriteLine(nameof(StopModel_Dispose_CancelStop));
        const int maxTime = 500;
        CancellationTokenSource cts = new();
        IModelScraper model =
            new ModelScraper<EndlessWhileExecution, SimpleData>
            (
                50,
                () => new EndlessWhileExecution(cts.Token, maxTime),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        var ctsStop = new CancellationTokenSource();

        ctsStop.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => model.StopAsync(cancellationToken: ctsStop.Token));

        cts.Cancel();
        cts.Dispose();
        await model.DisposeAsync();
    }

    [Fact]
    public async void ExecuteModel_Exec_With0DataAnd1ThreadWithoutError()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_With0DataAnd1ThreadWithoutError));
        BlockingCollection<DateTime> blockList = new();
        var isFinished = false;
        var monitor = new SimpleMonitor();
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                1,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(0); },
                whenAllWorksEnd: (finishList) => { isFinished = true; monitor.Resume(); Assert.True(finishList.All(f => f.IsSuccess)); },
                whenDataFinished: (result) => { Assert.True(false); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        if (!isFinished)
            monitor.Wait(1 * 1000, () => Assert.True(isFinished));

        Assert.True(!blockList.Any());

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Exec_With0DataAnd10ThreadWithoutError()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_With0DataAnd10ThreadWithoutError));
        BlockingCollection<DateTime> blockList = new();
        var isFinished = false;
        var monitor = new SimpleMonitor();
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                10,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(0); },
                whenAllWorksEnd: (finishList) => { isFinished = true; monitor.Resume(); Assert.True(finishList.All(f => f.IsSuccess)); },
                whenDataFinished: (result) => { Assert.True(false); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        if (!isFinished)
            monitor.Wait(1 * 1000);

        Assert.True(!blockList.Any());

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess && model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Exec_With0DataAnd100ThreadWithoutError()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_With0DataAnd100ThreadWithoutError));
        BlockingCollection<DateTime> blockList = new();
        var monitor = new SimpleMonitor();
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                100,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(0); },
                whenAllWorksEnd: (finishList) => { monitor.Resume(); Assert.True(finishList.All(f => f.IsSuccess)); },
                whenDataFinished: (result) => { Assert.True(false); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        Assert.True(!blockList.Any());

        var resultStop = await model.StopAsync();

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.True(resultStop.IsSuccess);
    }

    [Fact]
    public async void ExecuteModel_Exec_WhenAllWorkFinished()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_WhenAllWorkFinished));
        BlockingCollection<DateTime> blockList = new();
        bool isWhenAllWorksFinished = false;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                50,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(0); },
                whenAllWorksEnd: (finishList) => { isWhenAllWorksFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        Assert.True(!blockList.Any());

        var resultStop = await model.StopAsync();

        await Task.Delay(20);

        Assert.True(isWhenAllWorksFinished);
        Assert.True(model.State == ModelStateEnum.Disposed);
        Assert.True(resultStop.IsSuccess);
    }

    [Fact]
    public async void ExecuteModel_Exec_WhenDataFinished()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_WhenDataFinished));
        BlockingCollection<DateTime> blockList = new();
        bool isWhenDataFinished = false;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                50,
                () => new SimpleExecution() { OnSearch = (timer) => { blockList.Add(timer); } },
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(1); },
                whenDataFinished: (data) => { isWhenDataFinished = true; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(blockList.Any());

        Assert.True(model.State == ModelStateEnum.Disposed && isWhenDataFinished);

        await model.DisposeAsync();
    }

    [Fact]
    public async void ExecuteModel_Exec_WhenOccursExceptionOk()
    {
        _output.WriteLine(nameof(ExecuteModel_Exec_WhenOccursExceptionOk));
        BlockingCollection<int> blockList = new();
        bool isWhenDataFinished = false;
        bool isSuccessDataSearch = false;
        IModelScraper model =
            new ModelScraper<ThrowExcIntegerExecution, IntegerData>
            (
                50,
                () => new ThrowExcIntegerExecution(1) { OnSearch = (data) => { blockList.Add(data.Id); } },
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(1); },
                whenDataFinished: (data) => { isSuccessDataSearch = data.IsSuccess; isWhenDataFinished = true; },
                whenOccursException: (ex, data) => { return QuestResult.Ok(); }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        Assert.True(!blockList.Any());

        await WaitFinishModel(model);

        Assert.True(model.State == ModelStateEnum.Disposed &&
            isWhenDataFinished && isSuccessDataSearch);

        await model.DisposeAsync();
    }

    [Fact]
    public async void ExecuteModel_Pause_WhenPauseDoesNotCollectData()
    {
        _output.WriteLine(nameof(ExecuteModel_Pause_WhenPauseDoesNotCollectData));
        const int maxWaiting = 100;
        const int maxData = 100;

        bool isPaused = false;
        bool collectDataInPause = false;
        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);

                    if (isPaused)
                        collectDataInPause = true;
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        var resultPause = await model.PauseAsync();

        Assert.True(resultPause.IsSuccess);

        monitor.Wait(maxWaiting * 3);

        Assert.True(!collectDataInPause);

        isPaused = false;

        resultPause = await model.PauseAsync(false);

        Assert.True(resultPause.IsSuccess, resultPause.Result.Message ?? "");

        await WaitFinishModel(model);

        Assert.Equal(maxData, blockList.Count);

        Assert.True(model.State == ModelStateEnum.Disposed);

        await model.DisposeAsync();
    }

    [Fact]
    public async void ExecuteModel_Pause_WhenPauseDoesCheckCollectData()
    {
        _output.WriteLine(nameof(ExecuteModel_Pause_WhenPauseDoesCheckCollectData));
        const int maxWaiting = 1000;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 2);

        var resultPause = await model.PauseAsync();

        Assert.True(resultPause.IsSuccess);

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        resultPause = await model.PauseAsync(false);

        Assert.True(resultPause.IsSuccess, resultPause.Result.Message ?? "");

        await WaitFinishModel(model);

        Assert.Equal(maxData, blockList.Count);

        Assert.True(model.State == ModelStateEnum.Disposed);

        await model.DisposeAsync();
    }

    [Fact]
    public async void ExecuteModel_Stop_WhenStopDoesNotCollectData()
    {
        _output.WriteLine(nameof(ExecuteModel_Stop_WhenStopDoesNotCollectData));
        const int maxWaiting = 100;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 10);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess);

        await WaitFinishModel(model);

        Assert.True(blockList.Count < maxData);

        Assert.True(model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_Stop_WhenStopDoesCheckCollectData()
    {
        _output.WriteLine(nameof(ExecuteModel_Stop_WhenStopDoesCheckCollectData));
        const int maxWaiting = 1000;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 2);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(maxData / 2, blockList.Count);

        Assert.True(model.State == ModelStateEnum.Disposed);
    }

    [Fact]
    public async void ExecuteModel_PauseAndDispose_DisposeWhenHavePaused()
    {
        _output.WriteLine(nameof(ExecuteModel_PauseAndDispose_DisposeWhenHavePaused));
        const int maxWaiting = 1000;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 2);

        var resultPause = await model.PauseAsync();

        Assert.True(resultPause.IsSuccess, resultPause.Result.Message ?? "");

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess, resultStop.Result.Message ?? "");

        Assert.Equal(maxData / 2, blockList.Count);

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void ExecuteModel_PauseAndDispose_DisposeAndPause()
    {
        _output.WriteLine(nameof(ExecuteModel_PauseAndDispose_DisposeAndPause));
        const int maxWaiting = 1000;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 2);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess, resultStop.Result.Message ?? "");

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        var resultPause = await model.PauseAsync();

        Assert.True(!resultPause.IsSuccess, resultPause.Result.Message ?? "");

        Assert.Equal(maxData / 2, blockList.Count);

        resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess, resultStop.Result.Message ?? "");

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void ExecuteModel_Pause_PauseAndUnPauseSeveralTimes()
    {
        _output.WriteLine(nameof(ExecuteModel_Pause_PauseAndUnPauseSeveralTimes));
        const int maxWaiting = 1000;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 2);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => model.PauseAsync(cancellationToken: cts.Token));

        Assert.True(model.State == ModelStateEnum.WaitingPause);

        var unPause = await model.PauseAsync(false);

        Assert.True(unPause.IsSuccess, unPause.Result.Message ?? "");

        await WaitFinishModel(model);

        Assert.Equal(maxData, blockList.Count);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess, resultStop.Result.Message ?? "");

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void ExecuteModel_Pause_PauseSeveralTimes()
    {
        _output.WriteLine(nameof(ExecuteModel_Pause_PauseSeveralTimes));
        const int maxWaiting = 1000;
        const int maxData = 100;

        BlockingCollection<int> blockList = new();

        var monitor = new SimpleMonitor();

        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                maxData / 2,
                () => new WaitingExecution(maxWaiting),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(maxData); },
                whenDataFinished: (data) =>
                {
                    if (data.IsSuccess)
                        blockList.Add(data.Result.Id);
                }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        monitor.Wait(maxWaiting / 2);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => model.PauseAsync(cancellationToken: cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(() => model.PauseAsync(cancellationToken: cts.Token));

        Assert.True(model.State == ModelStateEnum.WaitingPause);

        var resultPause = await model.PauseAsync();

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        var resultStop = await model.StopAsync();

        Assert.True(resultStop.IsSuccess, resultStop.Result.Message ?? "");

        monitor.Wait(maxWaiting);

        Assert.Equal(maxData / 2, blockList.Count);

        Assert.Equal(ModelStateEnum.Disposed, model.State);
    }

    [Fact]
    public async void ExecuteModel_CollectSearchList_SuccessSameElements()
    {
        _output.WriteLine(nameof(ExecuteModel_CollectSearchList_SuccessSameElements));
        IEnumerable<IntegerData> dataToSearch = IntegerDataFactory.GetData(100);
        IEnumerable<IntegerData> dataToCheck = Enumerable.Empty<IntegerData>();
        IModelScraper model =
            new ModelScraper<IntegerExecution, IntegerData>
            (
                1,
                () => new IntegerExecution(),
                async () => { await Task.CompletedTask; return dataToSearch; },
                whenDataWasCollected: (searchListCollected) => { dataToCheck = searchListCollected; }
            );

        var resultRun = await model.Run();

        Assert.True(resultRun.IsSuccess);

        Assert.Equal(dataToSearch, dataToCheck);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        await model.DisposeAsync();
    }

    [Fact]
    public async void DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInExecuteNothing()
    {
        _output.WriteLine(nameof(DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInExecuteNothing));
        BlockingCollection<IntegerData> dataToCheck = new();
        IModelScraper model =
            new ModelScraper<DisposeAllExecution, IntegerData>
            (
                20,
                () => new DisposeAllExecution(0),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(1000); },
                whenDataFinished: (resultData) => { if (resultData.IsSuccess) dataToCheck.Add(resultData.Result); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.Empty(dataToCheck);

        await model.DisposeAsync();
    }

    [Fact]
    public async void DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInFinishListAllError()
    {
        _output.WriteLine(nameof(DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInFinishListAllError));
        IEnumerable<Results.ResultBase<Exception?>>? resultList = null;
        IModelScraper model =
            new ModelScraper<DisposeAllExecution, IntegerData>
            (
                20,
                () => new DisposeAllExecution(0),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(1000); },
                whenAllWorksEnd: (resultListFunc) => resultList = resultListFunc
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.NotNull(resultList);

        Assert.All(resultList, (item) => {
            Assert.False(item.IsSuccess);
        });

        await model.DisposeAsync();
    }

    [Fact]
    public async void DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInExecuteAny()
    {
        _output.WriteLine(nameof(DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInExecuteAny));
        BlockingCollection<IntegerData> dataToCheck = new();
        IModelScraper model =
            new ModelScraper<DisposeAllExecution, IntegerData>
            (
                1,
                () => new DisposeAllExecution(1),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(1000); },
                whenDataFinished: (resultData) => { if (resultData.IsSuccess) dataToCheck.Add(resultData.Result); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.NotEmpty(dataToCheck);

        await model.DisposeAsync();
    }
    
    [Fact(Timeout = 1000)]
    public async void CountSearched_RunAndCheckCount_SuccessTotalSearchsEquals1000()
    {
        _output.WriteLine(nameof(DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInExecuteAny));
        const int total = 1000;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                1,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.Equal(total, model.CountSearched);
    }
    
    [Fact(Timeout = 1000)]
    public async void CountSearched_RunAndCheckCountWith100Threads_SuccessTotalSearchsEquals1000()
    {
        _output.WriteLine(nameof(DisposeAllQuest_DisposeAllWithoutSearchs_SuccessInExecuteAny));
        const int total = 1000;
        const int threads = 100;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.Equal(total, model.CountSearched);
    }
    
    [Fact(Timeout = 1000)]
    public async void CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero()
    {
        _output.WriteLine(nameof(CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero));
        const int threads = 100;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(100); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);

        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.Equal(0, model.CountProgress);
    }

    [Fact(Timeout = 1000)]
    public async void CountProgress_RunAndCheckCountWith100Threads_SuccessTotalInProgress100()
    {
        _output.WriteLine(nameof(CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero));
        const int threads = 100;
        const int total = 100;
        IModelScraper model =
            new ModelScraper<EndlessWhileExecution, SimpleData>
            (
                threads,
                () => new EndlessWhileExecution(default),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        Assert.True((await model.Run()).IsSuccess);

        try
        {
            await WaitFinishModel(model, new CancellationTokenSource(total).Token);
        }
        catch (OperationCanceledException) { }
        
        Assert.Equal(ModelStateEnum.Running, model.State);

        Assert.Equal(total, model.CountProgress);

        await model.DisposeAsync();
    }

    [Fact(Timeout = 1000)]
    public async void EndDate_RunAndCheckEndDate_SuccessEndDateLargerThanStart()
    {
        _output.WriteLine(nameof(CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero));
        const int threads = 100;
        const int total = 100;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        Assert.True((await model.Run()).IsSuccess);

        await WaitFinishModel(model);
        
        Assert.Equal(ModelStateEnum.Disposed, model.State);

        Assert.True(model.DtEnd > model.DtRun);
    }

    [Fact(Timeout = 1000)]
    public async void SearchesCollected_RunAndCheckReturn_Success100DataCollected()
    {
        _output.WriteLine(nameof(CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero));
        const int threads = 1;
        const int total = 100;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        var result = await model.Run();

        Assert.True(result.IsSuccess);

        Assert.Equal(result.Result.Searches.Count(), total);

        await WaitFinishModel(model);
    }

    [Fact(Timeout = 1000)]
    public async void SearchesCollected_RunAndTryRunAgainCheckReturn_FailedZeroData()
    {
        _output.WriteLine(nameof(CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero));
        const int threads = 1;
        const int total = 10;
        IModelScraper model =
            new ModelScraper<WaitingExecution, IntegerData>
            (
                threads,
                () => new WaitingExecution(50),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(total); }
            );

        var result = await model.Run();

        result = await model.Run();

        Assert.Equal(Results.Models.RunModelEnum.AlreadyExecuted, result.Result.Status);

        Assert.NotEqual(result.Result.Searches.Count(), total);

        await WaitFinishModel(model);
    }

    [Fact(Timeout = 1000)]
    public async void SearchesCollected_RunAndCheckReturnAfterDispose_FailedZeroData()
    {
        _output.WriteLine(nameof(CountProgress_RunAndCheckCountWith100Threads_SuccessTotalDisposedZero));
        const int threads = 1;
        const int total = 100;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        result = await model.Run();

        Assert.Equal(Results.Models.RunModelEnum.Disposed, result.Result.Status);

        Assert.NotEqual(result.Result.Searches.Count(), total);
    }

    [Fact(Timeout = 1000)]
    public async void AllWorksEnd_RunAndCheckCountOfFinishedEventWith100Threads_SuccessOneExecuted()
    {
        _output.WriteLine(nameof(AllWorksEnd_RunAndCheckCountOfFinishedEventWith100Threads_SuccessOneExecuted));
        const int threads = 100;
        const int total = 1000;
        BlockingCollection<int> threadsIdsFinisheds = new();
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { threadsIdsFinisheds.Add(Thread.CurrentThread.ManagedThreadId); }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.Single(threadsIdsFinisheds);
    }

    [Fact(Timeout = 1000)]
    public async void AllWorksEnd_RunAndCheckIfAllDataIsSearched_FailedAllDataDoesntCollected()
    {
        _output.WriteLine(nameof(AllWorksEnd_RunAndCheckIfAllDataIsSearched_FailedAllDataDoesntCollected));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<EndlessExecution, SimpleData>
            (
                threads,
                () => new EndlessExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { 
                    endResult = result; }
            );

        var result = await model.Run();

        await model.DisposeAsync();

        await WaitFinishModel(model);

        Assert.False(endResult!.AllSearched);
    }
    
    [Fact(Timeout = 1000)]
    public async void AllWorksEnd_RunAndCheckIfAllDataIsSearched_SuccessAllDataCollected()
    {
        _output.WriteLine(nameof(AllWorksEnd_RunAndCheckIfAllDataIsSearched_SuccessAllDataCollected));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { endResult = result; }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.True(endResult!.AllSearched);
    }
    
    [Fact(Timeout = 1000)]
    public async void AllWorksEnd_RunAndCheckIfAllDataIsSearchedAndExceptions_FailedAllDataDoesntCollected()
    {
        _output.WriteLine(nameof(AllWorksEnd_RunAndCheckIfAllDataIsSearchedAndExceptions_FailedAllDataDoesntCollected));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<ThrowExcIntegerExecution, IntegerData>
            (
                threads,
                () => new ThrowExcIntegerExecution(total/2),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { endResult = result; }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.False(endResult!.AllSearched);

        Assert.True(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void WhenOccursException_ThrowExceptionAndSetOk_SuccessRun()
    {
        _output.WriteLine(nameof(WhenOccursException_ThrowExceptionAndSetOk_SuccessRun));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<ThrowExcIntegerExecution, IntegerData>
            (
                threads,
                () => new ThrowExcIntegerExecution(total/2),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(total); },
                whenOccursException: (e, data) => {
                    return QuestResult.Ok();
                },
                whenAllWorksEnd: (result) => { endResult = result; }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.True(endResult!.AllSearched);

        Assert.False(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void WhenOccursException_ThrowExceptionInAction_FailedOneQuestRun()
    {
        _output.WriteLine(nameof(WhenOccursException_ThrowExceptionInAction_FailedOneQuestRun));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<ThrowExcIntegerExecution, IntegerData>
            (
                threads,
                () => new ThrowExcIntegerExecution(total/2),
                async () => { await Task.CompletedTask; return IntegerDataFactory.GetData(total); },
                whenOccursException: (e, data) => {
                    throw e;
                },
                whenAllWorksEnd: (result) => { endResult = result; }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.True(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void WhenDataWasCollected_ThrowExceptionInAction_SuccessRun()
    {
        _output.WriteLine(nameof(WhenDataWasCollected_ThrowExceptionInAction_SuccessRun));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenDataWasCollected: (list) => {
                    throw new Exception();
                },
                whenAllWorksEnd: (result) => { endResult = result; }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.True(endResult!.AllSearched);

        Assert.False(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void WhenAllWorksEnd_ThrowExceptionInAction_SuccessRun()
    {
        _output.WriteLine(nameof(WhenAllWorksEnd_ThrowExceptionInAction_FailedOneQuestRun));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { endResult = result; throw new Exception(); }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.True(endResult!.AllSearched);

        Assert.False(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void WhenAllWorksEnd_ThrowExceptionInAction_FailedOneQuestRun()
    {
        _output.WriteLine(nameof(WhenAllWorksEnd_ThrowExceptionInAction_FailedOneQuestRun));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { endResult = result; throw new Exception(); }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.True(endResult!.AllSearched);

        Assert.False(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void WhenDataFinished_ThrowExceptionInAction_FailedOneQuestRun()
    {
        _output.WriteLine(nameof(WhenDataFinished_ThrowExceptionInAction_FailedOneQuestRun));
        const int threads = 2;
        const int total = 10;
        Results.Models.EndEnumerableModel? endResult = null;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); },
                whenAllWorksEnd: (result) => { endResult = result; throw new Exception(); },
                whenDataFinished: (result) => { throw new Exception(); }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.NotNull(endResult);

        Assert.False(endResult!.AllSearched);

        Assert.True(endResult!.ContainsError);
    }

    [Fact(Timeout = 1000)]
    public async void CountData_RunAndCount_SuccessTotal()
    {
        _output.WriteLine(nameof(CountData_RunAndCount_SuccessTotal));
        const int threads = 1;
        const int total = 10;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        var result = await model.Run();

        await WaitFinishModel(model);

        Assert.Equal(total, result.Result.Searches.Count());
        Assert.Equal(total, model.TotalSearch);
    }

    [Fact(Timeout = 1000)]
    public async void TypeQuest_RunAndTest()
    {
        _output.WriteLine(nameof(CountData_RunAndCount_SuccessTotal));
        const int threads = 1;
        const int total = 10;
        IModelScraper model =
            new ModelScraper<SimpleExecution, SimpleData>
            (
                threads,
                () => new SimpleExecution(),
                async () => { await Task.CompletedTask; return SimpleDataFactory.GetData(total); }
            );

        await Task.CompletedTask;
        Assert.Equal(typeof(SimpleExecution), model.TypeScrap);
    }

    /// <summary>
    /// Wait to finish the model
    /// </summary>
    /// <returns>async</returns>
    /// <exception cref="OperationCanceledException"/>
    private async Task WaitFinishModel(IModelScraper model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (model.State != ModelStateEnum.Disposed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(250);
        }

        return;
    }
}