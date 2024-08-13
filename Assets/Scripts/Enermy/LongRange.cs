using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponentInParent<NPCController>().isLongRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponentInParent<NPCController>().isLongRange = false;
        }
    }
}
