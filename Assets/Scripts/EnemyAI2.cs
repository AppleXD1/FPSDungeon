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

        if (!agent.isOnNavMesh)
            Debug.LogError("Enemy is not placed on NavMesh!");
    }

    // Update is called once per frame
    void Update()
    {
        VisionCheck();
        timer += Time.deltaTime;
        if (playerSeen)
        {
            isCalm = false;
            isChasing = true;
        }
        else if (!playerSeen && isChasing)
        {
            agent.SetDestination(playerPreviousPosition);
        }
        if (isCalm && !isChasing && !isAttack)
        {
            CalmWalking();
        }
        if(!isCalm && isChasing && !isAttack)
        {
            Chasing();
        }
        if(!isCalm && !isChasing && isAttack)
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

                if (hit.collider.CompareTag("Player") && isCalm)
                {
                    playerDetectedThisFrame = true;
                    Debug.Log("Player Detected!");
                    playerPreviousPosition = playerObj.transform.position;
                    Debug.Log(playerDetectedThisFrame);
                  
                }
                if (!hit.collider.CompareTag("Player"))
                {
                    playerDetectedThisFrame = false;

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
        Vector3 enemyLocation = transform.position;
        Vector3 playerTarget = playerObj.transform.position;

        float distanceToPlayer = Vector3.Distance(enemyLocation, playerTarget);
        if(distanceToPlayer < AttackRange)
        {
            Debug.Log("In range");
        }
        Debug.Log("Attack");
    }

    void Chasing()
    {
        
        if(playerSeen)
        {
            agent.SetDestination(playerObj.transform.position);
        }
        else
        {
            agent.SetDestination(playerPreviousPosition);
        }


        if (!agent.pathPending && agent.remainingDistance < 0.5)
        {
            playerSeen = false;
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
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            walkpoint = hit.position;
            walkPointSet = true;

            agent.SetDestination(walkpoint);
        }
    }

    
}
