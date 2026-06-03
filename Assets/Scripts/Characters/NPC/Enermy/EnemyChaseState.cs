using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyAIController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StartHesitate(enemy.DecisionInterval);
        enemy.Agent.isStopped = false;
        Debug.Log("Enemy Enter Chase");
    }

    public override void Update()
    {
        if (enemy.IsDead())
        {
            enemy.ChangeState(enemy.DeadState);
            return;
        }

        if (!enemy.SeePlayer)
        {
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        if (enemy.IsPlayerInAttackRange())
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }

        //enemy.Agent.SetDestination(enemy.player.position);
    }

    public override void Exit()
    {
        Debug.Log("Exit Chase");
    }
}