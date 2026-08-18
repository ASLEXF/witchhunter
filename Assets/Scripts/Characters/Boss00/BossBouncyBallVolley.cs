using System.Collections;
using UnityEngine;

public enum BossVolleyDirectionMode
{
    TowardPlayer = 0,
    Facing = 1,
    Custom = 2,
}

/// <summary>
/// 朝一个基准方向同时打出 3 颗弹球，三向夹角覆盖 120~180 度。
/// </summary>
public class BossBouncyBallVolley : MonoBehaviour
{
    [Header("弹球")]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] float ballSpeed = 8f;
    [SerializeField] float ballLifetime = 6f;
    [SerializeField] float spawnOffset = 0.6f;

    [Header("扇形")]
    [SerializeField] [Range(120f, 180f)] float spreadAngle = 150f;
    [SerializeField] BossVolleyDirectionMode directionMode = BossVolleyDirectionMode.TowardPlayer;
    [SerializeField] Vector2 customDirection = Vector2.left;

    [Header("持续发射")]
    [Tooltip("技能持续秒数。0 表示只打一波，弹球仍按存活时间存在。")]
    [SerializeField] float volleyDuration = 0f;
    [SerializeField] float spawnInterval = 0.8f;
    [SerializeField] bool retargetEachWave = true;
    [Tooltip("一波打出后，IsFiring 是否保持到弹球存活结束（方便行为树等待）。")]
    [SerializeField] bool holdUntilBallsExpire = true;

    public bool IsFiring { get; private set; }

    Coroutine _routine;
    GameObject _owner;

    void Awake()
    {
        if (muzzle == null)
            muzzle = transform;
        _owner = transform.root.gameObject;
    }

    public void Fire()
    {
        Fire(ResolveDirection(), volleyDuration);
    }

    public void FireTowardPlayer()
    {
        Fire(DirectionToPlayer(), volleyDuration);
    }

    public void Fire(Vector2 baseDirection, float duration)
    {
        if (!isActiveAndEnabled)
            return;
        if (IsFiring)
            return;

        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(FireRoutine(baseDirection, duration));
    }

    public void StopVolley()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        IsFiring = false;
    }

    IEnumerator FireRoutine(Vector2 baseDirection, float duration)
    {
        IsFiring = true;
        float holdDuration = duration > 0f ? duration : (holdUntilBallsExpire ? ballLifetime : 0f);
        float endTime = Time.time + Mathf.Max(0f, holdDuration);
        Vector2 dir = baseDirection;

        SpawnFan(dir);

        if (duration > 0f && spawnInterval > 0f)
        {
            while (Time.time < endTime)
            {
                float wait = spawnInterval;
                if (Time.time + wait > endTime)
                    break;
                yield return new WaitForSeconds(wait);
                if (retargetEachWave)
                    dir = ResolveDirection();
                SpawnFan(dir);
            }
        }

        float remain = endTime - Time.time;
        if (remain > 0f)
            yield return new WaitForSeconds(remain);

        IsFiring = false;
        _routine = null;
    }

    void SpawnFan(Vector2 baseDirection)
    {
        Vector2 center = baseDirection.sqrMagnitude > 0.0001f ? baseDirection.normalized : Vector2.left;
        float spread = Mathf.Clamp(spreadAngle, 120f, 180f);
        float half = spread * 0.5f;
        float centerAngle = Mathf.Atan2(center.y, center.x) * Mathf.Rad2Deg;
        float[] offsets = { -half, 0f, half };

        Vector3 origin = muzzle != null ? muzzle.position : transform.position;

        for (int i = 0; i < offsets.Length; i++)
        {
            float rad = (centerAngle + offsets[i]) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector3 pos = origin + (Vector3)(dir * spawnOffset);
            SpawnBall(pos, dir);
        }
    }

    void SpawnBall(Vector3 position, Vector2 direction)
    {
        GameObject go = ballPrefab != null
            ? Instantiate(ballPrefab, position, Quaternion.identity)
            : CreateRuntimeBall(position);

        BossBouncyBall ball = go.GetComponent<BossBouncyBall>();
        if (ball == null)
            ball = go.AddComponent<BossBouncyBall>();

        ball.Launch(direction, ballSpeed, ballLifetime, _owner);
    }

    GameObject CreateRuntimeBall(Vector3 position)
    {
        GameObject go = new GameObject("Boss00_BouncyBall");
        go.transform.position = position;
        go.AddComponent<CircleCollider2D>();
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        go.AddComponent<BossBouncyBall>();
        return go;
    }

    Vector2 ResolveDirection()
    {
        switch (directionMode)
        {
            case BossVolleyDirectionMode.Facing:
                return GetFacing();
            case BossVolleyDirectionMode.Custom:
                return customDirection.sqrMagnitude > 0.0001f ? customDirection.normalized : Vector2.left;
            default:
                return DirectionToPlayer();
        }
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
}
