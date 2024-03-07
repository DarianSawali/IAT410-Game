using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Pigeon : MonoBehaviour
{
    public float moveSpeed = 0.9f;
    public float jumpForce = 1.4f;
    public Tilemap groundTilemap;
    public float gravity = 6f;
    public LayerMask groundLayer;
    protected float groundedCheckDist = 0.1f;
    public Transform spawnPoint;

    public PlayerInput playerInput;
    protected bool isPigeonActive = false;

    protected Rigidbody rb;
    protected bool isGrounded;
    //protected Vector3 fixedEulerRotation = new Vector3(45f, 0f, 0f);

    protected bool controlsEnabled = true;

    public GameObject playerModel; // Assign your player model in the inspector

    private PlayerController player;


    protected void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        DisablePigeonInput();

        player = FindObjectOfType<PlayerController>();
    }

    public void OnDispossess(InputValue value)
    {
        Debug.Log("OnDispossess called");
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("SkunkMove").Disable();
        input.actions.FindAction("SkunkJump").Disable();
        input.actions.FindAction("Dispossess").Disable();

        
        playerModel.SetActive(true); // Show the player model again
        // player.EnablePlayerInput();
        player.DispossessAnimal();
        // isSkunkActive = false;
    }

    protected void Update()
    {
        //transform.rotation = Quaternion.Euler(fixedEulerRotation);

        if (!isGrounded)
        {
            isGrounded = CheckGrounded();
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
        }

        if (transform.position.y < -2f)
        {
            transform.position = spawnPoint.position;
        }

    }

    protected void OnPigeonMove(InputValue value)
    {
        // Get the movement input vector
        Vector2 moveInput = value.Get<Vector2>();

        // Calculate the horizontal movement direction based on the input vector
        Vector3 horizontalMoveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // Normalize the horizontal movement direction to maintain consistent speed regardless of input magnitude
        horizontalMoveDirection.Normalize();

        // Preserve the current vertical velocity to maintain gravity's effect
        float verticalVelocity = rb.velocity.y;

        // Combine the horizontal movement with the existing vertical velocity
        Vector3 movement = horizontalMoveDirection * moveSpeed + Vector3.up * verticalVelocity;

        // Apply the movement to the player's velocity
        rb.velocity = movement;
    }

    // protected void OnSkunkJump() {
    //     if(!isGrounded) return;
    //     rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    // }

    //     protected void Update()
    // {
    //     transform.rotation = Quaternion.Euler(fixedEulerRotation);

    //     if (!isGrounded && rb.velocity.y < 0)
    //     {
    //         isGrounded = CheckGrounded();
    //         rb.velocity += Vector3.down * gravity * Time.deltaTime;
    //     }

    //     if (transform.position.y < -2f)
    //     {
    //         transform.position = spawnPoint.position;
    //     }
    // }

    protected void FixedUpdate()
    {
        isGrounded = CheckGrounded();
    }

    protected void EnableJump()
    {
        playerInput.actions["SkunkJump"].Enable();
    }

    public void EnablePigeonInput()
    {
        PlayerInput input = GetComponent<PlayerInput>();

        input.actions.FindAction("PlayerMove").Disable();
        input.actions.FindAction("SkunkMove").Enable();
        input.actions.FindAction("SkunkJump").Enable();
        input.actions.FindAction("Dispossess").Enable();
    }

    public void DisablePigeonInput()
    {
        PlayerInput input = GetComponent<PlayerInput>();

        input.actions.FindAction("PlayerMove").Enable();
        input.actions.FindAction("SkunkMove").Disable();
        input.actions.FindAction("SkunkJump").Disable();
        input.actions.FindAction("Dispossess").Disable();
    }

    protected bool CheckGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, groundedCheckDist, groundLayer))
        {
            return true;
        }
        return false;
    }
}

