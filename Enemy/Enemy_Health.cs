using System.Collections;
using UnityEngine;
using System;

public class Enemy_Health : MonoBehaviour
{
    [Header("Fallback Health")]
    [SerializeField] private int maxHealth = 40;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("References")]
    [SerializeField] private Enemy_Movement enemyMovement;
    [SerializeField] private Stats stats;

    [Header("Experience Reward")]
    [SerializeField] private int experienceReward = 25;

    [Header("Hit Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float hitFlashDuration = 0.08f;

    [Header("Gold Drop")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private int minGoldDrop = 1;
    [SerializeField] private int maxGoldDrop = 3;
    [SerializeField] private float goldDropRadius = 0.4f;

    private int currentHealth;
    private Coroutine flashRoutine;
    private Coroutine burnRoutine;
    private Coroutine freezeVisualRoutine;

    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;
    public event Action<Enemy_Health> OnEnemyDied;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (enemyMovement == null)
            enemyMovement = GetComponent<Enemy_Movement>();

        if (stats == null)
            stats = GetComponent<Stats>();

        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    public void ApplyBurn(int damagePerTick, float duration, float tickInterval)
    {
        if (IsDead)
            return;

        if (damagePerTick <= 0 || duration <= 0f || tickInterval <= 0f)
            return;

        if (burnRoutine != null)
            StopCoroutine(burnRoutine);

        burnRoutine = StartCoroutine(BurnRoutine(damagePerTick, duration, tickInterval));
    }

    private IEnumerator BurnRoutine(int damagePerTick, float duration, float tickInterval)
    {
        Debug.Log(name + " zostal podpalony.");

        float elapsed = 0f;

        while (elapsed < duration && !IsDead)
        {
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;

            if (!IsDead)
                TakeDamage(damagePerTick);
        }

        burnRoutine = null;
    }

    public void ApplyFreeze(float duration)
    {
        if (IsDead)
            return;

        if (enemyMovement != null)
            enemyMovement.ApplyFreeze(duration);

        if (spriteRenderer != null)
        {
            if (freezeVisualRoutine != null)
                StopCoroutine(freezeVisualRoutine);

            freezeVisualRoutine = StartCoroutine(FreezeVisualRoutine(duration));
        }
    }

    private IEnumerator FreezeVisualRoutine(float duration)
    {
        if (spriteRenderer == null)
            yield break;

        Color originalColor = spriteRenderer.color;

        spriteRenderer.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        freezeVisualRoutine = null;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (IsDead)
            return;

        int finalDamage = damage;

        if (stats != null)
            finalDamage = stats.CalculateDamageAfterArmor(damage);

        maxHealth = GetMaxHealth();
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(name + " dostał obrażenia: " + finalDamage + ". HP: " + currentHealth + "/" + maxHealth);

        if (spriteRenderer != null)
        {
            if (flashRoutine != null)
                StopCoroutine(flashRoutine);

            flashRoutine = StartCoroutine(HitFlash());
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (enemyMovement != null && attacker != null)
        {
            enemyMovement.ApplyKnockbackAndStun(attacker);
        }
    }

    private int GetMaxHealth()
    {
        return stats != null ? stats.MaxHealth : maxHealth;
    }

    private IEnumerator HitFlash()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(hitFlashDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private void GiveExperienceToPlayer()
    {
        if (experienceReward <= 0)
            return;

        Player_Experience playerExperience = FindFirstObjectByType<Player_Experience>();

        if (playerExperience != null)
            playerExperience.AddExperience(experienceReward);
    }

    private void DropGold()
    {
        if (goldPrefab == null)
            return;

        int goldAmount = new System.Random().Next(minGoldDrop, maxGoldDrop + 1);

        for (int i = 0; i < goldAmount; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * goldDropRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            Instantiate(goldPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void Die()
    {
        GiveExperienceToPlayer();

        GameRunStats.RegisterEnemyKilled();

        Debug.Log(name + " zginął.");

        OnEnemyDied?.Invoke(this);

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
