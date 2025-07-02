using UnityEngine;

// Quest types for different activities
public enum QuestType
{
    Gather,   // Gathering resources, collecting items
    Combat,   // Fighting enemies, battles
    Explore,  // Exploring new areas, discovering locations
    Craft,    // Crafting items, smithing
    Escort    // Escorting NPCs, protecting someone
}

public class Quest : ScriptableObject
{
    public string questDescription;
    public string questTitle;

    public int minLevelRequired;
    public int maxLevelAllowed; // Maximum level for this quest
    public int clickRequirement;
    public Rarity rewardRarityBase;
    public float rewardMultiplier = 1.0f;

    // 💥 Quest type field
    public QuestType questType = QuestType.Gather;

    public string GetRewardDescription(int playerLevel)
    {
        string chanceText = $"{rewardMultiplier * 100f}% шанс предмета +1 редкости";
        return $"Предмет ({rewardRarityBase}), {chanceText}";
    }

    public int GetRewardPoints(int playerLevel)
    {
        return Mathf.Max(1, Mathf.FloorToInt(playerLevel * rewardMultiplier));
    }
}
