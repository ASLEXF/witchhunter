using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    int maxHealth = 10;
    public int currentHealth = 10;

    public bool isInvincible = false;
    public bool isDead = false;
    public float invincibleDUration = 1.0f;

    public GameObject UI;


    void Awake()
    {
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            if (!isDead)
            {
                currentHealth -= damage;
                // TODO: ÊÜÉË¶¯»­
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    isDead = true;
                }
            }
        }
        StartCoroutine(Invincible());
    }

    IEnumerator Invincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDUration);
        isInvincible = false;
    }

    void Die()
    {
        currentHealth = 0;
        isDead = true;
        UI.SetActive(true);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
