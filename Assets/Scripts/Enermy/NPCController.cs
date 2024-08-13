using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCController : MonoBehaviour
{
    Rigidbody2D rb;

    //public Matrix2x2 facePosition = new Matrix2x2(Mathf.PI);
    public Vector2 targetPosition;
    [SerializeField] public Vector2 posToPlayer;

    Vector2 originalPosition;
    float decisionInterval = 0.5f;

    // behavior
    public bool isAlert = false;
    public bool isTracking = false;

    // distance
    public bool isMoveRange = false;
    public bool isLongRange = false;
    public bool isCloseRange = false;
    public bool isClosestRange = false;

    public float moveSpeed = 5.0f;

    // preference
    [SerializeField] WolfStats stats;
    [SerializeField] float searchTime = 12.0f;
    [SerializeField] List<float> prefs;

    bool isFlip = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originalPosition = transform.position;
        InvokeRepeating("decideBehavior", 2f, decisionInterval);
    }

    private void Update()
    {
        posToPlayer = (targetPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        if (targetPosition != Vector2.zero)
        {
            Debug.DrawLine(transform.position, targetPosition);
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
                // tracking
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
            wander();
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
                        Vector3 movement = posToPlayer * moveSpeed * Time.deltaTime;
                        rb.MovePosition(transform.position + movement);

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
        while (Time.time < startTime + decisionInterval)
        {
            Vector3 movement = posToPlayer * moveSpeed * Time.deltaTime * new Vector2(-1, -1);
            rb.MovePosition(transform.position + movement);

            yield return null;
        }
    }

    private void attack()
    {
        Debug.Log($"{gameObject.name} attack");
    }

    private void wait()
    {
        Debug.Log($"{gameObject.name} wait");
    }

    private void wander()
    {
        Debug.Log($"{gameObject.name} wander");
    }

    public IEnumerator track()
    {
        isTracking = true;

        yield return new WaitForSeconds(searchTime);

        isAlert = false;
        isTracking = false;
    }

    private void OnDestroy()
    {
        CancelInvoke("decideBehavior");
    }
}
