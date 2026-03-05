using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float minJumpForce = 8f;
    public float maxJumpForce = 20f;
    public float chargeSpeed = 1f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private bool isCharging;
    private float jumpCharge;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        float controlFactor = isCharging ? 0f : 1f;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed * controlFactor, rb.linearVelocity.y);

        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            jumpCharge += chargeSpeed * Time.deltaTime;
            jumpCharge = Mathf.Clamp01(jumpCharge);
        }

        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            float force = Mathf.Lerp(minJumpForce, maxJumpForce, jumpCharge);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
            isCharging = false;
            jumpCharge = 0f;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
