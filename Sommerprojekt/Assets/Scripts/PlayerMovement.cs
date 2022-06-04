using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	public int amountOfJumps = 2;

	[SerializeField]
	private float MovementSpeed = 5;
	[SerializeField]
	private float JumpForce = 5;
	[SerializeField]
	private float variableJumpForceMuliplyer = 0.5f;
	[SerializeField]
	private float movementForceInAir = 5;
	[SerializeField]
	private float airDragMultiplayer;
	[SerializeField]
	private float wallCheckDistance = 5;
	[SerializeField]
	private float groundCheckRadius = 5;
	[SerializeField]
	private float wallSlideSpeed = 5;
	[SerializeField]
	private float wallHopForce;
	[SerializeField]
	private float wallJumpForce;
	[SerializeField]
	private float sprintModifier = 1.3f;
	[SerializeField]
	private Transform groundCheck = null;
	[SerializeField]
	private Transform wallCheck = null;
	[SerializeField]
	private LayerMask whatIsGround;
	[SerializeField]
	private Vector2 wallHopDirection;
	[SerializeField]
	private Vector2 wallJumpDirection;
	[SerializeField]
	private IngameUI ui;

	private bool isFascingRight = true;
	private bool isJumping = false;
	private bool canJump;
	private bool isWalking = false;
	private bool isGrounded = false;
	private bool isTouchingWall;
	private bool isWallSliding;
	private bool isJumpingFromLeft = false;
	private bool isJumpingFromRight = false;
	private bool isSprinting = false;
	private bool isPaused = false;

	private int amountOfJumpsLeft;
	private int facingDirection = 1;

	private float movementInputDirection;
	private float time = 2;
	private float timeElapsed;
	private float speedModifier = 1;

	private Rigidbody2D rb;
	private PlayerInput input;
	private Animator anim;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		input = GetComponent<PlayerInput>();
		anim = GetComponent<Animator>();

		amountOfJumpsLeft = amountOfJumps;

		input.actions["Jump"].started += Jump;
		input.actions["Pause"].started += PauseMenu;

		wallJumpDirection.Normalize();
		wallHopDirection.Normalize();
	}

	private void Update()
	{
		CheckInput();
		CheckMovementDirection();
		CheckIfCanJump();
		CheckIfWallSliding();
		checkIfIsSprinting();
		CheckSurroundings();
		ApplyMovement();
		UpdateAnimations();
	}

	private void FixedUpdate()
	{
	}

	private void CheckIfWallSliding()
	{
		if (isTouchingWall && !isGrounded && rb.velocity.y < 0.01f)
		{
			isWallSliding = true;
		}
		else
		{
			isWallSliding = false;
		}
	}

	private void CheckIfCanJump()
	{
		if (isGrounded && rb.velocity.y <= 0.01f && !isJumping)
		{
			amountOfJumpsLeft = amountOfJumps;
			canJump = true;
		}
		else
		{
			canJump = false;
		}

		if (amountOfJumpsLeft <= 0)
		{
			canJump = false;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
		Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
	}

	private void CheckSurroundings()
	{
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

		isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
	}

	private void CheckInput()
	{
		if (input.actions["MoveLeft"].IsPressed())
		{
			movementInputDirection = -1;
		}
		else if (input.actions["MoveRight"].IsPressed())
		{
			movementInputDirection = 1;
		}
		else
		{
			movementInputDirection = 0;
		}

		if (input.actions["Jump"].IsPressed())
		{
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpForceMuliplyer);
		}
	}

	private void checkForMenuInput()
	{
		
	}

	public void callPauseMenu()
	{
		PauseMenu(new InputAction.CallbackContext());
	}

	private void PauseMenu(InputAction.CallbackContext context)
	{
		if (isPaused)
		{
			Time.timeScale = 1;
			input.actions["Jump"].Enable();
			input.actions["Sprint"].Enable();
			input.actions["MoveLeft"].Enable();
			input.actions["MoveRight"].Enable();
			isPaused = false;
			ui.HidePauseMenu();
		}
		else
		{
			Time.timeScale = 0;
			input.actions["Jump"].Disable();
			input.actions["Sprint"].Disable();
			input.actions["MoveLeft"].Disable();
			input.actions["MoveRight"].Disable();
			isPaused = true;
			ui.ShowPauseMenu();
		}
	}

	private	void checkIfIsSprinting()
	{
		if (input.actions["Sprint"].IsPressed() && !isTouchingWall && isGrounded && isWalking)
		{
			isSprinting = true;
			speedModifier = sprintModifier;
		}
		else
		{
			isSprinting = false;
			speedModifier = 1.0f;
		}
	}

	private void UpdateAnimations()
	{
		anim.SetBool("isWalking", isWalking);
		anim.SetBool("isGrounded", isGrounded);
		anim.SetFloat("yVelocity", rb.velocity.y);
		anim.SetBool("isSprinting", isSprinting);
		//anim.SetBool("isLedgeClimbing", isLedgeClimbing);
	}

	private void Jump(InputAction.CallbackContext context)
	{
		if (canJump || ((amountOfJumpsLeft > 0) && !isTouchingWall))
		{
			rb.velocity = new Vector2(rb.velocity.x, JumpForce);
			amountOfJumpsLeft--;
		}
		else if (isWallSliding && movementInputDirection == 0)
		{
			isWallSliding = false;
			amountOfJumpsLeft--;
			Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
			rb.AddForce(forceToAdd, ForceMode2D.Impulse);
		}
		else if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && movementInputDirection != facingDirection)
		{
			isWallSliding = false;
			amountOfJumpsLeft--;
			Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -facingDirection, wallJumpForce * wallJumpDirection.y);
			rb.AddForce(forceToAdd, ForceMode2D.Impulse);

			if (facingDirection == 1)
			{
				isJumpingFromRight = true;
				isJumpingFromLeft = false;
			}
			else if (facingDirection == -1)
			{
				isJumpingFromLeft = true;
				isJumpingFromRight = false;
			}

			if (isJumpingFromLeft)
			{
				isJumpingFromLeft = false;
			}
			else if (isJumpingFromRight)
			{
				isJumpingFromRight = false;
			}
		}
	}

	IEnumerator lockLeft()
	{
		while (time > timeElapsed)
		{
			timeElapsed += Time.deltaTime;
			input.actions["MoveLeft"].Disable();
			input.actions["MoveLeft"].Dispose();
			yield return new WaitForEndOfFrame();
		}
		if (time <= timeElapsed)
		{
			input.actions["MoveRight"].Enable();
			timeElapsed = 0;
			yield return null;
		}
	}

	IEnumerator lockRight()
	{
		while (time > timeElapsed)
		{
			timeElapsed += Time.deltaTime;
			input.actions["MoveRight"].Disable();
			input.actions["MoveRight"].Dispose();
			yield return new WaitForEndOfFrame();
		}
		if (time <= timeElapsed)
		{
			input.actions["MoveLeft"].Enable();
			timeElapsed = 0;
			yield return null;
		}
	}

	private void ApplyMovement()
	{
		if (isGrounded)
		{
			rb.velocity = new Vector2(MovementSpeed * movementInputDirection * speedModifier, rb.velocity.y);
		}
		else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
		{
			Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
			rb.AddForce(forceToAdd);

			if (Mathf.Abs(rb.velocity.x) > MovementSpeed)
			{
				rb.velocity = new Vector2(MovementSpeed * movementInputDirection, rb.velocity.y);
			}
		}
		else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
		{
			rb.velocity = new Vector2(rb.velocity.x * airDragMultiplayer, rb.velocity.y);
		}

		if (isWallSliding)
		{
			if (rb.velocity.y < -wallSlideSpeed)
			{
				rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
			}
		}

	}

	private void CheckMovementDirection()
	{
		if (isFascingRight && movementInputDirection < 0)
		{
			Flip();
		}
		else if (!isFascingRight && movementInputDirection > 0)
		{
			Flip();
		}

		if (rb.velocity.x != 0)
		{
			isWalking = true;
		}
		else
		{
			isWalking = false;
		}
	}

	private void Flip()
	{
		if (!isWallSliding)
		{
			facingDirection *= -1;
			isFascingRight = !isFascingRight;
			transform.Rotate(0.0f, 180.0f, 0.0f);

		}
	}











































	//   [SerializeField]
	//   private float movementSpeed;
	//   [SerializeField]
	//   private float groundCheckRadius;
	//   [SerializeField]
	//   private float jumpForce;
	//   [SerializeField]
	//   private float slopeCheckDistance;
	//   [SerializeField]
	//   private float maxSlopeAngle;
	//   [SerializeField]
	//   private Transform groundCheck;
	//   [SerializeField]
	//   private LayerMask whatIsGround;
	//   [SerializeField]
	//   private PhysicsMaterial2D noFriction;
	//   [SerializeField]
	//   private PhysicsMaterial2D fullFriction;

	//   private float xInput;
	//   private float slopeDownAngle;
	//   private float slopeSideAngle;
	//   private float lastSlopeAngle;

	//   private int facingDirection = 1;

	//   private bool isGrounded;
	//   private bool isOnSlope;
	//   private bool isJumping;
	//   private bool canWalkOnSlope;
	//   private bool canJump;

	//   private Vector2 newVelocity;
	//   private Vector2 newForce;
	//   private Vector2 capsuleColliderSize;

	//   private Vector2 slopeNormalPerp;

	//   private Rigidbody2D rb;
	//   private CapsuleCollider2D cc;
	//   private PlayerInput input;
	//   private SpriteRenderer sr;


	//   void Start()
	//   {
	//       rb = GetComponent<Rigidbody2D>();
	//       input = GetComponent<PlayerInput>();
	//       cc = GetComponent<CapsuleCollider2D>();
	//       sr = GetComponent<SpriteRenderer>();

	//       input.actions["Jump"].started += Jump;
	//       input.actions["Jump"].performed += Jump;
	//       input.actions["Jump"].canceled += Jump;

	//       capsuleColliderSize = cc.size;
	//   }

	//   void Update()
	//   {
	//       Move();
	//       rb.rotation = 0;
	//   }


	//private void FixedUpdate()
	//{
	//       CheckGround();
	//	//SlopeCheck();
	//}





	//private void SlopeCheck()
	//{
	//       Vector2 checkPos = transform.position - new Vector3(0.0f, capsuleColliderSize.y /2);

	//       SlopeCheckVertical(checkPos);
	//       SlopeCheckHorizontal(checkPos);
	//}

	//   private void SlopeCheckHorizontal(Vector2 checkPos)
	//{
	//       RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
	//       RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

	//       if (slopeHitFront)
	//       {
	//           isOnSlope = true;

	//           slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

	//       }
	//       else if (slopeHitBack)
	//       {
	//           isOnSlope = true;

	//           slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
	//       }
	//       else
	//       {
	//           slopeSideAngle = 0.0f;
	//           isOnSlope = false;
	//       }

	//}
	//   private void SlopeCheckVertical(Vector2 checkPos)
	//{
	//       RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

	//       if (hit)
	//       {

	//           slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            

	//           slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

	//           if(slopeDownAngle != lastSlopeAngle)
	//           {
	//               isOnSlope = true;
	//           }                       

	//           lastSlopeAngle = slopeDownAngle;

	//           Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
	//           Debug.DrawRay(hit.point, hit.normal, Color.green);

	//       }

	//       if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
	//       {
	//           canWalkOnSlope = false;
	//       }
	//       else
	//       {
	//           canWalkOnSlope = true;
	//       }

	//       if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
	//       {
	//           rb.sharedMaterial = fullFriction;
	//       }
	//       else
	//       {
	//           rb.sharedMaterial = noFriction;
	//       }
	//}





	//   private void CheckGround()
	//   {
	//       isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

	//       if(rb.velocity.y <= 0.0f)
	//       {
	//           isJumping = false;
	//       }

	//       if(isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
	//       {
	//           canJump = true;
	//       }

	//   }
	//public void Jump(InputAction.CallbackContext context)
	//   {
	//	if (canJump && context.performed)
	//	{
	//           rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
	//           Debug.Log("Jump" + context);
	//           isJumping = true;
	//	}
	//   }

	//private void Move()
	//{
	//	MoveLeft();
	//       MoveRight();
	//}

	//private void MoveLeft()
	//   {
	//	if (input.actions["MoveLeft"].IsPressed())
	//	{
	//           rb.AddForce(Vector2.left * movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
	//           Debug.Log("MoveLeft");
	//	}
	//       sr.flipX = true;
	//   }

	//   private void MoveRight()
	//   {
	//	if (input.actions["MoveRight"].IsPressed())
	//	{
	//           rb.AddForce(Vector2.right * movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
	//           Debug.Log("MoveRight");
	//	}
	//       sr.flipX = false;
	//   }
}
