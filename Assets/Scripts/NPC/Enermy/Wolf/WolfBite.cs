using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WolfBite : MonoBehaviour
{
    PolygonCollider2D NPCCollider;
    [SerializeField] int damage = 1;
    [SerializeField] float force = 0.2f;

    private void Awake()
    {
        NPCCollider = transform.parent.parent.GetComponent<PolygonCollider2D>();
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
                collision.gameObject.transform.parent.GetChild(0).GetComponent<PlayerAttacked>().GetAttacked(damage,  force, NPCCollider);
            }
        }
    }
}
