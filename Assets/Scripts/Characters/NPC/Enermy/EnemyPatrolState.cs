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

    public override void Enter()
    {
        Debug.Log("Enemy Patrol");
        if (originalPosition == Vector3.zero)
        {
            originalPosition = enemy.transform.position;
        }
        //enemy.Agent.enabled = true;
        enemy.Agent.isStopped = false;
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
        if (!enemy.Agent.hasPath)
        {
            wander();
        }
    }

    //private IEnumerator Wander()
    //{
    //    Debug.Log("Wandering...");
    //    // set a random target position within the wander radius
    //    Vector3 randomDirection = Random.insideUnitCircle.normalized;
    //    Vector3 targetPosition = originalPosition + randomDirection * enemy.Stats.wanderRadius.Value;
    //    // check if the target position is valid
    //    NavMeshHit hit;
    //    if (NavMesh.SamplePosition(targetPosition, out hit, enemy.Stats.wanderRadius.Value, NavMesh.AllAreas))
    //    {
    //        targetPosition = hit.position;
    //    }
    //    else
    //    {
    //        targetPosition = originalPosition; // fallback to original position if no valid position found
    //    }
    //    // set the agent's destination to the target position
    //    if (Vector2.Distance(enemy.transform.position, targetPosition) < enemy.Stats.minDistance)
    //    {
    //        // skip if already close to the target position
    //        wanderCoroutine = null;
    //        yield break;
    //    }
    //    enemy.Agent.SetDestination(targetPosition);
    //    enemy.LookPosition = (targetPosition - enemy.transform.position).normalized;
    //    Debug.Log($"Wandering to {targetPosition}");

    //    enemy.Animator.SetBool("IsWalking", true);

    //    yield return null;

    //    wanderCoroutine = null;
    //}

    private void wander()
    {
        Debug.Log("Wandering...");
        // set a random target position within the wander radius
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 targetPosition = originalPosition + randomDirection * enemy.Stats.wanderRadius.Value;
        // check if the target position is valid
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, enemy.Stats.wanderRadius.Value, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }
        else
        {
            targetPosition = originalPosition; // fallback to original position if no valid position found
        }
        // set the agent's destination to the target position
        if (Vector2.Distance(enemy.transform.position, targetPosition) < enemy.Stats.minDistance)
            // skip if already close to the target position
            return;
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
