using BlScraper.Model;

namespace BlScraper.DependencyInjection.Tests.Extension;

internal static class ModelScraperExtension
{
    /// <summary>
    /// Wait model finish
    /// </summary>
    /// <param name="modelScraper">IModelScraper</param>
    /// <param name="cancellationToken">Token to cancel wait</param>
    /// <returns>true : disposed, false : wait cancelled</returns>
    public static async Task<bool> WaitModelDispose(this IModelScraper modelScraper, CancellationToken cancellationToken = default)
    {
        while (modelScraper.State != ModelStateEnum.Disposed)
        {
            await Task.Delay(100);
            if (cancellationToken.IsCancellationRequested)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Wait model finish
    /// </summary>
    /// <param name="model">current model</param>
    /// <param name="cancellationToken">token to cancel</param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"/>
    public static async Task WaitModelFinish(this IModelScraper model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (model.State != ModelStateEnum.Disposed)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(400);
        }
    }

    

    /// <summary>
    /// Wait model finish
    /// </summary>
    /// <param name="model">current model</param>
    /// <param name="cancellationToken">token to cancel</param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"/>
    public static async Task RunAndWaitModelFinish(this IModelScraper model, CancellationToken cancellationToken = default)
    {
        if (!(await model.Run()).IsSuccess)
        {
            throw new InvalidOperationException();
        }

        await WaitModelFinish(model, cancellationToken);
    }
}