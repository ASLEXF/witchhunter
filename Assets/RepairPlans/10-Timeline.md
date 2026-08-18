# 10 Timeline

## 问题

- `LoadBindingTable` / `WaitDialogBoxEnded` 每次 `+=` 且不 `-=`，重复绑定、重复恢复播放。
- `StopTimeLine` 把 `_playableDirector` 置 null，之后再也播不了。
- `StoryNode.Play` 在 Addressables 未完成时同步 `PlayTimeLine`，此时 `playableAsset` 仍空。
- Timeline 结束没有通知 `StoryManager.EndStoryMode`。

## 涉及文件

- `Assets/Scripts/Managers/TimeLineManager.cs`
- `Assets/Scripts/Managers/StoryManager.cs`

## 根因

事件生命周期与 Director 引用管理不完整。播放时机早于资源加载完成。

## 改法

1. `bindTimelineTracks` / `WaitDialogBoxEnded`：先 `-=` 再 `+=`，或只订一次。
2. `StopTimeLine`：`Stop()` 后清空 `playableAsset`，保留 `_playableDirector` 引用。
3. `_playableDirector.stopped` → 若无对话进行中，调 `StoryManager.EndStoryMode()`。
4. `PlayTimeLine` 在 `LoadPlayableAsset` 完成且 binding 完成后再播。去掉 `StoryNode.Play` 里那次同步空 Play，改由 `PlayableAssetLoaded` 触发。

与 05-PlayerControlAndInput、08-StoryAndDialog 的约定：Timeline 停 + 无对话 → `EndStoryMode`。

## 验收

- 连续播两段 Timeline 不重复绑。
- Stop 后还能再播。
- 播完恢复玩家控制。
