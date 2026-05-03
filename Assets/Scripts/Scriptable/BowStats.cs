using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BowStats : ScriptableObject
{
    [Header("Normal Attacks")]
    public int normalAttackDamage = 1;


    [Header("Heavy Attacks")]
    public int heavyAttackDamage = 2;
}
