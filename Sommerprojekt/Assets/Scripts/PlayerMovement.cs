using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody;
    private PlayerInput playerInput;
    public float speed = 5f;
    public float jumpPower = 2f;
    private bool IsGrounded = false;
    private bool MovingLeft = false;
    private bool MovingRight = false;


    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        
        playerInput.actions["Jump"].started += Jump;
        playerInput.actions["Jump"].performed += Jump;
        playerInput.actions["Jump"].canceled += Jump;
        playerInput.actions["MoveLeft"].started += MoveLeft;
        playerInput.actions["MoveLeft"].performed += MoveLeft;
        playerInput.actions["MoveLeft"].canceled += MoveLeft;
        playerInput.actions["MoveRight"].started += MoveRight;
        playerInput.actions["MoveRight"].performed += MoveRight;
        playerInput.actions["MoveRight"].canceled += MoveRight;

        var left = new InputAction("MoveLeft");

    }


    void Update()
    {
        bool LwasPressed = playerInput.actions["MoveLeft"].WasPerformedThisFrame() && playerInput.actions["MoveLeft"].ReadValue<float>() > 0;
        bool LwasReleased = playerInput.actions["MoveLeft"].WasPerformedThisFrame() && playerInput.actions["MoveLeft"].ReadValue<float>() == default;

        bool RwasPressed = playerInput.actions["MoveRight"].WasPerformedThisFrame() && playerInput.actions["MoveRight"].ReadValue<float>() > 0;
        bool RwasReleased = playerInput.actions["MoveRight"].WasPerformedThisFrame() && playerInput.actions["MoveRight"].ReadValue<float>() == default;

        if(LwasPressed)
            MovingLeft = true;

        if(LwasReleased)
            MovingLeft = false;

        if(RwasPressed)
            MovingRight = true;

        if(RwasReleased)
            MovingRight = false;

        Move();
    }




    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
		if (IsGrounded && context.performed)
		{
            _playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            Debug.Log("Jump" + context);
            IsGrounded = false;
		}
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
		
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
		
    }

	private void Move()
	{
		MoveLeft();
        MoveRight();
	}

	public void MoveLeft()
    {
		if (MovingLeft)
		{
            _playerRigidbody.AddForce(Vector2.left * speed * Time.deltaTime, ForceMode2D.Impulse);
            Debug.Log("MoveLeft");
		}
    }

    public void MoveRight()
    {
		if (MovingRight)
		{
            _playerRigidbody.AddForce(Vector2.right * speed * Time.deltaTime, ForceMode2D.Impulse);
            Debug.Log("MoveRight");
		}
    }
}
