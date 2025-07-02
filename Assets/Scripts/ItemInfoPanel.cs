using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoPanel : MonoBehaviour
{
    

    public static ItemInfoPanel instance;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI statText;
    public GameObject panelRoot; // сам объект панели (если нужно скрыть/показать)

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Hide(); // спрячем окно при запуске
    }



    public void Show(Item item)
    {
        if (item == null || panelRoot == null) return;

        panelRoot.SetActive(true);

        if (item == null) return;

        panelRoot.SetActive(true);

        icon.sprite = item.icon;
        nameText.text = item.itemName;

        rarityText.text = item.rarity.ToString();
        rarityText.color = GetRarityColor(item.rarity);

        statText.text = GetStatDescription(item);

        transform.SetAsLastSibling(); // поверх других UI
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Uncommon: return Color.green;
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return new Color(0.6f, 0, 0.8f); // фиолетовый
            case Rarity.Legendary: return Color.yellow;
            default: return Color.white;
        }
    }

    string GetStatDescription(Item item)
    {
        string desc = "";

        for (int i = 0; i < item.statBonuses.Length; i++)
        {
            int bonus = item.statBonuses[i];
            if (bonus != 0 && UpgradeSystem.instance.statNameTexts.Length > i)
            {
                string statName = UpgradeSystem.instance.statNameTexts[i].text;
                desc += $"{statName}: +{bonus}\n";
            }
        }

        return desc == "" ? "Нет бонусов" : desc.TrimEnd();
    }
    public bool IsVisible()
    {
        return panelRoot != null && panelRoot.activeSelf;
    }

}
