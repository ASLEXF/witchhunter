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
        if (IsHesitating)
        {
            return;
        }
        //if (enemy.seePlayer)
        //{
        //    enemy.ChangeState(enemy.ChaseState);
        //    return;
        //}

        //if (enemy.isAlerted)
        //{
        //    enemy.ChangeState(enemy.ChaseState);
        //    return;
        //}
        enemy.ChangeState(enemy.PatrolState);
    }

    public override void Exit()
    {
        base.Exit();
        //Debug.Log("Exit Idle");
    }
}
