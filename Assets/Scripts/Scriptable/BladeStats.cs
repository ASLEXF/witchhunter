using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BladeStats : ScriptableObject
{
    [Header("Normal Attacks")]
    public int normalAttackDamage = 2;
    public float normalAttackForce = 0.2f;


    [Header("Heavy Attacks")]
    public int heavyAttackDamage = 4;
}
