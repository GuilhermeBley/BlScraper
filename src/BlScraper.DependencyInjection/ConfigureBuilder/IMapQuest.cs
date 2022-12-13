using BlScraper.DependencyInjection.Builder;
using BlScraper.DependencyInjection.Builder.Internal;

namespace BlScraper.DependencyInjection.ConfigureBuilder;

/// <summary>
/// Map quests and data
/// </summary>
public interface IMapQuest
{
    /// <summary>
    /// Get available quests and datas
    /// </summary>
    IEnumerable<(Type Quest, Type Data)> GetAvailableQuestsAndData();
    
    /// <summary>
    /// Get available quests
    /// </summary>
    IEnumerable<Type> GetAvailableQuests();
}

internal class MapQuest : IMapQuest
{
    private IEnumerable<(Type Quest, Type Data)> _availableQuests;

    public MapQuest(ScrapBuilderConfig assemblyBuilderAdd)
    {
        _availableQuests = assemblyBuilderAdd.Assemblies.SelectMany((assembly) =>
        {
            return assembly.GetTypes().Where(t => TypeUtils.IsTypeValidQuest(t)).Select((type) =>
            {
                var model = new ScrapModelInternal(type);
                return (model.QuestType, model.DataType);
            });
        }).ToList();
    }

    public IEnumerable<Type> GetAvailableQuests()
    {
        return _availableQuests.Select(tuple => tuple.Quest);
    }

    public IEnumerable<(Type Quest, Type Data)> GetAvailableQuestsAndData()
    {
        return _availableQuests;
    }
}