using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRange : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponentInParent<NPCController>().isCloseRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponentInParent<NPCController>().isCloseRange = false;
        }
    }
}
