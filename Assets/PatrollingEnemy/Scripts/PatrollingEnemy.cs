using UnityEngine;
using System.Collections;

public class PatrollingEnemy : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float arriveDistance = 0.05f;

    [Header("Animation")]
    [SerializeField] private bool useHeroKnightAnimator = true;
    [SerializeField] private string movementParameter = "AnimState";
    [SerializeField] private int idleValue = 0;
    [SerializeField] private int movingValue = 1;
    [SerializeField] private bool flipSpriteRenderer = true;

    [Header("Attack")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string attackTriggerName = "Attack1";
    [SerializeField] private float attackDuration = 0.45f;
    [SerializeField] private bool destroyPlayerOnAttack = true;

    [Header("Death")]
    [SerializeField] private string deathTriggerName = "Death";

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float leftBoundX;
    private float rightBoundX;
    private int moveDirection = 1;
    private bool isAttacking;
    private bool isDead;
    private Coroutine attackRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (pointA == null || pointB == null)
        {
            Debug.LogWarning($"{name}: Assign both patrol points (pointA and pointB).", this);
            enabled = false;
            return;
        }

        leftBoundX = Mathf.Min(pointA.position.x, pointB.position.x);
        rightBoundX = Mathf.Max(pointA.position.x, pointB.position.x);

        float startX = rb != null ? rb.position.x : transform.position.x;
        moveDirection = startX <= leftBoundX ? 1 : -1;

        UpdateFacingDirection();
        UpdateAnimationParameters(isMoving: moveSpeed > 0f);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (isAttacking)
        {
            UpdateAnimationParameters(isMoving: false);
            return;
        }

        if (moveSpeed <= 0f)
        {
            UpdateAnimationParameters(isMoving: false);
            return;
        }

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        int previousDirection = moveDirection;
        float nextX = currentPosition.x + (moveDirection * moveSpeed * Time.fixedDeltaTime);

        if (nextX >= rightBoundX)
        {
            nextX = rightBoundX;
            moveDirection = -1;
        }
        else if (nextX <= leftBoundX)
        {
            nextX = leftBoundX;
            moveDirection = 1;
        }

        Vector2 nextPosition = new Vector2(nextX, currentPosition.y);

        if (rb != null)
        {
            rb.MovePosition(nextPosition);
        }
        else
        {
            transform.position = nextPosition;
        }

        if (previousDirection != moveDirection)
        {
            UpdateFacingDirection();
        }

        UpdateAnimationParameters(isMoving: true);
    }

    public void HandleAttackRangeTrigger(Collider2D other)
    {
        if (other == null || isAttacking || isDead)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        float targetX = other.transform.position.x;
        int attackDirection = targetX >= transform.position.x ? 1 : -1;

        if (attackDirection != moveDirection)
        {
            moveDirection = attackDirection;
            UpdateFacingDirection();
        }

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(AttackRoutine(other.gameObject));
    }

    private IEnumerator AttackRoutine(GameObject target)
    {
        isAttacking = true;
        UpdateAnimationParameters(isMoving: false);

        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        if (destroyPlayerOnAttack && target != null)
        {
            Destroy(target);
        }

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        attackRoutine = null;
    }

    public void DieFromStomp()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        isAttacking = false;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleAttackRangeTrigger(other);
    }

    private void UpdateFacingDirection()
    {
        if (!flipSpriteRenderer || spriteRenderer == null)
        {
            return;
        }

        
        spriteRenderer.flipX = moveDirection < 0;
    }

    private void UpdateAnimationParameters(bool isMoving)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetInteger(movementParameter, isMoving ? movingValue : idleValue);

        if (useHeroKnightAnimator)
        {
            
            animator.SetBool("Grounded", true);
            animator.SetBool("WallSlide", false);
            animator.SetFloat("AirSpeedY", 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawSphere(pointA.position, 0.08f);
        Gizmos.DrawSphere(pointB.position, 0.08f);
    }
}
