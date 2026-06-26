using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttack : MonoBehaviour
{
    EnemyAIController controller;
    [SerializeField] WolfStats stats;
    [SerializeField] GameObject biteTrigger;

    Vector2 attackDirection;
    float attackDistance;

    private void Awake()
    {
        controller = transform.parent.GetComponent<EnemyAIController>();
    }

    public void EnableBiteTrigger()
    {
        //biteTrigger.transform.position = (Vector2)transform.position + PositionLine.Instance.GetAttackPosition(controller.TargetPosition - (Vector2)transform.position, stats.bitePostion);
        biteTrigger.SetActive(true);
    }

    public void DisableBiteTrigger()
    {
        biteTrigger.SetActive(false);
    }
}
