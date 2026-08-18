using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Boss00 行为树运行器。树可挂 ScriptableObject，也可直接在本组件里配任务列表。
/// 结构是优先选择（Selector）+ 条件序列，Inspector 里按行配置即可。
/// </summary>
public class BossAIController : MonoBehaviour
{
    [Header("行为树")]
    [SerializeField] BossBehaviorTreeAsset treeAsset;
    [SerializeField] List<BossBTTask> inlineTasks = new List<BossBTTask>();
    [SerializeField] BossAIStartMode startMode = BossAIStartMode.AfterDrawSword;
    [SerializeField] string startSignalId = "Boss00_DrawSword";

    [Header("引用")]
    [SerializeField] Animator animator;
    [SerializeField] BossDrawSword drawSword;
    [SerializeField] BossBouncyBallVolley volley;
    [SerializeField] NPCHealth health;
    [SerializeField] NPCStatusEffect statusEffect;
    [SerializeField] string runningBool = "IsRunning";

    [Header("调试")]
    [SerializeField] string currentTaskName;

    NavMeshAgent _nav;
    Rigidbody2D _rb;
    BossBTExecutor _executor;
    BossBTContext _ctx;
    bool _running;
    bool _subscribedDrawSword;

    public bool IsRunning => _running;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (drawSword == null)
            drawSword = GetComponent<BossDrawSword>();
        if (volley == null)
            volley = GetComponent<BossBouncyBallVolley>();
        if (health == null)
            health = GetComponentInChildren<NPCHealth>();
        if (statusEffect == null)
            statusEffect = GetComponentInChildren<NPCStatusEffect>();

        _nav = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody2D>();

        if (_nav != null)
        {
            _nav.updateRotation = false;
            _nav.updateUpAxis = false;
        }

        _ctx = new BossBTContext
        {
            Agent = this,
            Transform = transform,
            Animator = animator,
            Nav = _nav,
            Rb = _rb,
            DrawSword = drawSword,
            Volley = volley,
            Health = health,
            Status = statusEffect
        };

