using System.Collections.Generic;
using UnityEngine;

public enum Boss01ArenaBreakMode
{
    AllConfigured = 0,
    NextInOrder = 1,
    RandomUnused = 2,
    ByIds = 3,
}

/// <summary>
/// 破坏场地上的指定块：改造型并封锁通行。
/// </summary>
public class Boss01ArenaBreaker : MonoBehaviour
{
    [Header("目标")]
    [SerializeField] List<Boss01ArenaPiece> pieces = new List<Boss01ArenaPiece>();
    [SerializeField] bool autoFindPieces = true;
    [SerializeField] Transform searchRoot;
    [SerializeField] Boss01ArenaBreakMode breakMode = Boss01ArenaBreakMode.AllConfigured;
    [SerializeField] string[] targetIds;

    [Header("时序")]
    [SerializeField] float stagger = 0f;

    int _nextIndex;
    Coroutine _routine;

    public bool IsBreaking { get; private set; }

    void Awake()
    {
        if (autoFindPieces)
            CollectPieces();
    }

    public void Break()
    {
        List<Boss01ArenaPiece> targets = ResolveTargets();
        BreakPieces(targets);
    }

    public void BreakAll()
    {
        BreakPieces(GetAlivePieces());
    }

    public void BreakById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        List<Boss01ArenaPiece> matches = new List<Boss01ArenaPiece>();
        List<Boss01ArenaPiece> all = GetAllPieces();
        for (int i = 0; i < all.Count; i++)
        {
            if (all[i] != null && all[i].PieceId == id && !all[i].IsDestroyed)
                matches.Add(all[i]);
        }
        BreakPieces(matches);
    }

    public void BreakAt(int index)
    {
        List<Boss01ArenaPiece> all = GetAllPieces();
        if (index < 0 || index >= all.Count || all[index] == null)
            return;
        BreakPieces(new List<Boss01ArenaPiece> { all[index] });
    }

    public void BreakInWorldRect(Vector2 center, Vector2 size)
    {
        List<Boss01ArenaPiece> matches = new List<Boss01ArenaPiece>();
        List<Boss01ArenaPiece> all = GetAllPieces();
        for (int i = 0; i < all.Count; i++)
        {
            Boss01ArenaPiece piece = all[i];
            if (piece == null || piece.IsDestroyed)
                continue;
            if (piece.OverlapsWorldRect(center, size, 0f))
                matches.Add(piece);
        }
        BreakPieces(matches);
    }

    public void RestoreAll()
    {
        StopBreaking();
        List<Boss01ArenaPiece> all = GetAllPieces();
        for (int i = 0; i < all.Count; i++)
        {
            if (all[i] != null)
                all[i].Restore();
        }
        _nextIndex = 0;
    }

    public void StopBreaking()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        IsBreaking = false;
    }

    void BreakPieces(List<Boss01ArenaPiece> targets)
    {
        if (targets == null || targets.Count == 0)
            return;

        if (stagger > 0f && targets.Count > 1)
        {
            if (_routine != null)
                StopCoroutine(_routine);
            _routine = StartCoroutine(BreakStaggered(targets));
            return;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
                targets[i].Break();
        }
    }

    System.Collections.IEnumerator BreakStaggered(List<Boss01ArenaPiece> targets)
    {
        IsBreaking = true;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
                targets[i].Break();
            if (i < targets.Count - 1 && stagger > 0f)
                yield return new WaitForSeconds(stagger);
        }
        IsBreaking = false;
        _routine = null;
    }

    List<Boss01ArenaPiece> ResolveTargets()
    {
        List<Boss01ArenaPiece> alive = GetAlivePieces();
        List<Boss01ArenaPiece> result = new List<Boss01ArenaPiece>();

        switch (breakMode)
        {
            case Boss01ArenaBreakMode.NextInOrder:
            {
                List<Boss01ArenaPiece> all = GetAllPieces();
                for (int step = 0; step < all.Count; step++)
                {
                    int index = (_nextIndex + step) % all.Count;
                    Boss01ArenaPiece piece = all[index];
                    if (piece != null && !piece.IsDestroyed)
                    {
                        result.Add(piece);
                        _nextIndex = (index + 1) % all.Count;
                        break;
                    }
                }
                break;
            }
            case Boss01ArenaBreakMode.RandomUnused:
                if (alive.Count > 0)
                    result.Add(alive[Random.Range(0, alive.Count)]);
                break;
            case Boss01ArenaBreakMode.ByIds:
                for (int i = 0; i < alive.Count; i++)
                {
                    if (ContainsId(alive[i].PieceId))
                        result.Add(alive[i]);
                }
                break;
            default:
                result.AddRange(alive);
                break;
        }

        return result;
    }

    bool ContainsId(string id)
    {
        if (targetIds == null || string.IsNullOrEmpty(id))
            return false;
        for (int i = 0; i < targetIds.Length; i++)
        {
            if (targetIds[i] == id)
                return true;
        }
        return false;
    }

    List<Boss01ArenaPiece> GetAlivePieces()
    {
        List<Boss01ArenaPiece> all = GetAllPieces();
        List<Boss01ArenaPiece> alive = new List<Boss01ArenaPiece>();
        for (int i = 0; i < all.Count; i++)
        {
            if (all[i] != null && !all[i].IsDestroyed)
                alive.Add(all[i]);
        }
        return alive;
    }

    List<Boss01ArenaPiece> GetAllPieces()
    {
        if (autoFindPieces && (pieces == null || pieces.Count == 0))
            CollectPieces();
        return pieces ?? new List<Boss01ArenaPiece>();
    }

    void CollectPieces()
    {
        if (pieces == null)
            pieces = new List<Boss01ArenaPiece>();

        Boss01ArenaPiece[] found = searchRoot != null
            ? searchRoot.GetComponentsInChildren<Boss01ArenaPiece>(true)
            : FindObjectsOfType<Boss01ArenaPiece>();

        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null && !pieces.Contains(found[i]))
                pieces.Add(found[i]);
        }
    }

    void OnDisable()
    {
        StopBreaking();
    }
}
