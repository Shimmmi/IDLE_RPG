using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class QuestGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Quests From File")]
    public static void GenerateQuests()
    {
        string path = "Assets/Resources/Quests/quest_list.txt";
        if (!File.Exists(path))
        {
            Debug.LogWarning("❌ Файл quest_list.txt не найден по пути: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        List<Quest> newQuests = new List<Quest>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length < 7) // Updated to require 7 parts including QuestType
            {
                Debug.LogWarning("❗ Неверный формат строки (ожидается 7 частей с QuestType): " + line);
                continue;
            }

            // Check if any essential parts are empty
            if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            {
                Debug.LogWarning("❗ Пустое название или описание квеста: " + line);
                continue;
            }

            string title = parts[0].Trim();
            string description = parts[1].Trim();
            int minLevel;
            int maxLevel;
            int clicks;
            float multiplier = 1f;
            Rarity baseRarity;
            QuestType questType = QuestType.Gather; // Default quest type

            if (!int.TryParse(parts[2].Trim(), out minLevel) || !int.TryParse(parts[3].Trim(), out maxLevel) || !int.TryParse(parts[4].Trim(), out clicks))
            {
                Debug.LogWarning("❗ Ошибка парсинга числовых значений: " + line);
                continue;
            }
            
            if (!System.Enum.TryParse(parts[5].Trim(), true, out baseRarity))
            {
                Debug.LogWarning("❗ Ошибка парсинга редкости: " + line);
                continue;
            }

            // Parse multiplier (index 6)
            if (parts.Length > 6 && !string.IsNullOrWhiteSpace(parts[6]))
            {
                if (!float.TryParse(parts[6].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out multiplier))
                {
                    Debug.LogWarning("❗ Ошибка парсинга множителя: " + line);
                    continue;
                }
            }
            
            // Parse quest type (index 7)
            if (parts.Length > 7 && !string.IsNullOrWhiteSpace(parts[7]))
            {
                if (!System.Enum.TryParse(parts[7].Trim(), true, out questType))
                {
                    Debug.LogWarning("❗ Ошибка парсинга типа квеста: " + parts[7].Trim() + " в строке: " + line);
                    Debug.LogWarning("❗ Доступные типы: Gather, Combat, Explore, Craft, Escort");
                    questType = QuestType.Gather; // Default fallback
                }
            }

            Quest quest = ScriptableObject.CreateInstance<Quest>();
            quest.questTitle = title;
            quest.questDescription = description;
            quest.minLevelRequired = minLevel;
            quest.maxLevelAllowed = maxLevel;
            quest.clickRequirement = clicks;
            quest.rewardRarityBase = baseRarity;
            quest.rewardMultiplier = multiplier;
            quest.questType = questType; // Set the quest type

            string assetPath = $"Assets/Resources/Quests/{title.Replace(" ", "_")}.asset";
            AssetDatabase.CreateAsset(quest, assetPath);
            newQuests.Add(quest);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        QuestDatabase db = Resources.Load<QuestDatabase>("QuestDatabase");
        if (db != null)
        {
            db.SetQuests(newQuests);
            EditorUtility.SetDirty(db);
            Debug.Log($"✅ Квесты успешно обновлены в QuestDatabase. Создано: {newQuests.Count} квестов");
            
            // Log quest type distribution
            var typeCount = new System.Collections.Generic.Dictionary<QuestType, int>();
            foreach (var quest in newQuests)
            {
                if (!typeCount.ContainsKey(quest.questType))
                    typeCount[quest.questType] = 0;
                typeCount[quest.questType]++;
            }
            
            Debug.Log("🔍 Распределение по типам квестов:");
            foreach (var pair in typeCount)
            {
                Debug.Log($"  {pair.Key}: {pair.Value} квестов");
            }
        }
        else
        {
            Debug.LogWarning("❌ QuestDatabase не найден в Resources!");
        }
    }
    
    /// <summary>
    /// Validates if a string represents a valid QuestType
    /// </summary>
    private static bool IsValidQuestType(string typeString)
    {
        string[] validTypes = { "Gather", "Combat", "Explore", "Craft", "Escort" };
        return System.Array.Exists(validTypes, t => t.Equals(typeString, System.StringComparison.OrdinalIgnoreCase));
    }
}
