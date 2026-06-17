using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int maxSlots = 24;
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

    [Header("Gold")]
    [SerializeField] private int gold = 0;

    [Header("References")]
    [SerializeField] private Stats stats;
    [SerializeField] private player_helf playerHealth;

    public event Action OnInventoryChanged;
    public event Action<int> OnGoldChanged;

    public IReadOnlyList<InventorySlot> Slots => slots;
    public int Gold => gold;
    public int MaxSlots => maxSlots;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<Stats>();

        if (playerHealth == null)
            playerHealth = GetComponent<player_helf>();

        while (slots.Count < maxSlots)
            slots.Add(new InventorySlot(null, 0));
    }

    private void Start()
    {
        OnInventoryChanged?.Invoke();
        OnGoldChanged?.Invoke(gold);
    }

    public bool AddItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;

        if (item.canStack)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot = slots[i];

                if (slot.item == item && slot.amount < item.maxStackAmount)
                {
                    int freeSpace = item.maxStackAmount - slot.amount;
                    int amountToAdd = Mathf.Min(freeSpace, remaining);
                    slot.amount += amountToAdd;
                    remaining -= amountToAdd;

                    if (remaining <= 0)
                    {
                        NotifyInventoryChanged();
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];

            if (slot.IsEmpty)
            {
                int amountToAdd = item.canStack ? Mathf.Min(item.maxStackAmount, remaining) : 1;
                slot.item = item;
                slot.amount = amountToAdd;
                remaining -= amountToAdd;

                if (remaining <= 0)
                {
                    NotifyInventoryChanged();
                    return true;
                }
            }
        }

        NotifyInventoryChanged();
        return false;
    }

    public bool RemoveItem(ItemSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        if (GetItemAmount(item) < amount)
            return false;

        int remaining = amount;

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];

            if (slot.item != item)
                continue;

            int removeAmount = Mathf.Min(slot.amount, remaining);
            slot.amount -= removeAmount;
            remaining -= removeAmount;

            if (slot.amount <= 0)
                slot.Clear();

            if (remaining <= 0)
            {
                NotifyInventoryChanged();
                return true;
            }
        }

        NotifyInventoryChanged();
        return true;
    }

    public int GetItemAmount(ItemSO item)
    {
        if (item == null)
            return 0;

        int total = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
                total += slots[i].amount;
        }

        return total;
    }

    public bool UseItem(ItemSO item)
    {
        if (item == null || !item.canUse)
            return false;

        if (GetItemAmount(item) <= 0)
            return false;

        ApplyItemEffect(item);

        if (item.consumeOnUse)
            RemoveItem(item, 1);
        else
            NotifyInventoryChanged();

        return true;
    }

    private void ApplyItemEffect(ItemSO item)
    {
        switch (item.effectType)
        {
            case ItemEffectType.Heal:
                if (playerHealth != null)
                    playerHealth.changeHelf(item.effectAmount);
                break;

            case ItemEffectType.MaxHealth:
                if (stats != null)
                    stats.AddModifier(StatType.MaxHealth, item.effectAmount);
                break;

            case ItemEffectType.Damage:
                if (stats != null)
                    stats.AddModifier(StatType.Damage, item.effectAmount);
                break;

            case ItemEffectType.MoveSpeed:
                if (stats != null)
                    stats.AddModifier(StatType.MoveSpeed, item.effectAmount);
                break;

            case ItemEffectType.Armor:
                if (stats != null)
                    stats.AddModifier(StatType.Armor, item.effectAmount);
                break;

            case ItemEffectType.Gold:
                AddGold(item.effectAmount);
                break;
        }
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount < 0)
            return false;

        if (gold < amount)
            return false;

        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    private void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}
