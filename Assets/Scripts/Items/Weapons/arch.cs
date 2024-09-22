using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arch : MonoBehaviour, IItem
{
    public WeaponItem item;

    private void Start()
    {
        item = new WeaponItem(11, "arch", "initial weapon", "Assets/Addressables/Icons/arch.png", 1, 1, 12, 0, false, true, 5, 1);
    }

    public Item GetItem() => item;
}
