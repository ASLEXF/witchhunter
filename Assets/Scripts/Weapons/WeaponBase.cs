using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    int damage;
    int maxDamage;  // 用于计算蓄力伤害
    float distance;  // 远程攻击距离
    float attackInternal = 0f;  // 攻击最小间隔

    bool isTurnInstant;  // 朝不同方向攻击时，是否瞬间转向
    bool hasChargeTime;  // 是否有蓄力过程

    float maxChargeTime;  // 最大蓄力时长

    public int attackNumber;  // 最大攻击段数

    public WeaponBase()
    {

    }

    int GetDamage()
    {
        if (hasChargeTime)
        {
            // TODO：根据蓄力时间计算伤害
            return (int)this.damage;
        }
        else
            return this.damage;
    }
}
