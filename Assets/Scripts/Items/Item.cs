/* sample */
using UnityEngine;

[SerializeField]
public class Item
{
    public int id;
    public string itemName;
    public string description;
    public string icon;
    public bool isConsumable;
    public bool isSalable;
    public bool isMaterial;
    public bool isThrowable;
    public int prise;

    public Item(int id = 0, string name = "test", string description = "test", string icon = "", bool isConsumable = false, bool isSalable = false, bool isThrowable = true, int prise = 0)
    {
        this.id = id;
        this.itemName = name;
        this.description = description;
        this.icon = icon;
        this.isConsumable = isConsumable;
        this.isSalable = isSalable;
        this.prise = prise;
    }

    public Item DeepCopy()
    {
        return new Item(id, itemName, description, icon, isConsumable, isSalable);
    }
}

[SerializeField]
public class WeaponItem: Item
{
    public struct WeaponAttackInfo
    {
        public bool hasAttack;
        public int damage;
        public int comboCount;
        public string animationTriggerName;
        public string animationIntegerName;
    }

    public struct WeaponChargingAttackInfo
    {
        public bool hasAttack;
        public int damage;
        public int comboCount;
        public string animationTriggerName;
    }

    public struct WeaponChargeAttackInfo
    {
        public bool hasAttack;
        public int damage;
        public int additionalDamage;
        public int comboCount;
        public string animationTriggerName;
    }

    public WeaponAttackInfo weaponAttackInfo;
    public WeaponChargingAttackInfo weaponChargingAttackInfo;
    public WeaponChargeAttackInfo weaponChargeAttackInfo;

    public WeaponItem(int id, string itemName, string description, string icon, WeaponAttackInfo weaponAttackInfo,WeaponChargingAttackInfo weaponChargingAttackInfo, WeaponChargeAttackInfo weaponChargeAttackInfo) : base(id, itemName, description, icon, false, false, false, 0)
    {
        this.weaponAttackInfo = weaponAttackInfo;
        this.weaponChargeAttackInfo = weaponChargeAttackInfo;
    }

    #region Attack


    public WeaponAnimationStructure Attack()
    {
        return new WeaponAnimationStructure
        {
            hasAttack = weaponAttackInfo.hasAttack,
            damage = weaponAttackInfo.damage,
            comboCount = weaponAttackInfo.comboCount,
            triggerName = weaponAttackInfo.animationTriggerName,
            integerName = weaponAttackInfo.animationIntegerName
        };
    }

    #endregion

    #region Charge Attack

    public WeaponAnimationStructure ChargingAttack()
    {
        return new WeaponAnimationStructure
        {
            hasAttack = weaponChargingAttackInfo.hasAttack,
            damage = weaponChargingAttackInfo.damage,
            triggerName = weaponChargingAttackInfo.animationTriggerName
        };
    }

    public WeaponAnimationStructure ChargeAttack()
    {
        return new WeaponAnimationStructure
        {
            hasAttack = weaponChargeAttackInfo.hasAttack,
            damage = weaponChargeAttackInfo.damage + weaponChargeAttackInfo.additionalDamage,
            triggerName = weaponChargeAttackInfo.animationTriggerName
        };
    }

    #endregion

    //public int GetDamage(float intervalSeconds)
    //{
    //    if (weaponChargeAttackInfo.hasChargeAttack)
    //    {
    //        return Mathf.RoundToInt(intervalSeconds > maxChargingTime ? additionalDamage : damage + intervalSeconds * additionalDamage / maxChargingTime);
    //    }
    //    else
    //    {
    //        return weaponAttackInfo.damage;
    //    }
    //}

    public new WeaponItem DeepCopy()
    {
        WeaponItem copy = new WeaponItem(id, itemName, description, icon, weaponAttackInfo, weaponChargingAttackInfo, weaponChargeAttackInfo);
        return copy;
    }
}

public class ConsumableItem: Item
{
    public int amount;

    public ConsumableItem(int id, string name, string description, string icon, int amount) : base(id, name, description, icon, true, true, true, 1)
    {
        this.amount = amount;
    }

    public System.Action<ConsumableItem> onUse = DefaultUse;
    private static void DefaultUse(ConsumableItem item)
    {
        Debug.LogWarning($"DefaultUsing Item: {item.itemName}");
        if (item.amount > 0)
            item.amount--;
        GameEvents.Instance.ItemsUpdated();
    }

    public void Use(int i = -1)
    {
        if (amount < 1) return;

        if (amount > 0)
            amount--;
        GameEvents.Instance.ItemsUpdated(i);
        onUse?.Invoke(this);
    }

    public new ConsumableItem DeepCopy()
    {
        ConsumableItem copy = new ConsumableItem(id, itemName, description, icon, amount);
        copy.onUse = onUse;
        return copy;
    }
}

public class ImportantItem: Item
{
    public ImportantItem(int id, string name, string description, string icon) : base(id, name, description, icon, false, false, false, 0)
    { }

    public new ImportantItem DeepCopy()
    {
        return new ImportantItem(id, itemName, description, icon);
    }
}

public class MaterialItem: Item 
{
    public int amount;

    public MaterialItem(int id, string name, string description, string icon, int amount) : base(id, name, description, icon, true, true, true, 1)
    {
        this.amount = amount;
    }

    public new MaterialItem DeepCopy()
    {
        return new MaterialItem(id, itemName, description, icon, amount);
    }
}
