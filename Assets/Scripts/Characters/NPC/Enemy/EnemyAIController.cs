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
    public bool SeePlayer = false;
    public Vector2 LookPosition;

    // status
    [SerializeField]
    private bool isWandering = false;
    public bool IsWandering 
    {
        get 
        { return currentState == PatrolState && currentState.IsHesitating == false; }
        private set { }
    } 
    [SerializeField]
    private bool isAlerted = false;
    public bool IsAlerted
    {
        get { return isAlerted; }
        private set
        {
            isAlerted = value;
        }
    }

    // player distance
    public bool IsCombatRange = false;
    public bool IsMoveRange = false;
    public bool IsLongRange = false;
    public bool IsCloseRange = false;
    public bool IsClosestRange = false;

    // states
    [SerializeField] private string currentStateName;
    private EnemyState currentState;
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyCombatState CombatState { get; private set; }
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
        CombatState = new EnemyCombatState(this);
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
        Agent.speed = 0;
        Agent.angularSpeed = 999;
        Agent.acceleration = Stats.Acceleration;
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    public void ChangeState(EnemyState newState, EnemyState prevState = default)
    {
        if (currentState == newState)
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter(prevState);

        currentStateName = currentState.GetType().Name;
    }

    public void CanSeePlayer(bool canSee = true, Vector2 playerPosition = default)
    {
        if (statusEffect.Dead) return;

        SeePlayer = canSee;
        if (canSee)
        {
            currentState?.StopHesitate();
            TargetPosition = playerPosition;
            LookPosition = playerPosition - (Vector2)transform.position;
        }
    }

    public bool IsDead()
    {
        //return hp <= 0f;
        return false;
    }

    private void UpdateEnemyDirection()
    {
        // update sprite direction, future improvement: use 4 directional sprites
        if (SeePlayer)
        {
            if (LookPosition.x > 0.1f)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (LookPosition.x < -0.1f)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
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
}