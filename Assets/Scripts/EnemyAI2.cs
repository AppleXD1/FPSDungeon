using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;


public class EnemyAI2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject playerObj;
    public Vector3 playerPreviousPosition;
    public float AttackRange = 10;

    [Header("VisionCheck")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public int rayCount = 10;
    public bool playerSeen;

    [Header("Phases")]
    public bool isAttack;
    public bool isCalm;
    public bool isChasing;
    public bool isSeaching;

    [Header("Patrolling")]
    public float walkPointRange;
    public Vector3 walkpoint;
    bool walkPointSet;
    public float timer;

    public LayerMask detectionLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCalm = true;
        isChasing = false;
        isAttack = false;
        isSeaching = false;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.isStopped = false;

        if (!agent.isOnNavMesh)
            Debug.LogError("Enemy is not placed on NavMesh!");
    }

    // Update is called once per frame
    void Update()
    {
        VisionCheck();

        Vector3 enemyLocation = transform.position;
        Vector3 playerTarget = playerObj.transform.position;

        float distanceToPlayer = Vector3.Distance(enemyLocation, playerTarget);

        
        
        if(distanceToPlayer <= AttackRange && playerSeen)
        {
            isChasing = false;
            isAttack = true;
        }
        else if (distanceToPlayer >= AttackRange && playerSeen)
        {
            isAttack = false;
            isChasing = true;
        }
        else if(distanceToPlayer >= AttackRange && !playerSeen)
        {
            isCalm = true;
            isAttack = false;
            isChasing=false;
        }




            timer += Time.deltaTime;
        if (playerSeen && !isAttack)
        {
            isCalm = false;
            isChasing = true;
        }

        if (isCalm && !isChasing && !isAttack)
        {
            CalmWalking();
        }
        if (!isCalm && isChasing && !isAttack)
        {
            Chasing();
        }
        if (!isCalm && !isChasing && isAttack)
        {
            Attacking();
        }



    }

    void VisionCheck() //Line of Sight
    {
        bool playerDetectedThisFrame = false;
        float startAngle = -viewAngle / 2;
        float angleStep = viewAngle / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, viewDistance, detectionLayer))
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                if (hit.collider.CompareTag("Player"))
                {
                    playerDetectedThisFrame = true;
                    Debug.Log("Player Detected!");
                    playerPreviousPosition = playerObj.transform.position;


                }


            }
            else
            {
                Debug.DrawRay(transform.position, direction * viewDistance, Color.green);
            }
        }
        playerSeen = playerDetectedThisFrame;
        


    }

    void Attacking()
    {
          
        
        Debug.Log("Attack");
    }

    void Chasing()
    {

        if (playerSeen)
        {
            agent.SetDestination(playerObj.transform.position);
        }
        else
        {
            agent.SetDestination(playerPreviousPosition);
        }


        if (!agent.pathPending && agent.remainingDistance < 0.5)
        {

            isCalm = true;
            isChasing = false;
            isSeaching = false;

        }
    }


    void CalmWalking() //Calm
    {
        
        if (!walkPointSet)
            SearchWalkPoint();


        if (!agent.pathPending && walkPointSet && agent.remainingDistance <= agent.stoppingDistance + 0.2f && timer >= 3f)
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
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, NavMesh.AllAreas))
            {
                if (Vector3.Distance(transform.position, hit.position) > 2f)
                {
                    walkpoint = hit.position;
                    walkPointSet = true;
                    agent.SetDestination(walkpoint);
                    return;
                }
            }
        }

    }
}