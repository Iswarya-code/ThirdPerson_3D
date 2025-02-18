using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Input actions
    InputAction moveAction;
    InputAction jumpaction;
    InputAction lookAction;

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
    // Transform CameraTransform;
    // [SerializeField] float CamRotationSpeed = 5f;
    // Camera rotation variables
    [SerializeField] float lookSensitivity = 3f; // Sensitivity of mouse movement
    [SerializeField] float maxVerticalAngle = 80f; // Limit for vertical rotation
    private float cameraPitch = 0f; // Track vertical rotation
    [SerializeField] Transform cameraHolder; 


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpaction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];

        // CameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for FPS-style control


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
      //  move = move.x * CameraTransform.right.normalized + move.z * CameraTransform.forward.normalized;
       // move.y = 0f;
        controller.Move(move * Time.deltaTime * moveSpeed);

        // Rotate character based on movement direction (if moving)
        if (input.magnitude > 0.1f)
        {
            RotateCharacter(move);
        }

        #region Jump action
        if (jumpaction.phase == InputActionPhase.Performed && groundedPlayer)
        {
            PlayerVelocity.y += Mathf.Sqrt(-2 * gravity * jumpHeight); // kinematic equation ,u = sqrt of -2as [a-acceleration, s-jumpheight, u-initial velocity, v - final velocity]

        }

        PlayerVelocity.y += gravity * Time.deltaTime;
        controller.Move(PlayerVelocity * Time.deltaTime);
        #endregion
        HandleCameraRotation();
        Debug.Log($"Camera Pitch: {cameraPitch}");


    }

    private void RotateCharacter(Vector3 input)
    {
        if (input.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleCameraRotation()
    {/*
        Vector2 lookInput = Mouse.current.delta.ReadValue();
        Debug.Log($"Mouse Input: {lookInput}");

        if (lookInput.magnitude < 0.01f) return; // Prevent unnecessary updates

        lookInput *= lookSensitivity * Time.deltaTime;

        // Rotate the camera (NOT the player)
        cameraPitch -= lookInput.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxVerticalAngle, maxVerticalAngle);
        cameraHolder.localRotation = Quaternion.Euler(cameraPitch, cameraHolder.localEulerAngles.y, 0);

        // Rotate the player left/right
        transform.Rotate(Vector3.up * lookInput.x);
*/

        // Get the mouse input for rotating the camera
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // Vertical camera rotation (pitch)
        cameraPitch -= lookInput.y * lookSensitivity * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxVerticalAngle, maxVerticalAngle);
        cameraHolder.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f); // Only rotate the camera vertically

        // Horizontal player rotation (yaw)
        float lookHorizontal = lookInput.x * lookSensitivity * Time.deltaTime;

        // Rotate the player horizontally around the Y-axis, not affecting the camera's horizontal rotation
        transform.Rotate(Vector3.up * lookHorizontal);
    }

}
