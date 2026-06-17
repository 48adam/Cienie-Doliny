using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text stockText;
    [SerializeField] private Button buyButton;

    private ShopStockEntry entry;
    private ShopUI shopUI;

    public void Initialize(ShopStockEntry entry, ShopUI shopUI)
    {
        this.entry = entry;
        this.shopUI = shopUI;

        if (buyButton == null)
            buyButton = GetComponent<Button>();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(Buy);
            buyButton.onClick.AddListener(Buy);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (entry == null || entry.item == null)
            return;

        if (iconImage != null)
        {
            iconImage.sprite = entry.item.icon;
            iconImage.enabled = entry.item.icon != null;
        }

        if (nameText != null)
            nameText.text = entry.item.itemName;

        if (descriptionText != null)
            descriptionText.text = entry.item.description;

        if (priceText != null)
            priceText.text = "Price: " + entry.GetPrice();

        if (stockText != null)
            stockText.text = entry.stock < 0 ? "Stock: ∞" : "Stock: " + entry.stock;

        if (buyButton != null)
            buyButton.interactable = entry.stock != 0;
    }

    private void Buy()
    {
        if (shopUI == null || entry == null)
            return;

        shopUI.TryBuyItem(entry);
        RefreshUI();
    }
}
