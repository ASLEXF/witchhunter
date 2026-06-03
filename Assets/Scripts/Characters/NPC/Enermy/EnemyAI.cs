//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyAI : MonoBehaviour
//{
//    private enum EnemyState
//    {
//        Idle,
//        Patrol,
//        Chase,
//        Attack,
//        Attacked,
//        Dead
//    }

//    [Header("Target")]
//    public Transform player;

//    [Header("Ranges")]
//    public float detectRange = 10f;
//    public float attackRange = 2f;

//    [Header("Patrol")]
//    public Transform[] patrolPoints;
//    private int patrolIndex = 0;

//    private EnemyState currentState;
//    private NavMeshAgent agent;

//    private void Start()
//    {
//        ChangeState(EnemyState.Patrol);
//    }

//    private void Update()
//    {
//        switch (currentState)
//        {
//            case EnemyState.Idle:
//                UpdateIdle();
//                break;

//            case EnemyState.Patrol:
//                UpdatePatrol();
//                break;

//            case EnemyState.Chase:
//                UpdateChase();
//                break;

//            case EnemyState.Attack:
//                UpdateAttack();
//                break;

//            case EnemyState.Dead:
//                UpdateDead();
//                break;
//        }
//    }

//    private void ChangeState(EnemyState newState)
//    {
//        if (currentState == newState)
//            return;

//        ExitState(currentState);
//        currentState = newState;
//        EnterState(currentState);
//    }

//    private void EnterState(EnemyState state)
//    {
//        switch (state)
//        {
//            case EnemyState.Idle:
//                agent.isStopped = true;
//                break;

//            case EnemyState.Patrol:
//                agent.isStopped = false;
//                GoToNextPatrolPoint();
//                break;

//            case EnemyState.Chase:
//                agent.isStopped = false;
//                break;

//            case EnemyState.Attack:
//                agent.isStopped = true;
//                break;

//            case EnemyState.Dead:
//                agent.isStopped = true;
//                // 播放死亡动画、关闭碰撞体等
//                break;
//        }
//    }

//    private void ExitState(EnemyState state)
//    {
//        switch (state)
//        {
//            case EnemyState.Attack:
//                // 退出攻击状态时可以重置攻击动画、冷却等
//                break;
//        }
//    }
//}
