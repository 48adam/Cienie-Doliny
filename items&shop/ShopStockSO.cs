using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopStockEntry
{
    public ItemSO item;
    public int stock = -1; // -1 = infinite
    public bool useCustomPrice = false;
    public int customBuyPrice = 10;

    public int GetPrice()
    {
        if (item == null)
            return 0;

        return useCustomPrice ? customBuyPrice : item.buyPrice;
    }
}

[CreateAssetMenu(fileName = "New Shop Stock", menuName = "RPG/Shop/Shop Stock")]
public class ShopStockSO : ScriptableObject
{
    public string shopName = "Shop";
    public List<ShopStockEntry> items = new List<ShopStockEntry>();
}
