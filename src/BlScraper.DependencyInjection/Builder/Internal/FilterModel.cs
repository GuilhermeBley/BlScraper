using System.Reflection;
using BlScraper.DependencyInjection.ConfigureBuilder;
using BlScraper.DependencyInjection.ConfigureModel;
using BlScraper.DependencyInjection.ConfigureModel.Filter;
using BlScraper.DependencyInjection.Extension.Internal;
using BlScraper.Model;
using Microsoft.Extensions.DependencyInjection;

namespace BlScraper.DependencyInjection.Builder.Internal;

/// <summary>
/// Create Filters
/// </summary>
internal sealed class FilterModel
{
    private IModelScraperInfo? _currentInfo;
    private readonly IServiceProvider _serviceProvider;
    private readonly Model.Context.ScrapContextAcessor _contextAcessor = new();
    private PoolFilter _poolFilter;

    public IModelScraperInfo? CurrentInfo { 
        get { return _currentInfo; } 
        set { 
            if (value is null) 
                throw new ArgumentNullException(nameof(CurrentInfo)); 
            _currentInfo = value;
        }}

    public FilterModel(IServiceProvider serviceProvider, ScrapBuilderConfig builderConfig)
    {
        _serviceProvider = serviceProvider;
        _poolFilter = builderConfig.Filters;
    }

    public void CreateFilterByModel(ScrapModelInternal model)
    {
        _poolFilter = _poolFilter.Union(model.Filters);
    }
}