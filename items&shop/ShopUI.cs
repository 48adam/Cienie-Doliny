using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private ShopItemButtonUI shopItemPrefab;
    [SerializeField] private TMP_Text shopNameText;
    [SerializeField] private TMP_Text goldText;

    [Header("Input")]
    [SerializeField] private Key closeKey = Key.Escape;
    [SerializeField] private bool pauseGameWhenOpen = true;

    private ShopStockSO currentShopStock;
    private PlayerInventory currentInventory;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (canvasGroup == null && shopPanel != null)
            canvasGroup = shopPanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetShopVisible(false);
    }

    private void OnDisable()
    {
        UnsubscribeInventory();

        if (pauseGameWhenOpen && IsOpen)
            Game_Pause_Manager.ForceUnpause();
    }

    private void Update()
    {
        if (!IsOpen || Keyboard.current == null)
            return;

        if (Keyboard.current[closeKey].wasPressedThisFrame)
            CloseShop();
    }

    public void OpenShop(ShopStockSO shopStock, PlayerInventory inventory)
    {
        currentShopStock = shopStock;
        currentInventory = inventory;

        SubscribeInventory();
        BuildShopItems();
        RefreshGold();
        SetShopVisible(true);
    }

    public void CloseShop()
    {
        SetShopVisible(false);
        UnsubscribeInventory();
        currentShopStock = null;
        currentInventory = null;
    }

    private void BuildShopItems()
    {
        if (itemsParent == null || shopItemPrefab == null || currentShopStock == null)
            return;

        for (int i = itemsParent.childCount - 1; i >= 0; i--)
            Destroy(itemsParent.GetChild(i).gameObject);

        if (shopNameText != null)
            shopNameText.text = currentShopStock.shopName;

        foreach (ShopStockEntry entry in currentShopStock.items)
        {
            if (entry == null || entry.item == null)
                continue;

            ShopItemButtonUI itemUI = Instantiate(shopItemPrefab, itemsParent);
            itemUI.Initialize(entry, this);
        }
    }

    public bool TryBuyItem(ShopStockEntry entry)
    {
        if (entry == null || entry.item == null || currentInventory == null)
            return false;

        if (entry.stock == 0)
        {
            Debug.Log("Brak towaru w sklepie: " + entry.item.itemName);
            return false;
        }

        int price = entry.GetPrice();

        if (!currentInventory.SpendGold(price))
        {
            Debug.Log("Za mało golda na: " + entry.item.itemName);
            return false;
        }

        bool added = currentInventory.AddItem(entry.item, 1);

        if (!added)
        {
            currentInventory.AddGold(price);
            Debug.Log("Inventory pełne. Zakup anulowany.");
            return false;
        }

        if (entry.stock > 0)
            entry.stock--;

        BuildShopItems();
        RefreshGold();
        return true;
    }

    private void SubscribeInventory()
    {
        if (currentInventory != null)
            currentInventory.OnGoldChanged += OnGoldChanged;
    }

    private void UnsubscribeInventory()
    {
        if (currentInventory != null)
            currentInventory.OnGoldChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int gold)
    {
        RefreshGold();
    }

    private void RefreshGold()
    {
        if (goldText != null && currentInventory != null)
            goldText.text = "Gold: " + currentInventory.Gold;
    }

    private void SetShopVisible(bool visible)
    {
        IsOpen = visible;

        if (shopPanel == null)
            return;

        if (canvasGroup != null)
        {
            shopPanel.SetActive(true);
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        else
        {
            shopPanel.SetActive(visible);
        }

        if (pauseGameWhenOpen)
            Game_Pause_Manager.SetPaused(visible);
    }
}
