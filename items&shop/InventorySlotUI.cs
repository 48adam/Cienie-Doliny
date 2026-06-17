using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Auto / Manual References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button useButton;

    private PlayerInventory inventory;
    private InventorySlot currentSlot;

    private void Awake()
    {
        AutoBindReferences();
        ClearSlot();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AutoBindReferences();
    }
#endif

    public void Initialize(PlayerInventory inventory)
    {
        this.inventory = inventory;

        AutoBindReferences();

        if (useButton != null)
        {
            useButton.onClick.RemoveListener(UseCurrentItem);
            useButton.onClick.AddListener(UseCurrentItem);
        }

        ClearSlot();
    }

    private void AutoBindReferences()
    {
        if (useButton == null)
            useButton = GetComponent<Button>();

        Image[] images = GetComponentsInChildren<Image>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        if (iconImage == null)
        {
            foreach (Image image in images)
            {
                string objectName = image.gameObject.name.ToLower();

                if (objectName.Contains("icon") || objectName.Contains("item"))
                {
                    iconImage = image;
                    break;
                }
            }
        }

        if (iconImage == null && images.Length > 1)
        {
            foreach (Image image in images)
            {
                if (image.gameObject != gameObject)
                {
                    iconImage = image;
                    break;
                }
            }
        }

        if (amountText == null)
        {
            foreach (TMP_Text text in texts)
            {
                string objectName = text.gameObject.name.ToLower();

                if (objectName.Contains("amount") || objectName.Contains("count") || objectName.Contains("ilosc") || objectName.Contains("ilość"))
                {
                    amountText = text;
                    break;
                }
            }
        }

        if (nameText == null)
        {
            foreach (TMP_Text text in texts)
            {
                string objectName = text.gameObject.name.ToLower();

                if (objectName.Contains("name") || objectName.Contains("nazwa"))
                {
                    nameText = text;
                    break;
                }
            }
        }
    }

    public void SetSlot(InventorySlot slot)
    {
        currentSlot = slot;

        if (slot == null || slot.IsEmpty || slot.item == null)
        {
            ClearSlot();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.enabled = slot.item.icon != null;
            iconImage.preserveAspect = true;

            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.localScale = Vector3.one;

                iconRect.sizeDelta = new Vector2(32f, 32f);
            }
        }

        if (amountText != null)
        {
            amountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
        }

        if (nameText != null)
        {
            nameText.text = slot.item.itemName;
        }

        if (useButton != null)
        {
            useButton.interactable = slot.item.canUse;
        }
    }

    private void ClearSlot()
    {
        currentSlot = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;

            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.localScale = Vector3.one;
            }
        }

        if (amountText != null)
        {
            amountText.text = "";
        }

        if (nameText != null)
        {
            nameText.text = "";
        }

        if (useButton != null)
        {
            useButton.interactable = false;
        }
    }
    private void UseCurrentItem()
    {
        if (inventory == null || currentSlot == null || currentSlot.IsEmpty)
            return;

        inventory.UseItem(currentSlot.item);
    }
}
