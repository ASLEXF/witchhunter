using UnityEngine;

public class EnemyCombatState : EnemyState
{
    public EnemyCombatState(EnemyAIController enemy) : base(enemy) { }

    private float attackCooldown = 1.5f;
    private float attackTimer;

    private float random;

    public override void Enter()
    {
        enemy.Agent.isStopped = true;
        attackTimer = 0f;
        Debug.Log("Enemy Enter Attack");
    }

    public override void Update()
    {
        if (enemy.IsDead())
        {
            enemy.ChangeState(enemy.DeadState);
            return;
        }
        if (IsHesitating)
        {
            return;
        }
        if (enemy.IsClosestRange)
        {
            MakeDesision(enemy.Stats.closestRange);
        }
        else if (enemy.IsCloseRange)
        {
            MakeDesision(enemy.Stats.closeRange);
        }
        else if (enemy.IsLongRange)
        {
            MakeDesision(enemy.Stats.longRange);
        }
        else if (enemy.IsMoveRange)
        {
            MakeDesision(enemy.Stats.moveRange);
        }
        else if (enemy.IsCombatRange)
        {
            MakeDesision(enemy.Stats.combatRange);
        }
        else
            enemy.ChangeState(enemy.ChaseState);
    }

    private void MakeDesision(ActionProbabilityGroup input)
    {
        random = Random.value;
        if (random < input.moveClose)
        {
            Approach();
        }
        else if (random < input.moveAway)
        {
            MoveAway();
        }
        else if (random < input.attack)
        {
            Attack();
        }
        else
        {
            StartHesitate(enemy.Stats.decisionInterval.RandomValue);
        }
    }

    private void Approach()
    {
        // different approach strategies can be implemented here, such as strafing or circling around the player
    }

    private void MoveAway()
    {
        // different retreat strategies can be implemented here, such as backing up or sidestepping
    }

    private void Attack()
    {
        Debug.Log("Enemy Attack");
        // 可以在这里触发动画：
        // enemy.Animator.SetTrigger("Attack");
        //
        // 真正造成伤害建议放在动画事件中
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack");
    }
}