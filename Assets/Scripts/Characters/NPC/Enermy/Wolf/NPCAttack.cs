using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttack : MonoBehaviour
{
    NPCController controller;
    [SerializeField] WolfStats stats;
    [SerializeField] GameObject biteTrigger;

    Vector2 attackDirection;
    float attackDistance;

    private void Awake()
    {
        controller = GetComponentInParent<NPCController>();
    }

    public void EnableBiteTrigger()
    {
        biteTrigger.transform.position = transform.position.ToVector2() + PositionLine.Instance.GetAttackPosition(controller.posToPlayer, stats.bitePostion);
        biteTrigger.SetActive(true);
    }

    public void DisableBiteTrigger()
    {
        biteTrigger.SetActive(false);
    }
}
