using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class VisualRange : MonoBehaviour
{
    NPCController controller;

    [SerializeField][Range(5, 25)] public float distance = 12.0f;
    [SerializeField][Range(8, 48)] public int lineNum = 24;
    [SerializeField][Range(90, 180)] public float range = 90.0f;

    Vector2 facePosition;  // rotate
    Vector2 posToPlayer;  // rotate

    PolygonCollider2D collider;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    List<Vector2> points = new List<Vector2>();

    List<LineRenderer> lineRenderers = new List<LineRenderer>();
    List<LineRenderer> edges = new List<LineRenderer>();

    private void Awake()
    {
        controller = transform.parent.GetComponentInParent<NPCController>();
        collider = GetComponent<PolygonCollider2D>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        syncValues();

        addPoints();

        genCollider();

        displayColliderRegion();
    }

    private void syncValues()
    {
        //facePosition = transform.GetComponentInParent<NPCController>().facePosition;
        posToPlayer = controller.posToPlayer;
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
            direction = (new Matrix2x2(Mathf.Atan2(posToPlayer.y, posToPlayer.x)) * new Vector2(1, Mathf.Tan(angle / 180.0f * Mathf.PI))).normalized;
        }

        Vector2 point = new Vector2();

        //ContactFilter2D filter = new ContactFilter2D();
        //filter.layerMask = LayerMask.GetMask("Player");
        //filter.useTriggers = false;
        //filter.useLayerMask = false;
        //filter.useDepth = false;
        //filter.useNormalAngle = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance, LayerMask.GetMask("Default"));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
            {
                continue;
            }

            if (hit.collider.CompareTag("Player"))
            {
                continue;
            }
            else if (hit.collider.CompareTag("NPC"))
            {
                continue;
            }
            else if (hit.collider.CompareTag("Enermy"))
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
            point = new Vector2(transform.position.x, transform.position.y) + direction * distance;
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
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[collider.points.Length];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = collider.points[i];
        }

        Triangulator triangulator = new Triangulator(collider.points);
        int[] indices = triangulator.Triangulate();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.name == "Player")
            {
                controller.Alert();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.name == "Player")
            {
                if (!controller.seePlayer)
                {
                    controller.Alert();
                }
                controller.targetPosition = getColliderCenter(collider);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            controller.Track();
        }
    }

    private Vector2 getColliderCenter(Collider2D collider)
    {
        PolygonCollider2D polygonCollider = collider as PolygonCollider2D;

        Vector2[] points = polygonCollider.points;

        Vector2[] worldPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            worldPoints[i] = polygonCollider.transform.TransformPoint(points[i]);
        }

        Vector2 center = Vector2.zero;
        foreach (var point in worldPoints)
        {
            center += point;
        }
        center /= points.Length;

        return center;
    }
}
