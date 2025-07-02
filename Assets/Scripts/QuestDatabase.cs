using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Game/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    public static QuestDatabase instance;
    public List<Quest> quests = new List<Quest>();

    public void OnEnable()
    {
        instance = this;
        LoadQuestsFromResources();
    }

    public Quest GetRandomQuestForLevel(int playerLevel)
    {
        Debug.Log($"🔍 GetRandomQuestForLevel called with playerLevel: {playerLevel}");
        Debug.Log($"🔍 Total quests in database: {quests.Count}");
        
        if (quests == null || quests.Count == 0)
        {
            Debug.LogError("❌ Quests list is null or empty!");
            return null;
        }
        
        var availableQuests = quests
            .Where(q => q != null && q.minLevelRequired <= playerLevel && q.maxLevelAllowed >= playerLevel)
            .ToList();

        Debug.Log($"🔍 Available quests for level {playerLevel}: {availableQuests.Count}");
        
        if (availableQuests.Count == 0)
        {
            Debug.LogWarning($"❌ No available quests found for level {playerLevel}");
            // Debug: Show all quest level requirements
            foreach (var q in quests)
            {
                if (q != null)
                    Debug.Log($"🔍 Quest '{q.questTitle}': minLevel={q.minLevelRequired}, maxLevel={q.maxLevelAllowed}");
            }
            return null;
        }

        var selectedQuest = availableQuests[Random.Range(0, availableQuests.Count)];
        Debug.Log($"✅ Selected quest: {selectedQuest.questTitle}");
        return selectedQuest;
    }

    private void LoadQuestsFromResources()
    {
        if (quests == null || quests.Count == 0)
        {
            quests = Resources.LoadAll<Quest>("Quests").ToList();
            Debug.Log($"🔄 Загружено квестов из Resources: {quests.Count}");
        }
    }

    public void SetQuests(List<Quest> newQuests)
    {
        quests = newQuests;
    }
}
