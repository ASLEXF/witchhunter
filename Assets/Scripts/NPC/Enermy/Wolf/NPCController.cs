using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    NavMeshAgent agent;

    //public Matrix2x2 facePosition = new Matrix2x2(Mathf.PI);
    public Vector2 targetPosition;
    [SerializeField] public Vector2 posToPlayer;

    Vector2 originalPosition;

    // behavior
    float decisionInterval;
    public bool isAlert = false;
    public bool isTracking = false;
    public bool isWandering = false;

    // distance
    public bool isMoveRange = false;
    public bool isLongRange = false;
    public bool isCloseRange = false;
    public bool isClosestRange = false;

    // preference
    [SerializeField] public WolfStats stats;
    [SerializeField] List<float> prefs;

    public bool isFlip = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        originalPosition = transform.position;
        decisionInterval = stats.decisionInterval;

        InitializeNavMeshAgent();
        InvokeRepeating("decideBehavior", 2f, decisionInterval);
    }

    private void Update()
    {
        posToPlayer = (targetPosition - new Vector2(transform.position.x, transform.position.y)).normalized;

        if (targetPosition != Vector2.zero)
        {
            Debug.DrawLine(transform.position, targetPosition);
        }

        if (posToPlayer.x > 0)
        {
            spriteRenderer.flipX = true;
            isFlip = true;
        }
        else
        {
            spriteRenderer.flipX = false;
            isFlip = false;
        }

        //if (isTracking)
        //{
        //    float angle = facePosition.getDegree();
        //    float targetY = targetPosition.y - transform.position.y;
        //    float targetX = targetPosition.x - transform.position.x;

        //    if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, Mathf.PI))
        //    {
        //        facePosition = new Matrix2x2(targetY < 0 ? -Mathf.PI / 2 : Mathf.PI / 2);
        //    }
        //    else if (Mathf.Approximately(angle, Mathf.PI / 2) || Mathf.Approximately(angle, -Mathf.PI / 2))
        //    {
        //        facePosition = new Matrix2x2(targetX < 0 ? Mathf.PI : 0.0f);
        //    }
        //}
    }

    private void InitializeNavMeshAgent()
    {
        agent.speed = stats.walkSpeed;
        agent.angularSpeed = 0;
        agent.acceleration = stats.Acceleration;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private Coroutine trackCoroutine = null;
    private Coroutine approachCoroutine = null;
    private Coroutine stayOffCoroutine = null;
    private Coroutine wanderCoroutine = null;

    private void decideBehavior()
    {
        if (isWandering || isTracking) return;
        Debug.Log($"stop coroutines at {Time.time}");
        StopAllCoroutines();
        if (isAlert)
        {
            if (isTracking)
            {
                trackCoroutine = StartCoroutine(track());
            }
            else
            {
                initializePrefs();

                float value = UnityEngine.Random.value;
                if (0 <= value && value < prefs[0])
                {
                    approachCoroutine = StartCoroutine(approach());
                }
                else if (prefs[0] <= value && value < prefs[1])
                {
                    stayOffCoroutine = StartCoroutine(stayOff());
                }
                else if (prefs[1] <= value && value < prefs[2])
                {
                    attack();
                }
                else
                {
                    wait();
                }
            }
        }
        else
        {
            wanderCoroutine = StartCoroutine(wander()); 
        }
    }

    private void initializePrefs()
    {
        if (isClosestRange)
        {
            prefs = new List<float>()
                    {
                        stats.closestRange_moveClose,
                        stats.closestRange_moveClose + stats.closestRange_moveAway,
                        stats.closestRange_moveClose + stats.closestRange_moveAway + stats.closestRange_attack,
                        1
                    };
        }
        else if (isCloseRange)
        {
            prefs = new List<float>()
                    {
                        stats.closeRange_moveClose,
                        stats.closeRange_moveClose + stats.closeRange_moveAway,
                        stats.closeRange_moveClose + stats.closeRange_moveAway + stats.closeRange_attack,
                        1
                    };
        }
        else if (isLongRange)
        {
            prefs = new List<float>()
                    {
                        stats.longRange_moveClose,
                        stats.longRange_moveClose + stats.longRange_moveAway,
                        stats.longRange_moveClose + stats.longRange_moveAway + stats.longRange_attack,
                        1
                    };
        }
        else if (isMoveRange)
        {
            prefs = new List<float>()
                    {
                        stats.moveRange_moveClose,
                        stats.moveRange_moveClose + stats.moveRange_moveAway,
                        stats.moveRange_moveClose + stats.moveRange_moveAway + stats.moveRange_attack,
                        1
                    };
        }
        else
        {
            prefs = new List<float>()
                    {
                        stats.normal_moveClose,
                        stats.normal_moveClose + stats.normal_moveAway,
                        stats.normal_moveClose + stats.normal_moveAway + stats.normal_attack,
                        1
                    };
        }
    }

    private IEnumerator approach()
    {
        float startTime = Time.time;

        switch (stats.approachMethod)
        {
            case ApproachMethod.straight:
                {
                    while (Time.time < startTime + decisionInterval)
                    {
                        Vector3 movement = posToPlayer * stats.runSpeed * Time.deltaTime;
                        rb.MovePosition(transform.position + movement);
                        animator.SetBool("IsWalking", true);

                        yield return null;
                    }
                    break;
                }
            case ApproachMethod.curve:
                {
                    break;
                }
            case ApproachMethod.back:
                {
                    break;
                }
            case ApproachMethod.detour:
                {
                    break;
                }
        }

        approachCoroutine = null;
        //yield break;
    }

    private IEnumerator stayOff()
    {
        float startTime = Time.time;
        while (Time.time < startTime + decisionInterval)
        {
            Vector3 movement = posToPlayer * stats.runSpeed * Time.deltaTime * new Vector2(-1, -1);
            rb.MovePosition(transform.position + movement);
            animator.SetBool("IsWalking", true);

            yield return null;
        }

        animator.SetBool("IsWalking", false);
        stayOffCoroutine = null;
    }

    private void attack()
    {
        Debug.Log($"{gameObject.name} attack");
        animator.SetTrigger(Animator.StringToHash("Bite"));
    }

    private void wait()
    {
        Debug.Log($"{gameObject.name} wait");
        animator.SetBool("IsWalking", false);
        wanderCoroutine = null;
    }

    private IEnumerator wander()
    {
        Debug.Log($"{gameObject.name} wander");
        isWandering = true;
        agent.enabled = true;
        agent.speed = stats.walkSpeed;
        float startTime = Time.time;
        
        Vector2 randomDirection = UnityEngine.Random.insideUnitSphere.ToVector2() * stats.wanderRadius;
        randomDirection += originalPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, stats.wanderRadius, NavMesh.AllAreas);
        if (Vector2.Distance(originalPosition, hit.position.ToVector2()) >= stats.minDisatnce)
        {
            Debug.Log($"{gameObject.name} wander walk {hit.position.ToVector2()}");
            animator.SetBool("IsWalking", true);
            agent.SetDestination(hit.position.ToVector2());
            posToPlayer = hit.position.ToVector2();
        }

        while (Time.time < startTime + stats.wanderTime)
        {
            if (Mathf.Approximately(transform.position.x, hit.position.ToVector2().x) && Mathf.Approximately(transform.position.y, hit.position.ToVector2().y))
                {
                wait();
            }
            if (isAlert)
            {
                agent.enabled = false;
                break;
            }
            yield return null;
        }

        agent.enabled = false;
        animator.SetBool("IsWalking", false);
        wanderCoroutine = null;
        isWandering = false;
    }

    private IEnumerator track()
    {
        Debug.Log($"{gameObject.name} track");
        agent.enabled = true;
        agent.speed = stats.runSpeed;
        float startTime = Time.time;

        agent.SetDestination(targetPosition);
        animator.SetBool("IsWalking", true);

        while (Time.time < startTime + stats.trackTime)
        {
            if (isAlert)
            {
                agent.enabled = false;
                break;
            }
            yield return null;
        }

        if (isTracking)
        {
            isAlert = false;
            isTracking = false;
        }
        
        agent.enabled = false;
        animator.SetBool("IsWalking", false);
        trackCoroutine = null;
    }

    public void Alert()
    {
        isAlert = true;
        isTracking = false;
        isWandering = false;
        agent.enabled = false;
        decisionInterval = stats.decisionInterval;
    }

    private void setPositionAndSprite(Vector2 targetPostion)
    {
        agent.SetDestination(targetPostion);


    }

    private void OnDestroy()
    {
        CancelInvoke("decideBehavior");
    }
}
