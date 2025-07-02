using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public GameObject inventoryPanel;
    public GameObject questPanel;
    public static TabManager instance;

    public void ShowUpgrade()
    {
        CloseAll();
        upgradePanel.SetActive(true);
    }

    public void ShowInventory()
    {
        CloseAll();
        inventoryPanel.SetActive(true);
    }

    public void ShowQuest()
    {
        CloseAll();
        questPanel.SetActive(true);
    }

    public void CloseAll()
    {
        upgradePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        questPanel.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
        public void HideAllTabs()
    {
        upgradePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        questPanel.SetActive(false);

        // Скрываем всплывающее окно предмета, если открыто
        if (ItemInfoPanel.instance != null && ItemInfoPanel.instance.IsVisible())
        {
            ItemInfoPanel.instance.Hide();
        }
    }

}
