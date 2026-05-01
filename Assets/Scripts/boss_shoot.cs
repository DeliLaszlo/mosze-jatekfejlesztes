using UnityEngine;
using System.Collections;

public class BossSpawner : MonoBehaviour
{
    [Header("Lövedék Beállítások")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 3.5f;
    public float fireRate = 0.5f;
    public int bulletCount = 3; // Ez az alapérték, amit az Inspectorban állíthatsz

    [Header("Boss Állapot")]
    public int hp = 12;
    private int startHP; // Eltároljuk a kezdő HP-t a matekhoz

    [Header("Boss Speciális")]
    public SpriteRenderer bossSprite; // Húzd be ide a Boss SpriteRendererét
    public float teleportRange = 5f;  // Milyen messzire ugorhat maximum
    private BossSequence movementScript; // Hivatkozás a mozgás scriptre

    void Start()
    {
        startHP = hp;
        movementScript = GetComponent<BossSequence>(); // Megkeressük a mozgást
        StartCoroutine(BossLogicRoutine());
    }

    IEnumerator BossLogicRoutine()
    {
        while (true)
        {
            // Csak akkor tüzel, ha a movementScript szerint mozgásban vagyunk
            if (movementScript != null && movementScript.IsMoving())
            {
                ShootFan();
                // FireRate-enként próbál újra tüzelni
                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                // Ha épp várakozik (HandleWait), akkor a kód megáll itt egy pillanatra, 
                // majd a következő képkockánál újra ellenőrzi az IsMoving-ot.
                yield return null; 
            }
        }
    }

void ShootFan()
{
    if (bulletPrefab == null) return;

    // 1. Megkeressük a Player-t
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null) return;

    // 2. Kiszámoljuk az irányt a Player felé
    Vector2 directionToPlayer = player.transform.position - transform.position;
    
    // Kiszámoljuk a szöget fokban (Atan2 radiánt ad, amit átszámolunk)
    // A -90f azért kell, mert a Unity-ben a 0 fok jobbra van, 
    // de a lövedék "up" iránya (teteje) fog előre nézni.
    float centerAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;

    // 3. Lövedékszám számítása (maradt az eredeti matek)
    int lostHP = startHP - hp;
    int currentBulletCount = bulletCount + (lostHP / 4) * 2;

    // 4. Szögtávolság (maradt az eredeti)
    float angleStep = (currentBulletCount >= 7) ? 20f : 30f;
    if (currentBulletCount <= 3) angleStep = 45f;

    float totalSpread = angleStep * (currentBulletCount - 1);
    
    // 5. A startAngle most már a Player felé néző centerAngle-ből indul ki
    float startAngle = centerAngle - (totalSpread / 2f);

    for (int i = 0; i < currentBulletCount; i++)
    {
        float currentAngle = startAngle + (i * angleStep);
        Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
        
        // Ütközés tiltása a Boss-szal
        if (bullet.TryGetComponent<Collider2D>(out Collider2D bulletCol))
        {
            Physics2D.IgnoreCollision(bulletCol, GetComponent<Collider2D>());
        }

        // Sebesség beállítása (ha a SimpleBullet script fent van)
        if (bullet.TryGetComponent(out SimpleBullet sBullet))
        {
            sBullet.Setup(bulletSpeed);
        }
    }
}


    private bool isInvulnerable = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInvulnerable)
        {
            TakeDamage(1);
            //Destroy(gameObject);
        }
    }
        public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Boss HP: " + hp);

        if (hp > 0)
        {
            StartCoroutine(HitRoutine());
        }
        else
        {
            Debug.Log("Boss legyőzve!");
            Destroy(gameObject);
        }
    }

    IEnumerator HitRoutine()
    {
        isInvulnerable = true;

        // 1. VILLOGÁS (3-szor eltűnik és megjelenik)
        for (int i = 0; i < 3; i++)
        {
            bossSprite.enabled = false; // Eltűnik
            yield return new WaitForSeconds(0.1f);
            bossSprite.enabled = true;  // Megjelenik
            yield return new WaitForSeconds(0.1f);
        }

        // 2. TELEPORTÁLÁS
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

        // Addig generálunk új pozíciót, amíg az legalább 3 egységre nincs a playertől
        int attempts = 0;
        do
        {
            // Random pozíció a Boss környezetében
            newPos = new Vector2(
                Random.Range(playerPos.x - teleportRange, playerPos.x + teleportRange),
                Random.Range(playerPos.y - teleportRange, playerPos.y + teleportRange)
            );
            distance = Vector2.Distance(newPos, playerPos);
            attempts++;
            
            // Biztonsági fék, hogy ne fagyjon le a játék ha nincs hely
            if (attempts > 100) break; 
            
        } while (distance < 3f);

        transform.position = newPos;
    }
}
