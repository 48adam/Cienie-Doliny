using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private TMP_Text goldText;

    [Header("Input")]
    [SerializeField] private bool allowKeyboardToggle = true;
    [SerializeField] private Key toggleKey = Key.I;
    [SerializeField] private Key closeKey = Key.Escape;
    [SerializeField] private bool showOnStart = false;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private InventorySlotUI[] slotUIs;
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (canvasGroup == null && inventoryPanel != null)
            canvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += RefreshInventory;
            playerInventory.OnGoldChanged += RefreshGold;
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshInventory;
            playerInventory.OnGoldChanged -= RefreshGold;
        }

        if (pauseGameWhenOpen && IsOpen)
            Game_Pause_Manager.ForceUnpause();
    }

    private void Start()
    {
        BuildSlots();
        SetInventoryVisible(showOnStart);
        RefreshInventory();

        if (playerInventory != null)
            RefreshGold(playerInventory.Gold);
    }

    private void Update()
    {
        if (!allowKeyboardToggle || Keyboard.current == null)
            return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
            ToggleInventory();

        if (IsOpen && Keyboard.current[closeKey].wasPressedThisFrame)
            HideInventory();
    }

    private void BuildSlots()
    {
        if (slotsParent == null || slotPrefab == null || playerInventory == null)
            return;

        for (int i = slotsParent.childCount - 1; i >= 0; i--)
            Destroy(slotsParent.GetChild(i).gameObject);

        slotUIs = new InventorySlotUI[playerInventory.MaxSlots];

        for (int i = 0; i < playerInventory.MaxSlots; i++)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotsParent);
            slotUI.Initialize(playerInventory);
            slotUIs[i] = slotUI;
        }
    }

    public void RefreshInventory()
    {
        if (playerInventory == null || slotUIs == null)
            return;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            InventorySlot slot = i < playerInventory.Slots.Count ? playerInventory.Slots[i] : null;
            slotUIs[i].SetSlot(slot);
        }
    }

    private void RefreshGold(int gold)
    {
        if (goldText != null)
            goldText.text = "Gold: " + gold;
    }

    public void ToggleInventory()
    {
        SetInventoryVisible(!IsOpen);
    }

    public void ShowInventory()
    {
        SetInventoryVisible(true);
    }

    public void HideInventory()
    {
        SetInventoryVisible(false);
    }

    public void SetInventoryVisible(bool visible)
    {
        IsOpen = visible;

        if (inventoryPanel == null)
            return;

        if (canvasGroup != null)
        {
            inventoryPanel.SetActive(true);
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        else
        {
            inventoryPanel.SetActive(visible);
        }

        if (pauseGameWhenOpen)
            Game_Pause_Manager.SetPaused(visible);
    }
}
