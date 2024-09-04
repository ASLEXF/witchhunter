using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Animator))]
public class NPCAttacked : MonoBehaviour
{
    NPCController controller;
    Animator animator;
    NPCHealth health;
    NPCStatusEffect statusEffect;

    // knockback
    Vector3 _position;

    private void Awake()
    {
        controller = transform.parent.GetComponent<NPCController>();
        animator = GetComponent<Animator>();
        health = transform.parent.GetChild(1).GetComponent<NPCHealth>();
        statusEffect = transform.parent.GetChild(1).GetComponent<NPCStatusEffect>();
    }

    public void GetAttacked(int damage, float force, PolygonCollider2D PlayerCollider)
    {
        if (!health.isInvincible && !statusEffect.Dead)
        {
            health.TakeDamage(damage);

            Vector3 position = transform.position - PlayerCollider.bounds.center;
            position.z = 0;  // prevent wrong displacements
            _position += position.normalized * force;  // add all forces if get attacked at the same time

            knockback();
            getStunned();

            animator.SetTrigger("GetAttacked");
        }
    }

    private void knockback()
    {
        if (_position != Vector3.zero)
        {
            controller.gameObject.transform.position += _position;
            _position = Vector3.zero;
        }
    }

    private void getStunned()
    {
        controller.StopAllCoroutines();
        controller.GetComponent<NavMeshAgent>().ResetPath();
        statusEffect.Stunned = true;
    }

    void stopStunned()
    {
        statusEffect.Stunned = false;
    }
}
