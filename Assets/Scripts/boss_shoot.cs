using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossSpawner : MonoBehaviour
{
    public int hp = 12;
    private int startHP; 

    [Header("Lövedék Beállítások")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;
    public float fireRate = 1.2f;
    public int bulletCount = 3; 

    [Header("Boss reSpawn")]
    public SpriteRenderer bossSprite; 
    public float teleportRange = 5f;  
    private BossSequence movementScript;

    [Header("Audio")]
    [SerializeField] private AudioSource fireballShootAudioPrefab;

    private AudioSource fireballShootAudioInstance;
    private GameObject playerTarget;

    void Start()
    {
        startHP = hp;
        movementScript = GetComponent<BossSequence>(); 

        if (fireballShootAudioPrefab != null)
        {
            fireballShootAudioInstance = Instantiate(fireballShootAudioPrefab, transform);
        }

        playerTarget = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(BossLogicRoutine());
    }

    IEnumerator BossLogicRoutine()
    {
        while (true)
        {
            if (movementScript != null && movementScript.IsMoving())
            {
                ShootFan();
                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                yield return null; 
            }
        }
    }

    void ShootFan()
    {
        if (bulletPrefab == null) return;

        if (fireballShootAudioInstance != null) fireballShootAudioInstance.Play();

        Vector2 directionToPlayer = playerTarget.transform.position - transform.position;
        float centerAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;

        int lostHP = startHP - hp;
        int currentBulletCount = bulletCount + (lostHP / 4) * 2;
        float angleStep = (currentBulletCount >= 7) ? 20f : 30f;
        if (currentBulletCount <= 3) angleStep = 45f;

        float totalSpread = angleStep * (currentBulletCount - 1);
        float startAngle = centerAngle - (totalSpread / 2f);

        for (int i = 0; i < currentBulletCount; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);

            if (bullet.TryGetComponent<Collider2D>(out Collider2D bulletCol))
            {
                Physics2D.IgnoreCollision(bulletCol, GetComponent<Collider2D>());
            }
            if (bullet.TryGetComponent(out SimpleBullet sBullet))
            {
                sBullet.Setup(bulletSpeed);
            }
        }
    }

    private bool isInvulnerable = false;
    private bool isDead = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInvulnerable)
        {
            TakeDamage(1);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        Debug.Log("Boss HP: " + hp);

        if (hp > 0)
        {
            StartCoroutine(HitRoutine());
        }
        else
        {
            isDead = true;
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        if (TryGetComponent<Collider2D>(out Collider2D col))
        {
            col.enabled = false;
        }

        Debug.Log("Boss legyőzve!");

        yield return new WaitForEndOfFrame();

        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayMusicForScene("Outro");
        }

        SceneManager.LoadScene("Outro"); 
    }

    IEnumerator HitRoutine()
    {
        isInvulnerable = true;

        for (int i = 0; i < 3; i++)
        {
            bossSprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            bossSprite.enabled = true;  
            yield return new WaitForSeconds(0.1f);
        }
        TeleportToSafeDistance();
        isInvulnerable = false;
    }

    void TeleportToSafeDistance()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 playerPos = player.transform.position;
        Vector2 newPos;
        float distance;

        int attempts = 0;
        do
        {
            newPos = new Vector2(
                Random.Range(playerPos.x - teleportRange, playerPos.x + teleportRange),
                Random.Range(playerPos.y - teleportRange, playerPos.y + teleportRange)
            );
            distance = Vector2.Distance(newPos, playerPos);
            attempts++;
            if (attempts > 100) break; 
            
        } while (distance < 3f);

        transform.position = newPos;
    }
}

