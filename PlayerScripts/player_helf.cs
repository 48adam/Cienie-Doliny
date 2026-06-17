using System;
using UnityEngine;
using TMPro;

public class player_helf : MonoBehaviour
{
    [Header("Fallback Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("References")]
    public TMP_Text healthText;
    [SerializeField] private Stats stats;

    public event Action<int, int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event System.Action OnPlayerDied;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<Stats>();
    }

    private void OnEnable()
    {
        if (stats != null)
            stats.OnStatsChanged += RefreshMaxHealthFromStats;
    }

    private void OnDisable()
    {
        if (stats != null)
            stats.OnStatsChanged -= RefreshMaxHealthFromStats;
    }

    private void Start()
    {
        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;
        UpdateHealthText();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void changeHelf(int amount)
    {
        if (amount < 0 && stats != null)
        {
            int incomingDamage = Mathf.Abs(amount);
            int finalDamage = stats.CalculateDamageAfterArmor(incomingDamage);
            amount = -finalDamage;
        }

        currentHealth += amount;
        maxHealth = GetMaxHealth();

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthText();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            OnPlayerDied?.Invoke();

            gameObject.SetActive(false);
            return;
        }

        UpdateHealthText();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void RefreshMaxHealthFromStats()
    {
        int oldMaxHealth = maxHealth;
        maxHealth = GetMaxHealth();

        // Jeśli max HP wzrosło, dodajemy różnicę do obecnego HP.
        // Dzięki temu upgrade +20 Max Health od razu daje +20 aktualnego HP.
        if (maxHealth > oldMaxHealth)
        {
            currentHealth += maxHealth - oldMaxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthText();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private int GetMaxHealth()
    {
        return stats != null ? stats.MaxHealth : maxHealth;
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth.ToString() + "/" + maxHealth.ToString();
        }
    }
}
