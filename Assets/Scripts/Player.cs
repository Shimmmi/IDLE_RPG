using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int[] stats = new int[5]; // stat1, stat2, ..., stat5
    public List<Item> equippedItems = new List<Item>();
    public static Player instance;
    public InventorySlot equippedSlot;
    public Image equippedItemImage;
    // New quest type animation frames
    public Sprite[] gatherAnimationFrames;
    public Sprite[] combatAnimationFrames;
    public Sprite[] exploreAnimationFrames;
    public Sprite[] craftAnimationFrames;
    public Sprite[] escortAnimationFrames;
    // Анимации
    public Sprite[] attackAnimationFrames;
    public Sprite[] idleAnimationFrames;
    public float animationSpeed = 0.1f;

    // Скорость анимаций (настраивается в инспекторе)
    [Tooltip("Скорость анимации клика (быстрая)")]
    public float attackAnimationSpeed = 0.05f;

    [Tooltip("Скорость IDLE-анимации (плавная)")]
    public float idleAnimationSpeed = 0.15f;
    private QuestType currentQuestType = QuestType.Gather;

    private Image playerImage;
    private bool isAnimating = false;
    private Coroutine idleAnimationCoroutine;
    private Coroutine currentAnimationCoroutine = null;
    
    public GameObject playerCube;
    private Animator cubeAnimator;
    
    // Quest-specific animation parameters for cube
    [Header("Cube Animation Settings")]
    [Tooltip("Animation triggers for different quest types")]
    public string idleAnimationTrigger = "Idle";
    public string gatherAnimationTrigger = "Gather";
    public string combatAnimationTrigger = "Combat";
    public string exploreAnimationTrigger = "Explore";
    public string craftAnimationTrigger = "Craft";
    public string escortAnimationTrigger = "Escort";
    
    private string currentCubeAnimation = "";
    private bool hasActiveQuest = false;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (equippedItemImage == null)
        {
            equippedItemImage = GameObject.Find("EquippedItemImage")?.GetComponent<Image>();
            if (equippedItemImage == null)
                Debug.LogWarning("Не найден объект EquippedItemImage в сцене");
        }

        playerImage = GameObject.Find("PlayerImage")?.GetComponent<Image>();
        if (playerImage == null)
            Debug.LogError("Не найден объект PlayerImage");

        for (int i = 0; i < stats.Length; i++)
            stats[i] = 1;

        playerCube = GameObject.Find("PlayerCube");
        if (playerCube != null)
        {
            cubeAnimator = playerCube.GetComponent<Animator>();
            if (cubeAnimator == null)
            {
                Debug.LogError("PlayerCube doesn't have an Animator component!");
            }
            else
            {
                // Start with idle animation
                SetCubeAnimation(idleAnimationTrigger, false);
            }
        }
        else
        {
            Debug.LogError("PlayerCube not found in the scene.");
        }

        StartIdleAnimation(); // Начинаем IDLE-анимацию
    }

    void Update()
    {
        // Check for mouse click or any key press
        if ((Input.GetMouseButtonDown(0) || Input.anyKeyDown) && !isAnimating)
        {
            TriggerRotateAnimation();
        }
    }

    void TriggerRotateAnimation()
    {
        if (cubeAnimator != null)
        {
            // Trigger click animation based on current quest type
            TriggerQuestSpecificClickAnimation();
        }
        else
        {
            Debug.LogWarning("Cube animator not assigned.");
        }
    }
    
    void TriggerQuestSpecificClickAnimation()
    {
        if (hasActiveQuest)
        {
            // Trigger quest-specific click animation
            switch (currentQuestType)
            {
                case QuestType.Gather:
                    cubeAnimator.SetTrigger("GatherClick");
                    break;
                case QuestType.Combat:
                    cubeAnimator.SetTrigger("CombatClick");
                    break;
                case QuestType.Explore:
                    cubeAnimator.SetTrigger("ExploreClick");
                    break;
                case QuestType.Craft:
                    cubeAnimator.SetTrigger("CraftClick");
                    break;
                case QuestType.Escort:
                    cubeAnimator.SetTrigger("EscortClick");
                    break;
            }
        }
        else
        {
            // Default click animation when no quest is active
            cubeAnimator.SetTrigger("IdleClick");
        }
    }

    public void SetCurrentQuestType(QuestType type)
    {
        currentQuestType = type;
        hasActiveQuest = true;
        
        // Change cube animation based on quest type
        SetCubeAnimationForQuestType(type);
        
        Debug.Log($"🎯 Player quest type set to: {type}");
    }
    public QuestType GetCurrentQuestType()
    {
        return currentQuestType;
    }
    
    public void ClearCurrentQuest()
    {
        hasActiveQuest = false;
        currentQuestType = QuestType.Gather; // Default fallback
        
        // Return to idle animation
        SetCubeAnimation(idleAnimationTrigger, false);
        
        Debug.Log("🏁 Player quest cleared, returned to idle state");
    }
    
    void SetCubeAnimationForQuestType(QuestType type)
    {
        switch (type)
        {
            case QuestType.Gather:
                SetCubeAnimation(gatherAnimationTrigger, true);
                break;
            case QuestType.Combat:
                SetCubeAnimation(combatAnimationTrigger, true);
                break;
            case QuestType.Explore:
                SetCubeAnimation(exploreAnimationTrigger, true);
                break;
            case QuestType.Craft:
                SetCubeAnimation(craftAnimationTrigger, true);
                break;
            case QuestType.Escort:
                SetCubeAnimation(escortAnimationTrigger, true);
                break;
        }
    }
    
    void SetCubeAnimation(string animationTrigger, bool isQuestActive)
    {
        if (cubeAnimator == null || string.IsNullOrEmpty(animationTrigger))
            return;
            
        // Don't trigger the same animation repeatedly
        if (currentCubeAnimation == animationTrigger)
            return;
            
        currentCubeAnimation = animationTrigger;
        
        // Set quest state parameter
        cubeAnimator.SetBool("HasActiveQuest", isQuestActive);
        
        // Trigger the animation
        cubeAnimator.SetTrigger(animationTrigger);
        
        Debug.Log($"🎬 Cube animation changed to: {animationTrigger} (Quest Active: {isQuestActive})");
    }

    public void PlayAttackAnimation()
    {
        if (attackAnimationFrames == null || attackAnimationFrames.Length == 0 || playerImage == null)
            return;

        if (isAnimating)
            return;

        StopIdleAnimation(); // Останавливаем IDLE, если она играет

        StartCoroutine(AnimateAttack(attackAnimationSpeed));
    }

    IEnumerator AnimateAttack(float speed)
    {
        isAnimating = true;

        foreach (Sprite frame in attackAnimationFrames)
        {
            playerImage.sprite = frame;
            yield return new WaitForSeconds(speed);
        }

        isAnimating = false;

        StartIdleAnimation(); // Возобновляем IDLE после анимации клика
    }

    public void StartIdleAnimation()
    {
        if (idleAnimationFrames == null || idleAnimationFrames.Length == 0 || playerImage == null)
            return;

        if (idleAnimationCoroutine != null)
            return;

        idleAnimationCoroutine = StartCoroutine(AnimateIdle(idleAnimationSpeed));
    }

    public void StopIdleAnimation()
    {
        if (idleAnimationCoroutine != null)
        {
            StopCoroutine(idleAnimationCoroutine);
            idleAnimationCoroutine = null;
        }
    }

    IEnumerator AnimateIdle(float speed)
    {
        int index = 0;

        while (true)
        {
            playerImage.sprite = idleAnimationFrames[index];
            index = (index + 1) % idleAnimationFrames.Length;
            yield return new WaitForSeconds(speed);
        }
    }
    public void PlayAnimationByQuestType(QuestType type)
    {
        StopCurrentAnimation(); // Останавливаем текущую анимацию

        switch (type)
        {
            case QuestType.Gather:
                currentAnimationCoroutine = StartCoroutine(Animate(gatherAnimationFrames, animationSpeed));
                break;
            case QuestType.Combat:
                currentAnimationCoroutine = StartCoroutine(Animate(combatAnimationFrames, animationSpeed));
                break;
            case QuestType.Explore:
                currentAnimationCoroutine = StartCoroutine(Animate(exploreAnimationFrames, animationSpeed));
                break;
            case QuestType.Craft:
                currentAnimationCoroutine = StartCoroutine(Animate(craftAnimationFrames, animationSpeed));
                break;
            case QuestType.Escort:
                currentAnimationCoroutine = StartCoroutine(Animate(escortAnimationFrames, animationSpeed));
                break;
        }
    }

    // Примеры методов анимаций (можно реализовать по аналогии с PlayAttackAnimation())
    IEnumerator Animate(Sprite[] frames, float speed)
    {
        foreach (Sprite frame in frames)
        {
            playerImage.sprite = frame;
            yield return new WaitForSeconds(speed);
        }
    }
    private void StopCurrentAnimation()
    {
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
            currentAnimationCoroutine = null;
        }
    }

    // Individual quest type animation methods
    public void PlayGatherAnimation()
    {
        if (gatherAnimationFrames == null || gatherAnimationFrames.Length == 0 || playerImage == null)
            return;

        StopCurrentAnimation();
        StartCoroutine(Animate(gatherAnimationFrames, animationSpeed));
    }

    public void PlayCombatAnimation()
    {
        if (combatAnimationFrames == null || combatAnimationFrames.Length == 0 || playerImage == null)
            return;

        StopCurrentAnimation();
        StartCoroutine(Animate(combatAnimationFrames, animationSpeed));
    }

    public void PlayExploreAnimation()
    {
        if (exploreAnimationFrames == null || exploreAnimationFrames.Length == 0 || playerImage == null)
            return;

        StopCurrentAnimation();
        StartCoroutine(Animate(exploreAnimationFrames, animationSpeed));
    }

    public void PlayCraftAnimation()
    {
        if (craftAnimationFrames == null || craftAnimationFrames.Length == 0 || playerImage == null)
            return;

        StopCurrentAnimation();
        StartCoroutine(Animate(craftAnimationFrames, animationSpeed));
    }

    public void PlayEscortAnimation()
    {
        if (escortAnimationFrames == null || escortAnimationFrames.Length == 0 || playerImage == null)
            return;

        StopCurrentAnimation();
        StartCoroutine(Animate(escortAnimationFrames, animationSpeed));
    }
    public void RecalculateStats()
    {
        for (int i = 0; i < stats.Length; i++)
            stats[i] = 1;

        foreach (Item item in equippedItems)
        {
            for (int i = 0; i < stats.Length; i++)
                stats[i] += item.statBonuses[i];
        }

        Debug.Log($"Характеристики обновлены: {string.Join(", ", stats)}");
    }

    public void EquipItem(Item item, InventorySlot slot)
    {
        if (equippedSlot != null && equippedSlot != slot)
        {
            equippedSlot.ForceUnequip(); // Снять текущий слот
        }

        equippedSlot = slot;

        if (!equippedItems.Contains(item))
            equippedItems.Add(item);

        // ✅ Установим иконку
        if (equippedItemImage != null && equippedItemImage.gameObject != null)
        {
            equippedItemImage.sprite = item.icon;
            equippedItemImage.color = Color.white;
            equippedItemImage.enabled = true;
        }
        else
        {
            Debug.LogWarning("equippedItemImage не назначен или уничтожен");
        }

        RecalculateStats();
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < stats.Length; i++)
            stats[i] -= item.statBonuses[i];

        equippedItems.Remove(item);
        equippedSlot = null;

        // ✅ Скрываем иконку
        if (equippedItemImage != null)
        {
            equippedItemImage.enabled = false;
            equippedItemImage.sprite = null;
        }

        RecalculateStats();

        Debug.Log("Предмет снят, характеристики возвращены");
    }

    public int GetItemBonus(int index)
    {
        if (equippedSlot == null || equippedSlot.thisItem == null)
            return 0;

        int bonus = equippedSlot.thisItem.statBonuses[index];
        return bonus;
    }
}
