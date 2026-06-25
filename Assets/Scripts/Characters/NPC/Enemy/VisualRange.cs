using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class VisualRange : MonoBehaviour
{
    //NPCController? controller;
    EnemyAIController controller;
    PolygonCollider2D _collider;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    private float range;
    private float distance;
    private int lineNum;
    private float angleSpeed;

    Vector2 lookDirection;  // rotate

    List<Vector2> points = new List<Vector2>();

    List<LineRenderer> lineRenderers = new List<LineRenderer>();
    List<LineRenderer> edges = new List<LineRenderer>();

    private void Awake()
    {
        //controller = transform.parent.parent.GetComponent<NPCController>();
        controller = transform.parent.parent.GetComponent<EnemyAIController>();
        _collider = GetComponent<PolygonCollider2D>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        range = controller.Stats.sightAngle;
        distance = controller.Stats.sightDistance;
        lineNum = controller.Stats.sightLineNum;
        angleSpeed = controller.Stats.sightAngleSpeed;
    }

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
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
        if (controller.SeePlayer)
        {
            // look at player
            lookDirection = controller.LookPosition;
        }
        else if (controller.IsWandering)
        {
            // look at moving direction
            lookDirection = Vector2Utility.RotateTowards(lookDirection, controller.Agent.velocity.normalized, angleSpeed * Mathf.Deg2Rad * Time.deltaTime);
        }
        else
        {
            // reset lookDirection to forward when idle
            if (controller.transform.rotation == Quaternion.Euler(0, 180, 0)
                || controller.transform.rotation == Quaternion.Euler(0, -180, 0))
            {
                lookDirection = Vector2Utility.RotateTowards(lookDirection, Vector2.right, angleSpeed * Mathf.Deg2Rad * Time.deltaTime);
            }
            else if (controller.transform.rotation == Quaternion.Euler(0, 0, 0))
            {
                lookDirection = Vector2Utility.RotateTowards(lookDirection, Vector2.left, angleSpeed * Mathf.Deg2Rad * Time.deltaTime);
            }
                
        }
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
        // NPC 2D vision
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
            direction = (new Matrix2x2(Mathf.Atan2(lookDirection.y, lookDirection.x)) * new Vector2(1, Mathf.Tan(angle / 180.0f * Mathf.PI))).normalized;
        }

        Vector2 point = new Vector2();

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance, LayerMask.GetMask("Default"));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Cover"))
            {
                point = hit.point;
                break;
            }
            else if (hit.collider.isTrigger)
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
            else if (hit.collider.CompareTag("Enemy"))
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

        _collider.points = colliderPoints.ToArray();
    }

    private void displayColliderRegion()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[_collider.points.Length];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = _collider.points[i];
        }

        Triangulator triangulator = new Triangulator(_collider.points);
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
                //controller?.SeePlayer();
                controller?.CanSeePlayer(true, collider.transform.position);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.name == "Player")
            {
                //if (!(controller?.seePlayer ?? false))
                //{
                //    controller?.SeePlayer();
                //}
                controller?.CanSeePlayer(true, collider.transform.position);
                //if (controller != null)
                //    controller.TargetPosition = getColliderCenter();
                if (controller != null)
                    controller.TargetPosition = getColliderCenter();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //controller?.CantSeePlayer();
            controller?.CanSeePlayer(false);
        }
    }

    private Vector2 getColliderCenter()
    {
        PolygonCollider2D polygonCollider = _collider;

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
