using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blade : MonoBehaviour, IItem
{
    public WeaponItem item;

    private void Awake()
    {
        item = new WeaponItem(10, "blade", "initial weapon", "Assets/Addressables/Icons/blade.png", 1, 1, 0, 0, true, true, 1.0f, 2);
    }

    public Item GetItem() => item;
}
