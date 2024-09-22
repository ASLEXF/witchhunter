using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IItem
{
    public MaterialItem item;

    private void Start()
    {
        item = new MaterialItem(1, "Coin", "collected then buy things from mechant", "Assets/Addressables/Icons/coin1.png", 1);
    }

    public Item GetItem() => item;
}
