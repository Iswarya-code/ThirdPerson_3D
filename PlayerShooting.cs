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

    public Animator animator;

    PlayerInput playerInput;
    InputAction shootAction;
    private Color defaultCrosshairColor;


    private void Awake()
    {
        animator = GetComponent<Animator>(); // Auto-assigns Animator component

        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["Shoot"];  // Ensure this matches the action name in your Input System

        if (crosshair)
            defaultCrosshairColor = crosshair.color; // Store default color

    }

    private void OnEnable()
    {
        shootAction.performed += Shoot;

    }


    private void OnDisable()
    {
        shootAction.performed -= Shoot;

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

    void Shoot(InputAction.CallbackContext context)
    {
        if (animator)
        {
            animator.Play("Player_Shooting", 0, 0f); // Force animation to restart

            animator.SetTrigger("Shoot");
            StopCoroutine(ResetShootTrigger());
            StartCoroutine(ResetShootTrigger());

        }



        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, shootableLayers))
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (Physics.Raycast(ray, out hit, shootRange, shootableLayers))
            {
                StartCoroutine(SmoothLookAt(hit.point));  // Smooth rotation
            }
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

    IEnumerator ResetShootTrigger()
    {
        yield return new WaitForSeconds(0.3f); // Longer delay for smoother playback
        animator.ResetTrigger("Shoot");        // Reset AFTER animation has time to finish
    }

    IEnumerator SmoothLookAt(Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        direction.y = 0; // Keep rotation horizontal

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Increase rotation speed for faster response
        float rotationSpeed = 20f; // Increase this value for faster rotation

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Snap to final rotation for precision
        transform.rotation = targetRotation;
    }
}



