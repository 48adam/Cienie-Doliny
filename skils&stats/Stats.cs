using System;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    Damage,
    MoveSpeed,
    Armor
}

public class Stats : MonoBehaviour
{
    [Header("Core Stats")]
    public Stat maxHealth = new Stat(100);
    public Stat damage = new Stat(10);
    public Stat moveSpeed = new Stat(5);

    [Header("Combat Stats")]
    public Stat armor = new Stat(0);

    public event Action OnStatsChanged;

    public int MaxHealth => Mathf.Max(1, maxHealth.GetValue());
    public int Damage => Mathf.Max(0, damage.GetValue());
    public float MoveSpeed => Mathf.Max(0f, moveSpeed.GetValue());
    public int Armor => Mathf.Max(0, armor.GetValue());

    public int CalculateDamageAfterArmor(int incomingDamage)
    {
        return Mathf.Max(1, incomingDamage - Armor);
    }

    public int GetStatValue(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                return MaxHealth;
            case StatType.Damage:
                return Damage;
            case StatType.MoveSpeed:
                return Mathf.RoundToInt(MoveSpeed);
            case StatType.Armor:
                return Armor;
            default:
                return 0;
        }
    }

    public void SetBaseValue(StatType statType, int value)
    {
        GetStat(statType).SetBaseValue(value);
        NotifyStatsChanged();
    }

    public void AddModifier(StatType statType, int modifier)
    {
        GetStat(statType).AddModifier(modifier);
        NotifyStatsChanged();
    }

    public void RemoveModifier(StatType statType, int modifier)
    {
        GetStat(statType).RemoveModifier(modifier);
        NotifyStatsChanged();
    }

    public void ClearModifiers(StatType statType)
    {
        GetStat(statType).ClearModifiers();
        NotifyStatsChanged();
    }

    public void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }

    private Stat GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                return maxHealth;
            case StatType.Damage:
                return damage;
            case StatType.MoveSpeed:
                return moveSpeed;
            case StatType.Armor:
                return armor;
            default:
                return maxHealth;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        OnStatsChanged?.Invoke();
    }
#endif
}
