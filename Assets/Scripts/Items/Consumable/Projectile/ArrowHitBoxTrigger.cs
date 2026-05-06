using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ArrowHitBoxTrigger : MonoBehaviour
{
    Collider2D _collider;
    Rigidbody2D _rb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Arrow hit trigger: " + other.gameObject.name);
        gameObject.transform.parent.GetComponent<IProjectile>()?.Hit(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Arrow hit collision: " + collision.gameObject.name);
        gameObject.transform.parent.GetComponent<IProjectile>()?.Hit(collision.collider);
    }
}
