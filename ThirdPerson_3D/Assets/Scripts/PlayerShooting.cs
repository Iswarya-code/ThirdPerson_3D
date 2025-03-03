using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public Camera playerCamera;             //Assign the main camera
    public float shootRange = 100f;         // Maximum shooting distance
    public LayerMask shootableLayers;       // Assign enemy layer
    public GameObject hitEffectPrefab;  // Assign a particle effect prefab for hit impact
    public Image crosshair; // Assign the crosshair UI image

    PlayerInput playerInput;
    InputAction shootAction;
    private Color defaultCrosshairColor;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["Shoot"];  // Ensure this matches the action name in your Input System

        if (crosshair)
            defaultCrosshairColor = crosshair.color; // Store default color

    }

    private void OnEnable()
    {
        shootAction.performed += _ => Shoot(); // Subscribe to the shoot event

    }


    private void OnDisable()
    {
        shootAction.performed -= _ => Shoot(); // Unsubscribe when disabled

    }


    // Update is called once per frame
    void Update()
    {
        AimAtTarget();
    }

    void AimAtTarget()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, shootableLayers))
        {
            // Change crosshair color when aiming at an enemy
            if (crosshair)
                crosshair.color = Color.red;
        }
        else
        {
            // Reset crosshair color
            if (crosshair)
                crosshair.color = defaultCrosshairColor;
        }
    }

    void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, shootableLayers))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Show hit effect
            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Apply damage if enemy has health script
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy)
            {
                enemy.TakeDamage(10);
            }
        }
    }
}

