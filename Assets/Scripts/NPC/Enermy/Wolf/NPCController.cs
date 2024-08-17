using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    //public Matrix2x2 facePosition = new Matrix2x2(Mathf.PI);
    public Vector2 targetPosition;
    [SerializeField] public Vector2 posToPlayer;

    Vector2 originalPosition;

    // behavior
    public bool isAlert = false;
    public bool isTracking = false;

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
    }

    private void Start()
    {
        originalPosition = transform.position;
        InvokeRepeating("decideBehavior", 2f, stats.decisionInterval);
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

    private void decideBehavior()
    {
        if (isAlert)
        {
            if (isTracking)
            {
                StartCoroutine(track());
            }
            else
            {
                initializePrefs();

                float value = Random.value;
                if (0 <= value && value < prefs[0])
                {
                    StartCoroutine(approach());
                }
                else if (prefs[0] <= value && value < prefs[1])
                {
                    StartCoroutine(stayOff());
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

        //yield break;
    }

    private IEnumerator stayOff()
    {
        float startTime = Time.time;
        while (Time.time < startTime + stats.decisionInterval)
        {
            Vector3 movement = posToPlayer * stats.runSpeed * Time.deltaTime * new Vector2(-1, -1);
            rb.MovePosition(transform.position + movement);
            animator.SetBool("IsWalking", true);

            yield return null;
        }
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
    }

    private IEnumerator wander()
    {
        Debug.Log($"{gameObject.name} wander");

        float startTime = Time.time;
        float randomX = Random.value - 0.5f;

        if (randomX < 0.2)
        {
            wait();

            yield break;
        }

        float randomY = Random.value - 0.5f;
        Vector2 randomDirection = new Vector2( randomX, randomY).normalized;
        Vector2 directionToOriginalPosition = (originalPosition - transform.position.ToVector2()).normalized;

        float currentDistance = (transform.position.ToVector2() - originalPosition).magnitude;

        Vector2 direction = ((stats.wanderDistance - currentDistance) * randomDirection + currentDistance * directionToOriginalPosition).normalized;

        float waitTime = Random.value / 2;
        while (Time.time < startTime + stats.decisionInterval)
        {
            Vector3 deltaMovement = direction * stats.runSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + deltaMovement);
            animator.SetBool("IsWalking", true);

            yield return null;
        }

        yield return null;

        animator.SetBool("IsWalking", false);
    }

    public IEnumerator track()
    {
        isTracking = true;

        yield return new WaitForSeconds(stats.searchTime);

        isAlert = false;
        isTracking = false;
    }

    private void OnDestroy()
    {
        CancelInvoke("decideBehavior");
    }
}
