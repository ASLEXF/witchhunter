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
    [Header("Damage")]
    public int biteDamage = 1;

    [Header("Movement")]
    public float speed = 6;
    public float MaxSpeed = 14;
    public float Acceleration = 120;
    public float GroundDeceleration = 60;

    [Header("Behavior Preferences")]
    public ApproachMethod approachMethod = ApproachMethod.straight;

    [Range(0, 1)] public float normal_moveClose = 0.8f;
    [Range(0, 1)] public float normal_moveAway = 0;
    [Range(0, 1)] public float normal_attack = 0;
    [Range(0, 1)] public float normal_wait = 0.2f;

    [Range(0, 1)] public float moveRange_moveClose = 0.4f;
    [Range(0, 1)] public float moveRange_moveAway = 0.1f;
    [Range(0, 1)] public float moveRange_attack = 0.3f;
    [Range(0, 1)] public float moveRange_wait = 0.2f;

    [Range(0, 1)] public float longRange_moveClose = 0.4f;
    [Range(0, 1)] public float longRange_moveAway = 0.2f;
    [Range(0, 1)] public float longRange_attack = 0.3f;
    [Range(0, 1)] public float longRange_wait = 0.1f;

    [Range(0, 1)] public float closeRange_moveClose = 0.1f;
    [Range(0, 1)] public float closeRange_moveAway = 0.1f;
    [Range(0, 1)] public float closeRange_attack = 0.8f;
    [Range(0, 1)] public float closeRange_wait = 0;

    [Range(0, 1)] public float closestRange_moveClose = 0;
    [Range(0, 1)] public float closestRange_moveAway = 0.3f;
    [Range(0, 1)] public float closestRange_attack = 0.7f;
    [Range(0, 1)] public float closestRange_wait = 0;

    private void OnValidate()
    {
        normalize(normal_moveClose, normal_moveAway, normal_attack, normal_wait);
        normalize(moveRange_moveClose, moveRange_moveAway, moveRange_attack, moveRange_wait);
        normalize(longRange_moveClose, longRange_moveAway, longRange_attack, longRange_wait);
        normalize(closeRange_moveClose, closeRange_moveAway, closeRange_attack, closeRange_wait);
        normalize(closestRange_moveClose, closestRange_moveAway, closestRange_attack, closestRange_wait);
    }

    private void normalize(params float[] nums)
    {
        float sum = nums.Sum();
        for (int i = 0; i < nums.Length; i++)
        {
            nums[i] = nums[i] / sum;
        }

        Assert.IsTrue(nums.Sum() == 1);
    }
}
