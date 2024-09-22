using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cleaver : MonoBehaviour, IItem
{
    public WeaponItem item;

    private void Start()
    {
        item = new WeaponItem(12, "cleaver", "found in the basement", "Assets/Addressables/Icons/cleaver.png", 1, 0, 0, 0, true, false, 0, 2);
    }

    public Item GetItem() => item;
}
