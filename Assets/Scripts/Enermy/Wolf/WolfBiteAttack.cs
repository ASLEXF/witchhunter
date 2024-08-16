using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBiteAttack : MonoBehaviour
{
    WolfStats stats;

    private void Awake()
    {
        stats = transform.parent.transform.parent.gameObject.GetComponent<NPCController>().stats;
    }

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
                collision.gameObject.transform.parent.GetChild(1).GetComponent<PlayerHealth>().TakeDamage(stats.biteDamage);
                GameEvents.Instance.PlayerHealthChanged();
            }
        }
    }
}
