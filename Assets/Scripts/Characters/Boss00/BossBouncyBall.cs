using UnityEngine;

/// <summary>
/// 弹球：碰墙按法线反弹且不掉速度；碰到玩家造成伤害并播放受击特效。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BossBouncyBall : MonoBehaviour
{
    [Header("运动")]
    [SerializeField] float speed = 8f;
    [SerializeField] float lifetime = 6f;
    [SerializeField] bool rotateToVelocity = true;

    [Header("伤害")]
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 0.15f;
    [SerializeField] float hitCooldown = 0.6f;
    [SerializeField] GameObject hitVfxPrefab;
    [SerializeField] float hitVfxLifetime = 1.5f;

    [Header("碰撞")]
    [SerializeField] LayerMask bounceMask = ~0;
    [SerializeField] string playerTag = "Player";

    Rigidbody2D _rb;
    Collider2D _collider;
    Vector2 _lastVelocity;
    float _speed;
    float _expireAt;
    float _nextHitTime;
    GameObject _owner;
    bool _launched;

    public bool IsAlive => _launched && Time.time < _expireAt;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _rb.gravityScale = 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        PhysicsMaterial2D mat = new PhysicsMaterial2D("Boss00Bounce")
        {
            bounciness = 1f,
            friction = 0f
        };
        _collider.sharedMaterial = mat;
        _collider.isTrigger = false;

        int projectileLayer = LayerMask.NameToLayer("Projectile");
        if (projectileLayer >= 0)
            gameObject.layer = projectileLayer;
    }

    public void Launch(Vector2 direction, float launchSpeed, float duration, GameObject owner)
    {
        _owner = owner;
        _speed = launchSpeed > 0f ? launchSpeed : speed;
        lifetime = duration > 0f ? duration : lifetime;
        _expireAt = Time.time + lifetime;
        _launched = true;

        IgnoreOwnerColliders();
        IgnoreSiblingBalls();

        if (WitchHunter.Environment.HasInstance)
            WitchHunter.Environment.Instance.AddProjectile(gameObject);

        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        _rb.velocity = dir * _speed;
        _lastVelocity = _rb.velocity;
    }

    void IgnoreOwnerColliders()
    {
        if (_owner == null)
            return;

        Collider2D[] ownerCols = _owner.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < ownerCols.Length; i++)
        {
            if (ownerCols[i] != null)
                Physics2D.IgnoreCollision(_collider, ownerCols[i], true);
        }
    }

    void IgnoreSiblingBalls()
    {
        BossBouncyBall[] balls = FindObjectsOfType<BossBouncyBall>();
        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i] == null || balls[i] == this)
                continue;
            Collider2D other = balls[i]._collider;
            if (other != null)
                Physics2D.IgnoreCollision(_collider, other, true);
        }
    }

    void FixedUpdate()
    {
        if (!_launched)
            return;

        if (Time.time >= _expireAt)
        {
            Destroy(gameObject);
            return;
        }

        if (_rb.velocity.sqrMagnitude > 0.0001f)
            _rb.velocity = _rb.velocity.normalized * _speed;

        _lastVelocity = _rb.velocity;

        if (rotateToVelocity && _lastVelocity.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(_lastVelocity.y, _lastVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_launched)
            return;

        if (IsPlayer(collision.collider))
            TryHitPlayer(collision.collider, collision.contactCount > 0 ? collision.GetContact(0).point : transform.position);

        if (!ShouldBounce(collision.collider) || collision.contactCount == 0)
            return;

        Bounce(collision.GetContact(0).normal);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_launched)
            return;

        if (IsPlayer(other))
            TryHitPlayer(other, other.ClosestPoint(transform.position));
    }

    bool ShouldBounce(Collider2D other)
    {
        if (other == null)
            return false;
        if (((1 << other.gameObject.layer) & bounceMask) == 0)
            return false;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            return false;
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
            return false;
        return true;
    }

    void Bounce(Vector2 normal)
    {
        Vector2 incoming = _lastVelocity.sqrMagnitude > 0.0001f ? _lastVelocity : _rb.velocity;
        if (incoming.sqrMagnitude < 0.0001f)
            incoming = Vector2.right * _speed;

        Vector2 reflected = Vector2.Reflect(incoming, normal);
        if (reflected.sqrMagnitude < 0.0001f)
            reflected = -incoming;

        _rb.velocity = reflected.normalized * _speed;
        _lastVelocity = _rb.velocity;
    }

    bool IsPlayer(Collider2D other)
    {
        return other != null && other.CompareTag(playerTag);
    }

    void TryHitPlayer(Collider2D other, Vector3 hitPoint)
    {
        if (Time.time < _nextHitTime)
            return;

        PlayerHealth health = PlayerHealth.HasInstance ? PlayerHealth.Instance : null;
        if (health != null && health.isInvincible)
            return;

        PlayerAttacked attacked = other.GetComponent<PlayerAttacked>()
            ?? other.GetComponentInParent<PlayerAttacked>()
            ?? other.GetComponentInChildren<PlayerAttacked>();

        if (attacked != null)
            attacked.GetAttacked(damage, knockback, transform.position);
        else if (health != null)
            health.TakeDamage(damage);
        else
            return;

        SpawnHitVfx(hitPoint);
        _nextHitTime = Time.time + hitCooldown;
    }

    void SpawnHitVfx(Vector3 point)
    {
        if (hitVfxPrefab == null)
            return;

        GameObject vfx = Instantiate(hitVfxPrefab, point, Quaternion.identity);
        if (hitVfxLifetime > 0f)
            Destroy(vfx, hitVfxLifetime);
    }
}
