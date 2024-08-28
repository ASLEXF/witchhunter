using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    PolygonCollider2D PlayerCollider;
    [SerializeField] int damage = 1;
    [SerializeField] float force = 0.2f;

    private void Awake()
    {
        PlayerCollider = transform.parent.parent.GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enermy"))
        {
            if (collision.name == "Animator")
            {
                collision.GetComponent<NPCAttacked>().GetAttacked(damage, force, PlayerCollider);
            }
        }
    }
}
