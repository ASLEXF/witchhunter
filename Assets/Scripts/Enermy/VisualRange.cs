using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class VisualRange : MonoBehaviour
{
    [SerializeField][Range(5, 25)] public float distance = 12.0f;
    [SerializeField][Range(8, 48)] public int lineNum = 24;
    [SerializeField][Range(90, 180)] public float range = 90.0f;

    Vector2 basePosition;
    Vector2 facePosition;  // rotate
    bool isAlert;
    PolygonCollider2D collider;

    List<Vector2> points = new List<Vector2>();

    List<LineRenderer> lineRenderers = new List<LineRenderer>();
    List<LineRenderer> edges = new List<LineRenderer>();

    private void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        syncValues();

        addPoints();

        genCollider();

        //displayColliderRegion();
    }

    private void syncValues()
    {
        basePosition = transform.position;
        facePosition = transform.GetComponentInParent<NPCController>().facePosition.normalized;
        isAlert = transform.GetComponentInParent<NPCController>().isAlert;

        //if (facePosition.x * facePosition.y < 0)
        //{
        //    range = 180.0f - range;
        //}
    }

    private void addPoints()
    {
        if (lineNum < points.Count)
        {
            int count = 0;
            for (; count < lineNum; count++)
            {
                points.Add(getCollidedPoint(range / 2 - range / (lineNum - 1) * count));
            }
            for (; count < points.Count;)
            {
                points.RemoveAt(count);
            }
        }
        else
        {
            int count = 0;
            for (; count < points.Count; count++)
            {
                points[count] = getCollidedPoint(range / 2 - range / (lineNum - 1) * count);
            }
            for (; count < lineNum; count++)
            {
                points.Add(getCollidedPoint(range / 2 - range / (lineNum - 1) * count));
            }
        }
    }

    private Vector2 getCollidedPoint(float angle)
    {
        Vector2 direction;
        if (angle == 90.0f)
        {
            direction = new Vector2(0, 1);
        }
        else if (angle == -90.0f)
        {
            direction = new Vector2(0, -1);
        }
        else
        {
            direction = (new Vector2(1, Mathf.Tan(angle / 180.0f * Mathf.PI)) * facePosition).normalized;
        }

        Vector2 point = new Vector2();

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Default");
        filter.useTriggers = false;
        filter.useLayerMask = false;
        filter.useDepth = false;
        filter.useNormalAngle = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(basePosition, direction, distance, filter.layerMask);

        //Debug.DrawRay(basePosition, direction * distance, Color.blue);  // debug

        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                continue;
            }
            else if (hit.collider.CompareTag("NPC"))
            {
                continue;
            }
            // add ignored tags
            else
            {
                point = hit.point;
                break;
            }
        }

        if (point.Equals(Vector2.zero))
        {
            point = basePosition + direction * distance;
        }

        return point;
    }

    private void genCollider()
    {
        List<Vector2> colliderPoints = new List<Vector2>();

        colliderPoints.Add(new Vector2(0, 0));
        foreach (Vector2 point in points)
        {
            colliderPoints.Add(transform.InverseTransformPoint(point));
        }

        collider.points = colliderPoints.ToArray();
    }

    private void displayColliderRegion()
    {
        Color color;
        if (isAlert)
        {
            color = Color.red;
        }
        else
        {
            color = Color.green;
        }
        
        for (int i = 0; i < points.Count - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], color);
        }

        Debug.DrawLine(basePosition, points[0], color);
        Debug.DrawLine(basePosition, points[points.Count - 1], color);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (transform.GetComponentInParent<NPCController>().isTracking)
            {
                StopCoroutine(transform.GetComponentInParent<NPCController>().track());
            }
            transform.GetComponentInParent<NPCController>().isAlert = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.GetComponentInParent<NPCController>().targetPosition = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(transform.GetComponentInParent<NPCController>().track());
        }
    }
}
