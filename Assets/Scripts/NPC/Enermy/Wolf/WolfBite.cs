using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBite : MonoBehaviour
{
    [SerializeField] int damage = 1;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.name == "Animator")
            { 
                collision.gameObject.transform.parent.GetChild(1).GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }
}
