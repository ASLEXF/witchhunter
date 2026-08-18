# 08 Story And Dialog

## 问题

- `NPCChecker` 用未定义 tag `"Enermy"`，`FindGameObjectsWithTag` 每帧抛异常；工程里只有 `"Enemy"`。
- 就算不抛，活敌人数永远是 0，序章 1-2 会提前触发。
- `StoryTree.RemoveNode` 把扁平列表的“上一项”当成父节点，播完一个节点树就乱。
- `EndStoryMode` 从未调用，`isPlaying` 不复位，后续节点不播，玩家不能动。
- `DialogBox.TypingSentence` 跳过转场时无边界，纯转场剧本会越界；`Clear` 不停协程。
- `CheckConditiion` 遇到第一个 key 就 return，多条件无法同时成立；`conditions == null` 会 NRE。

## 涉及文件

- `Assets/Scripts/Tools/NPCChecker.cs`
- `Assets/Scripts/Utility/Story/StoryNode.cs`
- `Assets/Scripts/Utility/Story/StoryTree.cs`
- `Assets/Scripts/Managers/StoryManager.cs`
- `Assets/Scripts/UI/DialogBox.cs`
- `Assets/Scripts/Tools/ScriptReader.cs`

## 根因

tag 拼写错误。剧情树删除按列表邻接而不是父子关系。剧情结束没有统一出口。打字机假设剧本一定有对白句。

## 改法

1. `GetEnermyAliveNum`：`FindGameObjectsWithTag("Enemy")`。跳过子物体（`name == "Animator"` 可改成只数带 `NPCStatusEffect` 的根）。
2. `CheckConditiion`：`conditions == null || Count == 0` 视为 true；多条件要全部满足再 `return true`，不要第一个 key 就 return。
3. `StoryTree.RemoveNode`：从**父节点**的 `next` 里移除，并把该节点的 `next` 接到父上（或按设计只移除自身、子节点顶上来）。不要用扁平列表的“上一项”。
4. `StoryManager`：Timeline / 对话结束后调 `EndStoryMode()`；`isPlaying` 复位，否则后续节点永不播。
5. `DialogBox.TypingSentence`：`while (index < input.Count && (type == 1 || type == 2))`；协程用 token / `StopCoroutine`，`Clear` 时停掉。
6. `ScriptReader` 去掉运行时 `using UnityEditor`（见 01-BuildCompatibility）。

## 验收

- Cliff 有狼时序章 1-2 不触发；杀光后再触发。
- 剧情结束后能移动。
- 纯转场剧本不越界。
