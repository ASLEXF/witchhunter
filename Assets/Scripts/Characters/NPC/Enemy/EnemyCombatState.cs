using UnityEngine;

public class EnemyCombatState : EnemyState
{
    public EnemyCombatState(EnemyAIController enemy) : base(enemy) { }

    private float actionTimer;
    private float random;

    public override void Enter(EnemyState prevState = default)
    {
        enemy.Agent.isStopped = true;
        actionTimer = Time.time;
        Debug.Log("Enemy Enter Attack");
    }

    public override void Update()
    {
        if (enemy.IsDead())
        {
            enemy.ChangeState(enemy.DeadState);
            return;
        }
        if (IsHesitating || Time.time < actionTimer)
        {
            if (!enemy.Agent.hasPath)
            {
                enemy.Animator.SetBool("IsWalking", false);
                enemy.Animator.SetBool("IsRunning", false);
            }
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

        actionTimer += enemy.Stats.decisionInterval.RandomValue;
    }

    private void MakeDesision(ActionProbabilityGroup input)
    {
        random = Random.value;
        if (random < input.value1)
        {
            Approach();
        }
        else if (random < input.value2)
        {
            MoveAway();
        }
        else if (random < input.value3)
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
        Debug.Log("Enemy Approach");
        enemy.Agent.ResetPath();
        enemy.Agent.speed = enemy.Stats.chaseSpeed;
        enemy.Agent.angularSpeed = enemy.Stats.chaseAngularSpeed;
        enemy.Agent.isStopped = false;
        enemy.Agent.SetDestination(enemy.TargetPosition);
        enemy.Animator.SetBool("IsRunning", true);
    }

    private void MoveAway()
    {
        Debug.Log("Enemy Move Away");
        enemy.Agent.ResetPath();
        enemy.Agent.speed = enemy.Stats.wanderSpeed;
        enemy.Agent.angularSpeed = enemy.Stats.wanderAngularSpeed;
        enemy.Agent.isStopped = false;
        enemy.Agent.SetDestination(enemy.transform.position - (Vector3)enemy.LookPosition * 3);
        enemy.Animator.SetBool("IsWalking", true);
    }

    private void Attack()
    {
        Debug.Log("Enemy Attack");
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = true;
        enemy.Animator.SetBool("IsWalking", false);
        enemy.Animator.SetBool("IsRunning", false);
        enemy.Animator.SetTrigger("Bite");
    }

    public override void Exit()
    {

        Debug.Log("Exit Combat");
    }
}