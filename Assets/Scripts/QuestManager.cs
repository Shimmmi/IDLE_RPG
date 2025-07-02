using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private Quest currentQuest;
    private int currentQuestClicksRemaining;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public bool HasActiveQuest => currentQuest != null;

    public void TryStartQuest(int playerLevel)
    {
        Debug.Log($"🔍 QuestManager.TryStartQuest called with playerLevel: {playerLevel}");
        
        if (currentQuest != null)
        {
            Debug.Log("❌ Cannot start quest - already has active quest");
            return;
        }
        
        if (QuestDatabase.instance == null)
        {
            Debug.LogError("❌ QuestDatabase.instance is null in QuestManager!");
            return;
        }
        
        Debug.Log($"🔍 QuestDatabase has {QuestDatabase.instance.quests.Count} quests");

        Quest quest = QuestDatabase.instance.GetRandomQuestForLevel(playerLevel);
        if (quest == null)
        {
            Debug.LogWarning($"❌ No quest found for level {playerLevel}");
            return;
        }
        
        Debug.Log($"✅ Found quest: {quest.questTitle}");

        currentQuest = quest;
        currentQuestClicksRemaining = quest.clickRequirement;

        // 💥 Сохраняем тип квеста
        if (Player.instance != null)
        {
            Player.instance.SetCurrentQuestType(quest.questType);
            Debug.Log($"✅ Set quest type: {quest.questType}");
        }
        else
        {
            Debug.LogError("❌ Player.instance is null!");
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowQuestPanel(quest);
            Debug.Log("✅ Quest panel shown");
        }
        else
        {
            Debug.LogError("❌ UIManager.instance is null!");
        }
        
        Debug.Log($"🧭 Новый квест: {quest.questDescription} (Нужно кликов: {quest.clickRequirement})");
    }

    public void RegisterClick()
    {
        if (currentQuest == null || currentQuestClicksRemaining <= 0) return;

        currentQuestClicksRemaining--;
        UIManager.instance.UpdateQuestProgress(currentQuest.clickRequirement - currentQuestClicksRemaining, currentQuest.clickRequirement);

        if (currentQuestClicksRemaining <= 0)
            CompleteQuest();
    }

    private void CompleteQuest()
    {
        Debug.Log("✅ Квест выполнен!");
        GrantQuestReward();

        UIManager.instance.HideQuestPanel();
        
        // Clear quest state from player
        if (Player.instance != null)
        {
            Player.instance.ClearCurrentQuest();
        }
        
        currentQuest = null;
    }

    private void GrantQuestReward()
    {
        if (currentQuest == null) return;

        Rarity rarity = currentQuest.rewardRarityBase;
        float roll = Random.value * 100f;

        if (roll < currentQuest.rewardMultiplier * 100f)
            rarity = (Rarity)Mathf.Min((int)rarity + 1, 4);

        Item item = ItemDatabase.instance.GetRandomItemWithExactRarity(rarity);
        if (item != null)
        {
            MainGameManager.instance.inventorySystem.AddItem(item);

            Debug.Log($"🎁 Получен предмет: {item.itemName} ({rarity})");

            // Показываем иконку предмета на UI
            FloatingIconUI.instance.ShowIcon(item.icon); // Можно указать длительность, например: ShowIcon(item.icon, 3f)
        }
        else
        {
            Debug.LogWarning("❌ Не удалось найти предмет нужной редкости");
        }
    }
}
