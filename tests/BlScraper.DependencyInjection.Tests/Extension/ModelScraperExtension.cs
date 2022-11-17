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
}