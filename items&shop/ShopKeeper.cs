using UnityEngine;
using UnityEngine.InputSystem;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private ShopStockSO shopStock;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private Key interactKey = Key.E;

    private bool playerInRange;
    private PlayerInventory playerInventory;

    private void Awake()
    {
        if (shopUI == null)
            shopUI = FindFirstObjectByType<ShopUI>();
    }

    private void Update()
    {
        if (!playerInRange || Keyboard.current == null)
            return;

        if (Keyboard.current[interactKey].wasPressedThisFrame)
        {
            if (shopUI != null && playerInventory != null)
                shopUI.OpenShop(shopStock, playerInventory);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = true;
        playerInventory = collision.GetComponent<PlayerInventory>();

        if (playerInventory == null)
            playerInventory = collision.GetComponentInParent<PlayerInventory>();

        Debug.Log("Naciśnij E, aby otworzyć sklep.");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;
        playerInventory = null;

        if (shopUI != null && shopUI.IsOpen)
            shopUI.CloseShop();
    }
}
