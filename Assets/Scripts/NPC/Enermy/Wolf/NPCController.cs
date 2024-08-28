using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Comparers;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    NavMeshAgent agent;
    NPCStatusEffect statusEffect;

    //public Matrix2x2 facePosition = new Matrix2x2(Mathf.PI);
    public Vector2 targetPosition;
    [SerializeField] public Vector2 posToPlayer;

    Vector2 originalPosition;

    // behavior
    public bool seePlayer = false;

    // status
    public bool isWandering = false;
    public bool isTracking = false;

    // distance
    public bool isMoveRange = false;
    public bool isLongRange = false;
    public bool isCloseRange = false;
    public bool isClosestRange = false;

    // preference
    [SerializeField] public WolfStats stats;
    [SerializeField] List<float> prefs;

    // optimization
    int maxAttackNum = 3;
    int attackCounter = 0;

    private float startTime;
    public bool isFlip = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        statusEffect = transform.GetChild(1).GetComponent<NPCStatusEffect>();
    }

    private void Start()
    {
        startTime = Time.time;
        originalPosition = transform.position;

        InitializeNavMeshAgent();
    }

    private void Update()
    {
        updateTargetPosition();

        spriteRenderer.flipX = isFlip;

        if (Time.time >= startTime + stats.decisionInterval)
        {
            decideBehavior();
            startTime = Time.time;
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

    private void decideBehavior()
    {
        if (statusEffect.Dead || statusEffect.Stunned) return;
        if (isWandering || isTracking) return;
        //Debug.Log($"stop coroutines at {Time.time}");
        StopAllCoroutines();
        agent.enabled = true;
        if (seePlayer)
        {
            initializePrefs();

            float value = UnityEngine.Random.value;
            if (0 <= value && value < prefs[0])
            {
                StartCoroutine(approach());

                attackCounter = 0;
            }
            else if (prefs[0] <= value && value < prefs[1])
            {
                StartCoroutine(moveAway());

                attackCounter = 0;
            }
            else if (prefs[1] <= value && value < prefs[2])
            {
                if (attackCounter > maxAttackNum)
                {
                    StartCoroutine(moveAway());
                }
                else
                {
                    StartCoroutine(attack());
                    attackCounter += 1;
                }
            }
            else
            {
                wait();

                attackCounter = 0;
            }
        }
        else
        {
            StartCoroutine(wander()); 
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
                    while (Time.time < startTime + stats.decisionInterval)
                    {
                        animator.SetBool("IsWalking", true);
                        agent.SetDestination(targetPosition);

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
    }

    private IEnumerator moveAway()
    {
        agent.ResetPath();
        Debug.Log($"{gameObject.name} move away");
        float startTime = Time.time;
        animator.SetBool("IsWalking", true);

        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        while (Time.time < startTime + stats.decisionInterval)
        {
            Vector3 movement = posToPlayer * direction * stats.runSpeed * Time.deltaTime;
            Debug.Log($"{movement}");
            rb.MovePosition(transform.position + movement);

            yield return null;
        }

        animator.SetBool("IsWalking", false);
    }

    private IEnumerator attack()
    {
        Debug.Log($"{gameObject.name} attack");
        animator.SetTrigger(Animator.StringToHash("Bite"));
        
        yield return null;
    }

    private void wait()
    {
        Debug.Log($"{gameObject.name} wait");
        animator.SetBool("IsWalking", false);
        //wanderCoroutine = null;
    }

    private IEnumerator wander()
    {
        Debug.Log($"{gameObject.name} wander");
        targetPosition = Vector2.zero;
        isWandering = true;
        agent.speed = stats.walkSpeed;
        float startTime = Time.time;
        
        Vector2 randomDirection = UnityEngine.Random.insideUnitSphere.ToVector2() * stats.wanderRadius;
        randomDirection += originalPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, stats.wanderRadius, NavMesh.AllAreas);
        if (Vector2.Distance(originalPosition, hit.position.ToVector2()) >= stats.minDisatnce)
        {
            animator.SetBool("IsWalking", true);
            agent.SetDestination(hit.position.ToVector2());
            posToPlayer = (hit.position.ToVector2() - transform.position.ToVector2()).normalized;
        }

        while (Time.time < startTime + stats.wanderTime)
        {
            if (Mathf.Approximately(transform.position.x, hit.position.ToVector2().x) && Mathf.Approximately(transform.position.y, hit.position.ToVector2().y))
                {
                wait();
            }
            if (seePlayer)
            {
                break;
            }
            yield return null;
        }

        animator.SetBool("IsWalking", false);
        //wanderCoroutine = null;
        isWandering = false;
    }

    private IEnumerator track(Vector2 targetPosition, float time)
    {
        Debug.Log($"{gameObject.name} track");

        float startTime = Time.time;
        agent.speed = stats.runSpeed;
        agent.SetDestination(targetPosition);

        animator.SetBool("IsWalking", true);

        while (Time.time < startTime + time)
        {
            if (seePlayer)
            {
                break;
            }
            if (FloatCompare.IsApproximately(targetPosition.x, transform.position.x, 0.2f) && FloatCompare.IsApproximately(targetPosition.x, transform.position.x, 0.2f))
            {
                break;
            }
            yield return null;
        }

        isTracking = false;

        animator.SetBool("IsWalking", false);
    }

    public void SeePlayer()
    {
        seePlayer = true;
        isTracking = false;
        isWandering = false;
    }

    public void CantSeePlayer()
    {
        seePlayer = false;
        isWandering = false;
        isTracking = true;

        StartCoroutine(track(targetPosition, stats.trackTime));

    }

    public void Alert(PolygonCollider2D PlayerCollider = null)
    {
        // TODO: 对背后攻击、远程攻击的处理
        //if (source != null && targetPosition == Vector2.zero)
        //{
        //    if (Vector2.Distance(transform.position, source.position) > 5.0f)
        //    {
        //        posToPlayer = (source.position - transform.position).normalized;
        //    }
        //    else
        //    {
        //        targetPosition = source.position;
        //    }
        //}
        if (isMoveRange)
        {
            targetPosition = PlayerController.Instance.transform.position;
            SeePlayer();
        }
        else
        {
            targetPosition = PlayerController.Instance.transform.position;  //  TODO: give a position hint
            CantSeePlayer();
        }
    }

    private void updateTargetPosition()
    {
        if (statusEffect.Stunned) return;
        if (targetPosition != Vector2.zero)
        {
            posToPlayer = (targetPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
            Debug.DrawLine(transform.position, targetPosition);
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
        }
        else
        {
            updateSprite();
        }
    }

    private void updateSprite()
    {
        if (agent.velocity.x > 0)
        {
            isFlip = true;
        }
        else if (agent.velocity.x < 0)
        {
            isFlip = false;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke("decideBehavior");
    }
}
