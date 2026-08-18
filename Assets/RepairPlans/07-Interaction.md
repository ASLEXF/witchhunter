# 07 Interaction

## 问题

- 进入交互列表接受 `NPC` 和 `Enemy`，离开只处理 `NPC`，尸体提示卡住。
- 任意 trigger 退出都关掉对话框，路过地上的箭会掐剧情对话。
- 搜刮尸体后改成 `DeadEmpty` 不刷新可交互状态。
- 无 `NPCInteract` 的敌人在 Enter 时可能 NRE。

## 涉及文件

- `Assets/Scripts/Characters/Player/PlayerInteract.cs`
- `Assets/Scripts/Characters/NPC/NPCInteract.cs`
- `Assets/Scripts/Characters/NPC/NPCHealth.cs`（调用刷新，见 02）

## 根因

Enter / Exit 标签不对称。对话框清理范围过大。死亡与搜刮没有统一刷新交互事件。

## 改法

1. `OnTriggerExit`：与 Enter 对称，`NPC || Enemy` 都 `Remove`，并清 `currentCollider`。
2. 离开掉落物不要 `DialogBox.ClearAndHide()`；仅 `type == NPC` 或当前真在对话时再关。
3. `GetBodyItem` 末尾 `UpdateIsInteractable()` + `GameEvents.InteractableUpdated()`。
4. `NPCHealth.Die` 同样刷新（见 02-HealthAndDeath）。
5. `GetComponentInChildren<NPCInteract>()` 做空判断，避免无交互组件的敌人 NRE。

## 验收

- 靠近 / 离开尸体提示正确。
- 搜刮后切到“抬尸体 / 消失”。
- 路过地上箭不关剧情对话。
