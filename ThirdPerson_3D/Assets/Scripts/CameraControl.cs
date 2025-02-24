using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
 
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform player;
    [SerializeField] private float sensitivity = 1f; // Adjust mouse sensitivity

    private Vector2 lookInput;
    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>() * sensitivity;
    }

    private void LateUpdate()
    {
        // Rotate player horizontally
        player.Rotate(Vector3.up * lookInput.x * Time.deltaTime * 200f);

        // Rotate camera vertically
        xRotation -= lookInput.y * Time.deltaTime * 200f;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Limit vertical rotation

        virtualCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}




