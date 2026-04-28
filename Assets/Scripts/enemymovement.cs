using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 range = new Vector2(10f, 10f); // Kisebb értékre vettem a láthatóság kedvéért
    private Vector2 targetPosition;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetNewTarget();
    }

    void Update()
    {
        // Mozgás a célpont felé
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // --- IRÁNYBA FORDULÁS ---
        // Ha a célpont tőlünk jobbra van, ne legyen flip (vagy fordítva, sprite-tól függően)
        if (targetPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Jobbra néz
        }
        else if (targetPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Balra néz
        }
        // -------------------------

        // Animáció bekapcsolása (mivel folyamatosan megy)
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        // Ha elértük a célpontot, újabbat választunk
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTarget();
        }
    }

    void SetNewTarget()
    {
        float randomX = Random.Range(-range.x / 2, range.x / 2);
        float randomY = Random.Range(-range.y / 2, range.y / 2);
        targetPosition = new Vector2(randomX, randomY);
    }
}