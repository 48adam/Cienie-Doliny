using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    [SerializeField] private int goldAmount = 1;
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

        inventory.AddGold(goldAmount);

        Debug.Log("Podniesiono gold: " + goldAmount);

        if (destroyAfterPickup)
            Destroy(gameObject);
    }
}