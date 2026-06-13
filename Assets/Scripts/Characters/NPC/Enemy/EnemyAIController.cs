using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator Animator { get; private set; }
    SpriteRenderer spriteRenderer;
    public NavMeshAgent Agent { get; private set; }
    NPCStatusEffect statusEffect;

    // memory
    public Vector2 TargetPosition; // the latest player position

    // sight
    public bool SeePlayer { get; private set; } = false;
    public Vector2 LookPosition;

    // status
    public bool IsWandering 
    {
        get
        { return currentState == PatrolState && currentState.IsHesitating == false; }
        private set { }
    }
    public bool IsAlerted { get; private set; } = false;

    // distances
    public bool IsMoveRange = false;
    public bool IsLongRange = false;
    public bool IsCloseRange = false;
    public bool IsClosestRange = false;

    // states
    private EnemyState currentState;
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    //public EnemyAttackedState AttackedState { get; private set; }
    public EnemyDeadState DeadState { get; private set; }

    // preference
    [SerializeField] public WolfStats Stats;
    [SerializeField] public float DecisionInterval;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Agent = GetComponent<NavMeshAgent>();
        statusEffect = transform.GetChild(1).GetComponent<NPCStatusEffect>();

        IdleState = new EnemyIdleState(this);
        PatrolState = new EnemyPatrolState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
        DeadState = new EnemyDeadState(this);

        DecisionInterval = Stats.decisionInterval.RandomValue;
    }

    private void Start()
    {
        InitializeNavMeshAgent();
        ChangeState(IdleState);
    }

    private void Update()
    {
        currentState?.Update();
        UpdateEnemyDirection();
    }

    private void InitializeNavMeshAgent()
    {
        Agent.speed = Stats.walkSpeed;
        Agent.angularSpeed = 999;
        Agent.acceleration = Stats.Acceleration;
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void CanSeePlayer(bool canSee = true, Vector2 playerPosition = default)
    {
        if (statusEffect.Dead) return;

        SeePlayer = canSee;
        if (canSee)
        {
            currentState?.StopHesitate();
            TargetPosition = playerPosition;
            LookPosition = playerPosition;
        }
    }

    public bool IsPlayerInAttackRange()
    {
        //if (player == null)
        //    return false;

        //return Vector3.Distance(transform.position, player.position) <= attackRange;
        return false;
    }

    public bool IsDead()
    {
        //return hp <= 0f;
        return false;
    }

    private void UpdateEnemyDirection()
    {
        // update sprite direction, future improvement: use 4 directional sprites
        GameObject triggers = GameObject.Find("Triggers");
        if (Agent.velocity.x > 0.1f)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (Agent.velocity.x < -0.1f)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}