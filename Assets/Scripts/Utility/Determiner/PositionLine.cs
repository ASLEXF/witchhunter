using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PositionLine : Singleton<PositionLine>
{
    [SerializeField][Range(0, 1)] public float slope = 1.0f;

    LineRenderer _lineRenderer1, _lineRenderer2;

    protected override void Awake()
    {
        base.Awake();

        GameObject lineObject1 = new GameObject("Line1");
        GameObject lineObject2 = new GameObject("Line2");

        lineObject1.transform.SetParent(transform.parent);
        lineObject2.transform.SetParent(transform.parent);

        _lineRenderer1 = lineObject1.AddComponent<LineRenderer>();
        _lineRenderer2 = lineObject2.AddComponent<LineRenderer>();
    }

    private void Start()
    {
        _lineRenderer1.sortingLayerName = "Player";
        _lineRenderer1.sortingOrder = 0;
        _lineRenderer2.sortingLayerName = "Player";
        _lineRenderer2.sortingOrder = 0;
    }

    //void Update()
    //{
    //    _lineRenderer1.positionCount = 2;
    //    _lineRenderer1.SetPosition(0, transform.position + new Vector3(1, slope, 0).normalized * 10);
    //    _lineRenderer1.SetPosition(1, transform.position + new Vector3(-1, -slope, 0).normalized * 10);
    //    _lineRenderer1.startColor = Color.red;
    //    _lineRenderer1.endColor = Color.red;
    //    _lineRenderer1.startWidth = 0.05f;
    //    _lineRenderer1.endWidth = 0.05f;

    //    _lineRenderer2.positionCount = 2;
    //    _lineRenderer2.SetPosition(0, transform.position + new Vector3(1, -slope, 0).normalized * 10);
    //    _lineRenderer2.SetPosition(1, transform.position + new Vector3(-1, slope, 0).normalized * 10);
    //    _lineRenderer2.startColor = Color.red;
    //    _lineRenderer2.endColor = Color.red;
    //    _lineRenderer2.startWidth = 0.05f;
    //    _lineRenderer2.endWidth = 0.05f;
    //}

    public Vector2 GetFacePosition()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        return new Vector2(horizontalInput, verticalInput);
    }

    public Vector3 GetPosition(Vector2 vector)
    {
        if (vector.y < slope * vector.x && vector.y <= -slope * vector.x)
        {
            //Debug.Log("鼠标位置：下 ");
            return new Vector3(0, -1, 0);
        }
        else if (vector.y <= slope * vector.x && vector.y > -slope * vector.x)
        {
            //Debug.Log("鼠标位置：右 ");
            return new Vector3(1, 0, 0);
        }
        else if (vector.y >= slope * vector.x && vector.y < -slope * vector.x)
        {
            //Debug.Log("鼠标位置：左 ");
            return new Vector3(-1, 0, 0);
        }
        else
        {
            //Debug.Log("鼠标位置：上 ");
            return new Vector3(0, 1, 0);
        }
    }

    public Vector3 GetClickDirection()
    {
        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = transform.position;
        Vector3 relativePosition = clickPosition - position;

        if (relativePosition.y < slope * relativePosition.x && relativePosition.y <= -slope * relativePosition.x)
        {
            //Debug.Log("鼠标位置：下 ");
            return new Vector3(0, -1, 0);
        }
        else if (relativePosition.y <= slope * relativePosition.x && relativePosition.y > -slope * relativePosition.x)
        {
            //Debug.Log("鼠标位置：右 ");
            return new Vector3(1, 0, 0);
        }
        else if (relativePosition.y >= slope * relativePosition.x && relativePosition.y < -slope * relativePosition.x)
        {
            //Debug.Log("鼠标位置：左 ");
            return new Vector3(-1, 0, 0);
        }
        else
        {
            //Debug.Log("鼠标位置：上 ");
            return new Vector3(0, 1, 0);
        }
    }

    public Vector2 GetAttackPosition(Vector2 posToPlayer, Vector2 statsPosition)
    {
        /*
         * posToPlayer: the position of player relative to NPC, with NPC as origin
         * statsPosition
         */
        if (posToPlayer.y < slope * posToPlayer.x && posToPlayer.y <= -slope * posToPlayer.x)
        {
            return new Vector2(0, 0.5f * statsPosition.x + statsPosition.y);
        }
        else if (posToPlayer.y <= slope * posToPlayer.x && posToPlayer.y > -slope * posToPlayer.x)
        {
            return new Vector2(-statsPosition.x, statsPosition.y);
        }
        else if (posToPlayer.y >= slope * posToPlayer.x && posToPlayer.y < -slope * posToPlayer.x)
        {
            return new Vector2(statsPosition.x, statsPosition.y);
        }
        else
        {
            return new Vector2(0, -0.5f * statsPosition.x + statsPosition.y);
        }
    }
}
