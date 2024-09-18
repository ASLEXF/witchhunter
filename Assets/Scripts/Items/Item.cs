using UnityEngine;

[System.Serializable]
public class Item
{
    public int id;
    public string itemName;
    public string description;
    public string icon;
    public bool isConsumable;
    public bool isSalable;
    public bool isMaterial;

    public Item(int id = 0, string name = "", string description = "", string icon = "", bool isConsumable = false, bool isSalable = false)
    {
        this.id = id;
        this.itemName = name;
        this.description = description;
        this.icon = icon;
        this.isConsumable = isConsumable;
        this.isSalable = isSalable;
    }

    public Item DeepCopy()
    {
        return new Item(id, itemName, description, icon, isConsumable, isSalable);
    }
}


public class WeaponItem: Item
{
    public int damage;
    public int additionalDamage;  // 用于计算蓄力伤害
    public float distance;  // 远程攻击距离
    public float attackInternal;  // 攻击最小间隔
    public bool isTurnInstant;  // 朝不同方向攻击时，是否瞬间转向
    public bool hasChargingTime;  // 是否有蓄力过程
    public float maxChargingTime;  // 最大蓄力时长
    public int attackNumber;  // 最大攻击段数

    public WeaponItem(int id, string itemName, string description, string icon, int damage, int additionalDamage, float distance, float attackInternal, bool isTurnInstant, bool hasChargingTime, float maxChargeTime, int attackNumber) : base(id, itemName, description, icon, false, false)
    {
        this.damage = damage;
        this.additionalDamage = additionalDamage;
        this.distance = distance;
        this.attackInternal = attackInternal;
        this.isTurnInstant = isTurnInstant;
        this.hasChargingTime = hasChargingTime;
        this.maxChargingTime = maxChargeTime;
        this.attackNumber = attackNumber;
    }

    public int GetChargingDamage(float intervalSeconds)
    {
        if (this.hasChargingTime)
        {
            return Mathf.RoundToInt(intervalSeconds > this.maxChargingTime ? this.additionalDamage : this.damage + intervalSeconds * this.additionalDamage / this.maxChargingTime);
        }
        else
        {
            return this.damage;
        }
    }

    public new WeaponItem DeepCopy()
    {
        return new WeaponItem(id, itemName, description, icon, damage, additionalDamage, distance, attackInternal, isTurnInstant, hasChargingTime, maxChargingTime, attackNumber);
    }
}

public class ComsumableItem: Item
{
    public int amount;

    public ComsumableItem(int id, string name, string description, string icon, int amount) : base(id, name, description, icon, true, true)
    {
        this.amount = amount;
    }

    public virtual void Use()
    {
        Debug.Log("Using Item: " + itemName);
    }

    public new ComsumableItem DeepCopy()
    {
        return new ComsumableItem(id, itemName, description, icon, amount);
    }
}

public class ImportantItem: Item
{
    public ImportantItem(int id, string name, string description, string icon) : base(id, name, description, icon, false, false)
    { }

    public new ImportantItem DeepCopy()
    {
        return new ImportantItem(id, itemName, description, icon);
    }
}

public class MaterialItem: Item
{
    public int amount;

    public MaterialItem(int id, string name, string description, string icon, int amount) : base(id, name, description, icon, true, true)
    {
        this.amount = amount;
    }

    public new MaterialItem DeepCopy()
    {
        return new MaterialItem(id, itemName, description, icon, amount);
    }
}
