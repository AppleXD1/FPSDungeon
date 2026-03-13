using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject playerObj;
    public float DetectRange = 10;
    public float DetectAngle = 45;
    
    [Header("Patrolling")]
    public Vector3 walkpoint;
    bool walkPointSet;
    public float walkPointRange;
    public float timer;

    [Header("Attacking")]
    public bool alreadyAttack;

    public bool isInRange, isInAngle, isHideen;
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
       
        agent = GetComponent<NavMeshAgent>();  

    }

    void Start()
    {
        if (!agent.isOnNavMesh)
            UnityEngine.Debug.Log("NOT ON NAVMESH");
    }

    // Update is called once per frame
    void Update()
    {
        isInRange = false;
        isInAngle = false;
        isHideen = false;
        
        Vector3 enemyEye = transform.position + Vector3.up * 1.0f;
        Vector3 playerTarget = playerObj.transform.position + Vector3.up * 1.0f;

        Vector3 directionToPlayer = (playerTarget - enemyEye).normalized;
        float distanceToPlayer = Vector3.Distance(enemyEye, playerTarget);

        // Range check
        isInRange = distanceToPlayer <= DetectRange;

        // Angle check
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        isInAngle = angle <= DetectAngle;

        // Line of sight check
        RaycastHit hit;
        if (Physics.Raycast(enemyEye, directionToPlayer, out hit, DetectRange))
        {
            Debug.DrawRay(enemyEye, directionToPlayer * DetectRange, Color.red);

            isHideen = hit.transform.root.gameObject != playerObj;
        }
        else
        {
            isHideen = true;
        }

        // State logic
        if (isInRange && isInAngle && !isHideen)
        {
            if (distanceToPlayer <= attackRange)
                AttackPlayer();
            else
                ChasePlayer();
        }
        else
        {
            Patrolling();
        }

    }

    private void Patrolling()
    {

        Debug.Log("patrol");
        if (!walkPointSet)
            SearchWalkPoint();
        
       
        if (!agent.pathPending && walkPointSet && agent.remainingDistance <= agent.stoppingDistance + 0.2f && timer >= 2f)
        {
            walkPointSet = false;
            timer = 0;
        }


        Vector3 distanceToWalkPoint = transform.position - walkpoint;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3( transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            walkpoint = hit.position;
            walkPointSet = true;

            agent.SetDestination(walkpoint);
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("chase");
        UnityEngine.Debug.Log("Chase");
        agent.stoppingDistance = 1.5f;
        agent.updateRotation = true;
        agent.SetDestination(playerObj.transform.position);
    }

    private void AttackPlayer()
    {
        UnityEngine.Debug.Log("Attack");
        agent.SetDestination((Vector3)transform.position);
        transform.LookAt(playerObj.transform.position);
    }   
    
    private void ResetAttack()
    {
        alreadyAttack = false;
    }


}
