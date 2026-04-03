using UnityEngine;
using System.Collections;

public class MageEnemyController : MonoBehaviour
{
    public enum MageState { Shielded, Attacking, Vulnerable, Teleporting, Dead }
    public MageState currentState;

    public int maxHealth = 3;
    private int currentHealth;

    public bool isInvulnerable = true;
    
    public float timeBetweenAttacks = 3f;
    public float vulnerableDuration = 4f;
    private float timer = 0f;

    public Transform[] teleportPoints;
    private int currentPointIndex = -1;

    public Color damageTeleportOutColor = Color.red;
    private Color normalTeleportOutColor;
    private Color normalTeleportInColor;

    public ParticleSystem shieldParticles;
    public ParticleSystem slamParticles;
    public ParticleSystem teleportOutParticles;
    public ParticleSystem teleportInParticles;

    [SerializeField] private BoxCollider2D slamHitbox;
    private const string PlayerTag = "Player";
    
    public Transform rootTransform;
    public Renderer spriteRenderer;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (spriteRenderer == null) spriteRenderer = GetComponent<Renderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<Renderer>();
        if (slamHitbox == null) slamHitbox = GetComponentInChildren<BoxCollider2D>();
        if (slamHitbox != null) slamHitbox.isTrigger = true;

        if (teleportOutParticles != null)
        {
            normalTeleportOutColor = teleportOutParticles.main.startColor.color;
        }
        if (teleportInParticles != null)
        {
            normalTeleportInColor = teleportInParticles.main.startColor.color;
        }

        if (teleportPoints != null && teleportPoints.Length > 0)
        {
            float closestDistance = float.MaxValue;
            Transform myTransform = rootTransform != null ? rootTransform : transform;

            for (int i = 0; i < teleportPoints.Length; i++)
            {
                float dist = Vector2.Distance(myTransform.position, teleportPoints[i].position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    currentPointIndex = i;
                }
            }
        }

