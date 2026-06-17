using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f; // fallback, jeśli nie dodasz komponentu Stats
    public Rigidbody2D rb;
    public Animator animator;
    [SerializeField] private Stats stats;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 0.5f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private Vector2 input;
    private Vector2 lastMoveDirection = Vector2.right;
    private Vector2 dodgeDirection;
    private Vector2 knockbackDirection;

    public int facingDirection = 1;

    private bool isKnockedBack = false;
    private bool isDodging = false;
    private bool canDodge = true;

    private bool externalMovementLocked = false;

    public bool IsDodging => isDodging;
    public bool IsKnockedBack => isKnockedBack;
    public bool IsMovementLocked => externalMovementLocked;
    public bool CanBeHit => !isDodging && !isKnockedBack;
    public bool CanStartAction => !isDodging && !isKnockedBack && !externalMovementLocked;
    public Vector2 LastMoveDirection => lastMoveDirection;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<Stats>();
    }

    private void Update()
    {
        if (Game_Pause_Manager.IsPaused)
        {
            animator.SetFloat("horizontal", 0f);
            animator.SetFloat("vertical", 0f);
            return;
        }

        if (isKnockedBack || isDodging || externalMovementLocked)
        {
            input = Vector2.zero;

            if (animator != null)
            {
                animator.SetFloat("horizontal", 0f);
                animator.SetFloat("vertical", 0f);
            }

            return;
        }

        ReadMovementInput();

        if (input != Vector2.zero)
        {
            lastMoveDirection = input.normalized;
        }

        if ((input.x > 0 && transform.localScale.x < 0) ||
            (input.x < 0 && transform.localScale.x > 0))
        {
            FlipTool();
        }

        if (animator != null)
        {
            animator.SetFloat("horizontal", Mathf.Abs(input.x));
            animator.SetFloat("vertical", Mathf.Abs(input.y));
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && canDodge)
        {
            StartCoroutine(Dodge());
        }
    }

    private void FixedUpdate()
    {
        if (Game_Pause_Manager.IsPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDodging)
        {
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
            return;
        }

        if (isKnockedBack)
        {
            rb.linearVelocity = knockbackDirection * knockbackForce;
            return;
        }

        if (externalMovementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = input.normalized * GetMoveSpeed();
    }

    private float GetMoveSpeed()
    {
        return stats != null ? stats.MoveSpeed : speed;
    }

    private void ReadMovementInput()
    {
        input = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            input.x = -1;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            input.x = 1;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            input.y = 1;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            input.y = -1;
    }

    private IEnumerator Dodge()
    {
        if (externalMovementLocked)
            yield break;

        canDodge = false;
        isDodging = true;

        dodgeDirection = input != Vector2.zero ? input.normalized : lastMoveDirection;

        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dodgeCooldown);

        canDodge = true;
    }

    private void FlipTool()
    {
        facingDirection *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void SetExternalMovementLock(bool locked)
    {
        externalMovementLocked = locked;

        if (locked && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Knockback(Transform enemy)
    {
        if (!CanBeHit)
            return;

        SetExternalMovementLock(false);
        knockbackDirection = (transform.position - enemy.position).normalized;

        StopCoroutine(nameof(KnockbackRoutine));
        StartCoroutine(KnockbackRoutine());
    }

    public void Knokback(Transform enemy)
    {
        Knockback(enemy);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }
}
