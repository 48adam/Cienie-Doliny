using UnityEngine;

public class Enemy_combat : MonoBehaviour
{
    [SerializeField] private int damage = 10; // fallback, jeśli nie dodasz komponentu Stats
    [SerializeField] private Stats stats;

    private Collider2D hitboxCollider;
    private bool hasDealtDamage;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponentInParent<Stats>();

        hitboxCollider = GetComponent<Collider2D>();

        if (hitboxCollider != null)
        {
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;
        }
    }

    public void EnableHitbox()
    {
        hasDealtDamage = false;

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
            Debug.Log("Hitbox ataku WŁĄCZONY");
        }
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            Debug.Log("Hitbox ataku WYŁĄCZONY");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamagePlayer(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamagePlayer(collision);
    }

    private int GetDamage()
    {
        return stats != null ? stats.Damage : damage;
    }

    private void TryDamagePlayer(Collider2D collision)
    {
        if (hasDealtDamage)
            return;

        if (!collision.CompareTag("Player"))
            return;

        player_helf playerHealth = collision.GetComponent<player_helf>();

        if (playerHealth == null)
        {
            playerHealth = collision.GetComponentInParent<player_helf>();
        }

        NewMonoBehaviourScript playerMovement = collision.GetComponent<NewMonoBehaviourScript>();

        if (playerMovement == null)
        {
            playerMovement = collision.GetComponentInParent<NewMonoBehaviourScript>();
        }

        if (playerMovement != null && !playerMovement.CanBeHit)
        {
            Debug.Log("Gracz uniknął ataku albo jest już w knockbacku.");
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("Trafiono Playera, ale nie znaleziono skryptu player_helf!");
            return;
        }

        playerHealth.changeHelf(-GetDamage());

        if (playerMovement != null)
        {
            playerMovement.Knockback(transform.root);
        }

        hasDealtDamage = true;

        Debug.Log("Enemy trafił gracza, zadał obrażenia i odpalił knockback!");
    }
}
