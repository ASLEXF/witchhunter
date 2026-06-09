using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float attackTimer;

    public EnemyAttackState(EnemyAIController enemy) : base(enemy) { }

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

        if (!enemy.IsPlayerInAttackRange())
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        LookAtPlayer();

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    private void Attack()
    {
        Debug.Log("Enemy Attack");
        // 可以在这里触发动画：
        // enemy.Animator.SetTrigger("Attack");
        //
        // 真正造成伤害建议放在动画事件中
    }

    private void LookAtPlayer()
    {
        //Vector3 direction = enemy.player.position - enemy.transform.position;
        //direction.y = 0f;

        //if (direction.sqrMagnitude > 0.001f)
        //{
        //    enemy.transform.rotation = Quaternion.LookRotation(direction);
        //}
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack");
    }
}