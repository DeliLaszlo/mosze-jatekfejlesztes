using UnityEngine;
using System;
using System.Collections;

public class PlayerHealthManager : MonoBehaviour
{
    private static bool hasPersistedHealth;
    private static int persistedHealth;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Animation")]
    [SerializeField] private string takeDamageTriggerName = "takeDamage";
    [SerializeField] private string deathBoolName = "isDead";

    [Header("Death Behaviour")]
    [SerializeField] private bool disableControlsOnDeath = true;
    [SerializeField] private bool disableCollidersOnDeath = true;
    [SerializeField] private bool disablePhysicsOnDeath = true;

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject resetPromptUI;

    private Animator animator;
    private Rigidbody2D rb;
    private int currentHealth;
    private bool isDead;

    public event Action<int, int> HealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    public static void OverrideNextSpawnHealth(int health)
    {
        hasPersistedHealth = true;
        persistedHealth = Mathf.Max(0, health);
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        maxHealth = Mathf.Max(1, maxHealth);

        if (hasPersistedHealth)
        {
            currentHealth = Mathf.Clamp(persistedHealth, 0, maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }

        PersistHealth();
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            HandleDeath();
            TriggerGameOver();
        }
    }

    public void TakeDamage()
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - 1);
        PersistHealth();
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null && !string.IsNullOrEmpty(takeDamageTriggerName))
        {
            // #TODO: Add audio (player hurt SFX).
            animator.SetTrigger(takeDamageTriggerName);
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
            TriggerGameOver();
        }
    }

    public void SimulateDeath()
    {
        if (isDead)
        {
            return;
        }

        HandleDeath();

        StartCoroutine(ShowResetPromptDelayed());

        if (currentHealth == 1)
        {
            TriggerGameOver();
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
            // #TODO: Add audio (player death SFX).
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

    private void PersistHealth()
    {
        hasPersistedHealth = true;
        persistedHealth = currentHealth;
    }

    private void TriggerGameOver()
    {
        ResetManager resetManager = FindAnyObjectByType<ResetManager>();
        if (resetManager != null)
        {
            resetManager.enabled = false;
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    private IEnumerator ShowResetPromptDelayed()
    {
        yield return new WaitForSeconds(1f);

        if (resetPromptUI != null)
        {
            resetPromptUI.SetActive(true);
        }
    }
}
