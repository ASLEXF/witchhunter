using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMeat : MonoBehaviour, IItem
{
    public ComsumableItem item;

    private void Start()
    {
        item = new ComsumableItem(2, "raw meat", "heal 1 hp", "Assets/Addressables/Icons/raw_meat.png", 1);
    }

    public Item GetItem() => item;
}
