using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(EnemyAIController enemy) : base(enemy) { }

    private Vector3 originalPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    private float lastWanderTime;
    private float wanderStopTime;

    private float _wanderTime;

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Enemy Enter Patrol");
        if (originalPosition == Vector3.zero)
        {
            originalPosition = enemy.transform.position;
        }
        // reset path
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = false;
        // set wander timer
        _wanderTime = enemy.Stats.wanderTime.RandomValue;
        wanderStopTime = Time.time + _wanderTime;
        // balance wander time based on last wander time to avoid continuously short time or long time wanderings
        if (lastWanderTime != 0)
        {
            if (lastWanderTime - enemy.Stats.wanderTime.Min < enemy.Stats.wanderTime.Max - lastWanderTime)
            {
                wanderStopTime = (wanderStopTime + enemy.Stats.wanderTime.Max) / 2;
            }
            else
            {
                wanderStopTime = (wanderStopTime + enemy.Stats.wanderTime.Min) / 2;
            }
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
        if (IsHesitating)
        {
            return;
        }
        if (enemy.SeePlayer)
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }
        if (startWandered && !enemy.Agent.hasPath)
        {
            Debug.Log("Reached target position.");
            enemy.ChangeState(enemy.IdleState);
            return;
        }
        if (Time.time >= wanderStopTime)
        {
            Debug.Log("Wander time expired.");
            enemy.ChangeState(enemy.IdleState);
            return;
        }
        if (!enemy.Agent.hasPath)
        {
            wander();
        }
    }

    private bool startWandered = false;
    private void wander()
    {
        startWandered = true;
        // set wander timer
        wanderStopTime = Time.time + enemy.Stats.wanderTime.RandomValue;
        // set a random target position within the wander radius
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 targetPosition = originalPosition + randomDirection * enemy.Stats.wanderRadius.RandomValue;
        // check if the target position is valid
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, enemy.Stats.wanderRadius.RandomValue, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }
        else
        {
            targetPosition = originalPosition; // fallback to original position if no valid position found
        }
        // set the agent's destination to the target position
        enemy.Agent.SetDestination(targetPosition);
        // set look direction towards the target position
        enemy.LookPosition = (targetPosition - enemy.transform.position).normalized;
        // set walking animation
        enemy.Animator.SetBool("IsWalking", true);
        Debug.Log($"Wandering to {targetPosition}, stopping in {_wanderTime} seconds.");
    }

    public override void Exit()
    {
        base.Exit();
        startWandered = false;
        //Debug.Log("Exit Patrol");
    }
}