        EnterShieldedState();
    }

    void Update()
    {
        switch (currentState)
        {
            case MageState.Shielded:
                timer += Time.deltaTime;
                if (timer >= timeBetweenAttacks)
                {
                    StartAttack();
                }
                break;

            case MageState.Attacking:
                break;

            case MageState.Vulnerable:
                timer += Time.deltaTime;
                if (timer >= vulnerableDuration)
                {
                    StartCoroutine(HandleTeleportSequence(normalTeleportOutColor, normalTeleportInColor));
                }
                break;
                
            case MageState.Teleporting:
                break;
            case MageState.Dead:
                break;
        }
    }

    void EnterShieldedState()
    {
        currentState = MageState.Shielded;
        isInvulnerable = true;
        timer = 0f;

        if (shieldParticles != null) shieldParticles.gameObject.SetActive(true);
    }

    void StartAttack()
    {
        currentState = MageState.Attacking;
        anim.SetTrigger("Attack");
    }

    public void OnStaffHitGround()
    {
        // #TODO: Add audio (staff slam impact SFX).
        if (slamParticles != null) slamParticles.Play();
        ApplySlamDamage();
    }

    public void OnAttackFinished()
    {
        EnterVulnerableState();
    }

    void EnterVulnerableState()
    {
        currentState = MageState.Vulnerable;
        isInvulnerable = false;
        timer = 0f;

        if (shieldParticles != null) shieldParticles.gameObject.SetActive(false);
    }

    void SetParticleColor(ParticleSystem ps, Color newColor)
    {
        if (ps == null) return;
        
        ParticleSystem[] systems = ps.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem sys in systems)
        {
            ParticleSystem.MainModule main = sys.main;
            main.startColor = newColor;
        }
    }

    IEnumerator HandleTeleportSequence(Color outColor, Color inColor)
    {
        currentState = MageState.Teleporting;
        isInvulnerable = true;

        if (teleportOutParticles != null)
        {
            SetParticleColor(teleportOutParticles, outColor);
            teleportOutParticles.transform.position = rootTransform != null ? rootTransform.position : transform.position;
            // #TODO: Add audio (teleport out whoosh SFX).
            teleportOutParticles.Play();
        }
        
        Collider2D mainBodyCollider = GetComponentInParent<Collider2D>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (mainBodyCollider != null) mainBodyCollider.enabled = false;
        
        yield return new WaitForSeconds(0.5f);

        FindNewTeleportPosition();

        if (teleportInParticles != null)
        {
            teleportInParticles.transform.position = rootTransform != null ? rootTransform.position : transform.position;
            // #TODO: Add audio (teleport in materialize SFX).
            teleportInParticles.Play();
        }

        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (mainBodyCollider != null) mainBodyCollider.enabled = true;

        EnterShieldedState();
    }

    void ApplySlamDamage()
    {
        if (slamHitbox == null || !slamHitbox.enabled)
        {
            return;
        }

        Vector2 worldCenter = slamHitbox.transform.TransformPoint(slamHitbox.offset);
        Vector3 lossyScale = slamHitbox.transform.lossyScale;
        Vector2 worldSize = new Vector2(
            slamHitbox.size.x * Mathf.Abs(lossyScale.x),
            slamHitbox.size.y * Mathf.Abs(lossyScale.y)
        );
        float worldAngle = slamHitbox.transform.eulerAngles.z;

        Collider2D[] hitResults = Physics2D.OverlapBoxAll(worldCenter, worldSize, worldAngle);
        int hitCount = hitResults.Length;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hitResults[i];
            if (hit == null)
            {
                continue;
            }

            PlayerHealthManager health = hit.GetComponentInParent<PlayerHealthManager>();
            if (health == null || health.IsDead)
            {
                continue;
            }

            if (!health.CompareTag(PlayerTag))
            {
                continue;
            }

            health.TakeDamage();
            break;
        }
    }

    void FindNewTeleportPosition()
    {
        if (teleportPoints == null || teleportPoints.Length <= 1) return;

        int newRandomIndex = Random.Range(0, teleportPoints.Length);
        while (newRandomIndex == currentPointIndex)
        {
            newRandomIndex = Random.Range(0, teleportPoints.Length);
        }

        currentPointIndex = newRandomIndex;

        if (rootTransform != null)
        {
            rootTransform.position = teleportPoints[currentPointIndex].position;
        }
        else
        {
            transform.position = teleportPoints[currentPointIndex].position;
        }
    }

    public void HandleStomp(GameObject playerObject)
    {
        if (currentState == MageState.Dead) return;

        if (currentState == MageState.Vulnerable)
        {
            if (currentHealth <= 1)
            {
                currentHealth = 0;
                Die();
            }
            else
            {
                currentHealth--;
                StopAllCoroutines();
                StartCoroutine(HandleTeleportSequence(damageTeleportOutColor, normalTeleportInColor));
            }
        }
        else
        {
            StartCoroutine(PunishPlayerSequence(playerObject));
        }
    }

    private IEnumerator PunishPlayerSequence(GameObject playerObject)
    {

        PlayerHealthManager playerHealth = playerObject.GetComponent<PlayerHealthManager>();
        Transform playerRoot = playerHealth != null ? playerHealth.transform : playerObject.transform;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage();
        }

        if (teleportOutParticles != null)
        {
            SetParticleColor(teleportOutParticles, normalTeleportOutColor);
            teleportOutParticles.transform.position = playerRoot.position;
            // #TODO: Add audio (punish teleport out SFX).
            teleportOutParticles.Play();
        }

        yield return new WaitForSeconds(0.1f);

        if (teleportPoints != null && teleportPoints.Length > 1)
        {
            int randomIndex = Random.Range(0, teleportPoints.Length);
            while (randomIndex == currentPointIndex)
            {
                randomIndex = Random.Range(0, teleportPoints.Length);
            }

            Vector3 newPos = teleportPoints[randomIndex].position;
            playerRoot.position = newPos;

            Rigidbody2D playerRb = playerRoot.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero; 
            }

            if (teleportInParticles != null)
            {
                teleportInParticles.transform.position = newPos;
                // #TODO: Add audio (punish teleport in SFX).
                teleportInParticles.Play();
            }
        }
    }

    private void Die()
    {
        currentState = MageState.Dead;
        isInvulnerable = true;

        StopAllCoroutines(); 

        // #TODO: Add audio (mage death SFX).
        if (anim != null) anim.SetTrigger("Die");

        if (slamHitbox != null) slamHitbox.enabled = false;
        if (shieldParticles != null) shieldParticles.gameObject.SetActive(false);
        
        Collider2D mainBodyCollider = GetComponentInParent<Collider2D>();
        if (mainBodyCollider != null)
        {
            mainBodyCollider.enabled = false;
        }

        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; 
        }
    }

}
