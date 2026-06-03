using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyAIController enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("Enemy Idle");
        enemy.Agent.isStopped = true;
        enemy.Animator.SetBool("IsWalking", false);
    }

    public override void Update()
    {
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
        Debug.Log("Exit Idle");
    }
}
