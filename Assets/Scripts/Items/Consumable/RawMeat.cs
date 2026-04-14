using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMeat : MonoBehaviour, IItem
{
    public ConsumableItem item;

    private void Awake()
    {
        item = new ConsumableItem(2, "raw meat", "heal 1 hp", "Icons/raw_meat.png", 1);
        item.onUse = item =>
        {
            PlayerHealth.Instance.Heal(1);
            Debug.Log("Used raw meat, healed 1 hp");
        };
    }

    public Item GetItem() => item;

    public void Use() => item.Use();
}