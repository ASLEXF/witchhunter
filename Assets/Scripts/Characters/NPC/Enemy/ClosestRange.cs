using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestRange : MonoBehaviour
{
    EnemyAIController controller;

    private void Awake()
    {
        controller = transform.parent.parent.GetComponent<EnemyAIController>();
    }

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            controller.IsClosestRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            controller.IsClosestRange = false;
        }
    }
}
