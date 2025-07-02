using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public Image borderImage;
    public Item thisItem;
    public Image rarityFrameImage;
    public GameObject equipOverlay;
    public bool isEquipped = false;
    public static InventorySlot selectedSlot;
    private Player player;
    public TextMeshProUGUI countText;
    public GameObject countGroup;

    private int count = 1;

    void Start()
    {
        player = FindAnyObjectByType<Player>();

        if (itemImage != null && thisItem != null)
            itemImage.sprite = thisItem.icon;

        if (borderImage != null)
            borderImage.enabled = false;

        UpdateCountText();
    }

    public void SetItem(Item item)
    {
        thisItem = item;
        if (itemImage != null)
            itemImage.sprite = item.icon;

        SetRarityColor(item.rarity); // 🛠 не забываем установить цвет рамки!

        itemImage.enabled = true;
        borderImage.enabled = false;
        count = 1;
        UpdateCountText();

        if (rarityFrameImage != null)
            rarityFrameImage.transform.SetAsFirstSibling();
    }


    public void IncreaseCount()
    {
        count++;
        UpdateCountText();
    }

    public void DecreaseCount()
    {
        count = Mathf.Max(0, count - 1);
        UpdateCountText();

        if (count == 0)
            Destroy(gameObject);
    }

    private void UpdateCountText()
    {
        if (count > 1)
        {
            countText.text = count.ToString();
            if (countGroup != null)
                countGroup.SetActive(true);
        }
        else
        {
            countText.text = "";
            if (countGroup != null)
                countGroup.SetActive(false);
        }
    }

    public int GetCount() => count;
    public Item GetItem() => thisItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (thisItem == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (SmithPanelController.instance != null && SmithPanelController.instance.IsOpen())
            {
                // Если открыт smithPanel
                if (isEquipped)
                {
                    Debug.Log("❗ Нельзя перековать экипированный предмет");
                    return;
                }

                SmithPanelController.instance.AddItemToForge(thisItem, this);
                return;
            }

            // Стандартная экипировка
            if (!isEquipped)
            {
                Equip();
            }
            else
            {
                Unequip();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (thisItem != null && ItemInfoPanel.instance != null)
            {
                ItemInfoPanel.instance.Show(thisItem);
            }
            else
            {
                Debug.LogWarning("❗ Невозможно показать информацию — thisItem или ItemInfoPanel отсутствует");
            }
        }
    }

    private void SetRarityColor(Rarity rarity)
    {
        if (rarityFrameImage == null) return;

        switch (rarity)
        {
            case Rarity.Common: rarityFrameImage.color = Color.gray; break;
            case Rarity.Uncommon: rarityFrameImage.color = Color.green; break;
            case Rarity.Rare: rarityFrameImage.color = Color.cyan; break;
            case Rarity.Epic: rarityFrameImage.color = new Color(0.6f, 0.2f, 0.8f); break; // фиолетовый
            case Rarity.Legendary: rarityFrameImage.color = new Color(1f, 0.84f, 0f); break; // золотой
            default: rarityFrameImage.color = Color.white; break;
        }
    }

    public void Equip()
    {
        isEquipped = true;
        Player.instance.EquipItem(thisItem, this);
        equipOverlay?.SetActive(true);
        borderImage.enabled = true;
        UpgradeSystem.instance.UpdateUpgradeUI();
    }

    public void Unequip()
    {
        isEquipped = false;
        Player.instance.RemoveItem(thisItem);
        equipOverlay?.SetActive(false);
        borderImage.enabled = false;
        UpgradeSystem.instance.UpdateUpgradeUI();
    }

    public void ForceUnequip()
    {
        isEquipped = false;
        borderImage.enabled = false;
        UpgradeSystem.instance.UpdateUpgradeUI();
    }
}
