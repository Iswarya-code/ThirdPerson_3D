using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dragon_AI : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints; // Waypoints for random movement
    private int currentWaypointIndex = 0;

    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float retreatDistance = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private float distanceToPlayer;
    private bool isAttacking = false;
    private bool playerInRange = false;
    private bool playerIsDead = false;
    private bool isPatrolling = true; //  New flag for waypoint movement

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
            MoveToNextWaypoint(); //  Start patrolling
        }
    }

    void Update()
    {

        if (player == null || playerIsDead)
        {
            StopAllActions();
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            isPatrolling = false;
            playerInRange = true;
            if (!isAttacking)
            {
                StartAttack();
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            isPatrolling = false;
            playerInRange = false;
            ChasePlayer();
        }
        else
        {
            if (!isPatrolling)
            {
                StartPatrolling(); //  Resume patrolling if out of chase range
            }
        }

        //  If patrolling and reached waypoint, go to the next one
        if (isPatrolling && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; //  Move to the next waypoint in order
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        animator.SetBool("IsRunning", true);
    }

    void StartPatrolling()
    {
        isPatrolling = true;
        agent.isStopped = false;
        MoveToNextWaypoint();
    }

    void ChasePlayer()
    {
        if (playerIsDead) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsRunning", true);
        animator.ResetTrigger("Attack");
    }

    void StartAttack()
    {
        if (!isAttacking && !playerIsDead)
        {
            isAttacking = true;
            agent.isStopped = true;
            animator.SetTrigger("Attack");
            Invoke(nameof(DealDamage), 0.5f);
        }
    }

    void DealDamage()
    {
        if (player == null || playerIsDead) return;

        Player_Health playerHealth = player.GetComponent<Player_Health>();
        if (playerHealth != null)
        {
            Debug.Log("Dragon is attacking the player!");
            playerHealth.TakeDamage(20);

            if (playerHealth.GetCurrentHealth() <= 0)
            {
                playerIsDead = true;
                OnPlayerDeath();
            }
            else
            {
                StartCoroutine(ContinueAttacking());
            }
        }
    }

    IEnumerator ContinueAttacking()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        if (playerInRange && !playerIsDead)
        {
            StartAttack();
        }
        else
        {
            Idle();
        }
    }

    void Idle()
    {
        agent.isStopped = true;
        animator.SetBool("IsRunning", false);
        isAttacking = false;
    }

    void StopAllActions()
    {
        agent.isStopped = true;
        animator.SetBool("IsRunning", false);
        animator.ResetTrigger("Attack");
        isAttacking = false;
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Stopping Dragon Attack");
        animator.SetBool("Attack", false);
        animator.SetBool("IsRunning", false);
        animator.Play("idle01", 0, 0);
        StopAllCoroutines();
        this.enabled = false;
    }
}
