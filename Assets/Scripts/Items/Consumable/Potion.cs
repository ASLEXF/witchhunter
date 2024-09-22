using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour, IItem
{
    public ComsumableItem item;

    private void Start()
    {
        item = new ComsumableItem(9, "Potion", "add 1 max hp", "Assets/Addressables/Icons/potion.png", 1);
    }

    public Item GetItem() => item;
}
