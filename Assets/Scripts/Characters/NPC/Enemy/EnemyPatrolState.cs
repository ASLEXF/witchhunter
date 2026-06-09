using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(EnemyAIController enemy) : base(enemy) { }

    private Vector3 originalPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    private float wanderStopTime;

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Enemy Patrol");
        if (originalPosition == Vector3.zero)
        {
            originalPosition = enemy.transform.position;
        }
        
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = false;
        startWandered = false;
        wanderStopTime = Time.time + enemy.Stats.wanderTime.Max;
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

        if (startWandered 
            && enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance 
            && enemy.Agent.velocity.sqrMagnitude < 0.01f)
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
        Debug.Log("Wandering...");
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
        enemy.LookPosition = (targetPosition - enemy.transform.position).normalized;
        Debug.Log($"Wandering to {targetPosition}");
        enemy.Animator.SetBool("IsWalking", true);
    }

    public override void Exit()
    {
        base.Exit();
        //Debug.Log("Exit Patrol");
    }
}
