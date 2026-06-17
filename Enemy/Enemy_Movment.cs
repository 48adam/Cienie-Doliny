using System.Collections;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 20f;
    [SerializeField] private Enemy_combat enemyCombat;
    [SerializeField] private Stats stats;

    [Header("Knockback / Stun")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.15f;
    [SerializeField] private float stunDuration = 0.35f;

    private int facingDirection = 1;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    private EnemyState currentState;

    private bool isAttacking = false;
    private bool isKnockedBack = false;
    private bool isStunned = false;
    private float nextAttackTime = 0f;
    private Vector2 knockbackDirection;
    private Coroutine hitReactionRoutine;
    private Coroutine freezeRoutine;

    public bool IsStunned => isStunned;
    public bool IsKnockedBack => isKnockedBack;
    private bool isFrozen = false;

    public bool IsFrozen => isFrozen;
    public bool CanMove => !isAttacking && !isKnockedBack && !isStunned && !isFrozen;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (stats == null)
            stats = GetComponent<Stats>();

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (Game_Pause_Manager.IsPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (isStunned || isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            return;
        }

        if (isKnockedBack)
        {
            rb.linearVelocity = knockbackDirection * knockbackForce;
            ChangeState(EnemyState.Idle);
            return;
        }

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            FacePlayer();
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            FacePlayer();

            if (Time.time >= nextAttackTime)
            {
                StartAttack();
            }
            else
            {
                ChangeState(EnemyState.Idle);
            }

            return;
        }

        ChangeState(EnemyState.Chasing);
        ChasePlayer();
    }

    public void ApplyFreeze(float duration)
    {
        if (duration <= 0f)
            return;

        if (freezeRoutine != null)
            StopCoroutine(freezeRoutine);

        freezeRoutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        isAttacking = false;

        if (enemyCombat != null)
            enemyCombat.DisableHitbox();

        rb.linearVelocity = Vector2.zero;
        ChangeState(EnemyState.Idle);

        Debug.Log(name + " zostal zamrozony na " + duration + " sekund.");

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        freezeRoutine = null;
    }

    private void ChasePlayer()
    {
        if (!CanMove)
            return;

        FacePlayer();

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * GetMoveSpeed();
    }

    private float GetMoveSpeed()
    {
        return stats != null ? stats.MoveSpeed : speed;
    }

    private void StartAttack()
    {
        if (!CanMove)
            return;

        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = Vector2.zero;

        // Atak odpalamy triggerem, nie boolem.
        anim.SetTrigger("Attack");

        Debug.Log("Start ataku. Następny atak możliwy za: " + attackCooldown + " sekund");
    }

    public void EnableAttackHitbox()
    {
        if (isStunned || isKnockedBack)
            return;

        if (enemyCombat != null)
        {
            enemyCombat.EnableHitbox();
        }
        else
        {
            Debug.LogWarning("EnemyCombat nie jest przypisany w Enemy_Movement!");
        }
    }

    public void DisableAttackHitbox()
    {
        if (enemyCombat != null)
        {
            enemyCombat.DisableHitbox();
        }
    }

    public void AttackAnimationFinished()
    {
        isAttacking = false;

        if (enemyCombat != null)
        {
            enemyCombat.DisableHitbox();
        }

        Debug.Log("Koniec animacji ataku");
    }

    public void ApplyKnockbackAndStun(Transform attacker)
    {
        if (attacker == null)
            return;

        Vector2 direction = (transform.position - attacker.position).normalized;

        if (direction == Vector2.zero)
            direction = facingDirection == 1 ? Vector2.right : Vector2.left;

        ApplyKnockbackAndStun(direction);
    }

    public void ApplyKnockbackAndStun(Vector2 direction)
    {
        if (direction == Vector2.zero)
            direction = Vector2.right;

        knockbackDirection = direction.normalized;

        if (hitReactionRoutine != null)
            StopCoroutine(hitReactionRoutine);

        hitReactionRoutine = StartCoroutine(HitReactionRoutine());
    }

    private IEnumerator HitReactionRoutine()
    {
        // Trafienie przerywa atak przeciwnika.
        isAttacking = false;

        if (enemyCombat != null)
            enemyCombat.DisableHitbox();

        isKnockedBack = true;
        isStunned = true;
        rb.linearVelocity = knockbackDirection * knockbackForce;
        ChangeState(EnemyState.Idle);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        hitReactionRoutine = null;
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        if (player.position.x > transform.position.x && facingDirection == -1)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDirection *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void StartChasing(Transform target)
    {
        player = target;

        if (CanMove)
        {
            ChangeState(EnemyState.Chasing);
        }
    }

    public void StopChasing()
    {
        player = null;
        isAttacking = false;

        rb.linearVelocity = Vector2.zero;

        if (enemyCombat != null)
        {
            enemyCombat.DisableHitbox();
        }

        ChangeState(EnemyState.Idle);
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsChasing", false);

        switch (currentState)
        {
            case EnemyState.Idle:
                anim.SetBool("IsIdle", true);
                break;

            case EnemyState.Chasing:
                anim.SetBool("IsChasing", true);
                break;
        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing
}
