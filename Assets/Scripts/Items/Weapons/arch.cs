using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arch : MonoBehaviour, IWeaponItem
{
    public WeaponItem item;

    private void Awake()
    {
        item = new WeaponItem(
            11,
            "arch",
            "initial weapon",
            "Icons/arch.png",
            new WeaponItem.WeaponAttackInfo
            {
                hasAttack = false,
                damage = 0,
                comboCount = 0,
                animationTriggerName = null
            },
            new WeaponItem.WeaponChargingAttackInfo
            {
                hasAttack = true,
                damage = 0,
                comboCount = 1,
                animationTriggerName = "BowCharging"
            },
            new WeaponItem.WeaponChargeAttackInfo
            {
                hasAttack = true,
                damage = 1,
                additionalDamage = 1,
                comboCount = 1,
                animationTriggerName = "BowShoot"
            }
        );
    }

    public Item GetItem() => item;

    public WeaponAnimationStructure Attack() => item.Attack();

    public WeaponAnimationStructure ChargingAttack() => item.ChargingAttack();

    public WeaponAnimationStructure ChargeAttack() => item.ChargeAttack();
}
