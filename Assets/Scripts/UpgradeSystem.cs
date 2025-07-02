using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeSystem : MonoBehaviour
{
    public TextMeshProUGUI[] statValueTexts;
    public Button[] upgradeButtons;
    public TextMeshProUGUI upgradePointsText;
    public TextMeshProUGUI[] statNameTexts;
    public TextMeshProUGUI[] statBonusTexts;

    public static UpgradeSystem instance;

    private int upgradePoints = 0;
    private const int maxStatValue = 50;

    private Skills skills;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        skills = FindAnyObjectByType<Skills>();
    }

    void Start()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int index = i;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => UpgradeStat(index));
        }

        UpdateAllStats();
        UpdateUpgradeUI();
    }

    public void GainUpgradePoint()
    {
        upgradePoints++;
        UpdateUpgradeUI();
    }

    public void UpgradeStat(int index)
    {
        if (upgradePoints <= 0) return;

        int currentValue = GetSkillValue(index);
        if (currentValue >= maxStatValue) return;

        SetSkillValue(index, currentValue + 1);
        upgradePoints--;

        UpdateUpgradeUI();
    }

    public void UpdateAllStats()
    {
        for (int i = 0; i < statValueTexts.Length; i++)
        {
            int baseValue = GetSkillValue(i);
            statValueTexts[i].text = baseValue.ToString();
            statValueTexts[i].color = baseValue >= maxStatValue ? new Color(1f, 0.84f, 0f) : Color.white;

            int bonus = Player.instance != null ? Player.instance.GetItemBonus(i) : 0;
            if (bonus != 0)
            {
                statBonusTexts[i].text = $"({(bonus > 0 ? "+" : "")}{bonus})";
                statBonusTexts[i].color = bonus > 0 ? Color.green : Color.red;
            }
            else
            {
                statBonusTexts[i].text = "";
            }
        }
    }

    public void UpdateUpgradeUI()
    {
        bool showAnyButtons = upgradePoints > 0;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            bool show = showAnyButtons && GetSkillValue(i) < maxStatValue;
            upgradeButtons[i].gameObject.SetActive(show);
        }

        upgradePointsText.gameObject.SetActive(showAnyButtons);
        upgradePointsText.text = "Очки улучшения: " + upgradePoints;

        UpdateAllStats();
    }

    private int GetSkillValue(int index)
    {
        if (skills == null) return 0;

        return index switch
        {
            0 => skills.smithing,
            1 => skills.adventuring,
            2 => skills.endurance,
            3 => skills.gathering,
            4 => skills.combat,
            _ => 0
        };
    }

    private void SetSkillValue(int index, int value)
    {
        if (skills == null) return;

        switch (index)
        {
            case 0: skills.smithing = value; break;
            case 1: skills.adventuring = value; break;
            case 2: skills.endurance = value; break;
            case 3: skills.gathering = value; break;
            case 4: skills.combat = value; break;
        }
    }
}
