using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Vector2 facePosition = new Vector2(-1, 1);
    public bool isAlert = false;
    public bool isTracking = false;
    public Vector2 targetPosition;

    [SerializeField] float searchTime = 12.0f;
    

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
                // moving
                // find path
            }
        }
        else
        {
            // wandering
        }
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
