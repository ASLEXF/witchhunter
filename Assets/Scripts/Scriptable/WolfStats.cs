using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


public enum  PatrolType
{
    idle = 0,
    wander = 1,
    patrol = 2,
}

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
    [Header("Movement")]
    public float MaxSpeed = 14;
    public float Acceleration = 12;
    //public float GroundDeceleration = 60;

    [Header("Sight")]
    [Range(90, 180)] public float sightAngle = 90;
    [Range(5, 25)] public float sightDistance = 12.0f;
    [Range(8, 48)] public int sightLineNum = 24;
    [Range(60, 360)] public float sightAngleSpeed = 180;

    [Header("Behavior Preferences")]
    public MinMaxFloat decisionInterval = new MinMaxFloat {Min = 0.3f, Max = 1.2f};
    public PatrolType patrolType = PatrolType.wander;
    public ApproachMethod approachMethod = ApproachMethod.straight;
    public float trackTime = 6;

    [Header("Behavior Preferences - Wander")]
    public float wanderSpeed = 1.5f;
    public float wanderAngularSpeed = 999;
    public MinMaxFloat wanderTime = new MinMaxFloat {Min = 2f, Max = 6f};
    public MinMaxFloat wanderRadius = new MinMaxFloat {Min = 3f, Max = 7f};

    [Header("Behavior Preferences - Chase")]
    public float chaseSpeed = 6;
    public float chaseAngularSpeed = 600;
    public float repathDistance = 0.1f;

    [Header("Behavior Posibilities")]
    [SerializeField] public ActionProbabilityGroup combatRange = new ()
    {
        moveClose = 0.9f,
        moveAway = 0f,
        attack = 0f,
        wait = 0.1f
    };
    [SerializeField] public ActionProbabilityGroup moveRange = new ()
    {
        moveClose = 0.4f,
        moveAway = 0.1f,
        attack = 0.3f,
        wait = 0.2f
    };
    [SerializeField] public ActionProbabilityGroup longRange = new ()
    {
        moveClose = 0.4f,
        moveAway = 0.2f,
        attack = 0.3f,
        wait = 0.1f
    };
    [SerializeField] public ActionProbabilityGroup closeRange = new ()
    {
        moveClose = 0.1f,
        moveAway = 0.1f,
        attack = 0.8f,
        wait = 0f
    };
    [SerializeField] public ActionProbabilityGroup closestRange = new ()
    {
        moveClose = 0,
        moveAway = 0.3f,
        attack = 0.7f,
        wait = 0
    };

    [Header("Attack - Bite")]
    public int biteDamage = 1;
    public Vector2 bitePostion = new Vector2(-0.317f, 0.263f);

    [Header("Attack - Claw")]
    public int clawDamage = 1;
    public Vector2 clawPosition = new Vector2(0, 0);

    private void OnValidate()
    {
        decisionInterval.CheckValidity(nameof(decisionInterval), this);
        wanderTime.CheckValidity(nameof(wanderTime), this);
        wanderRadius.CheckValidity(nameof(wanderRadius), this);
    }
}
