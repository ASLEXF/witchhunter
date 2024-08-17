using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerSpawn playerSpawn;

    [SerializeField] int maxHealth = 5;

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    [SerializeField] int currentHealth = 5;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    private void Awake()
    {
        playerSpawn = GetComponent<PlayerSpawn>();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameEvents.Instance.PlayerHealthChanged();
    }

    void die()
    {
        playerSpawn.PlayerRespawn();
    }

    public void heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            GameEvents.Instance.PlayerHealthChanged();
        }
    }

    public void rest()
    {
        heal();
    }

    public void revive()
    {
        currentHealth = maxHealth;
        GameEvents.Instance.PlayerHealthChanged();
    }

    public void addMaxHealth()
    {
        // animation
        maxHealth++;
    }

    public void reduceMaxHealth()
    {
        // animation
        maxHealth--;
    }
}
