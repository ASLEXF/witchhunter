using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Vector2 facePosition = new Vector2(-1, 1);
    public Vector2 targetPosition;

    Vector2 originalPosition;

    // behavior
    public bool isAlert = false;
    public bool isTracking = false;

    // distance
    public bool isOrbit = false;

    // preference
    [SerializeField][Range(0, 1)] float moveClosePref = 0.7f;
    [SerializeField][Range(0, 1)] float moveAwayPref = 0.1f;
    [SerializeField][Range(0, 1)] float attackPref = 0.2f;
    [SerializeField][Range(0, 1)] float waitPref = 0f;

    [SerializeField] float searchTime = 12.0f;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (isAlert)
        {
            if (isTracking)
            {
                // tracking
            }
            else
            {
                // preference: move, wait
                // find path
                // orbit range
                // preference: move, attack, wait
            }
        }
        else
        {
            // wandering
        }
    }

    private void findPath(Vector2 position)
    {

    }

    public IEnumerator move()
    {
        yield return null;
    }

    public IEnumerator track()
    {
        isTracking = true;

        yield return new WaitForSeconds(searchTime);

        isAlert = false;
        isTracking = false;
    }
}
