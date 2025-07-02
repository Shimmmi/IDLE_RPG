using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    private Dictionary<Rarity, List<Item>> itemsByRarity = new();
    public List<Item> allItems = new List<Item>();

    private Dictionary<Rarity, float> rarityChances = new()
    {
        { Rarity.Legendary, 0.001f },
        { Rarity.Epic, 0.1f },
        { Rarity.Rare, 1f },
        { Rarity.Uncommon, 5f },
        { Rarity.Common, 93.899f } // Остаток до 100
    };

    void Awake()
    {
        instance = this;

        allItems = new List<Item>(Resources.LoadAll<Item>("Items"));

        foreach (var item in allItems)
        {
            if (!itemsByRarity.ContainsKey(item.rarity))
                itemsByRarity[item.rarity] = new List<Item>();

            itemsByRarity[item.rarity].Add(item);
        }

        Debug.Log($"🎒 Загружено предметов: {allItems.Count}");
    }


    public Item GetRandomItem()
    {
        Rarity selectedRarity = RollRarity();

        if (itemsByRarity.ContainsKey(selectedRarity) && itemsByRarity[selectedRarity].Count > 0)
        {
            var list = itemsByRarity[selectedRarity];
            return list[Random.Range(0, list.Count)];
        }

        // Fallback: вернуть случайный common
        if (itemsByRarity.ContainsKey(Rarity.Common))
        {
            var commons = itemsByRarity[Rarity.Common];
            return commons[Random.Range(0, commons.Count)];
        }

        Debug.LogWarning("❌ Нет предметов подходящей редкости!");
        return null;
    }

    private Rarity RollRarity()
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var pair in rarityChances.OrderByDescending(p => p.Value))
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        return Rarity.Common; // fallback (теоретически недостижим)
    }
    public Item GetRandomItemOfRarity(Rarity rarity)
    {
        var filtered = allItems.Where(i => i.rarity == rarity).ToList();
        if (filtered.Count == 0) return null;
        return filtered[Random.Range(0, filtered.Count)];
    }
    public Item GetRandomItemWithExactRarity(Rarity rarity)
    {
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogWarning("❗ База предметов пуста!");
            return null;
        }

        var filteredItems = allItems.Where(i => i.rarity == rarity).ToList();

        if (filteredItems.Count == 0)
        {
            Debug.LogWarning($"❗ Нет предметов с редкостью {rarity}");
            return null;
        }

        int index = Random.Range(0, filteredItems.Count);
        return filteredItems[index];
    }


}
