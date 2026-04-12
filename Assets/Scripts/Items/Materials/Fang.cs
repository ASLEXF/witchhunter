using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fang : MonoBehaviour, IItem
{
    public MaterialItem item;

    private void Awake()
    {
        item = new MaterialItem(3, "Fang", "collected then buy things from mechant", "Icons/fang.png", 1);
    }

    public Item GetItem() => item;
}
