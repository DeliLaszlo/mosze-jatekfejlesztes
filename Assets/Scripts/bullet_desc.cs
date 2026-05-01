using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    private Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Boss") && !collision.CompareTag("Projectile") && !collision.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }

    public void Setup(float setSpeed)
    {
        speed = setSpeed;
        
        // Frissítjük a sebességet az új irányba
        if (TryGetComponent<Rigidbody2D>(out rb))
        {
            rb.linearVelocity = transform.up * speed;
        }
    }

    void Start()
    {
        // Elindulunk abba az irányba, amerre a Spawner fordított minket
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            // A transform.up mindig a lövedék saját "teteje" felé mutat
            rb.linearVelocity = transform.up * speed;
        }

        Destroy(gameObject, lifetime);
    }
}