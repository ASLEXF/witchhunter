using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cleaver : MonoBehaviour, IItem
{
    public WeaponItem item;

    private void Awake()
    {
        item = new WeaponItem(
            12, 
            "cleaver", 
            "found in the basement", 
            "Icons/cleaver.png",
            new WeaponItem.WeaponAttackInfo
            {
                hasAttack = true,
                damage = 2,
                comboCount = 2,
                animationTriggerName = "CleaverAttack"
            },
            new WeaponItem.WeaponChargingAttackInfo
            {
                hasAttack = false,
                damage = 0,
                comboCount = 0,
                animationTriggerName = null
            },
            new WeaponItem.WeaponChargeAttackInfo
            {
                hasAttack = false,
                damage = 0,
                additionalDamage = 0,
                comboCount = 0,
                animationTriggerName = null
            }
        );
    }

    public Item GetItem() => item;

    public WeaponAnimationStructure Attack() => item.Attack();
}
