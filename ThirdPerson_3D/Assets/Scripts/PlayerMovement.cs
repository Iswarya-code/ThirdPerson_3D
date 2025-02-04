using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Input actions
    PlayerInput playerInput;   //component
    InputAction moveAction;
    InputAction jumpaction;

    //Player
    [SerializeField] CharacterController controller;  //component
    Vector3 PlayerVelocity;
    [SerializeField] bool groundedPlayer;

    //Move
    [SerializeField] float moveSpeed = 5f;

    //Jump
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -9.81f;

    //Rotation
    float targetRotation = 0f;
    [SerializeField] float rotationSpeed = 200f;

    //Camera Rotation
    Transform CameraTransform;
    [SerializeField] float CamRotationSpeed = 5f;
    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpaction = playerInput.actions["Jump"];

        CameraTransform = Camera.main.transform;

    }



    // Update is called once per frame
    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = 0f;
        }

        //Movement
        Vector2 input = moveAction.ReadValue<Vector2>();
         Vector3 move = new Vector3(input.x, 0, input.y);

        //Camera rotate towards the player rotation
        move = move.x * CameraTransform.right.normalized + move.z * CameraTransform.forward.normalized;
        move.y = 0f;
         controller.Move(move * Time.deltaTime * moveSpeed);

        // Smoothly rotate to the target rotation
        Quaternion targetRotationQuaternion = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotationQuaternion, CamRotationSpeed * Time.deltaTime);

        // RotateCharacter(input);
        // MoveCharacter(input);

        //Jump
        if (jumpaction.phase == InputActionPhase.Performed && groundedPlayer)
        {
            PlayerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);  // kinematic equation ,u = sqrt of -2as [a-acceleration, s-jumpheight]
        }

        // Apply gravity
       /* if (!groundedPlayer)
        {
            PlayerVelocity.y += gravity * Time.deltaTime;
        }*/

        PlayerVelocity.y += gravity * Time.deltaTime;
        controller.Move(PlayerVelocity * Time.deltaTime);


    }

    private void RotateCharacter(Vector2 input)
    {
        // float targetRotation = 0;  //the variable is being treated as a constant or has a specific value type

      /*  if (input.x > 0)
        {
            targetRotation = 90f;
        }
        else if (input.x < 0)
        {
            targetRotation = -90f;
        }*/

        // Calculate the desired rotation angle based on input
        float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg; // Convert to degrees

        // Smoothly rotate to the target rotation
        Quaternion targetRotationQuaternion = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotationQuaternion, rotationSpeed * Time.deltaTime);
    }

    private void MoveCharacter(Vector2 input)
    {
        /* // Calculate movement in the direction the player is facing
         Vector3 moveDirection = transform.forward * input.y; // Moving forward/backward based on forward vector

         // Apply movement
         controller.Move(moveDirection * Time.deltaTime * moveSpeed);*/

        if (input.magnitude > 0)
        {
            // Calculate movement in the direction the player is facing
            Vector3 moveDirection = transform.forward * input.y;

            // Apply movement
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);
        }
        else
        {
            // If there's no input, stop moving
            controller.Move(Vector3.zero);
        }
    }

}
