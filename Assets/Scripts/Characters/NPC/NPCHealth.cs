#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class NPCHealth : MonoBehaviour
{
    NPCController? controller;
    NPCStatusEffect? statusEffect;
    Animator? animator;

    PolygonCollider2D _collider;
    NPCAttack? NPCAttack;
    NPCAttacked? NPCAttacked;
    NPCInteract? NPCInteract;
    GameObject? ranges;
    GameObject? triggers;

    public bool isInvincible = false;

    [SerializeField] int maxHealth = 4;

    [SerializeField] int currentHealth = 4;

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    private void Awake()
    {
        controller = transform.GetComponentInParent<NPCController>();
        statusEffect = GetComponent<NPCStatusEffect>();
        animator = transform.parent.GetComponentInChildren<Animator>();

        _collider = transform.parent.GetComponent<PolygonCollider2D>();
        NPCAttack = transform.parent.GetComponentInChildren<NPCAttack>();
        NPCAttacked = transform.parent.GetComponentInChildren<NPCAttacked>();
        ranges = transform.parent.Find("Ranges")?.gameObject;
        triggers = transform.parent.Find("Triggers")?.gameObject;
    }

    private void Start()
    {
        if (currentHealth == 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        int resultHealth = currentHealth - damage;
        if (resultHealth < 0 && !isInvincible)
        {
            Die();
        }
        else
        {
            currentHealth -= damage;
        }
        
        if(controller != null)
            controller.Alert();
    }

    public void Die()
    {
        statusEffect?.SetDead();
        controller?.Die();

        animator?.SetBool("IsDead", true);
        if (NPCAttack != null) NPCAttack.enabled = false;
        if (NPCAttacked != null) NPCAttacked.enabled = false;

        NPCInteract?.UpdateIsInteractable();

        ranges?.SetActive(false);
        triggers?.SetActive(false);
    }

    public void Heal(int heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heal;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }
}
