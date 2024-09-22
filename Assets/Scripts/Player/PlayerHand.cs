using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private static PlayerHand instance;

    public static PlayerHand Instance
    { get { return instance; } }

    [SerializeField] private WeaponItem weaponL;

    public WeaponItem WeaponL
    {
        get { return weaponL; }
    }

    public bool IsLEmpty
    {
        get { return weaponL == null; }
    }

    [SerializeField] private WeaponItem weaponR;

    public WeaponItem WeaponR
    {
        get { return weaponR; }
    }

    public bool IsREmpty
    {
        get { return weaponR == null; }
    }

    private void Awake()
    {
        instance = this;
    }

    public void EquipL(WeaponItem item)
    {
        weaponL = item;
        GameEvents.Instance.ItemsUpdated();  // TODO: optimize here
    }

    public void EquipR(WeaponItem item)
    {
        weaponR = item;
        GameEvents.Instance.ItemsUpdated();  // TODO: optimize here
    }

    public void SwapLR()
    {
        WeaponItem temp = weaponL;
        weaponL = weaponR;
        weaponR = temp;
        GameEvents.Instance.ItemsUpdated();  // TODO: optimize here
    }

}
