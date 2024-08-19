using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    NPCController controller;

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
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Transform source)
    {
        currentHealth -= damage;
        controller.Alert(source);
    }

    public void Die()
    {
        Debug.Log("NPC DIE");
        //Destroy(gameObject.transform.parent.gameObject, 0.1f);
        //transform.parent.gameObject.SetActive(false);
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
