using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyAIController enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Enemy Idle");
        enemy.Agent.isStopped = true;
        enemy.Animator.SetBool("IsWalking", false);
        StartHesitate(enemy.Stats.wanderTime.RandomValue);
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
