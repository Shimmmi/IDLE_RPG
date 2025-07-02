using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SmithPanelController : MonoBehaviour
{
    public Transform smithGrid;
    public GameObject smithSlotPrefab;
    public TextMeshProUGUI forgeInfoText;
    public Button autoAddButton;
    public Button forgeButton;
    public static SmithPanelController instance;

    private List<Item> selectedItems = new List<Item>();
    private Rarity? currentRarity = null;

    private int requiredItems = 10;
    private float rareChance = 0f;
    private int maxAllowedItems = 10;

    public bool IsOpen() => gameObject.activeSelf;
    public int GetCurrentItemCount() => selectedItems.Count;
    public int GetRequiredItemCount() => requiredItems;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        UpdateForgeInfo();
    }

    public void UpdateForgeInfo()
    {
        if (forgeInfoText == null || Skills.instance == null) return;

        int smithLevel = Skills.instance.smithing;
        requiredItems = Mathf.Max(5, 10 - (smithLevel / 10));
        rareChance = Mathf.Min(0.5f * smithLevel, 25f);
        maxAllowedItems = requiredItems;

        forgeInfoText.text = $"Для перековки нужно: {requiredItems} предметов\nШанс +1 редкость: {rareChance}%";
    }

    public void AddItemManually(InventorySlot slot)
    {
        if (slot == null || slot.GetItem() == null) return;

        if (selectedItems.Count >= maxAllowedItems)
        {
            Debug.Log("❗ Нельзя добавить больше предметов, чем требуется для перековки!");
            return;
        }

        if (selectedItems.Count > 0 && selectedItems[0].rarity != slot.GetItem().rarity)
        {
            Debug.Log("❗ Нельзя добавить предмет другой редкости!");
            return;
        }

        selectedItems.Add(slot.GetItem());

        GameObject newSlot = Instantiate(smithSlotPrefab, smithGrid);
        newSlot.GetComponent<SmithSlot>().SetItem(slot.GetItem());

        slot.DecreaseCount();
        InventorySystem.instance.DecreaseRarityCounter(slot.GetItem().rarity);

        UpdateForgeInfo();
    }

    public void OnForgePressed()
    {
        if (selectedItems.Count < requiredItems)
        {
            Debug.Log("Недостаточно предметов для перековки!");
            return;
        }

        var baseItem = selectedItems[0];
        var newRarity = (Rarity)Mathf.Min((int)baseItem.rarity + 1, 4);

        if (Random.value < rareChance / 100f)
            newRarity = (Rarity)Mathf.Min((int)newRarity + 1, 4);

        Item resultItem = ItemDatabase.instance.GetRandomItemWithExactRarity(newRarity);

        if (resultItem == null)
        {
            Debug.LogWarning("❌ Не удалось создать новый предмет");
            return;
        }

        InventorySystem.instance.AddItem(resultItem);
        Debug.Log($"🔥 Перековка завершена! Получен предмет: {resultItem.itemName}");

        ClearGrid();
        UpdateForgeInfo();
    }

    public void ClearGrid()
    {
        foreach (Transform child in smithGrid)
        {
            Destroy(child.gameObject);
        }
        selectedItems.Clear();
        currentRarity = null;
    }

    public void OnSmithPanelClosed()
    {
        ReturnItemsToInventory();
        ClearGrid();
    }

    public void CloseSmithPanel()
    {
        ReturnItemsToInventory();
        ClearGrid();
        gameObject.SetActive(false);
    }

    public void AddItemToForge(Item item, InventorySlot fromSlot)
    {
        if (selectedItems.Count >= maxAllowedItems)
        {
            Debug.LogWarning("❗ Превышено максимальное количество предметов для перековки!");
            return;
        }

        if (selectedItems.Count == 0)
        {
            currentRarity = item.rarity;
        }
        else if (item.rarity != currentRarity)
        {
            Debug.LogWarning("❌ Нельзя смешивать предметы разной редкости!");
            return;
        }

        GameObject newSlot = Instantiate(smithSlotPrefab, smithGrid);
        var smithSlot = newSlot.GetComponent<SmithSlot>();
        smithSlot.SetItem(item);

        selectedItems.Add(item);

        fromSlot.DecreaseCount();
        InventorySystem.instance.DecreaseRarityCounter(item.rarity);

        UpdateForgeInfo();
    }

    public void ReturnItemsToInventory()
    {
        foreach (Transform child in smithGrid)
        {
            var slot = child.GetComponent<SmithSlot>();
            if (slot != null && slot.GetItem() != null)
            {
                InventorySystem.instance.ReturnItem(slot.GetItem());
            }
        }
    }

    public void RemoveItemFromForge(SmithSlot slot)
    {
        if (slot == null || slot.GetItem() == null) return;

        Item itemToReturn = slot.GetItem();
        InventorySystem.instance.ReturnItem(itemToReturn);

        selectedItems.Remove(itemToReturn);
        Destroy(slot.gameObject);

        UpdateForgeInfo();
    }

    public void OnSmithSlotClicked(SmithSlot slot)
    {
        if (slot == null || slot.GetItem() == null) return;

        Item itemToReturn = slot.GetItem();
        InventorySystem.instance.ReturnItem(itemToReturn);

        selectedItems.Remove(itemToReturn);
        Destroy(slot.gameObject);

        UpdateForgeInfo();
    }

    public void OnAutoAddPressed()
    {
        UpdateForgeInfo();

        var allSlots = InventorySystem.instance.GetAllSlots();

        foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
        {
            if (selectedItems.Count > 0 && rarity != selectedItems[0].rarity)
                continue;

            var sameRaritySlots = allSlots.FindAll(s => s.GetItem() != null && s.GetItem().rarity == rarity);

            int totalCount = 0;
            foreach (var s in sameRaritySlots) totalCount += s.GetCount();
            if (totalCount < requiredItems) continue;

            foreach (var slot in sameRaritySlots)
            {
                while (slot.GetCount() > 0 && selectedItems.Count < maxAllowedItems)
                {
                    AddItemManually(slot);
                }
                if (selectedItems.Count >= maxAllowedItems)
                    break;
            }
            break;
        }

        UpdateForgeInfo();
    }

    private void OnDisable()
    {
        if (selectedItems.Count > 0)
        {
            ReturnItemsToInventory();
            ClearGrid();
        }
    }
}
