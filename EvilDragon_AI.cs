using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EvilDragon_AI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 15f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isAttacking = false;
    private bool isChasingPlayer = false;

    [Header("Fire Attack")]
    public GameObject firePrefab; // Fire prefab
    public Transform fireSpawnPoint; // Fire spawn location (mouth)
    public float fireDestroyTime = 2f;

    [Header("Waypoints")]
    public Transform[] waypoints; // Assign waypoints in the Inspector
    private int currentWaypointIndex = 0;

    private bool isPlayerDead = false; // Add this flag at the top


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    void Update()
    {
        if (isPlayerDead) return; // Prevent any movement or attacking

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            PatrolWaypoints();
        }
    }

    // Move Dragon to the next waypoint
    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        animator.SetBool("IsRunning", true);
    }

    void PatrolWaypoints()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            MoveToNextWaypoint();
        }
    }

    void ChasePlayer()
    {
        isChasingPlayer = true;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsRunning", true);
    }

    IEnumerator AttackPlayer()
    {
        if (isPlayerDead) yield break; // Stop attack if player is dead

        isAttacking = true;
        isChasingPlayer = false;
        agent.isStopped = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);
        SpawnFire();

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        agent.isStopped = false; // Allow movement again
    }

    void SpawnFire()
    {
        /* if (firePrefab != null && fireSpawnPoint != null)
         {
             GameObject fire = Instantiate(firePrefab, fireSpawnPoint.position, fireSpawnPoint.rotation);
             fire.transform.parent = fireSpawnPoint; // Attach to mouth
             Destroy(fire, fireDestroyTime);
         }
         else
         {
             Debug.LogError("FirePrefab or FireSpawnPoint is missing! Assign them in the Inspector.");
         }*/

        if (isPlayerDead) return; // Prevent fire spawning if player is dead

        if (firePrefab != null && fireSpawnPoint != null && player != null)
        {
            GameObject fire = Instantiate(firePrefab, fireSpawnPoint.position, Quaternion.identity);

            Vector3 direction = (player.position - fireSpawnPoint.position).normalized;
            fire.transform.rotation = Quaternion.LookRotation(direction);

            Rigidbody rb = fire.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // Fire speed
            }
            else
            {
                Debug.LogError("FirePrefab needs a Rigidbody component!");
            }

            Destroy(fire, fireDestroyTime);
        }
    }

    public void OnPlayerDeath()
    {
        if (isPlayerDead) return; // Prevent multiple calls
        isPlayerDead = true;

        Debug.Log("Player is dead! Dragon stops attacking.");

        // Stop movement and go idle
        agent.isStopped = true;
        animator.SetBool("IsRunning", false);
        animator.ResetTrigger("Attack");
        animator.Play("Idle01", 0, 0);

        // Stop fire particle system
        if (firePrefab != null)
        {
            ParticleSystem fireEffect = firePrefab.GetComponent<ParticleSystem>();
            if (fireEffect != null)
            {
                fireEffect.Stop();
                fireEffect.Clear(); // Optional: Clears existing particles
            }
        }

        // Stop all coroutines & prevent further attacks
        isAttacking = false;
        StopAllCoroutines();
    }
}
