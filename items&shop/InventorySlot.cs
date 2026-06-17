using System;

[Serializable]
public class InventorySlot
{
    public ItemSO item;
    public int amount;

    public ItemSO Item => item;
    public int Amount => amount;

    public bool IsEmpty => item == null || amount <= 0;

    public InventorySlot(ItemSO item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}
