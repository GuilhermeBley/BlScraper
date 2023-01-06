using BlScraper.Model;

namespace BlScraper.DependencyInjection.Model.Context;

/// <summary>
/// Controls async context
/// </summary>
internal class ScrapContextAcessor : IScrapContextAccessor
{
    private static readonly AsyncLocal<ModelScraperInfoHolder> _scrapInfoCurrent = new AsyncLocal<ModelScraperInfoHolder>();

    /// <inheritdoc cref="IScrapContextAccessor.ScrapContext" path="*"/>
    public IModelScraperInfo? ScrapContext
    {
        get { return _scrapInfoCurrent.Value?.ScrapContext; }
        set
        {
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

    /// <inheritdoc cref="IScrapContextAccessor.RequiredScrapContext" path="*"/>
    /// <exception cref="ArgumentNullException"/>
    public IModelScraperInfo RequiredScrapContext => 
        ScrapContext ?? throw new ArgumentNullException(nameof(RequiredScrapContext));
    
    internal ScrapContextAcessor() { }

    /// <summary>
    /// Private unique holder class to context
    /// </summary>
    private class ModelScraperInfoHolder
    {
        public IModelScraperInfo? ScrapContext;
    }
}