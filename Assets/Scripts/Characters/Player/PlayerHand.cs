#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : Singleton<PlayerHand>
{
    [SerializeField] private WeaponItem? weaponL;

    public WeaponItem? WeaponL
    {
        get { return weaponL; }
    }

    public bool IsLEmpty
    {
        get { return weaponL == null; }
    }

    [SerializeField] private WeaponItem? weaponR;

    public WeaponItem? WeaponR
    {
        get { return weaponR; }
    }

    public bool IsREmpty
    {
        get { return weaponR == null; }
    }

    public void EquipL(WeaponItem item)
    {
        weaponL = item;
    }

    public void UnequipL()
    {
        weaponL = null;
    }

    public void EquipR(WeaponItem item)
    {
        weaponR = item;
    }

    public void UnequipR()
    {
        weaponR = null;
    }

    public void SwapLR()
    {
        WeaponItem? temp = weaponL;
        weaponL = weaponR;
        weaponR = temp;
    }

}
