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
    public bool hasDealtDamage;


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

    [Header("Other Scripts/other stuff")]
    public bool isStunned;
    public float stunTimmer;
    public Swords swords;
    public FPSBody body;
    public HitPlayer hitBox;
    public Animator animator;

    public LayerMask detectionLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        body = player.GetComponent<FPSBody>();
        playerObj = GameObject.FindWithTag("Player");
        GameObject swordObj = GameObject.FindWithTag("Sword");
        swords = swordObj.GetComponent<Swords>();
        hitBox = GameObject.FindWithTag("HitBox").GetComponent<HitPlayer>();

    }

    void Start()
    {

        isCalm = true;
        isChasing = false;
        isAttack = false;
        isSeaching = false;
        stunTimmer = 3f;

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

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }


        if (distanceToPlayer <= AttackRange && playerSeen)
        {
            isChasing = false;
            isAttack = true;
        }
        else if (distanceToPlayer >= AttackRange && playerSeen)
        {
            isAttack = false;
            isChasing = true;
        }
        else if (distanceToPlayer >= AttackRange && !playerSeen)
        {
            isCalm = true;
            isAttack = false;
            isChasing = false;
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
        if (isStunned)
        {
            animator.SetBool("IsStunned", true);
            Stunned();
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
        // Debug.Log("Attack");

        if (!animator.GetBool("isAttacking"))
        {
            agent.isStopped = true;
            animator.SetBool("isAttacking", true);
            hasDealtDamage = false;
            StartCoroutine(DamagePlayer(0.5f)); 
        }
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


    void CalmWalking()
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (body != null && body.isAttacking && !isStunned)
            {
                isStunned = true;
                swords.Durability--;
                
            }
        }
    }
    void Stunned()
    {
        agent.enabled = false;
        stunTimmer -= Time.deltaTime;

        if (stunTimmer <= 0f)
        {
            agent.enabled = true;
            isStunned = false;
            stunTimmer = 3f;
            animator.SetBool("IsStunned", false);
        }
    }


    IEnumerator DamagePlayer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        

        if (stateInfo.IsName("Punch") && hitBox.hitPlayer && !hasDealtDamage)
        {
            Debug.Log("hit");
            hasDealtDamage = true;
            hitBox.hitPlayer = false;
            body.Health--;
        }

        animator.SetBool("isAttacking", false);
        agent.isStopped = false;
    }

}