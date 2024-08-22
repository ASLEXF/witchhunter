using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator))]
public class PlayerAttacked : MonoBehaviour
{
    Animator animator;
    PlayerHealth playerHealth;
    BoxCollider2D collider;

    // getPush
    Vector3 _position;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerHealth = transform.parent.GetChild(1).GetComponent<PlayerHealth>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        
    }

    public void GetAttacked(int damage, float force, PolygonCollider2D collider)
    {
        if (!playerHealth.isInvincible)
        {
            playerHealth.TakeDamage(damage);

            Vector3 position = transform.position - collider.bounds.center;
            position.z = 0;  // prevent wrong displacements
            _position += position.normalized * force;  // add all forces if get attacked at the same time

            animator.SetTrigger("GetAttacked");
        }
    }

    private void getPush()
    {
        if (_position != Vector3.zero)
        {
            PlayerController.Instance.gameObject.transform.position += _position;
            _position = Vector3.zero;
        }
    }

    private void invincible()
    {
        playerHealth.isInvincible = true;
    }

    private void invincibleEnd()
    {
        playerHealth.isInvincible = false;
    }
}
