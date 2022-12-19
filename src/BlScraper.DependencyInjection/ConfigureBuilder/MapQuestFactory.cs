using BlScraper.DependencyInjection.Builder.Internal;

namespace BlScraper.DependencyInjection.ConfigureBuilder;

/// <summary>
/// Factory of <see cref="IMapQuest"/>>
/// </summary>
public static class MapQuestFactory
{
    /// <summary>
    /// Create new <see cref="IMapQuest"/>
    /// </summary>
    /// <param name="assemblies">assemblies to find the quests</param>
    public static IMapQuest Create(params System.Reflection.Assembly[] assemblies)
    {
        return new MapQuest(assemblies);
    }

    private class MapQuest : IMapQuest
    {
        private IEnumerable<(Type Quest, Type Data)> _availableQuests;

        public MapQuest(params System.Reflection.Assembly[] assemblies)
        {
            _availableQuests = assemblies.SelectMany((assembly) =>
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
}