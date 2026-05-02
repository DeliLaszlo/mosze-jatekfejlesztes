
using UnityEngine;

public class BossSequence : MonoBehaviour
{
    private enum State { EightShape, Waiting, Triangle }
    private State currentState = State.EightShape;
    private State nextStateAfterWait;

    public float waittime = 4f;
    public int repeatNum = 4; 

    [Header("8-as alak beállítások")]
    public float eightWidth = 12f;
    public float eightHeight = 7f;
    public float speed = 1.5f;
    private int completedEightCircles = 0;

    [Header("Háromszög beállítások")]
    public float triWidth = 16f;
    public float triHeight = 8f;
    private int completedTriangles = 0;

    private Vector3 startPosition;
    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.EightShape:
                MoveInEight();
                break;
            case State.Waiting:
                HandleWait();
                break;
            case State.Triangle:
                MoveInTriangle();
                break;
        }
    }

    void MoveInEight()
    {
        timer += Time.deltaTime * speed;
        float x = Mathf.Sin(timer) * eightWidth;
        float y = Mathf.Sin(timer * 2f) * eightHeight;
        Vector3 nextPos = startPosition + new Vector3(x, y, 0);

        if (timer >= Mathf.PI * 2f)
        {
            completedEightCircles++;
            timer = 0;

            if (completedEightCircles >= repeatNum)
            {
                completedEightCircles = 0;
                GoToWait(State.Triangle);
                return;
            }
        }

        FlipSprite(nextPos.x);
        transform.position = nextPos;
    }

    void MoveInTriangle()
    {
        timer += Time.deltaTime * speed * 0.5f;
        float t = timer; 

        if (timer >= 3f)
        {
            completedTriangles++;
            timer = 0;

            if (completedTriangles >= repeatNum)
            {
                completedTriangles = 0;
                GoToWait(State.EightShape);
                return;
            }
        }

        Vector3 p0 = startPosition;
        Vector3 p1 = startPosition + new Vector3(triWidth / 2, -triHeight, 0);
        Vector3 p2 = startPosition + new Vector3(-triWidth / 2, -triHeight, 0);

        Vector3 nextPos;
        if (t < 1f) nextPos = Vector3.Lerp(p0, p1, t);
        else if (t < 2f) nextPos = Vector3.Lerp(p1, p2, t - 1f);
        else nextPos = Vector3.Lerp(p2, p0, t - 2f);

        FlipSprite(nextPos.x);
        transform.position = nextPos;
    }

    void GoToWait(State nextTask)
    {
        transform.position = startPosition;
        timer = 0;
        nextStateAfterWait = nextTask;
        currentState = State.Waiting;
        if (animator != null) animator.enabled = false;
    }

    void HandleWait()
    {
        timer += Time.deltaTime;
        if (timer >= waittime)
        {
            timer = 0;
            currentState = nextStateAfterWait;
            if (animator != null) animator.enabled = true;
        }
    }

    void FlipSprite(float currentX)
    {
        if (currentX > transform.position.x) spriteRenderer.flipX = false;
        else if (currentX < transform.position.x) spriteRenderer.flipX = true;
    }

    public bool IsMoving()
    {
        return currentState == State.EightShape || currentState == State.Triangle;
    }
}