using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    Material,
    Quest,
    Currency
}

public enum ItemEffectType
{
    None,
    Heal,
    MaxHealth,
    Damage,
    MoveSpeed,
    Armor,
    Gold
}

[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName = "New Item";
    [TextArea(2, 5)] public string description;
    public Sprite icon;
    public ItemType itemType = ItemType.Consumable;

    [Header("Stacking")]
    public bool canStack = true;
    public int maxStackAmount = 99;

    [Header("Shop")]
    public int buyPrice = 10;
    public int sellPrice = 5;

    [Header("Use Effect")]
    public bool canUse = true;
    public bool consumeOnUse = true;
    public ItemEffectType effectType = ItemEffectType.None;
    public int effectAmount = 0;
}
