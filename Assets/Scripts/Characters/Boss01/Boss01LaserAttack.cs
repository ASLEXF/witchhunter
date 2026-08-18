using System.Collections;
using UnityEngine;

public enum Boss01LaserAimMode
{
    StartToEnd = 0,
    TowardPlayer = 1,
    Facing = 2,
    Custom = 3,
}

/// <summary>
/// 从起点到终点打出一条矩形激光，持续结算伤害。
/// </summary>
public class Boss01LaserAttack : MonoBehaviour
{
    [Header("端点")]
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] Transform muzzle;
    [SerializeField] Boss01LaserAimMode aimMode = Boss01LaserAimMode.TowardPlayer;
    [SerializeField] Vector2 customDirection = Vector2.left;
    [SerializeField] float maxLength = 12f;
    [SerializeField] float startOffset = 0.4f;

    [Header("激光")]
    [SerializeField] GameObject beamPrefab;
    [SerializeField] float width = 0.6f;
    [SerializeField] float duration = 1.6f;
    [SerializeField] float tickInterval = 0.35f;
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 0.1f;
    [SerializeField] Color beamColor = new Color(1f, 0.2f, 0.15f, 0.7f);
    [SerializeField] bool followAimWhileActive = false;

    [Header("墙体截断（可选）")]
    [SerializeField] bool stopAtWalls = false;
    [SerializeField] LayerMask wallMask = ~0;

    public bool IsFiring { get; private set; }

    Coroutine _routine;
    Boss01LaserBeam _beam;
    GameObject _owner;

    void Awake()
    {
        if (muzzle == null)
            muzzle = transform;
        if (startPoint == null)
            startPoint = muzzle;
        _owner = transform.root.gameObject;
    }

    public void Fire()
    {
        Vector2 start;
        Vector2 end;
        ResolveSegment(out start, out end);
        Fire(start, end, duration);
    }

    public void FireTowardPlayer()
    {
        Vector2 start = GetStart();
        Vector2 end = ClampEnd(start, DirectionToPlayer());
        Fire(start, end, duration);
    }

    public void Fire(Vector2 start, Vector2 end)
    {
        Fire(start, end, duration);
    }

    public void Fire(Vector2 start, Vector2 end, float beamDuration)
    {
        if (!isActiveAndEnabled)
            return;

        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(FireRoutine(start, end, beamDuration));
    }

    public void StopLaser()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (_beam != null)
        {
            _beam.Deactivate();
            _beam = null;
        }

        IsFiring = false;
    }

    IEnumerator FireRoutine(Vector2 start, Vector2 end, float beamDuration)
    {
        IsFiring = true;
        float hold = Mathf.Max(0.01f, beamDuration);
        float endTime = Time.time + hold;

        SpawnOrReuseBeam(start, end, hold);

        while (Time.time < endTime)
        {
            if (followAimWhileActive)
            {
                Vector2 nextStart;
                Vector2 nextEnd;
                ResolveSegment(out nextStart, out nextEnd);
                if (_beam != null)
                    _beam.Retarget(nextStart, nextEnd);
            }

            if (_beam == null || !_beam.IsActive)
                break;

            yield return null;
        }

        if (_beam != null)
        {
            _beam.Deactivate();
            _beam = null;
        }

        IsFiring = false;
        _routine = null;
    }

    void SpawnOrReuseBeam(Vector2 start, Vector2 end, float beamDuration)
    {
        if (_beam != null)
        {
            _beam.Deactivate();
            _beam = null;
        }

        GameObject go = beamPrefab != null
            ? Instantiate(beamPrefab, start, Quaternion.identity)
            : CreateRuntimeBeam(start);

        _beam = go.GetComponent<Boss01LaserBeam>();
        if (_beam == null)
            _beam = go.AddComponent<Boss01LaserBeam>();

        _beam.Activate(start, end, width, damage, knockback, tickInterval, beamDuration, _owner, beamColor);
    }

    GameObject CreateRuntimeBeam(Vector3 position)
    {
        GameObject go = new GameObject("Boss01_LaserBeam");
        go.transform.position = position;
        BoxCollider2D box = go.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        go.AddComponent<Boss01LaserBeam>();
        return go;
    }

    void ResolveSegment(out Vector2 start, out Vector2 end)
    {
        start = GetStart();

        switch (aimMode)
        {
            case Boss01LaserAimMode.StartToEnd:
                end = endPoint != null ? (Vector2)endPoint.position : ClampEnd(start, GetFacing());
                break;
            case Boss01LaserAimMode.Facing:
                end = ClampEnd(start, GetFacing());
                break;
            case Boss01LaserAimMode.Custom:
                end = ClampEnd(start, customDirection.sqrMagnitude > 0.0001f ? customDirection.normalized : Vector2.left);
                break;
            default:
                end = ClampEnd(start, DirectionToPlayer());
                break;
        }

        if (stopAtWalls)
            end = RaycastEnd(start, end);
    }

    Vector2 GetStart()
    {
        Transform origin = startPoint != null ? startPoint : (muzzle != null ? muzzle : transform);
        Vector2 pos = origin.position;
        Vector2 dir = DirectionToPlayer();
        if (aimMode == Boss01LaserAimMode.Facing)
            dir = GetFacing();
        else if (aimMode == Boss01LaserAimMode.Custom)
            dir = customDirection.sqrMagnitude > 0.0001f ? customDirection.normalized : Vector2.left;
        else if (aimMode == Boss01LaserAimMode.StartToEnd && endPoint != null)
        {
            Vector2 toEnd = (Vector2)endPoint.position - pos;
            if (toEnd.sqrMagnitude > 0.0001f)
                dir = toEnd.normalized;
        }

        return pos + dir * startOffset;
    }

    Vector2 ClampEnd(Vector2 start, Vector2 direction)
    {
        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.left;
        return start + dir * Mathf.Max(0.1f, maxLength);
    }

    Vector2 RaycastEnd(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;
        float dist = delta.magnitude;
        if (dist < 0.01f)
            return end;

        RaycastHit2D hit = Physics2D.Raycast(start, delta.normalized, dist, wallMask);
        if (hit.collider == null || IsOwner(hit.collider) || hit.collider.CompareTag("Player"))
            return end;

        return hit.point;
    }

    bool IsOwner(Collider2D other)
    {
        return _owner != null && other.transform.IsChildOf(_owner.transform);
    }

    Vector2 DirectionToPlayer()
    {
        Transform player = GetPlayer();
        if (player == null)
            return GetFacing();
        Vector2 dir = (Vector2)player.position - (Vector2)transform.position;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : GetFacing();
    }

    Vector2 GetFacing()
    {
        return transform.rotation.eulerAngles.y > 90f ? Vector2.right : Vector2.left;
    }

    static Transform GetPlayer()
    {
        if (PlayerController.HasInstance)
            return PlayerController.Instance.transform;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.transform : null;
    }

    void OnDisable()
    {
        StopLaser();
    }

    void OnDrawGizmosSelected()
    {
        Vector2 start;
        Vector2 end;
        if (!Application.isPlaying)
        {
            Transform origin = startPoint != null ? startPoint : (muzzle != null ? muzzle : transform);
            start = origin.position;
            if (aimMode == Boss01LaserAimMode.StartToEnd && endPoint != null)
                end = endPoint.position;
            else
                end = start + Vector2.left * Mathf.Max(0.1f, maxLength);
        }
        else
        {
            ResolveSegment(out start, out end);
        }

        Vector2 delta = end - start;
        Vector2 mid = start + delta * 0.5f;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        Gizmos.color = new Color(1f, 0.25f, 0.15f, 0.35f);
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(mid, Quaternion.Euler(0f, 0f, angle), Vector3.one);
        Gizmos.DrawCube(Vector3.zero, new Vector3(Mathf.Max(0.01f, delta.magnitude), width, 0.01f));
        Gizmos.matrix = old;
    }
}
