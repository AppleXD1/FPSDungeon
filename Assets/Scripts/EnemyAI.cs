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
        //float distanceToTarget = Vector3.Distance(transform.position, playerObj.transform.position);



        timer += Time.deltaTime;
        
        //Check if player is in range of the AI
        if(Vector3.Distance(transform.position, playerObj.transform.position) < DetectRange)
        {
            isInRange=true;
            
        }
        else
        {
            
            isInRange =false;
            
        }
        Vector3 directionToPlayer = (playerObj.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);

        // Check range
        isInRange = distanceToPlayer < DetectRange;

        // Check angle
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        isInAngle = angle < DetectAngle;

        // Check line of sight
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer, out hit, DetectRange))
        {
            if (hit.transform.gameObject == playerObj)
            {
                isHideen = false; // player visible
            }
            else
            {
                isHideen = true; // wall blocking
            }
        }
        //Check if the player is correct angle of vision
        Vector3 side1 = playerObj.transform.position - transform.position;
        Vector3 side2 = transform.forward;
       
        float anglee = Vector3.SignedAngle(side1, side2, Vector3.up);
        if (angle < DetectAngle && angle > -DetectAngle)
        {
            isInAngle = true;
           
        }
        else
        {
            isInAngle=false;
           
        }

        if (isInRange && isInAngle && !isHideen)
        {
            AttackPlayer();
        }
        else if (isInRange && !isHideen)
        {
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
        
        Debug.Log(walkPointSet);
        if (walkPointSet)
        {
            agent.SetDestination(walkpoint);
            
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
