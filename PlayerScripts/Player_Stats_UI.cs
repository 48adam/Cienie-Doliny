using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player_Stats_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private Stats playerStats;
    [SerializeField] private player_helf playerHealth;

    [Header("Text Fields")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text maxHealthText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text armorText;

    [Header("Input")]
    [SerializeField] private bool allowKeyboardToggle = true;
    [SerializeField] private bool showOnStart = false;
    [SerializeField] private Key toggleKey = Key.C;
    [SerializeField] private Key closeKey = Key.Escape;

    [Header("Pause")]
    [SerializeField] private bool pauseGameWhenPanelOpen = true;

    private void Awake()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<Stats>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<player_helf>();
    }

    private void OnEnable()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged += UpdateUI;

        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateUI;

        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthUI;

        // Jeśli ten obiekt zostanie wyłączony podczas pauzy, zawsze przywracamy grę.
        if (pauseGameWhenPanelOpen)
            Game_Pause_Manager.ForceUnpause();
    }

    private void Start()
    {
        SetStatsPanelVisible(showOnStart);
        UpdateUI();
    }

    private void Update()
    {
        if (!allowKeyboardToggle || Keyboard.current == null)
            return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            ToggleStatsPanel();
        }

        if (statsPanel != null && statsPanel.activeSelf && Keyboard.current[closeKey].wasPressedThisFrame)
        {
            HideStatsPanel();
        }
    }

    public void ToggleStatsPanel()
    {
        if (statsPanel == null)
            return;

        SetStatsPanelVisible(!statsPanel.activeSelf);
    }

    public void ShowStatsPanel()
    {
        SetStatsPanelVisible(true);
    }

    public void HideStatsPanel()
    {
        SetStatsPanelVisible(false);
    }

    public void SetStatsPanelVisible(bool visible)
    {
        if (statsPanel == null)
            return;

        statsPanel.SetActive(visible);

        if (visible)
            UpdateUI();

        if (pauseGameWhenPanelOpen)
            Game_Pause_Manager.SetPaused(visible);
    }

    public void UpdateUI()
    {
        if (playerStats != null)
        {
            SetText(maxHealthText, "Max Health: " + playerStats.MaxHealth);
            SetText(damageText, "Damage: " + playerStats.Damage);
            SetText(moveSpeedText, "Move Speed: " + playerStats.MoveSpeed);
            SetText(armorText, "Armor: " + playerStats.Armor);
        }

        if (playerHealth != null)
        {
            UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        SetText(healthText, "Health: " + currentHealth + "/" + maxHealth);
    }

    private void SetText(TMP_Text textField, string value)
    {
        if (textField != null)
            textField.text = value;
    }
}
