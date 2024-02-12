using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Skunk : MonoBehaviour
{
    public float moveSpeed = 0.9f;
    public float jumpForce = 1.4f;
    public Tilemap groundTilemap;
    public float gravity = 6f;
    public LayerMask groundLayer;
    protected float groundedCheckDist = 0.1f;
    public Transform spawnPoint;

    public PlayerInput playerInput;
    protected bool isSkunkActive = false;
    public PlayerController player;

    protected Rigidbody rb;
    protected bool isGrounded;
    protected Vector3 fixedEulerRotation = new Vector3(45f, 0f, 0f);


    protected bool controlsEnabled = true;

    protected void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        DisableSkunkInput();

        if (isSkunkActive)
        {
            input.actions.FindAction("PlayerMove").Disable();
            input.actions.FindAction("SkunkMove").Enable();
            input.actions.FindAction("Jump").Enable();
        }
    }

    protected void Update()
    {
        transform.rotation = Quaternion.Euler(fixedEulerRotation);

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

    protected void FixedUpdate()
    {
        isGrounded = CheckGrounded();
    }

    protected void EnableJump(){
        playerInput.actions["Jump"].Enable();
    }

    protected void OnJump(InputValue value)
    {
        if (!controlsEnabled || !isGrounded) return;
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void EnableSkunkInput()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        isSkunkActive = true;
        input.actions.FindAction("PlayerMove").Disable();
        input.actions.FindAction("SkunkMove").Enable();
        input.actions.FindAction("Jump").Enable();
    }

    protected void DisableSkunkInput()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        isSkunkActive = false;
        input.actions.FindAction("SkunkMove").Disable();
        input.actions.FindAction("Jump").Disable();
    }

    protected void OnSkunkMove(InputValue value)
    {
        if (!controlsEnabled) return;

        Vector3 moveInput = value.Get<Vector3>();
        Vector3 movement = new Vector3(moveInput.x * moveSpeed, moveInput.y * moveSpeed, moveInput.z * moveSpeed);
        rb.velocity = movement;
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



    // private void Start()
    // {
    //     rb = GetComponent<Rigidbody>();
    //     rb.useGravity = false; 
    //     rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    // }

    // private void Update()
    // {
    //     transform.rotation = Quaternion.Euler(fixedEulerRotation);
    // }

    // private void FixedUpdate()
    // {
    //     // Check if the skunk is grounded
    //     isGrounded = CheckGrounded();

    //     // Apply friction to stop gliding when not pushed
    //     if (isGrounded)
    //     {
    //         // Apply friction only if the skunk is not being pushed
    //         if (Mathf.Approximately(rb.velocity.magnitude, 0f))
    //         {
    //             rb.velocity = Vector3.zero;
    //         }
    //     }
    // }

    // private bool CheckGrounded()
    // {
    //     // Raycast down to check for collisions with the groundTilemap
    //     RaycastHit hit;
    //     if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
    //     {
    //         return true;
    //     }
    //     return false;
    // }