using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    //Input actions
    InputAction moveAction;
    InputAction jumpaction;

    //Components
    CharacterController controller;  
    PlayerInput playerInput;
    Animator animator;

    //Move
    [SerializeField] float moveSpeed = 5f;

    //Move with animation
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    private float currentSpeed;


    //Jump & gravity
    Vector3 PlayerVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -9.81f; //float gravity = -15f; // Stronger gravity for a faster, snappier jump

    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    //Rotation
    [SerializeField] float rotationSpeed = 200f;

    //Camera
    Transform cameraTransform; // Reference to main camera




    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        moveAction = playerInput.actions["Move"];
        jumpaction = playerInput.actions["Jump"];

        cameraTransform = Camera.main.transform; // Get the main camera

        animator.SetFloat("Speed", 0f); // Ensure Idle animation on start

        // Cursor.lockState = CursorLockMode.Locked; // Lock cursor for FPS-style control

        animator.SetFloat("Speed", 0f); // Ensure Idle animation on start

    }



    // Update is called once per frame
    void Update()
    {
        groundedPlayer = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (groundedPlayer && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = -2f;  // Keeps the player firmly on the ground
        }

        Char_Movement();
        Char_Jump();
        if (jumpaction.triggered)
        {
            Debug.Log("Jump Button Pressed!"); // Check if jump is triggered
        }

    }

   
    private void Char_Movement()
    {

            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);

            // Determine if running or walking
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isRunning ? runSpeed : walkSpeed;


            if (input.magnitude > 0.1f)
            {
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;

                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 moveDirection = (camForward * move.z + camRight * move.x).normalized;
                controller.Move(moveDirection * currentSpeed * Time.deltaTime);

                // Rotate player to face movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

        // Convert speed to Animator range (0 to 1)
        float normalizedSpeed = (input.magnitude > 0.1f) ? (currentSpeed / runSpeed) : 0f;
        animator.SetFloat("Speed", normalizedSpeed);
    }


    private void Char_Jump()
    {

        if (groundedPlayer) // Ensure grounded check
        {
            animator.SetBool("IsJumping", false); // Reset animation when on ground
            if (jumpaction.phase == InputActionPhase.Performed) // phase for continuous jump
            {
                Debug.Log("Jump Pressed!"); // Debugging jump press
                PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("IsJumping", true); // Start jump animation
            }
        }

        // Apply gravity continuously
        PlayerVelocity.y += gravity * Time.deltaTime;
        controller.Move(PlayerVelocity * Time.deltaTime);
    }

   
}
