using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHit : MonoBehaviour
{
    public float damage = 50f; // Damage amount

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if fire hits the player
        {
            Player_Health playerHealth = other.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Apply damage to player
                Destroy(gameObject); // Destroy fire on impact
            }
        }
    }
}
