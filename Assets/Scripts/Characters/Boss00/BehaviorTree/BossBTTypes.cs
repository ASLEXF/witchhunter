using System;
using System.Collections.Generic;
using UnityEngine;

public enum BossBTConditionType
{
    Always = 0,
    PlayerInRange = 1,
    PlayerOutsideRange = 2,
    HealthPercentBelow = 3,
    HealthPercentAbove = 4,
    HasStorySignal = 5,
    DrawSwordCompleted = 6,
    VolleyIdle = 7,
}

public enum BossBTActionType
{
    Wait = 0,
    Idle = 1,
    FacePlayer = 2,
    ChasePlayer = 3,
    StopMove = 4,
    PlayAnimation = 5,
    DrawSword = 6,
    FireBouncyVolley = 7,
}

public enum BossAIStartMode
{
    Manual = 0,
    OnEnable = 1,
    AfterDrawSword = 2,
    OnStorySignal = 3,
}

/// <summary>
/// 一条行为：条件全部成立后，按顺序执行动作。列表从上到下是选择节点优先级。
/// </summary>
[Serializable]
public class BossBTTask
{
    public string name = "Task";
    public bool enabled = true;
    [Tooltip("执行中若条件不再成立，立刻中断并重新选择")]
    public bool abortIfConditionFails = true;
    [Tooltip("整段序列完成后的冷却（秒）")]
    public float cooldown;
    public List<BossBTCondition> conditions = new List<BossBTCondition>();
    public List<BossBTAction> actions = new List<BossBTAction>();
}

[Serializable]
public class BossBTCondition
{
    public BossBTConditionType type = BossBTConditionType.Always;
    [Tooltip("取反")]
    public bool invert;
    [Tooltip("PlayerInRange / PlayerOutsideRange")]
    public float minDistance;
    [Tooltip("PlayerInRange / PlayerOutsideRange")]
    public float maxDistance = 20f;
    [Tooltip("HealthPercentBelow / HealthPercentAbove，0~1")]
    [Range(0f, 1f)] public float healthPercent = 0.5f;
    [Tooltip("HasStorySignal")]
    public string signalId = "Boss00_DrawSword";
}

[Serializable]
public class BossBTAction
{
    public BossBTActionType type = BossBTActionType.Wait;
    [Tooltip("Wait / Idle / Chase / DrawSword / Volley 的持续或覆盖时间")]
    public float duration = 1f;
    [Tooltip("ChasePlayer")]
    public float moveSpeed = 3.5f;
    [Tooltip("ChasePlayer 进入此距离后结束")]
    public float stopDistance = 2.5f;
    [Tooltip("PlayAnimation 的 Trigger 名；Chase 时可选 IsRunning 一类 Bool")]
    public string animationName = "";
}
