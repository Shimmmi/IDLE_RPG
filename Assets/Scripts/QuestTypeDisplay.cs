using UnityEngine;

public static class QuestTypeDisplay
{
    /// <summary>
    /// Get localized display text for quest types with appropriate icons
    /// </summary>
    public static string GetDisplayText(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Gather:
                return "[G] Сбор ресурсов";
            case QuestType.Combat:
                return "[C] Сражения";
            case QuestType.Explore:
                return "[E] Исследования";
            case QuestType.Craft:
                return "[R] Ремесло";
            case QuestType.Escort:
                return "[S] Сопровождение";
            default:
                return "[?] Неизвестно";
        }
    }

    /// <summary>
    /// Get short display text without icons for limited space
    /// </summary>
    public static string GetShortDisplayText(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Gather:
                return "Сбор";
            case QuestType.Combat:
                return "Бой";
            case QuestType.Explore:
                return "Поиск";
            case QuestType.Craft:
                return "Крафт";
            case QuestType.Escort:
                return "Эскорт";
            default:
                return "???";
        }
    }

    /// <summary>
    /// Get color code for quest type (useful for UI theming)
    /// </summary>
    public static Color GetQuestTypeColor(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Gather:
                return new Color(0.4f, 0.8f, 0.2f); // Green
            case QuestType.Combat:
                return new Color(0.9f, 0.2f, 0.2f); // Red
            case QuestType.Explore:
                return new Color(0.2f, 0.5f, 0.9f); // Blue
            case QuestType.Craft:
                return new Color(0.8f, 0.6f, 0.1f); // Orange
            case QuestType.Escort:
                return new Color(0.7f, 0.3f, 0.8f); // Purple
            default:
                return Color.gray;
        }
    }

    /// <summary>
    /// Get quest type description for tooltips or help text
    /// </summary>
    public static string GetDescription(QuestType questType)
    {
        switch (questType)
        {
            case QuestType.Gather:
                return "Собирайте ресурсы, травы и материалы для крафта";
            case QuestType.Combat:
                return "Сражайтесь с врагами и побеждайте в битвах";
            case QuestType.Explore:
                return "Исследуйте новые области и находите скрытые сокровища";
            case QuestType.Craft:
                return "Создавайте предметы и улучшайте снаряжение";
            case QuestType.Escort:
                return "Сопровождайте и защищайте NPC в опасных путешествиях";
            default:
                return "Неизвестный тип квеста";
        }
    }
}
