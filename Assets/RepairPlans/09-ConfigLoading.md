# 09 Config Loading

## 问题

`ItemManager`、`NPCInteract`、`TimeLineManager` 用 `File.Exists("Assets/Config/...")` 读表。编辑器能读到工程目录；Player 包里没有这套路径，物品生成、NPC 对话 / 掉落、Timeline 绑定在真机上会全部失败。

## 涉及文件

- `Assets/Scripts/Managers/ItemManager.cs`
- `Assets/Scripts/Characters/NPC/NPCInteract.cs`
- `Assets/Scripts/Managers/TimeLineManager.cs`

## 根因

把 Unity 工程内路径当成运行时文件系统路径。

## 改法

三选一，推荐 A。统一一个 `ConfigLoader`，三处共用。

- **A.** JSON 做成 Addressable TextAsset（`Config/Items.json` 等），`Addressables.LoadAssetAsync<TextAsset>` 后 `JsonConvert.Deserialize`。
- **B.** 拷到 `StreamingAssets`，`Path.Combine(Application.streamingAssetsPath, ...)`。
- **C.** 已有 `Assets/Scripts/Gen/Tb*.cs` 则运行时走生成表，不再读原始 JSON。

反序列化失败只 return / log，不碰 null 集合。

`ItemManager.GenerateItem` 在 `items == null` 时必须 return null（见 06-InventoryAndItems）。

## 验收

- 编辑器与 Windows 包都能生成物品。
- 能读 NPC 对话配置。
- 能绑 Timeline 轨道。
