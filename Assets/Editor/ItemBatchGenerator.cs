using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ItemBatchGenerator : MonoBehaviour
{
    private static readonly Dictionary<string, Rarity> rarityMap = new()
    {
        { "Common", Rarity.Common },
        { "Uncommon", Rarity.Uncommon },
        { "Rare", Rarity.Rare },
        { "Epic", Rarity.Epic },
        { "Legendary", Rarity.Legendary }
    };

    private static readonly Dictionary<Rarity, int> rarityStatPoints = new()
    {
        { Rarity.Common, 3 },
        { Rarity.Uncommon, 5 },
        { Rarity.Rare, 7 },
        { Rarity.Epic, 10 },
        { Rarity.Legendary, 15 }
    };

    [MenuItem("Tools/Генерация предметов/Сканировать и создать все предметы")]
    public static void GenerateAllItems()
    {
        string baseSpritePath = "Assets/Resources/Sprites/Items/";
        string baseItemPath = "Assets/Resources/Items/";

        foreach (string rarityFolder in Directory.GetDirectories(baseSpritePath))
        {
            string rarityName = Path.GetFileName(rarityFolder);
            if (!rarityMap.TryGetValue(rarityName, out Rarity rarity))
                continue;

            foreach (string categoryFolder in Directory.GetDirectories(rarityFolder))
            {
                string category = Path.GetFileName(categoryFolder);
                string relativeSpritePath = $"Sprites/Items/{rarityName}/{category}";
                string outputFolder = $"{baseItemPath}{rarityName}/{category}/";

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                Sprite[] sprites = Resources.LoadAll<Sprite>(relativeSpritePath);

                int index = 1;
                foreach (Sprite sprite in sprites)
                {
                    string itemName = $"{rarityName}_{category}_{index}";
                    string assetPath = $"{outputFolder}{itemName}.asset";

                    if (File.Exists(assetPath))
                    {
                        index++;
                        continue;
                    }

                    Item newItem = ScriptableObject.CreateInstance<Item>();
                    newItem.itemName = itemName;
                    newItem.icon = sprite;
                    newItem.rarity = rarity;
                    newItem.statBonuses = GenerateStatBonuses(rarityStatPoints[rarity]);

                    AssetDatabase.CreateAsset(newItem, assetPath);
                    Debug.Log($"Создан предмет: {itemName}");

                    index++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Генерация завершена.");
    }

    private static int[] GenerateStatBonuses(int totalPoints)
    {
        int[] stats = new int[5];
        for (int i = 0; i < totalPoints; i++)
        {
            int randomIndex = Random.Range(0, stats.Length);
            stats[randomIndex]++;
        }
        return stats;
    }
}
