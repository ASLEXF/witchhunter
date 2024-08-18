using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum ApproachMethod
{
    straight = 0,   // find the shortest path
    curve = 1,      // find a curved path
    back = 2,       // find a curved path to approach the back
    detour = 3,     // find a collider, then find shortest path
}

[CreateAssetMenu]
public class WolfStats : ScriptableObject
{
    [Header("Attack - Bite")]
    public int biteDamage = 1;
    public Vector2 bitePostion = new Vector2(-0.317f, 0.263f);

    [Header("Attack - Claw")]
    public int clawDamage = 1;
    public Vector2 clawPosition = new Vector2(0, 0);

    [Header("Movement")]
    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float MaxSpeed = 14;
    public float Acceleration = 12;
    //public float GroundDeceleration = 60;

    [Header("Behavior Preferences")]
    public float decisionInterval = 0.5f;
    public ApproachMethod approachMethod = ApproachMethod.straight;
    public float trackTime = 6;

    [Header("Behavior Preferences - Wander")]
    public float wanderTime = 2;
    public float wanderRadius = 5;
    public float minDisatnce = 0.2f;

    [Header("Behavior Preferences - Normal")]
    [Range(0, 1)] public float normal_moveClose = 0.9f;
    [Range(0, 1)] public float normal_moveAway = 0;
    [Range(0, 1)] public float normal_attack = 0;
    [Range(0, 1)] public float normal_wait = 0.1f;

    [Header("Behavior Preferences - Move Range")]
    [Range(0, 1)] public float moveRange_moveClose = 0.4f;
    [Range(0, 1)] public float moveRange_moveAway = 0.1f;
    [Range(0, 1)] public float moveRange_attack = 0.3f;
    [Range(0, 1)] public float moveRange_wait = 0.2f;

    [Header("Behavior Preferences - Long Range")]
    [Range(0, 1)] public float longRange_moveClose = 0.4f;
    [Range(0, 1)] public float longRange_moveAway = 0.2f;
    [Range(0, 1)] public float longRange_attack = 0.3f;
    [Range(0, 1)] public float longRange_wait = 0.1f;

    [Header("Behavior Preferences - Close Range")]
    [Range(0, 1)] public float closeRange_moveClose = 0.1f;
    [Range(0, 1)] public float closeRange_moveAway = 0.1f;
    [Range(0, 1)] public float closeRange_attack = 0.8f;
    [Range(0, 1)] public float closeRange_wait = 0;

    [Header("Behavior Preferences - Closest Range")]
    [Range(0, 1)] public float closestRange_moveClose = 0;
    [Range(0, 1)] public float closestRange_moveAway = 0.3f;
    [Range(0, 1)] public float closestRange_attack = 0.7f;
    [Range(0, 1)] public float closestRange_wait = 0;

    private void OnValidate()
    {
        normalize(ref normal_moveClose, ref normal_moveAway, ref normal_attack, ref normal_wait);
        normalize(ref moveRange_moveClose, ref moveRange_moveAway, ref moveRange_attack, ref moveRange_wait);
        normalize(ref longRange_moveClose, ref longRange_moveAway, ref longRange_attack, ref longRange_wait);
        normalize(ref closeRange_moveClose, ref closeRange_moveAway, ref closeRange_attack, ref closeRange_wait);
        normalize(ref closestRange_moveClose, ref closestRange_moveAway, ref closestRange_attack, ref closestRange_wait);
    }

    private void normalize(ref float moveClose, ref float moveAway, ref float attack, ref float wait)
    {
        float sum = moveClose + moveAway + attack + wait;
        moveClose = moveClose / sum > 1 ? 1 : moveClose / sum;
        moveAway = moveAway / sum > 1 - moveClose ? 1 - moveClose : moveAway / sum;
        attack = attack / sum > 1 - moveClose - moveAway ? 1 - moveClose - moveAway : attack / sum;
        wait = 1.0f - moveClose - moveAway - attack;

        Assert.IsTrue(Mathf.Approximately(moveClose + moveAway + attack + wait, 1.0f));
    }
}
