using System.Collections.Generic;
using UnityEngine;

public enum BossBTStatus
{
    Success,
    Failure,
    Running
}

public class BossBTContext
{
    public BossAIController Agent;
    public Transform Transform;
    public Transform Player;
    public Animator Animator;
    public UnityEngine.AI.NavMeshAgent Nav;
    public Rigidbody2D Rb;
    public BossDrawSword DrawSword;
    public BossBouncyBallVolley Volley;
    public NPCHealth Health;
    public NPCStatusEffect Status;
    public HashSet<string> Signals = new HashSet<string>();
    public float DeltaTime;
    public float TimeNow;

    public float DistanceToPlayer
    {
        get
        {
            if (Player == null)
                return float.MaxValue;
            return Vector2.Distance(Transform.position, Player.position);
        }
    }

    public float HealthPercent
    {
        get
        {
            if (Health == null || Health.MaxHealth <= 0)
                return 1f;
            return (float)Health.CurrentHealth / Health.MaxHealth;
        }
    }

    public bool IsDead => Status != null && Status.Dead;
    public bool IsStunned => Status != null && Status.Stunned;
}

public class BossBTExecutor
{
    readonly IList<BossBTTask> _tasks;
    readonly Dictionary<string, float> _readyAt = new Dictionary<string, float>();

    int _currentIndex = -1;
    int _actionIndex;
    bool _actionStarted;
    float _actionStartTime;
    bool _volleyStarted;

    public string CurrentTaskName { get; private set; } = "None";

    public BossBTExecutor(IList<BossBTTask> tasks)
    {
        _tasks = tasks ?? new List<BossBTTask>();
    }

    public void Reset()
    {
        _currentIndex = -1;
        _actionIndex = 0;
        _actionStarted = false;
        _volleyStarted = false;
        CurrentTaskName = "None";
    }

    public void Tick(BossBTContext ctx)
    {
        if (ctx == null || ctx.IsDead)
        {
            Reset();
            return;
        }
        if (ctx.IsStunned)
            return;

        if (_currentIndex >= 0)
        {
            BossBTTask running = _tasks[_currentIndex];
            if (running.abortIfConditionFails && !EvaluateConditions(running, ctx))
            {
                ctx.Agent.StopMovement();
                Reset();
            }
            else
            {
                BossBTStatus status = TickTask(running, ctx);
                if (status == BossBTStatus.Running)
                    return;

                if (status == BossBTStatus.Success && running.cooldown > 0f)
                    _readyAt[TaskKey(running, _currentIndex)] = ctx.TimeNow + running.cooldown;

                Reset();
            }
        }

        for (int i = 0; i < _tasks.Count; i++)
        {
            BossBTTask task = _tasks[i];
            if (!task.enabled)
                continue;
            if (!IsCooldownReady(task, i, ctx.TimeNow))
                continue;
            if (!EvaluateConditions(task, ctx))
                continue;

            _currentIndex = i;
            _actionIndex = 0;
            _actionStarted = false;
            _volleyStarted = false;
            CurrentTaskName = string.IsNullOrEmpty(task.name) ? $"Task {i}" : task.name;

            if (TickTask(task, ctx) == BossBTStatus.Running)
                return;

            if (task.cooldown > 0f)
                _readyAt[TaskKey(task, i)] = ctx.TimeNow + task.cooldown;
            Reset();
            return;
        }
    }

    bool IsCooldownReady(BossBTTask task, int index, float now)
    {
        string key = TaskKey(task, index);
        return !_readyAt.TryGetValue(key, out float ready) || now >= ready;
    }

    static string TaskKey(BossBTTask task, int index)
    {
        return string.IsNullOrEmpty(task.name) ? $"#{index}" : task.name;
    }

    bool EvaluateConditions(BossBTTask task, BossBTContext ctx)
    {
        if (task.conditions == null || task.conditions.Count == 0)
            return true;

        for (int i = 0; i < task.conditions.Count; i++)
        {
            if (!EvaluateCondition(task.conditions[i], ctx))
                return false;
        }
        return true;
    }

    bool EvaluateCondition(BossBTCondition condition, BossBTContext ctx)
    {
        bool result;
        switch (condition.type)
        {
            case BossBTConditionType.PlayerInRange:
                result = ctx.Player != null
                    && ctx.DistanceToPlayer >= condition.minDistance
                    && ctx.DistanceToPlayer <= condition.maxDistance;
                break;
            case BossBTConditionType.PlayerOutsideRange:
                result = ctx.Player == null
                    || ctx.DistanceToPlayer < condition.minDistance
                    || ctx.DistanceToPlayer > condition.maxDistance;
                break;
            case BossBTConditionType.HealthPercentBelow:
                result = ctx.HealthPercent < condition.healthPercent;
                break;
            case BossBTConditionType.HealthPercentAbove:
                result = ctx.HealthPercent > condition.healthPercent;
                break;
            case BossBTConditionType.HasStorySignal:
                result = !string.IsNullOrEmpty(condition.signalId) && ctx.Signals.Contains(condition.signalId);
                break;
            case BossBTConditionType.DrawSwordCompleted:
                result = ctx.DrawSword != null && ctx.DrawSword.HasPlayed && !ctx.DrawSword.IsPlaying;
                break;
            case BossBTConditionType.VolleyIdle:
                result = ctx.Volley == null || !ctx.Volley.IsFiring;
                break;
            default:
                result = true;
                break;
        }
        return condition.invert ? !result : result;
    }

