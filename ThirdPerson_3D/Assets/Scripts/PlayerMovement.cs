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

    //Move
    [SerializeField] float moveSpeed = 5f;

    //Jump
    Vector3 PlayerVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -9.81f;

    //Rotation
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

        // Rotate character based on movement direction (if moving)
        if (input.magnitude > 0.1f)
        {
            RotateCharacter(move);
        }

       if(jumpaction.phase == InputActionPhase.Performed && groundedPlayer)
        {
            PlayerVelocity.y += Mathf.Sqrt(-2 * gravity * jumpHeight); // kinematic equation ,u = sqrt of -2as [a-acceleration, s-jumpheight, u-initial velocity, v - final velocity]

        }

        PlayerVelocity.y += gravity * Time.deltaTime;
        controller.Move(PlayerVelocity * Time.deltaTime);
            


    }

    private void RotateCharacter(Vector3 input)
    {
        if (input.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


}
