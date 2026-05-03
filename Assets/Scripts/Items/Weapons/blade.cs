using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blade : MonoBehaviour, IWeaponItem
{
    public WeaponItem item;
    [SerializeField] BladeStats bladeStats;

    private void Awake()
    {
        if (bladeStats == null)
        {
            Debug.LogWarning("BladeStats is not assigned in the inspector.");
            return;
        }

        item = new WeaponItem(
            10, 
            "blade", 
            "initial weapon", 
            "Icons/blade.png",
            new WeaponItem.WeaponAttackInfo
            {
                hasAttack = true,
                damage = bladeStats.normalAttackDamage,
                comboCount = 2,
                animationTriggerName = "BladeAttack",
                animationIntegerName = "BladeCounter"
            },
            new WeaponItem.WeaponChargingAttackInfo
            {
                hasAttack = true,
                damage = 0,
                comboCount = 1,
                maxChargingTime = 1.2f,
                animationTriggerName = "BladeChargingAttack"
            },
            new WeaponItem.WeaponChargeAttackInfo
            {
                hasAttack = true,
                damage = bladeStats.heavyAttackDamage,
                additionalDamage = 0,
                comboCount = 1,
                animationTriggerName = "BladeChargeAttack"
            }
        );
    }

    public Item GetItem() => item;

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);

    public WeaponAnimationStructure Attack() => item.Attack();

    public WeaponAnimationStructure ChargingAttack() => item.ChargingAttack();

    public WeaponAnimationStructure ChargeAttack() => item.ChargeAttack();
}
