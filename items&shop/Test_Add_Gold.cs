using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Add_Gold : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private int goldAmount = 50;
    [SerializeField] private Key testKey = Key.G;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (Keyboard.current == null || inventory == null)
            return;

        if (Keyboard.current[testKey].wasPressedThisFrame)
            inventory.AddGold(goldAmount);
    }
}
