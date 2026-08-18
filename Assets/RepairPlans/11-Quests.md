# 11 Quests

## 问题

打开对话就会走 `CheckFinish()`。村长任务在 fang ≥ 5 时立刻扣 5 个并发奖，只是看一眼对话也会交任务。

`ActivateTasksByStage` 写死 stage `1`，忽略 `GameManager.Stage`。

## 涉及文件

- `Assets/Scripts/Tasks/Mayor/task1_1.cs`
- `Assets/Scripts/Characters/NPC/NPCInteract.cs`
- `Assets/Scripts/Characters/NPC/NPCTasks.cs`

## 根因

“查询是否可完成”和“执行完成结算”写在同一个方法里，又被对话流程调用。

## 改法

1. `CheckFinish` **只读**：fang ≥ 5 返回 true，不扣、不发奖、不改 `taskStatus`。
2. 新增 `Complete()`：扣 5 fang、`ItemsUpdated`、`taskStatus = finished`、`getReward()`。
3. `Talk()`：`CheckFinish()` 为真时播 finish 对话；在对话结束回调或玩家确认交任务时再 `Complete()`。
4. `ActivateTasksByStage` 用 `GameManager.Instance.Stage`，不要写死 `1`。

`ITask` 若增加 `Complete()`，所有实现类一起补；若不想改接口，可把 `Complete` 留在 `task1_1`，由 `NPCInteract` 对具体类型调用（不推荐，第二期再收口）。

## 验收

- 对话查看任务不扣 fang。
- 确认完成才扣 5 个并发奖。
- 非 stage 1 时不会误激活该任务（若有多阶段）。
