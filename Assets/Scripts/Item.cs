using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public Rarity rarity;
    public int[] statBonuses = new int[5];

    [HideInInspector]
    public string id; // можно оставить пустым или сгенерировать
}

public enum Rarity
{
    Common, Uncommon, Rare, Epic, Legendary
}
