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