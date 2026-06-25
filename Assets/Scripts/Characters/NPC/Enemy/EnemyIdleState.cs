using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyAIController enemy) : base(enemy) { }

    public override void Enter(EnemyState prevState = default)
    {
        base.Enter();
        //Debug.Log("Enemy Idle");
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = true;
        enemy.Animator.SetBool("IsWalking", false);
        if (prevState is EnemyPatrolState)
        {
            StartHesitate(enemy.Stats.decisionInterval.RandomValue);
        }
        else
        {
            StartHesitate(enemy.Stats.wanderTime.RandomValue);
        }
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsDead())
        {
            enemy.ChangeState(enemy.DeadState);
            return;
        }
        if (enemy.IsCloseRange 
            || (enemy.IsCombatRange && enemy.SeePlayer))
        {
            enemy.ChangeState(enemy.CombatState);
            return;
        }
        if (enemy.SeePlayer || enemy.IsAlerted)
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }
        if (IsHesitating)
        {
            return;
        }
        
        enemy.ChangeState(enemy.PatrolState);
    }

    public override void Exit()
    {
        base.Exit();
        //Debug.Log("Exit Idle");
    }
}
