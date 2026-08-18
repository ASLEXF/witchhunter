# 01 Build Compatibility

## 问题

运行时脚本引用了 `UnityEditor*` 命名空间。编辑器内能编过，Player / 正式包会编译失败。

## 涉及文件

- `Assets/Scripts/Characters/Player/PlayerController.cs`
- `Assets/Scripts/Characters/NPC/Enemy/Wolf/NPCController.cs`
- `Assets/Scripts/UI/HUD/ItemUI.cs`
- `Assets/Scripts/Managers/ItemManager.cs`
- `Assets/Scripts/Scenes/MainMenu/MainMenu.cs`
- `Assets/Scripts/Scenes/SceneLoader/SceneLoader.cs`
- `Assets/Scripts/Utility/Find/MyFindItem.cs`
- `Assets/Scripts/Tools/ScriptReader.cs`

## 根因

误加或调试残留的 Editor using。这些程序集只在编辑器里存在。

## 改法

1. 删除未使用的 Editor using（多数是误加，代码里并未调用对应类型）。
2. `MainMenu.QuitGame` 里的 `UnityEditor.EditorApplication` 用 `#if UNITY_EDITOR` 包住（已有则保持）。
3. `ScriptReader` 的 CustomEditor 若要恢复，拆到 `Assets/Editor/ScriptReaderEditor.cs`，不要放在运行时程序集。
4. 出包前在 `Assets/Scripts` 下搜索 `using UnityEditor`，结果应为 0。

## 验收

- 切 Dedicated Server 或 Windows Player 目标，工程能编过。
- 编辑器 Play Mode 行为不变。
