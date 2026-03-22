using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float minJumpForce = 8f;
	[SerializeField] private float maxJumpForce = 20f;
	[SerializeField] private float chargeSpeed = 1f;
	[SerializeField] private bool lockHorizontalWhileCharging = true;
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;

	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius = 0.2f;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float jumpGroundDetachTime = 0.08f;

	[SerializeField] private string walkParameterName = "isWalking";
	[SerializeField] private string groundedParameterName = "isGrounded";
	[SerializeField] private string yVelocityParameterName = "yVelocity";
	[SerializeField] private bool forceGroundMoveStates = true;
	[SerializeField] private string idleStateName = "Idle";
	[SerializeField] private string runStateName = "Running";
	[SerializeField] private bool forceAscendStateOnJump = true;
	[SerializeField] private string ascendStateName = "Ascend";
	[SerializeField] private bool forceDescendStateOnWalkOff = true;
	[SerializeField] private string descendStateName = "Descend";
	[SerializeField] private Transform visualRoot;

	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private float horizontalInput;
	private bool isGrounded;
	private bool wasGrounded;
	private bool isCharging;
	private float jumpCharge;
	private float jumpGroundDetachTimer;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		if (visualRoot == null)
		{
			visualRoot = transform;
		}

		if (visualRoot != null)
		{
			animator = visualRoot.GetComponent<Animator>();
			spriteRenderer = visualRoot.GetComponent<SpriteRenderer>();
		}

		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void Update()
	{
		bool justLeftGround = wasGrounded && !isGrounded;

		horizontalInput = ReadHorizontalInput();

		if (horizontalInput > 0.01f)
		{
			ApplyFacing(1);
		}
		else if (horizontalInput < -0.01f)
		{
			ApplyFacing(-1);
		}

		if (isCharging && !isGrounded)
		{
			isCharging = false;
			jumpCharge = 0f;
		}

		if (isGrounded && Input.GetKey(jumpKey))
		{
			isCharging = true;
			jumpCharge += chargeSpeed * Time.deltaTime;
			jumpCharge = Mathf.Clamp01(jumpCharge);
		}

		if (isCharging && Input.GetKeyUp(jumpKey))
		{
			float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpCharge);
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
			isGrounded = false;
			jumpGroundDetachTimer = jumpGroundDetachTime;
			isCharging = false;
			jumpCharge = 0f;
			TrySnapToAscendState();
		}

		bool isTryingToWalk = Mathf.Abs(horizontalInput) > 0.01f && isGrounded && !isCharging;

		if (animator != null)
		{
			float animationYVelocity = rb != null ? rb.linearVelocity.y : 0f;

			if (justLeftGround && animationYVelocity > -0.01f)
			{
				animationYVelocity = -0.01f;
			}

			TrySetBool(walkParameterName, isTryingToWalk);
			TrySetBool(groundedParameterName, isGrounded);
			TrySetFloat(yVelocityParameterName, animationYVelocity);

			if (forceGroundMoveStates && isGrounded)
			{
				if (isTryingToWalk)
				{
					TrySnapToStateIfNeeded(runStateName);
				}
				else
				{
					TrySnapToStateIfNeeded(idleStateName);
				}
			}

			if (justLeftGround && !isCharging)
			{
				TrySnapToDescendState();
			}
		}

		wasGrounded = isGrounded;
	}

	private void FixedUpdate()
	{
		if (rb == null)
		{
			return;
		}

		if (jumpGroundDetachTimer > 0f)
		{
			jumpGroundDetachTimer -= Time.fixedDeltaTime;
			isGrounded = false;
		}
		else if (groundCheck != null)
		{
			isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
		}

		float horizontalFactor = lockHorizontalWhileCharging && isCharging ? 0f : 1f;
		rb.linearVelocity = new Vector2(horizontalInput * moveSpeed * horizontalFactor, rb.linearVelocity.y);
	}

	private float ReadHorizontalInput()
	{
		float value = 0f;

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			value -= 1f;
		}

		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			value += 1f;
		}

		if (Mathf.Abs(value) > 0.01f)
		{
			return Mathf.Clamp(value, -1f, 1f);
		}

		return Input.GetAxisRaw("Horizontal");
	}

	private void ApplyFacing(int direction)
	{
		if (visualRoot != null && visualRoot != transform)
		{
			Vector3 scale = visualRoot.localScale;
			scale.x = Mathf.Abs(scale.x) * direction;
			visualRoot.localScale = scale;
			return;
		}

		if (spriteRenderer != null)
		{
			spriteRenderer.flipX = direction < 0;
		}
	}

	private void TrySetBool(string parameterName, bool value)
	{
		if (!string.IsNullOrEmpty(parameterName))
		{
			animator.SetBool(parameterName, value);
		}
	}

	private void TrySetFloat(string parameterName, float value)
	{
		if (!string.IsNullOrEmpty(parameterName))
		{
			animator.SetFloat(parameterName, value);
		}
	}

	private void TrySnapToAscendState()
	{
		if (!forceAscendStateOnJump || animator == null || string.IsNullOrEmpty(ascendStateName))
		{
			return;
		}

		int stateHash = Animator.StringToHash(ascendStateName);
		if (animator.HasState(0, stateHash))
		{
			animator.Play(stateHash, 0, 0f);
		}
	}

	private void TrySnapToDescendState()
	{
		if (!forceDescendStateOnWalkOff || animator == null || string.IsNullOrEmpty(descendStateName))
		{
			return;
		}

		int stateHash = Animator.StringToHash(descendStateName);
		if (animator.HasState(0, stateHash))
		{
			animator.Play(stateHash, 0, 0f);
		}
	}

	private void TrySnapToStateIfNeeded(string stateName)
	{
		if (animator == null || string.IsNullOrEmpty(stateName))
		{
			return;
		}

		int shortStateHash = Animator.StringToHash(stateName);
		int baseLayerStateHash = Animator.StringToHash("Base Layer." + stateName);

		if (!animator.HasState(0, shortStateHash) && !animator.HasState(0, baseLayerStateHash))
		{
			return;
		}

		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.shortNameHash == shortStateHash || stateInfo.fullPathHash == baseLayerStateHash)
		{
			return;
		}

		int targetHash = animator.HasState(0, baseLayerStateHash) ? baseLayerStateHash : shortStateHash;
		animator.Play(targetHash, 0, 0f);
	}

	private void OnDrawGizmosSelected()
	{
		if (groundCheck == null)
		{
			return;
		}

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
	}
}