    BossBTStatus TickTask(BossBTTask task, BossBTContext ctx)
    {
        if (task.actions == null || task.actions.Count == 0)
            return BossBTStatus.Success;

        while (_actionIndex < task.actions.Count)
        {
            BossBTStatus status = TickAction(task.actions[_actionIndex], ctx);
            if (status == BossBTStatus.Running)
                return BossBTStatus.Running;
            if (status == BossBTStatus.Failure)
                return BossBTStatus.Failure;

            _actionIndex++;
            _actionStarted = false;
            _volleyStarted = false;
        }
        return BossBTStatus.Success;
    }

    BossBTStatus TickAction(BossBTAction action, BossBTContext ctx)
    {
        bool firstFrame = !_actionStarted;
        if (firstFrame)
        {
            _actionStarted = true;
            _actionStartTime = ctx.TimeNow;
        }

        switch (action.type)
        {
            case BossBTActionType.Wait:
                return Elapsed(ctx, action.duration) ? BossBTStatus.Success : BossBTStatus.Running;

            case BossBTActionType.Idle:
                ctx.Agent.StopMovement();
                ctx.Agent.SetMoveAnim(false);
                return Elapsed(ctx, action.duration) ? BossBTStatus.Success : BossBTStatus.Running;

            case BossBTActionType.FacePlayer:
                ctx.Agent.FacePlayer();
                return BossBTStatus.Success;

            case BossBTActionType.ChasePlayer:
                if (ctx.Player == null)
                    return BossBTStatus.Failure;
                ctx.Agent.FacePlayer();
                ctx.Agent.MoveToward(ctx.Player.position, action.moveSpeed);
                ctx.Agent.SetMoveAnim(true);
                if (ctx.DistanceToPlayer <= action.stopDistance)
                    return BossBTStatus.Success;
                if (action.duration > 0f && Elapsed(ctx, action.duration))
                    return BossBTStatus.Success;
                return BossBTStatus.Running;

            case BossBTActionType.StopMove:
                ctx.Agent.StopMovement();
                ctx.Agent.SetMoveAnim(false);
                return BossBTStatus.Success;

            case BossBTActionType.PlayAnimation:
                if (firstFrame && ctx.Animator != null && !string.IsNullOrEmpty(action.animationName))
                    ctx.Animator.SetTrigger(action.animationName);
                ctx.Agent.StopMovement();
                return Elapsed(ctx, Mathf.Max(0.05f, action.duration)) ? BossBTStatus.Success : BossBTStatus.Running;

            case BossBTActionType.DrawSword:
                if (ctx.DrawSword == null)
                    return BossBTStatus.Failure;
                if (!ctx.DrawSword.IsPlaying && !ctx.DrawSword.HasPlayed)
                    ctx.DrawSword.Play();
                else if (!ctx.DrawSword.IsPlaying && ctx.DrawSword.HasPlayed && action.duration <= 0f)
                    return BossBTStatus.Success;
                if (!ctx.DrawSword.IsPlaying && ctx.DrawSword.HasPlayed)
                    return BossBTStatus.Success;
                if (action.duration > 0f && Elapsed(ctx, action.duration))
                    return BossBTStatus.Success;
                return BossBTStatus.Running;

            case BossBTActionType.FireBouncyVolley:
                if (ctx.Volley == null)
                    return BossBTStatus.Failure;
                if (!_volleyStarted)
                {
                    ctx.Agent.StopMovement();
                    ctx.Agent.FacePlayer();
                    ctx.Agent.SetMoveAnim(false);
                    if (action.duration > 0f)
                        ctx.Volley.Fire(ctx.Agent.DirectionToPlayer(), action.duration);
                    else
                        ctx.Volley.Fire();
                    _volleyStarted = true;
                }
                if (!ctx.Volley.IsFiring)
                    return BossBTStatus.Success;
                return BossBTStatus.Running;

            default:
                return BossBTStatus.Success;
        }
    }

    bool Elapsed(BossBTContext ctx, float duration)
    {
        return ctx.TimeNow - _actionStartTime >= duration;
    }
}
