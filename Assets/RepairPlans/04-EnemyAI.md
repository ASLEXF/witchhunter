# 04 Enemy AI

## 问题

- 看见玩家后追的是视野锥形心，不是玩家。
- `IsDead()` 恒为 `false`，FSM 永不进死亡态。
- 用 `!Agent.hasPath` 当“到达”，`SetDestination` 后几帧无路径会被当成到达，立刻回 Idle。
- 追逐进战斗条件运算符优先级导致“近距但未见”也会进战斗。
- 旧 `NPCController.initializePrefs()` 整段注释，决策读空列表会越界。
- `track` 到达判定两处都在比 X 轴。

## 涉及文件

- `Assets/Scripts/Characters/NPC/Enemy/EnemyAIController.cs`
- `Assets/Scripts/Characters/NPC/Enemy/VisualRange.cs`
- `Assets/Scripts/Characters/NPC/Enemy/EnemyChaseState.cs`
- `Assets/Scripts/Characters/NPC/Enemy/EnemyPatrolState.cs`
- `Assets/Scripts/Characters/NPC/Enemy/EnemyCombatState.cs`
- `Assets/Scripts/Characters/NPC/Enemy/Wolf/NPCController.cs`
- `Assets/Scripts/Characters/NPC/NPCHealth.cs`（只负责通知死亡，见 02）

## 根因

- `OnTriggerStay` 用玩家位置调用 `CanSeePlayer` 后，立刻用视野多边形质心覆盖 `TargetPosition`。
- 新 AI 的死亡查询未接 `NPCStatusEffect`。
- NavMesh 寻路是异步的，`hasPath` 在路径算完前为 false。
- 场景里同时存在 `NPCController` 与 `EnemyAIController` 时会抢 `NavMeshAgent`。

## 改法

1. `VisualRange.OnTriggerStay`：`TargetPosition` 用玩家位置（`collider.bounds.center` 或 `collider.transform.position`），删掉 `getColliderCenter()` 覆盖。
2. `EnemyAIController.IsDead()`：`return statusEffect != null && statusEffect.Dead;`
3. 到达判定：`!hasPath` 改成 `!pathPending && remainingDistance <= stoppingDistance`（追逐还要保留“丢失目标再 Idle”）。
4. `EnemyChaseState` 条件写成 `SeePlayer && (IsCombatRange || IsCloseRange)`，避免未见敌因近距进战斗。
5. `NPCController`：
   - 恢复 `initializePrefs()`，按距离填 closest / close / long / move / normal。
   - `track` 第二处改为比 `targetPosition.y` 与 `transform.position.y`。
   - 若场景已切到 `EnemyAIController`，给 `NPCController` 加“禁用时不跑 Update”，或文档标明二选一，避免双 AI 抢 Agent。
6. 死亡：`NPCHealth.Die` → `EnemyAIController.ChangeState(DeadState)`，Agent `isStopped`、关范围触发器（已有 `ranges.SetActive(false)` 可保留）。

## 验收

- 看见玩家朝玩家跑，而不是朝视野锥内部。
- 死狼停 AI、停寻路。
- 巡逻不会出发即回 Idle。
- 近距但墙后不立刻咬。
