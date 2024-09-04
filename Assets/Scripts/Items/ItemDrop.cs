using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    bool isDrop;

    private void Start()
    {
        isDrop = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDrop && collision.gameObject.transform.root.CompareTag("Player") && collision.gameObject.name == "Interact")
        {

        }
    }
}
