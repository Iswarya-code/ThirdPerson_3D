using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class Player_Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthBar; // Assign a UI Slider in the Inspector
    public Animator animator; // Assign the player's Animator

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return; // Prevent negative health

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();

        // Ensure pain animation plays every time
        animator.ResetTrigger("Pain");
        animator.SetTrigger("Pain");
        animator.Play("Player_Pain", 0, 0f); // Force animation restart

        Debug.Log("Player took damage! Playing pain animation.");

        StartCoroutine(ResetPainTrigger()); //  Reset trigger after animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //  Coroutine to reset trigger after animation plays
    IEnumerator ResetPainTrigger()
    {
        yield return new WaitForSeconds(0.5f); // Adjust based on Pain animation length
        animator.ResetTrigger("Pain"); // Reset trigger to allow retriggering
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    void Die()
    {
        animator.SetTrigger("Die"); // Play death animation
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<CharacterController>().enabled = false; // Disable CharacterController

        Debug.Log("Player died! Playing death animation.");

        // Notify all dragons that player is dead
        Dragon_AI[] dragons = FindObjectsOfType<Dragon_AI>();
        foreach (var dragon in dragons)
        {
            Debug.Log("Stopping Dragon Attack"); // Debug to check if this runs
            dragon.OnPlayerDeath();
        }
        // Notify all EvilDragons separately
        EvilDragon_AI[] evilDragons = FindObjectsOfType<EvilDragon_AI>();
        foreach (var evilDragon in evilDragons)
        {
            evilDragon.OnPlayerDeath();
        }
        StartCoroutine(FallToGround()); // Ensure player lands properly
    }

    IEnumerator FallToGround()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller == null) yield break;

        float gravity = -9.81f;
        Vector3 velocity = Vector3.zero;

        while (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime); // Use Move() instead of modifying position directly

            yield return null;
        }

        // Snap to ground to avoid floating or sinking
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            transform.position = hit.point;
        }
    }

    //  Getter method for health (Fixes "currentHealth" error in Dragon AI)
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

}
