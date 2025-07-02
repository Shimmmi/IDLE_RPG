using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text levelText;
    public TMP_Text questText;
    public static UIManager instance;
    public GameObject questPanel;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questProgressText;
    public TextMeshProUGUI questRewardText;
    public TextMeshProUGUI questTypeText; // New field for quest type
    public TextMeshProUGUI activeQuestTitleOverlay;
    public TextMeshProUGUI activeQuestProgressOverlay;
    public TextMeshProUGUI activeQuestTypeOverlay; // New overlay for quest type
    public TextMeshProUGUI clickCounterText; // ← перетащим сюда текст кликов из Canvas

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void UpdateClickCounter(int clicks)
    {
        if (clickCounterText != null)
        {
            clickCounterText.text = "Клики: " + clicks.ToString();
        }
        else
        {
            Debug.LogWarning("ClickCounterText не назначен в UIManager!");
        }
    }
    void Update()
    {
        levelText.text = "Level: " + MainGameManager.instance.level;
        // Прогресс по квестам и прочее
    }

    public void ShowQuestPanel(Quest quest)
    {
        questPanel.SetActive(true);
        questTitleText.text = quest.questTitle;
        questDescriptionText.text = quest.questDescription;
        questProgressText.text = $"0/{quest.clickRequirement}";
        questRewardText.text = $"Награда: {quest.GetRewardDescription(MainGameManager.instance.level)}";
        
        // Display quest type with appropriate icon/color
        if (questTypeText != null)
        {
            questTypeText.text = QuestTypeDisplay.GetDisplayText(quest.questType);
            questTypeText.color = QuestTypeDisplay.GetQuestTypeColor(quest.questType);
        }

        activeQuestTitleOverlay.text = quest.questTitle;
        activeQuestProgressOverlay.text = $"0/{quest.clickRequirement}";
        
        if (activeQuestTypeOverlay != null)
        {
            activeQuestTypeOverlay.text = QuestTypeDisplay.GetDisplayText(quest.questType);
            activeQuestTypeOverlay.color = QuestTypeDisplay.GetQuestTypeColor(quest.questType);
        }
    }

    public void UpdateQuestProgress(int completed, int total)
    {
        string text = $"{completed}/{total}";
        questProgressText.text = text;
        activeQuestProgressOverlay.text = text;
    }

    public void HideQuestPanel()
    {
        questPanel.SetActive(false);
        activeQuestTitleOverlay.text = "";
        activeQuestProgressOverlay.text = "";
        
        if (activeQuestTypeOverlay != null)
            activeQuestTypeOverlay.text = "";
    }
    
}
