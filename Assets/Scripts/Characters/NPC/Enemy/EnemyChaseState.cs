using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyAIController enemy) : base(enemy) { }

    private Vector3 lastPosition;

    public override void Enter(EnemyState prevState = default)
    {
        base.Enter();
        Debug.Log("Enemy Enter Chase");
        StartHesitate(enemy.DecisionInterval);
        // reset path
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = false;
        // remember last position to limit unnecessary path recalculations
        lastPosition = enemy.TargetPosition;
        // set destination to player's position
        enemy.Agent.speed = enemy.Stats.chaseSpeed;
        enemy.Agent.angularSpeed = enemy.Stats.chaseAngularSpeed;
        enemy.Agent.SetDestination(enemy.TargetPosition);
        enemy.Animator.SetBool("IsRunning", true);
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsDead())
        {
            enemy.ChangeState(enemy.DeadState);
            return;
        }
        if (!enemy.Agent.hasPath)
        {
            Debug.Log("Reached target position.");
            enemy.ChangeState(enemy.IdleState);
            return;
        }
        if (enemy.SeePlayer && enemy.IsCombatRange || enemy.IsCloseRange)
        {
            enemy.ChangeState(enemy.CombatState);
            return;
        }
        if (Vector2.Distance(enemy.TargetPosition, lastPosition) > enemy.Stats.repathDistance)
        {
            Debug.Log("Repathing to target position.");
            enemy.Agent.SetDestination(enemy.TargetPosition);
            lastPosition = enemy.TargetPosition;
            enemy.Animator.SetBool("IsRunning", true);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = true;
        enemy.Agent.velocity = Vector3.zero;
        enemy.Animator.SetBool("IsRunning", false);
        Debug.Log("Exit Chase");
    }
}