using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

/// <summary>
/// 一块可被破坏的场地。破坏后切换造型，并加上阻挡，使该区域不可达。
/// </summary>
public class Boss01ArenaPiece : MonoBehaviour
{
    [Header("标识")]
    [SerializeField] string pieceId;

    [Header("造型")]
    [SerializeField] GameObject intactVisual;
    [SerializeField] GameObject destroyedVisual;
    [SerializeField] SpriteRenderer sharedRenderer;
    [SerializeField] Sprite intactSprite;
    [SerializeField] Sprite destroyedSprite;

    [Header("通行")]
    [Tooltip("完好时可走的碰撞（破坏后关闭）")]
    [SerializeField] Collider2D walkableCollider;
    [Tooltip("破坏后启用的阻挡碰撞；为空则按造型范围现场生成")]
    [SerializeField] Collider2D blockCollider;
    [SerializeField] bool createBlockerIfMissing = true;
    [SerializeField] bool addNavMeshObstacle = true;

    [Header("Tilemap（可选）")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] BoundsInt tileRegion;
    [SerializeField] TileBase destroyedTile;

    [Header("状态")]
    [SerializeField] bool startDestroyed;

    NavMeshObstacle _obstacle;
    BoxCollider2D _runtimeBlocker;
    TileBase[] _originalTiles;
    bool _capturedTiles;
    bool _destroyed;

    public string PieceId => pieceId;
    public bool IsDestroyed => _destroyed;

    void Awake()
    {
        if (sharedRenderer == null)
            sharedRenderer = GetComponent<SpriteRenderer>();
        if (intactVisual == null && destroyedVisual == null && sharedRenderer == null)
            intactVisual = gameObject;

        CacheOriginalTiles();
        ApplyState(startDestroyed, true);
    }

    public void Break()
    {
        if (_destroyed)
            return;
        ApplyState(true, false);
    }

    public void Restore()
    {
        if (!_destroyed && !startDestroyed)
            return;
        ApplyState(false, false);
    }

    public bool ContainsWorldPoint(Vector2 point)
    {
        return GetWorldBounds().Contains(point);
    }

    public bool OverlapsWorldRect(Vector2 center, Vector2 size, float angle)
    {
        Collider2D probe = blockCollider != null ? blockCollider : walkableCollider;
        if (probe != null)
            return probe.bounds.Intersects(new Bounds(center, size));

        return GetWorldBounds().Intersects(new Bounds(center, size));
    }

    public Bounds GetWorldBounds()
    {
        if (blockCollider != null && blockCollider.enabled)
            return blockCollider.bounds;
        if (walkableCollider != null)
            return walkableCollider.bounds;
        if (sharedRenderer != null)
            return sharedRenderer.bounds;
        if (intactVisual != null)
        {
            Renderer r = intactVisual.GetComponentInChildren<Renderer>();
            if (r != null)
                return r.bounds;
        }

        return new Bounds(transform.position, Vector3.one);
    }

    void ApplyState(bool destroyed, bool initializing)
    {
        _destroyed = destroyed;

        if (intactVisual != null && intactVisual != gameObject)
            intactVisual.SetActive(!destroyed);
        if (destroyedVisual != null)
            destroyedVisual.SetActive(destroyed);

        if (sharedRenderer != null)
        {
            if (destroyed && destroyedSprite != null)
                sharedRenderer.sprite = destroyedSprite;
            else if (!destroyed && intactSprite != null)
                sharedRenderer.sprite = intactSprite;
        }

        if (walkableCollider != null)
            walkableCollider.enabled = !destroyed;

        EnsureBlockCollider();
        if (blockCollider != null)
            blockCollider.enabled = destroyed;

        ApplyNavMeshObstacle(destroyed);
        ApplyTilemap(destroyed);

        if (!initializing && destroyed)
            EjectOverlappingPlayer();
    }

    void EnsureBlockCollider()
    {
        if (blockCollider != null || !createBlockerIfMissing)
            return;

        _runtimeBlocker = gameObject.AddComponent<BoxCollider2D>();
        _runtimeBlocker.isTrigger = false;
        Bounds bounds = GetVisualBoundsLocal();
        _runtimeBlocker.offset = bounds.center;
        _runtimeBlocker.size = new Vector2(Mathf.Max(0.2f, bounds.size.x), Mathf.Max(0.2f, bounds.size.y));
        _runtimeBlocker.enabled = false;
        blockCollider = _runtimeBlocker;
    }

