using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerSpawn playerSpawn;

    int maxHealth = 5;

    private void Awake()
    {
        playerSpawn = GetComponent<PlayerSpawn>();
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    int currentHealth = 5;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            die();
        }
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
        }
    }

    public void rest()
    {
        heal();
    }

    public void revive()
    {
        currentHealth = maxHealth;
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