        if ((treeAsset == null || treeAsset.tasks == null || treeAsset.tasks.Count == 0)
            && (inlineTasks == null || inlineTasks.Count == 0))
        {
            inlineTasks = CreateDefaultTasks();
        }
    }

    void OnEnable()
    {
        GameEvents.Instance.OnStorySignal += HandleStorySignal;

        if (drawSword != null && !_subscribedDrawSword)
        {
            drawSword.Completed += HandleDrawSwordCompleted;
            _subscribedDrawSword = true;
        }

        if (startMode == BossAIStartMode.OnEnable)
            BeginCombat();
    }

    void OnDisable()
    {
        if (GameEvents.HasInstance)
            GameEvents.Instance.OnStorySignal -= HandleStorySignal;

        if (drawSword != null && _subscribedDrawSword)
        {
            drawSword.Completed -= HandleDrawSwordCompleted;
            _subscribedDrawSword = false;
        }

        StopMovement();
        _running = false;
    }

    void Start()
    {
        if (startMode == BossAIStartMode.AfterDrawSword && drawSword != null && drawSword.HasPlayed)
            BeginCombat();
    }

    void Update()
    {
        if (!_running)
            return;

        _ctx.Player = ResolvePlayer();
        _ctx.DeltaTime = Time.deltaTime;
        _ctx.TimeNow = Time.time;
        _executor?.Tick(_ctx);
        currentTaskName = _executor != null ? _executor.CurrentTaskName : "None";
    }

    public void BeginCombat()
    {
        IList<BossBTTask> tasks = treeAsset != null && treeAsset.tasks != null && treeAsset.tasks.Count > 0
            ? treeAsset.tasks
            : inlineTasks;

        _executor = new BossBTExecutor(tasks);
        _running = true;
    }

    public void StopCombat()
    {
        _running = false;
        StopMovement();
        _executor?.Reset();
        currentTaskName = "None";
    }

    public void NotifyStorySignal(string signalId)
    {
        if (!string.IsNullOrEmpty(signalId))
            _ctx.Signals.Add(signalId);
    }

    void HandleStorySignal(string signalId)
    {
        NotifyStorySignal(signalId);
        if (startMode == BossAIStartMode.OnStorySignal
            && (string.IsNullOrEmpty(startSignalId) || signalId == startSignalId))
        {
            BeginCombat();
        }
    }

    void HandleDrawSwordCompleted()
    {
        if (startMode == BossAIStartMode.AfterDrawSword)
            BeginCombat();
    }

    public Vector2 DirectionToPlayer()
    {
        Transform player = _ctx != null ? _ctx.Player : ResolvePlayer();
        if (player == null)
            return GetFacing();
        Vector2 dir = (Vector2)player.position - (Vector2)transform.position;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : GetFacing();
    }

    public void FacePlayer()
    {
        Vector2 look = DirectionToPlayer();
        if (look.x > 0.1f)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (look.x < -0.1f)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public Vector2 GetFacing()
    {
        return transform.rotation.eulerAngles.y > 90f ? Vector2.right : Vector2.left;
    }

    public void MoveToward(Vector3 destination, float speed)
    {
        if (_nav != null && _nav.enabled)
        {
            _nav.isStopped = false;
            _nav.speed = speed;
            _nav.SetDestination(destination);
            return;
        }

        Vector2 dir = ((Vector2)destination - (Vector2)transform.position);
        if (dir.sqrMagnitude < 0.0001f)
            return;
        dir.Normalize();
        if (_rb != null)
            _rb.velocity = dir * speed;
        else
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    public void StopMovement()
    {
        if (_nav != null && _nav.enabled)
        {
            _nav.ResetPath();
            _nav.isStopped = true;
            _nav.velocity = Vector3.zero;
        }
        if (_rb != null)
            _rb.velocity = Vector2.zero;
    }

    public void SetMoveAnim(bool moving)
    {
        if (animator == null || string.IsNullOrEmpty(runningBool))
            return;
        animator.SetBool(runningBool, moving);
    }

    static Transform ResolvePlayer()
    {
        if (PlayerController.HasInstance)
            return PlayerController.Instance.transform;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.transform : null;
    }

    void Reset()
    {
        startMode = BossAIStartMode.AfterDrawSword;
        startSignalId = "Boss00_DrawSword";
        runningBool = "IsRunning";
        inlineTasks = CreateDefaultTasks();
    }

    public static List<BossBTTask> CreateDefaultTasks()
    {
        return new List<BossBTTask>
        {
            new BossBTTask
            {
                name = "弹球扇形",
                cooldown = 4f,
                abortIfConditionFails = false,
                conditions = new List<BossBTCondition>
                {
                    new BossBTCondition { type = BossBTConditionType.DrawSwordCompleted },
                    new BossBTCondition { type = BossBTConditionType.VolleyIdle },
                    new BossBTCondition { type = BossBTConditionType.PlayerInRange, minDistance = 0f, maxDistance = 14f }
                },
                actions = new List<BossBTAction>
                {
                    new BossBTAction { type = BossBTActionType.FacePlayer },
                    new BossBTAction { type = BossBTActionType.FireBouncyVolley, duration = 0f }
                }
            },
            new BossBTTask
            {
                name = "靠近玩家",
                abortIfConditionFails = true,
                conditions = new List<BossBTCondition>
                {
                    new BossBTCondition { type = BossBTConditionType.PlayerOutsideRange, minDistance = 0f, maxDistance = 3f }
                },
                actions = new List<BossBTAction>
                {
                    new BossBTAction { type = BossBTActionType.ChasePlayer, duration = 1.5f, moveSpeed = 3.5f, stopDistance = 2.5f }
                }
            },
            new BossBTTask
            {
                name = "待机",
                abortIfConditionFails = false,
                conditions = new List<BossBTCondition>
                {
                    new BossBTCondition { type = BossBTConditionType.Always }
                },
                actions = new List<BossBTAction>
                {
                    new BossBTAction { type = BossBTActionType.Idle, duration = 0.4f }
                }
            }
        };
    }
}
