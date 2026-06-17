using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Attack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NewMonoBehaviourScript playerMovement;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Stats stats;

    [Header("Attack Settings")]
    [SerializeField] private int damage = 20; // fallback, jeśli nie dodasz komponentu Stats
    [SerializeField] private float attackCooldown = 0.45f;
    [SerializeField] private float attackDuration = 0.25f;
    [SerializeField] private float attackRange = 0.7f;
    [SerializeField] private float attackPointDistance = 0.8f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Single Target Settings")]
    [SerializeField] private float attackAngle = 90f;

    [SerializeField] private PlayerMagicStats magicStats;

    private bool isAttacking = false;
    private float nextAttackTime = 0f;
    private readonly HashSet<Enemy_Health> enemiesHitThisAttack = new HashSet<Enemy_Health>();

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerMovement == null)
            playerMovement = GetComponent<NewMonoBehaviourScript>();

        if (stats == null)
            stats = GetComponent<Stats>();

        if (magicStats == null) 
            magicStats = GetComponent<PlayerMagicStats>();
    }

    private void Update()
    {
        if (Game_Pause_Manager.IsPaused)
            return;

        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (isAttacking)
            return;

        if (Time.time < nextAttackTime)
            return;

        if (playerMovement != null && !playerMovement.CanStartAction)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        enemiesHitThisAttack.Clear();
        nextAttackTime = Time.time + attackCooldown;

        if (playerMovement != null)
            playerMovement.SetExternalMovementLock(true);

        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration);

        FinishAttack();
    }

    private void ApplyMagicEffects(Enemy_Health enemy)
    {
        if (enemy == null || magicStats == null)
            return;

        if (magicStats.HasFire)
        {
            enemy.ApplyBurn(
                magicStats.FireDamagePerTick,
                magicStats.FireDuration,
                magicStats.FireTickInterval
            );
        }

        if (magicStats.HasFrost)
        {
            enemy.ApplyFreeze(magicStats.FrostDuration);
        }
    }
    public void AttackHitFrame()
    {
        if (!isAttacking)
            return;

        Vector2 center = GetAttackCenter();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(center, attackRange, enemyLayers);

        Enemy_Health closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        Vector2 attackDirection = GetAttackDirection();

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy_Health enemyHealth = enemyCollider.GetComponent<Enemy_Health>();

            if (enemyHealth == null)
                enemyHealth = enemyCollider.GetComponentInParent<Enemy_Health>();

            if (enemyHealth == null)
                continue;

            if (enemyHealth.IsDead)
                continue;

            if (enemiesHitThisAttack.Contains(enemyHealth))
                continue;

            Vector2 directionToEnemy =
                ((Vector2)enemyHealth.transform.position - (Vector2)transform.position).normalized;

            float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

            if (angleToEnemy > attackAngle / 2f)
                continue;

            float distance = Vector2.Distance(center, enemyHealth.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyHealth;
            }
        }

        if (closestEnemy != null)
        {
            Debug.Log("Trafiony przeciwnik: " + closestEnemy.name);

            closestEnemy.TakeDamage(GetDamage(), transform);
            enemiesHitThisAttack.Add(closestEnemy);
        }
    }

    private Vector2 GetAttackDirection()
    {
        if (attackPoint != null)
        {
            Vector2 direction = attackPoint.position - transform.position;

            if (direction.sqrMagnitude > 0.01f)
                return direction.normalized;
        }

        return Vector2.right;
    }

    public void FinishAttack()
    {
        if (!isAttacking)
            return;

        isAttacking = false;

        if (playerMovement != null)
            playerMovement.SetExternalMovementLock(false);
    }

    private int GetDamage()
    {
        return stats != null ? stats.Damage : damage;
    }

    private Vector2 GetAttackCenter()
    {
        if (attackPoint != null)
            return attackPoint.position;

        Vector2 direction = Vector2.right;

        if (playerMovement != null)
            direction = playerMovement.LastMoveDirection;
        else
            direction = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;

        if (direction == Vector2.zero)
            direction = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;

        return (Vector2)transform.position + direction.normalized * attackPointDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(GetAttackCenter(), attackRange);
    }
}
