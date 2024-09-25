using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

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

    private void Update()
    {
        float lerpSpeed = 3.0f;
        float lerpTime = Time.deltaTime * lerpSpeed;


        //Vector3 targetPosition = transform.position +  _position;
        //_position = Vector3.zero;

        //controller.gameObject.transform.position = Vector3.Lerp(controller.gameObject.transform.position, targetPosition, lerpTime);
    }

    public void GetAttacked(int damage, float force, PolygonCollider2D PlayerCollider)
    {
        if (!health.isInvincible && !statusEffect.Dead)
        {
            health.TakeDamage(damage);

            Vector3 position = transform.position - PlayerCollider.bounds.center;
            position.z = 0;  // prevent wrong displacements
            _position += position.normalized * force;  // add all forces if get attacked at the same time

            //knockback();
            getStunned();

            animator.SetTrigger("GetAttacked");
        }
    }

    private void knockback()
    {
        if (_position != Vector3.zero)
        {
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 targetPosition = controller.gameObject.transform.position + _position;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            controller.gameObject.transform.position = Vector3.Lerp(controller.gameObject.transform.position, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;

        _position = Vector3.zero;
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
