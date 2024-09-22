using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope : MonoBehaviour, IItem
{
    public MaterialItem item;

    private void Start()
    {
        item = new MaterialItem(8, "Rope", "a rope, can be used to make something", "Assets/Addressables/Icons/rope.png", 1);
    }

    public Item GetItem() => item;
}
