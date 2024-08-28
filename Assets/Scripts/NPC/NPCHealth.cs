using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCHealth : MonoBehaviour
{
    NPCController controller;
    NPCStatusEffect statusEffect;
    Animator animator;

    PolygonCollider2D _collider;
    NPCAttack NPCAttack;
    NPCAttacked NPCAttacked;
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

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            controller.Alert();
        }
    }

    public void Die()
    {
        statusEffect.SetDead();
        animator.SetBool("IsDead", true);
        controller.enabled = false;
        //animator. = false;
        _collider.enabled = false;
        NPCAttack.enabled = false;
        NPCAttacked.enabled = false;
        ranges.SetActive(false);
        triggers.SetActive(false);
    }

    public void heal(int heal)
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
