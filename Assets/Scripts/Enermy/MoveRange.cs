using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponentInParent<NPCController>().isMoveRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponentInParent<NPCController>().isMoveRange = false;
        }
    }
}
