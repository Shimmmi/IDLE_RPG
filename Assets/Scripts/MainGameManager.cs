using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager instance;

    public int clickCount = 0;
    public int totalClicks = 0;
    public int level = 1;
    public int itemClickRequirement = 5;
    public int levelClickRequirement = 10;

    public TextMeshProUGUI clickCountText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;

    public UpgradeSystem upgradeSystem;
    public InventorySystem inventorySystem;
    public Skills skills;

    public Button upgradeButton;
    public Button inventoryButton;
    public Button getItemButton;
    public Button getLevelButton;
    public Button getQuestButton;
    public Button debugAddPointButton;

    public Toggle testModeToggle;

    public Image upgradeButtonImage;
    public Image inventoryButtonImage;

    public GameObject questPanel;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questProgressText;
    public TextMeshProUGUI questRewardText;
    public TextMeshProUGUI questTypeText; // New field for quest type display
    public TextMeshProUGUI activeQuestTitleOverlay;
    public TextMeshProUGUI activeQuestProgressOverlay;
    public TextMeshProUGUI activeQuestTypeOverlay; // New overlay for quest type


    public bool isReforgeMode = false;
    public bool timerDisabled = false;
    private bool isCooldown { get { return !timerDisabled && cooldownTimer > 0f; } }
    private bool isTestMode { get { return testModeToggle != null && testModeToggle.isOn; } }

    public float cooldownDuration = 5f;
    private float cooldownTimer = 0f;

    public GameObject debugPanel;
    public Button debugAddClicksButton;
    public Button debugMaxStatsButton;
    public Button debugAddItemsButton;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (skills == null)
            skills = FindAnyObjectByType<Skills>();
    }

    void Start()
    {
        UpdateClickCounter();
        UpdateLevelText();

        TabManager.instance.HideAllTabs();

        getItemButton.gameObject.SetActive(false);
        getLevelButton.gameObject.SetActive(false);
        getQuestButton.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);

        if (testModeToggle != null)
        {
            testModeToggle.onValueChanged.AddListener(OnTestModeToggled);
            OnTestModeToggled(testModeToggle.isOn);
        }

        // Initialize QuestManager if it doesn't exist
        InitializeQuestManager();

        if (QuestDatabase.instance != null)
            Debug.Log("✅ QuestDatabase загружен");
    }

    void Update()
    {
        if (debugAddPointButton != null)
            debugAddPointButton.gameObject.SetActive(isTestMode);

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            HandlePlayerClick();

        if (isCooldown && !timerDisabled && timerText != null)
            UpdateCooldown();
    }

    void HandlePlayerClick()
    {
        clickCount++;
        totalClicks++;
        UpdateClickCounter();
        UpdateGetButtonsVisibility();

        if (!isCooldown)
        {
            getItemButton.gameObject.SetActive(clickCount >= itemClickRequirement);
            getLevelButton.gameObject.SetActive(clickCount >= levelClickRequirement);
        }

        if (QuestManager.instance != null && QuestManager.instance.HasActiveQuest)
        {
            QuestManager.instance.RegisterClick();
        }

        if (Player.instance != null)
        {
            // 💥 Теперь анимация зависит от текущего типа квеста
            Player.instance.PlayAnimationByQuestType(Player.instance.GetCurrentQuestType());

            if (PlayerEquipmentUI.instance != null && PlayerEquipmentUI.instance.equippedIcon != null)
                PlayerEquipmentUI.instance.RotateEquippedIcon();
        }
    }

    void UpdateCooldown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            timerText.gameObject.SetActive(false);
            cooldownTimer = 0f;
            UpdateGetButtonsVisibility();
        }
        else
        {
            timerText.text = FormatTime(cooldownTimer);
        }
    }

    void UpdateClickCounter()
    {
        if (clickCountText != null)
            clickCountText.text = "Клики: " + clickCount;
    }

    void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
    }

    public bool SpendClicks(int amount)
    {
        if (clickCount < amount) return false;

        clickCount -= amount;
        UpdateClickCounter();
        return true;
    }

    public void OnUpgradeButtonClicked()
    {
        if (!isCooldown)
            TabManager.instance.ShowUpgrade();
    }

    public void OnInventoryButtonClicked()
    {
        if (!isCooldown)
            TabManager.instance.ShowInventory();
    }

    string FormatTime(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        return $"{min:00}:{sec:00}";
    }

    public void TryGetItem()
    {
        if (isCooldown || !SpendClicks(itemClickRequirement)) return;

        inventorySystem.AddRandomItem();
        TabManager.instance.ShowInventory();

        StartCooldown();
    }

    public void TryGetLevel()
    {
        if (isCooldown || !SpendClicks(levelClickRequirement)) return;

        level++;
        UpdateLevelText();

        upgradeSystem.GainUpgradePoint();
        UpdateSkillUI();
        TabManager.instance.ShowUpgrade();

        StartCooldown();
    }

    public void TryGetQuest()
    {
        Debug.Log("🔍 TryGetQuest called");
        
        Debug.Log($"🔍 Current state - isCooldown: {isCooldown}, QuestManager.instance: {QuestManager.instance}, level: {level}");
        
        if (QuestManager.instance == null)
        {
            Debug.LogError("❌ QuestManager.instance is null!");
            return;
        }
        
        if (isCooldown)
        {
            Debug.Log("❌ Cannot get quest - on cooldown");
            return;
        }
        
        if (QuestManager.instance.HasActiveQuest)
        {
            Debug.Log("❌ Cannot get quest - already has active quest");
            return;
        }

        // Load QuestDatabase from Resources if not already loaded
        if (QuestDatabase.instance == null)
        {
            Debug.Log("🔍 Loading QuestDatabase from Resources...");
            QuestDatabase questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
            if (questDatabase == null)
            {
                Debug.LogError("❌ QuestDatabase not found in Resources folder. Make sure QuestDatabase.asset exists in Assets/Resources/");
                return;
            }
            
            // This will trigger OnEnable and set the instance
            QuestDatabase.instance = questDatabase;
            questDatabase.OnEnable();
            Debug.Log("✅ QuestDatabase loaded from Resources");
        }
        else
        {
            Debug.Log("✅ QuestDatabase already loaded");
        }

        Debug.Log($"🔍 Calling QuestManager.TryStartQuest with level: {level}");
        QuestManager.instance.TryStartQuest(level);
        
        Debug.Log("🔍 Starting cooldown...");
        StartCooldown();
        
        Debug.Log("✅ TryGetQuest completed");
    }

    void StartCooldown()
    {
        if (timerDisabled) return;

        cooldownTimer = cooldownDuration;
        timerText.gameObject.SetActive(true);
        UpdateGetButtonsVisibility();
    }

    void UpdateGetButtonsVisibility()
    {
        bool canClick = !isCooldown;

        getItemButton.gameObject.SetActive(canClick && clickCount >= itemClickRequirement);
        getLevelButton.gameObject.SetActive(canClick && clickCount >= levelClickRequirement);
        getQuestButton.gameObject.SetActive(true);
    }

    public void OnTestModeToggled(bool isOn)
    {
        timerDisabled = isOn;

        if (timerDisabled)
        {
            cooldownTimer = 0f;
            timerText.gameObject.SetActive(false);
        }

        debugPanel?.SetActive(isOn);
        debugAddPointButton?.gameObject.SetActive(isOn);
        debugAddClicksButton?.gameObject.SetActive(isOn);
        debugMaxStatsButton?.gameObject.SetActive(isOn);
        debugAddItemsButton?.gameObject.SetActive(isOn);

        UpdateGetButtonsVisibility();
    }

    public void AddDebugClicks()
    {
        if (!isTestMode) return;

        clickCount += 1000;
        totalClicks += 1000;
        UpdateClickCounter();
        UpdateGetButtonsVisibility();
    }

    public void MaxAllStats()
    {
        if (!isTestMode) return;

        for (int i = 0; i < Player.instance.stats.Length; i++)
            Player.instance.stats[i] = 50;

        skills.smithing = 50;
        skills.adventuring = 50;
        skills.endurance = 50;
        skills.gathering = 50;
        skills.combat = 50;

        upgradeSystem.UpdateUpgradeUI();
        Debug.Log("Все параметры установлены на 50");
    }

    public void AddDebugItems()
    {
        if (!isTestMode) return;

        for (int i = 0; i < 100; i++)
            inventorySystem.AddRandomItem();

        TabManager.instance.ShowInventory();
    }

    public void UpdateSkillUI()
    {
        upgradeSystem?.UpdateAllStats();
    }

    void InitializeQuestManager()
    {
        if (QuestManager.instance == null)
        {
            Debug.Log("🔍 QuestManager.instance is null, creating new QuestManager...");
            
            // Try to find existing QuestManager in scene
            QuestManager existingQM = FindAnyObjectByType<QuestManager>();
            if (existingQM != null)
            {
                Debug.Log("✅ Found existing QuestManager in scene");
                QuestManager.instance = existingQM;
            }
            else
            {
                // Create new GameObject with QuestManager component
                GameObject questManagerGO = new GameObject("QuestManager");
                QuestManager questManager = questManagerGO.AddComponent<QuestManager>();
                Debug.Log("✅ Created new QuestManager GameObject");
            }
        }
        else
        {
            Debug.Log("✅ QuestManager.instance already exists");
        }
    }
}
