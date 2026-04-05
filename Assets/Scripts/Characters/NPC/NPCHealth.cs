using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class NPCHealth : MonoBehaviour
{
    NPCController controller;
    NPCStatusEffect statusEffect;
    Animator animator;

    PolygonCollider2D _collider;
    NPCAttack NPCAttack;
    NPCAttacked NPCAttacked;
    NPCInteract NPCInteract;
    GameObject ranges;
    GameObject triggers;

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
        animator = transform.parent.GetChild(0).GetComponent<Animator>();

        _collider = transform.parent.GetComponent<PolygonCollider2D>();
        NPCAttack = transform.parent.GetChild(0).GetComponent<NPCAttack>();
        NPCAttacked = transform.parent.GetChild(0).GetComponent<NPCAttacked>();
        ranges = transform.parent.GetChild(2).gameObject;
        triggers = transform.parent.GetChild(3).gameObject;
    }

    private void Start()
    {
        if (currentHealth == 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        int resultHealth = currentHealth - damage;
        if (resultHealth < 0)
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
        statusEffect.SetDead();
        controller.Die();

        animator.SetBool("IsDead", true);
        NPCAttack.enabled = false;
        NPCAttacked.enabled = false;

        NPCInteract.UpdateIsInteractable();

        ranges.SetActive(false);
        triggers.SetActive(false);
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
