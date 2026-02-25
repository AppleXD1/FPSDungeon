using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundMask, playerMask;
    
    [Header("Patrolling")]
    public Vector3 walkpoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public bool alreadyAttack;

    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();  

    }

    void Start()
    {
        if (!agent.isOnNavMesh)
            Debug.Log("NOT ON NAVMESH");
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        

        if (!playerInSight && !playerInAttackRange)
            Patrolling();
        if(playerInSight && !playerInAttackRange)
            ChasePlayer();
        if(playerInSight && playerInAttackRange)
            AttackPlayer();
    }

    private void Patrolling()
    {
       

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkpoint);

        Vector3 distanceToWalkPoint = transform.position - walkpoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            walkpoint = hit.position;
            walkPointSet = true;
            Debug.Log("New walkpoint set on NavMesh");
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("Chase");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack");
        agent.SetDestination((Vector3)transform.position);
        transform.LookAt(player);
    }   
    
    private void ResetAttack()
    {
        alreadyAttack = false;
    }


}
