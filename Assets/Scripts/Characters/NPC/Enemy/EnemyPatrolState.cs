using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(EnemyAIController enemy) : base(enemy) { }

    private Vector3 originalPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    private Coroutine wanderCoroutine;
    private float wanderStopTime;
    private bool wanderFinished;

    public override void Enter()
    {
        Debug.Log("Enemy Patrol");
        if (originalPosition == Vector3.zero)
        {
            originalPosition = enemy.transform.position;
        }
        //enemy.Agent.enabled = true;
        enemy.Agent.isStopped = false;
        wanderStopTime = Time.time + enemy.Stats.wanderTime.Max;
    }

    public override void Update()
    {
        //if (enemy.seePlayer)
        //{
        //    enemy.ChangeState(enemy.ChaseState);
        //    return;
        //}

        //if (wanderCoroutine == null)
        //{
        //    wanderCoroutine = enemy.StartCoroutine(Wander());
        //}

        if (wanderFinished || Time.time >= wanderStopTime)
        {
            enemy.ChangeState(enemy.IdleState);
        }

        if (!enemy.Agent.hasPath)
        {
            wander();
        }
    }

    private void wander()
    {
        Debug.Log("Wandering...");
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
        if (Vector2.Distance(enemy.transform.position, targetPosition) < enemy.Stats.minDistance)
        {
            // skip if already close to the target position
            wanderFinished = true;
            return;
        }
        enemy.Agent.SetDestination(targetPosition);
        enemy.LookPosition = (targetPosition - enemy.transform.position).normalized;
        //Debug.Log($"Wandering to {targetPosition}");
        enemy.Animator.SetBool("IsWalking", true);
    }

    public override void Exit()
    {
        if (wanderCoroutine != null)
        {
            enemy.StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }
        Debug.Log("Exit Patrol");
    }
}
