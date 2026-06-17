using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool destroyAfterPickup = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

        if (inventory == null)
            inventory = collision.GetComponentInParent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("Player nie ma PlayerInventory.");
            return;
        }

        bool added = inventory.AddItem(item, amount);

        if (added)
        {
            Debug.Log("Podniesiono item: " + item.itemName + " x" + amount);

            if (destroyAfterPickup)
                Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory jest pełne. Nie można podnieść: " + item.itemName);
        }
    }
}
