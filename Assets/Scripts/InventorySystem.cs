using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;



public class InventorySystem : MonoBehaviour
{
    [SerializeField] private TMP_Text smithInfoText;
    [SerializeField] private Skills smithSkill; // скрипт с уровнем навыка кузнеца

    public GameObject slotPrefab;
    public Transform inventoryGrid; // ← сюда добавляем предметы
    public GameObject smithPanel;

    public static InventorySystem instance;
    public TextMeshProUGUI commonCountText;
    public TextMeshProUGUI uncommonCountText;
    public TextMeshProUGUI rareCountText;
    public TextMeshProUGUI epicCountText;
    public TextMeshProUGUI legendaryCountText;

    private Dictionary<Rarity, int> rarityCounters = new();
    private Dictionary<string, InventorySlot> itemSlots = new();

    void Awake()
    {
        if (instance == null) instance = this;

        foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
            rarityCounters[rarity] = 0;
    }

    void Start()
    {
        if (smithPanel != null)
            smithPanel.SetActive(false);
    }

    public void AddRandomItem()
    {
        if (ItemDatabase.instance == null) return;

        Item newItem = ItemDatabase.instance.GetRandomItem();
        AddItem(newItem); // 👈 добавляем через основной метод
    }

    public void AddItem(Item item)
    {
        string key = item.itemName;

        if (itemSlots.ContainsKey(key) && itemSlots[key] != null)
        {
            itemSlots[key].IncreaseCount();
        }
        else
        {
            if (itemSlots.ContainsKey(key))
                itemSlots.Remove(key); // удаляем невалидный слот

            GameObject slot = Instantiate(slotPrefab, inventoryGrid);
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            slotScript.SetItem(item);
            itemSlots.Add(key, slotScript);
        }

        rarityCounters[item.rarity]++;
        UpdateRarityCountersUI();
    }

    void UpdateRarityCountersUI()
    {
        Debug.Log("🔁 Обновление счётчиков");

        commonCountText.text = $"Common: {rarityCounters[Rarity.Common]}";
        uncommonCountText.text = $"Uncommon: {rarityCounters[Rarity.Uncommon]}";
        rareCountText.text = $"Rare: {rarityCounters[Rarity.Rare]}";
        epicCountText.text = $"Epic: {rarityCounters[Rarity.Epic]}";
        legendaryCountText.text = $"Legendary: {rarityCounters[Rarity.Legendary]}";
    }
    

    public void OpenSmithPanel()
    {
        smithPanel.SetActive(true);
        UpdateSmithPanelInfo();
    }
    private void UpdateSmithPanelInfo()
    {
        int skillLevel = smithSkill.smithing;
        float bonusChance = Mathf.Min(0.5f * skillLevel, 25f);
        int baseRequired = 10;
        int reduction = skillLevel / 10;
        int itemsRequired = Mathf.Max(baseRequired - reduction, 5);

        smithInfoText.text = $"Необходимо предметов: {itemsRequired}\n" +
                             $"Шанс улучшения редкости: {bonusChance}%";
    }
    public void AutoFillSmithItems()
    {
        var itemsGrouped = new Dictionary<string, List<Item>>();

        foreach (var slot in GetComponentsInChildren<InventorySlot>())
        {
            var item = slot.GetItem();
            if (item == null) continue;

            string key = item.itemName;
            if (!itemsGrouped.ContainsKey(key))
                itemsGrouped[key] = new List<Item>();

            itemsGrouped[key].Add(item);
        }

        var sortedGroups = itemsGrouped
            .Where(g => g.Value.Count >= smithSkill.GetSmithingRequiredItemCount())
            .OrderByDescending(g => g.Value.First().rarity)
            .ToList();

        if (sortedGroups.Count > 0)
        {
            var selectedGroup = sortedGroups.First();
            var itemsToUse = selectedGroup.Value.Take(smithSkill.GetSmithingRequiredItemCount()).ToList();
            // тут добавляем в сетку кузницы и удаляем из инвентаря (по нужному количеству)
        }
    }
    public void CloseSmithPanel()
    {
        SmithPanelController smithController = FindAnyObjectByType<SmithPanelController>();
        if (smithController != null)
            smithController.ReturnItemsToInventory();

        smithPanel.SetActive(false);
    }

    public List<InventorySlot> GetAllSlots()
    {
        return inventoryGrid.GetComponentsInChildren<InventorySlot>().ToList();
    }

    public Dictionary<string, List<Item>> GetGroupedItemsByName()
    {
        var result = new Dictionary<string, List<Item>>();

        foreach (var slot in GetAllSlots())
        {
            var item = slot.GetItem();
            if (item == null) continue;

            string key = item.itemName;
            if (!result.ContainsKey(key))
                result[key] = new List<Item>();

            result[key].Add(item);
        }

        return result;
    }


    public void RemoveOne(Item item)
    {
        string key = item.itemName;

        if (itemSlots.ContainsKey(key))
        {
            InventorySlot slot = itemSlots[key];
            if (slot == null)
            {
                itemSlots.Remove(key);
                return;
            }

            if (slot.GetCount() > 1)
            {
                slot.DecreaseCount();
            }
            else
            {
                Destroy(slot.gameObject);
                itemSlots.Remove(key);
            }

            rarityCounters[item.rarity] = Mathf.Max(0, rarityCounters[item.rarity] - 1);
            UpdateRarityCountersUI();
        }
    }


    public void RemoveItems(List<Item> itemsToRemove)
    {
        var groupedItems = itemsToRemove.GroupBy(item => item.itemName);

        foreach (var group in groupedItems)
        {
            foreach (var item in group)
            {
                RemoveOne(item);
            }
        }
    }
    public bool IsSmithPanelOpen()
    {
        return smithPanel != null && smithPanel.activeSelf;
    }

    public void AddItemToSmith(InventorySlot slot)
    {
        if (smithPanel == null) return;

        var smithController = smithPanel.GetComponent<SmithPanelController>();
        if (smithController != null)
        {
            smithController.AddItemManually(slot);
        }
    }

    public void ReturnItem(Item item)
    {
        if (item == null) return;
        string key = item.itemName;

        if (itemSlots.ContainsKey(key) && itemSlots[key] != null)
        {
            itemSlots[key].IncreaseCount();
        }
        else
        {
            if (itemSlots.ContainsKey(key)) itemSlots.Remove(key);

            GameObject newSlot = Instantiate(slotPrefab, inventoryGrid);
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            slotScript.SetItem(item);
            itemSlots.Add(key, slotScript);
        }

        rarityCounters[item.rarity]++;
        UpdateRarityCountersUI();
    }


    public void DecreaseRarityCounter(Rarity rarity)
    {
        if (rarityCounters.ContainsKey(rarity))
        {
            rarityCounters[rarity] = Mathf.Max(0, rarityCounters[rarity] - 1);
            UpdateRarityCountersUI();
        }
    }



}
