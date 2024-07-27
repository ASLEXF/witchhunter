using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NPCBase : MonoBehaviour
{
    [Header("Basic")]
    public int maxHealth;
    public int currentHealth;

    [Header("Visual")]

    [Header("Behavior")]

    [Header("Loot Items")]
    public int lootMoney;
    public int[] lootItems;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int GetLootMoney(int random = 0)
    {
        if (random > 0)
            return lootMoney + Random.Range(0, random + 1);
        else
            return lootMoney;
    }

    int[] GetLootItems(int random = 0)
    {
        return new int[] {random};
    }
}
