using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Animation")]
    [SerializeField] private string takeDamageTriggerName = "takeDamage";
    [SerializeField] private string deathBoolName = "isDead";

    [Header("Death Behaviour")]
    [SerializeField] private bool disableControlsOnDeath = true;
    [SerializeField] private bool disableCollidersOnDeath = true;
    [SerializeField] private bool disablePhysicsOnDeath = true;

    private Animator animator;
    private Rigidbody2D rb;
    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - 1);

        if (animator != null && !string.IsNullOrEmpty(takeDamageTriggerName))
        {
            animator.SetTrigger(takeDamageTriggerName);
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (animator != null && !string.IsNullOrEmpty(deathBoolName))
        {
            animator.SetBool(deathBoolName, true);
        }

        if (disableControlsOnDeath)
        {
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }

        if (disableCollidersOnDeath)
        {
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        }

        if (disablePhysicsOnDeath && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
    }
}
