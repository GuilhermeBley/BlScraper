using BlScraper.Model;

namespace BlScraper.DependencyInjection.Model.Context;

/// <summary>
/// Controls async context
/// </summary>
internal class ScrapContextAcessor : IScrapContextAcessor
{
    private static readonly AsyncLocal<ModelScraperInfoHolder> _scrapInfoCurrent = new AsyncLocal<ModelScraperInfoHolder>();

    public IModelScraperInfo? ScrapContext {
        get { return _scrapInfoCurrent.Value?.ScrapContext; }
        set {
            if (value is null)
                throw new ArgumentNullException(nameof(IModelScraperInfo));
            
            var holder = _scrapInfoCurrent.Value;
            if (holder != null)
            {
                holder.ScrapContext = null;
            }

            if (value != null)
            {
                _scrapInfoCurrent.Value = new ModelScraperInfoHolder() { ScrapContext = value };
            }
        }
    }

    /// <summary>
    /// Private unique holder class to context
    /// </summary>
    private class ModelScraperInfoHolder
    {
        public IModelScraperInfo? ScrapContext;
    }
}