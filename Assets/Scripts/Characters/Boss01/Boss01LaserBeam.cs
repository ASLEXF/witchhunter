using UnityEngine;

/// <summary>
/// 激光判定体：在起点到终点之间铺一块矩形，持续对范围内的玩家结算伤害。
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Boss01LaserBeam : MonoBehaviour
{
    [Header("判定")]
    [SerializeField] float width = 0.6f;
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 0.1f;
    [SerializeField] float tickInterval = 0.35f;
    [SerializeField] string playerTag = "Player";
    [SerializeField] LayerMask hitMask = ~0;

    [Header("显示")]
    [SerializeField] Color beamColor = new Color(1f, 0.2f, 0.15f, 0.7f);
    [SerializeField] SpriteRenderer beamRenderer;

    BoxCollider2D _box;
    SpriteRenderer _runtimeRenderer;
    GameObject _owner;
    Vector2 _start;
    Vector2 _end;
    float _expireAt;
    float _nextTickTime;
    bool _active;
    readonly Collider2D[] _hits = new Collider2D[16];

    public bool IsActive => _active && Time.time < _expireAt;
    public Vector2 StartPoint => _start;
    public Vector2 EndPoint => _end;
    public float Width => width;

    void Awake()
    {
        _box = GetComponent<BoxCollider2D>();
        _box.isTrigger = true;
        _box.enabled = false;

        if (beamRenderer == null)
            beamRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Activate(
        Vector2 start,
        Vector2 end,
        float beamWidth,
        int beamDamage,
        float beamKnockback,
        float interval,
        float duration,
        GameObject owner,
        Color color)
    {
        _owner = owner;
        width = Mathf.Max(0.05f, beamWidth);
        damage = Mathf.Max(0, beamDamage);
        knockback = beamKnockback;
        tickInterval = Mathf.Max(0f, interval);
        beamColor = color;
        _start = start;
        _end = end;
        _expireAt = Time.time + Mathf.Max(0.01f, duration);
        _nextTickTime = Time.time;
        _active = true;

        AlignToSegment(_start, _end);
        EnsureVisual();
        ApplyVisual();
        _box.enabled = true;

        if (WitchHunter.Environment.HasInstance)
            WitchHunter.Environment.Instance.AddProjectile(gameObject);
    }

    public void Retarget(Vector2 start, Vector2 end)
    {
        if (!_active)
            return;

        _start = start;
        _end = end;
        AlignToSegment(_start, _end);
        ApplyVisual();
    }

    public void Deactivate()
    {
        _active = false;
        if (_box != null)
            _box.enabled = false;
        Destroy(gameObject);
    }

    void Update()
    {
        if (!_active)
            return;

        if (Time.time >= _expireAt)
        {
            Deactivate();
            return;
        }

        if (Time.time >= _nextTickTime)
        {
            TickDamage();
            _nextTickTime = tickInterval <= 0f ? Time.time : Time.time + tickInterval;
        }
    }

    void AlignToSegment(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;
        float length = delta.magnitude;
        if (length < 0.01f)
        {
            length = 0.01f;
            delta = Vector2.right * length;
        }

        Vector2 mid = start + delta * 0.5f;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        transform.position = new Vector3(mid.x, mid.y, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        _box.size = new Vector2(length, width);
        _box.offset = Vector2.zero;
    }

    void TickDamage()
    {
        Vector2 size = _box.size;
        float angle = transform.eulerAngles.z;
        int count = Physics2D.OverlapBoxNonAlloc(transform.position, size, angle, _hits, hitMask);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = _hits[i];
            if (hit == null || !IsPlayer(hit) || IsOwner(hit))
                continue;

            TryHitPlayer(hit, hit.ClosestPoint(transform.position));
        }
    }

    bool IsOwner(Collider2D other)
    {
        return _owner != null && other.transform.IsChildOf(_owner.transform);
    }

    bool IsPlayer(Collider2D other)
    {
        if (other == null)
            return false;
        if (other.CompareTag(playerTag))
            return true;
        return other.GetComponentInParent<PlayerAttacked>() != null
            || other.GetComponentInParent<PlayerController>() != null;
    }

    void TryHitPlayer(Collider2D other, Vector3 hitPoint)
    {
        PlayerHealth health = PlayerHealth.HasInstance ? PlayerHealth.Instance : null;
        if (health != null && health.isInvincible)
            return;

        PlayerAttacked attacked = other.GetComponent<PlayerAttacked>()
            ?? other.GetComponentInParent<PlayerAttacked>()
            ?? other.GetComponentInChildren<PlayerAttacked>();

        if (attacked != null)
            attacked.GetAttacked(damage, knockback, hitPoint);
        else if (health != null)
            health.TakeDamage(damage);
    }

    void EnsureVisual()
    {
        if (beamRenderer != null)
            return;

        GameObject visual = new GameObject("LaserVisual");
        visual.transform.SetParent(transform, false);
        _runtimeRenderer = visual.AddComponent<SpriteRenderer>();
        _runtimeRenderer.sprite = CreateWhiteSprite();
        _runtimeRenderer.sortingOrder = 20;
        beamRenderer = _runtimeRenderer;
    }

    void ApplyVisual()
    {
        if (beamRenderer == null)
            return;

        beamRenderer.color = beamColor;
        beamRenderer.drawMode = SpriteDrawMode.Simple;
        beamRenderer.transform.localPosition = Vector3.zero;
        beamRenderer.transform.localRotation = Quaternion.identity;

        Vector2 size = _box.size;
        if (beamRenderer.sprite != null)
        {
            Vector2 spriteSize = beamRenderer.sprite.bounds.size;
            if (spriteSize.x < 0.0001f) spriteSize.x = 1f;
            if (spriteSize.y < 0.0001f) spriteSize.y = 1f;
            beamRenderer.transform.localScale = new Vector3(size.x / spriteSize.x, size.y / spriteSize.y, 1f);
        }
        else
        {
            beamRenderer.transform.localScale = new Vector3(size.x, size.y, 1f);
        }
    }

    static Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        tex.hideFlags = HideFlags.HideAndDontSave;
        return Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.15f, 0.35f);
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 size = _box != null ? (Vector3)_box.size : new Vector3(1f, width, 0f);
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.matrix = old;
    }
}
