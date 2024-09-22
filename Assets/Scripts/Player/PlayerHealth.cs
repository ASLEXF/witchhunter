using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private static PlayerHealth instance;

    public static PlayerHealth Instance
        { get { return instance; } }

    PlayerSpawn playerSpawn;
    Animator animator;

    public bool isInvincible = false;

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
        instance = this;

        playerSpawn = GetComponent<PlayerSpawn>();
        animator = transform.parent.GetChild(0).GetComponent<Animator>();
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
        if (!isInvincible) {
            currentHealth -= damage;
            GameEvents.Instance.PlayerHealthChanged();
        }
    }

    void die()
    {
        playerSpawn.PlayerRespawn();
    }

    public void Heal(int amount = 1)
    {
        int health = currentHealth + amount;
        if (health < maxHealth)
        {
            currentHealth = health;
            GameEvents.Instance.PlayerHealthChanged();
        }
    }

    private void rest()
    {
        Heal();
    }

    public void revive()
    {
        currentHealth = maxHealth;
        GameEvents.Instance.PlayerHealthChanged();
    }

    public void AddMaxHealth(int amount = 1)
    {
        // animation
        if (maxHealth + amount < 9)
            maxHealth += amount;
    }

    public void ReduceMaxHealth()
    {
        // animation
        maxHealth--;
    }

    public IEnumerator Invincible(float seconds = 2.0f)
    {
        isInvincible = true;
        yield return new WaitForSeconds(seconds);
        isInvincible = false;
    }
}
