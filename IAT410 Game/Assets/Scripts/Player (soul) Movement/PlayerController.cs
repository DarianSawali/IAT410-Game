using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Tilemap groundTilemap;
    public LayerMask groundLayer;
    protected float groundedCheckDist = 0.1f;
    public Transform spawnPoint;

    private float speed = 6;
    private float grav = -9.81f;

    private PlayerInput playerInput;
    private Skunk skunk;
    private Pigeon pigeon;
    private Fish fish;
    protected bool isPlayerActive = true;

    protected Rigidbody rb;

    public GameObject playerModel;
    private GameObject targetAnimal = null;

    public PlayerJump playerJump;

    public bool dug = false;

    public HealthManager health; // if player fall down, decrease health

    private Animator animator; // reference to animator
    protected bool isGrounded; // to handle falling
    protected bool isDamaged = false; // handle damage animation
    private float damageDuration = 1f; // damage animation duration

    public Transform platform; // Assign the reference to the platform in the inspector

    public AudioManager audioManager;
    public AudioClip possess;
    public AudioClip dispossess;

    private bool delayComplete = false;
    private float currentDelayTime = 0f;
    private float startDelay = 1f;
    private bool hasDelayedFixedUpdate = false;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //input.actions.FindAction("SkunkJump").Disable();

        skunk = FindObjectOfType<Skunk>();
        pigeon = FindObjectOfType<Pigeon>();
        fish = FindObjectOfType<Fish>();

        animator = GetComponent<Animator>(); // get animator component

        // if (isPlayerActive)
        // {
        //     // Disable SkunkMove action, enable PlayerMove action, enable possession
        //     input.actions.FindAction("PlayerMove").Enable();
        //     input.actions.FindAction("Possess").Enable();

        //     input.actions.FindAction("SkunkMove").Disable();
        // }
        // StartCoroutine(DelayedEnablePlayerInput());
        StartCoroutine(DelayedFixedUpdate());

    }

    IEnumerator DelayedFixedUpdate(){
        yield return new WaitForSeconds(1.0f);
        hasDelayedFixedUpdate = true;
    }


    protected void OnPlayerMove()
    {
        Vector2 moveInput = new((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0),
                                (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)    ? 1 : 0) - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1 : 0));
        Vector3 horizontalMoveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        horizontalMoveDirection.Normalize();

        float verticalVelocity = rb.velocity.y;
        Vector3 movement = horizontalMoveDirection * moveSpeed + Vector3.up * verticalVelocity;

        rb.velocity = movement;

        if (Input.GetKey(KeyCode.E)) OnPossess();
    }

    IEnumerator DelayedEnablePlayerInput()
    {
        yield return new WaitForSeconds(startDelay); // Wait for the specified delay
        // EnablePlayerInput(); // Enable player input after the delay
    }

    protected void Update()
    {
        isGrounded = IsGrounded();

        if (!isGrounded && transform.position.y < platform.position.y)
        {
            // Pigeon is falling below the platform
            animator.SetBool("isFalling", true);
        }
        else
        {
            // Pigeon is not falling below the platform
            animator.SetBool("isFalling", false);
        }

        if (transform.position.y < -2f)
        {
            transform.position = spawnPoint.position;
            health.decreaseHealth();
            isDamaged = true;
            animator.SetBool("Damaged", true);
        }
        if (isDamaged) // to handle damage animation duration
        {
            damageDuration -= Time.deltaTime;
            if (damageDuration <= 0)
            {
                animator.SetBool("Damaged", false);
                isDamaged = false;
                damageDuration = 1f;
            }
        }
    }

    protected void FixedUpdate()
    {
        if(hasDelayedFixedUpdate){
            if (isPlayerActive) {
                OnPlayerMove();
            }
        }
    }

    //possessing mechanic
    protected void OnPossess()
    {
        Debug.Log("OnPossess called");
        if (targetAnimal != null && isPlayerActive)
        {
            audioManager.PlaySoundEffect(possess);
            PossessAnimal(targetAnimal);
        }
    }

    public void PossessAnimal(GameObject animal)
    {
        Debug.Log("Possessing animal");
        CameraFollowVertical cameraFollowScript = Camera.main.GetComponent<CameraFollowVertical>(); // get camera

        Skunk skunkComponent = targetAnimal.GetComponent<Skunk>();
        if (skunkComponent != null)
        {
            skunk.GetComponent<CapsuleCollider>().enabled = true;
            Debug.Log("Possessing Skunk");

            skunk.setSkunkPossessedFlagOn();

            cameraFollowScript.SetTarget(skunk.transform); // set camera to follow skunk
        }

        Pigeon pigeonComponent = targetAnimal.GetComponent<Pigeon>();
        if (pigeonComponent != null)
        {
            pigeon.GetComponent<CapsuleCollider>().enabled = true;
            Debug.Log("Possessing Pigeon");

            pigeon.setPigeonPossessedFlagOn();

            cameraFollowScript.SetTarget(pigeon.transform); // set camera to follow pigeon
        }

        Fish fishComponent = targetAnimal.GetComponent<Fish>();
        if (fishComponent != null)
        {
            fish.GetComponent<CapsuleCollider>().enabled = true;
            Debug.Log("Possessing Fish");

            fish.setFishPossessedFlagOn();

            cameraFollowScript.SetTarget(fish.transform);
        }

        isPlayerActive = false;

        playerModel.SetActive(false);
    }

    public void DispossessAnimal()
    {
        Debug.Log("Dispossessing animal");
        audioManager.PlaySoundEffect(dispossess);

        playerModel.SetActive(true); // Show the player model again

        transform.position = new Vector3(targetAnimal.transform.position.x, targetAnimal.transform.position.y, (targetAnimal.transform.position.z - 0.005f));
        // upon dispossessing, player will spawn in front of the dispossessed animal

        targetAnimal = null; // Clear the target animal

        isPlayerActive = true;

        CameraFollowVertical cameraFollowScript = Camera.main.GetComponent<CameraFollowVertical>();
        cameraFollowScript.SetTarget(transform); // set camera to follow player back
    }
    // end of possessing mechanic

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pigeon") || other.CompareTag("Skunk") || other.CompareTag("Fish"))
        {
            targetAnimal = other.gameObject;
            targetAnimal.GetComponent<CapsuleCollider>().enabled = false;

            Debug.Log("No collision");
        }

        if (other.CompareTag("ArrowLeft") || other.CompareTag("ArrowRight")
        || other.CompareTag("ArrowUp") || other.CompareTag("ArrowDown")) // decrease health if hit by arrow
        {
            health.decreaseHealth();
            isDamaged = true;
            animator.SetBool("Damaged", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Skunk") || other.CompareTag("Pigeon") || other.CompareTag("Fish")) && other.gameObject == targetAnimal)
        {
            targetAnimal.GetComponent<CapsuleCollider>().enabled = true;
            targetAnimal = null;
        }
    }

    // Method to check if the player is grounded
    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f; // Start the ray slightly above the player's feet
        float distance = groundedCheckDist + 0.1f; // Extend the check slightly beyond the exact bottom of the player
        RaycastHit hit;

        Debug.DrawRay(origin, Vector3.down * distance, Color.red); // Visualize the ray in the Scene view

        if (Physics.Raycast(origin, Vector3.down, out hit, distance, groundLayer))
        {
            return true; // Grounded
        }
        else
        {
            // Debug.Log("Not Grounded"); // Log when not grounded
        }

        return false; // Not grounded
    }

    // Rigidbody animalRigidbody = targetAnimal.GetComponent<Rigidbody>();
    // animalRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = true;
    //         Debug.Log("collision enter");
    //     }
    // }

    // private void OnCollisionStay(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = false;
    //     }
    // }

    // protected void FixedUpdate()
    // {
    //     isGrounded = CheckGrounded();
    // }
}


// protected void FixedUpdate()
// {
//     isGrounded = CheckGrounded();
// }

// protected void EnableControls()
// {
//     controlsEnabled = true;
// }

// protected void DisableControls()
// {
//     controlsEnabled = false;
// }



// public bool CheckGrounded()
// {
//     if (Physics.Raycast(transform.position, Vector3.down, groundedCheckDist, groundLayer))
//     {
//         return true;
//     }
//     return false;
// }