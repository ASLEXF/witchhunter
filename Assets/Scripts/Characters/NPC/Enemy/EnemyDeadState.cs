using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(EnemyAIController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Agent.isStopped = true;
        Debug.Log("Enemy Dead");

        // enemy.Animator.SetTrigger("Dead");
    }
}