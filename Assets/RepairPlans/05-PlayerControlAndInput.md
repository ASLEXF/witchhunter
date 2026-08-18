# 05 Player Control And Input

## 问题

- `HideThePlayer` 把脚本 `enabled = false`，`ShowThePlayer` 不恢复，角色回不来。
- 左手蓄满后松手会再打一记普攻；右手已有完成守卫。
- `PlayerInputMapHandler.OnDisable` 对探索模式是 `+=` 不是 `-=`，会叠订阅。
- `StoryManager.EndStoryMode` 从未被调用，剧情后玩家一直不能动。

## 涉及文件

- `Assets/Scripts/Characters/Player/PlayerController.cs`
- `Assets/Scripts/Characters/Player/PlayerInputMapHandler.cs`
- `Assets/Scripts/Managers/StoryManager.cs`
- `Assets/Scripts/Utility/Story/StoryNode.cs`

## 根因

显示/隐藏与“能否接受输入”混在同一个 `enabled` 上。左右手蓄力收尾不对称。事件退订写反。剧情结束没有统一出口。

## 改法

1. `ShowThePlayer` 末尾 `enabled = true`；`HideThePlayer` 保持关。
2. 左手 `canceled` 对齐右手：若 `isChargingCompletedL` 则只清标志，不再 `AttackL()`。
3. `PlayerInputMapHandler.OnDisable`：`OnExplorationModeStarted -= InputActionsToExploration`。
4. `StoryManager.EndStoryMode` 在 Timeline `stopped` 或对话结束时调用（与 10-Timeline 约定：Timeline 停 + 无对话 → `EndStoryMode`）。
5. 删掉 `PlayerController` 里无用的 `UnityEditor` using（见 01-BuildCompatibility）。

## 验收

- 非 Debug 开局隐藏 → 剧情后能走。
- 左手蓄满松手只重击一次。
- 开关背包 / 菜单后探索 action map 不叠。
