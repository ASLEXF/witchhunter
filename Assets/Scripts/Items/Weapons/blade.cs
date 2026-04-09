using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blade : MonoBehaviour, IWeaponItem
{
    public WeaponItem item;

    private void Awake()
    {
        item = new WeaponItem(
            10, 
            "blade", 
            "initial weapon", 
            "Assets/Addressables/Icons/blade.png",
            new WeaponItem.WeaponAttackInfo
            {
                hasAttack = true,
                damage = 2,
                comboCount = 2,
                animationTriggerName = "BladeAttack",
                animationIntegerName = "BladeCounter"
            },
            new WeaponItem.WeaponChargingAttackInfo
            {
                hasAttack = true,
                damage = 0,
                comboCount = 1,
                animationTriggerName = "BladeChargingAttack"
            },
            new WeaponItem.WeaponChargeAttackInfo
            {
                hasAttack = true,
                damage = 4,
                additionalDamage = 0,
                comboCount = 1,
                animationTriggerName = "BladeChargeAttack"
            }
        );
    }

    public Item GetItem() => item;

    public WeaponAnimationStructure Attack() => item.Attack();

    public WeaponAnimationStructure ChargingAttack() => item.ChargingAttack();

    public WeaponAnimationStructure ChargeAttack() => item.ChargeAttack();
}
