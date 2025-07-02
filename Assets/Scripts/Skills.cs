using UnityEngine;

public class Skills : MonoBehaviour
{
    public static Skills instance;

    [Range(0, 50)] public int smithing = 1;
    [Range(0, 50)] public int adventuring = 1;
    [Range(0, 50)] public int endurance = 1;
    [Range(0, 50)] public int gathering = 1;
    [Range(0, 50)] public int combat = 1;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public int GetSmithingLevel()
    {
        return smithing;
    }
    // ✅ Методы для перековки
    public float GetSmithingBonusChance()
    {
        return Mathf.Min(smithing * 0.5f, 25f);
    }

    public int GetSmithingRequiredItemCount()
    {
        return Mathf.Max(10 - smithing / 10, 5);
    }

    // ✅ Можно добавить другие методы: GetMagicCooldown(), GetCombatCritChance() и т.д.
}