    Bounds GetVisualBoundsLocal()
    {
        Renderer renderer = sharedRenderer;
        if (renderer == null && intactVisual != null)
            renderer = intactVisual.GetComponentInChildren<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            Bounds world = renderer.bounds;
            Vector3 localCenter = transform.InverseTransformPoint(world.center);
            Vector3 localSize = transform.InverseTransformVector(world.size);
            localSize.x = Mathf.Abs(localSize.x);
            localSize.y = Mathf.Abs(localSize.y);
            return new Bounds(localCenter, localSize);
        }

        if (tilemap != null && tileRegion.size.x > 0 && tileRegion.size.y > 0)
        {
            Vector3 min = tilemap.CellToWorld(tileRegion.min);
            Vector3 max = tilemap.CellToWorld(tileRegion.max);
            Vector3 worldCenter = (min + max) * 0.5f;
            Vector3 worldSize = max - min;
            return new Bounds(transform.InverseTransformPoint(worldCenter), new Vector3(Mathf.Abs(worldSize.x), Mathf.Abs(worldSize.y), 1f));
        }

        return new Bounds(Vector3.zero, Vector3.one);
    }

    void ApplyNavMeshObstacle(bool destroyed)
    {
        if (!addNavMeshObstacle)
            return;

        if (_obstacle == null)
            _obstacle = GetComponent<NavMeshObstacle>();
        if (_obstacle == null)
            _obstacle = gameObject.AddComponent<NavMeshObstacle>();

        _obstacle.shape = NavMeshObstacleShape.Box;
        _obstacle.carving = true;
        Bounds local = GetVisualBoundsLocal();
        _obstacle.center = local.center;
        _obstacle.size = new Vector3(Mathf.Max(0.2f, local.size.x), Mathf.Max(0.2f, local.size.y), 1f);
        _obstacle.enabled = destroyed;
    }

    void CacheOriginalTiles()
    {
        if (_capturedTiles || tilemap == null || tileRegion.size.x <= 0 || tileRegion.size.y <= 0)
            return;

        _originalTiles = tilemap.GetTilesBlock(tileRegion);
        _capturedTiles = true;
    }

    void ApplyTilemap(bool destroyed)
    {
        if (tilemap == null || tileRegion.size.x <= 0 || tileRegion.size.y <= 0)
            return;

        CacheOriginalTiles();
        int count = tileRegion.size.x * tileRegion.size.y * Mathf.Max(1, tileRegion.size.z);
        if (count <= 0)
            return;

        if (destroyed)
        {
            TileBase[] fill = new TileBase[count];
            if (destroyedTile != null)
            {
                for (int i = 0; i < fill.Length; i++)
                    fill[i] = destroyedTile;
            }
            tilemap.SetTilesBlock(tileRegion, fill);
        }
        else if (_originalTiles != null)
        {
            tilemap.SetTilesBlock(tileRegion, _originalTiles);
        }
    }

    void EjectOverlappingPlayer()
    {
        if (blockCollider == null || !blockCollider.enabled)
            return;

        Transform player = PlayerController.HasInstance ? PlayerController.Instance.transform : null;
        if (player == null)
            return;

        Collider2D playerCol = player.GetComponent<Collider2D>() ?? player.GetComponentInChildren<Collider2D>();
        if (playerCol == null || !blockCollider.bounds.Intersects(playerCol.bounds))
            return;

        Vector2 playerPos = player.position;
        Vector2 closest = blockCollider.ClosestPoint(playerPos);
        Vector2 away = playerPos - (Vector2)blockCollider.bounds.center;
        if (away.sqrMagnitude < 0.0001f)
            away = Vector2.up;
        player.position = closest + away.normalized * 0.15f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = _destroyed ? new Color(0.6f, 0.15f, 0.1f, 0.4f) : new Color(0.2f, 0.6f, 0.9f, 0.25f);
        Bounds b = GetWorldBounds();
        Gizmos.DrawCube(b.center, b.size);
    }
}
