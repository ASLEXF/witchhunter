using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchHunter/Boss00/Behavior Tree", fileName = "Boss00BehaviorTree")]
public class BossBehaviorTreeAsset : ScriptableObject
{
    [Tooltip("自上而下的优先选择：第一条条件成立的任务会被执行")]
    public List<BossBTTask> tasks = new List<BossBTTask>();
}
