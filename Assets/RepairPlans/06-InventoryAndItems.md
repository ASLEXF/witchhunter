# 06 Inventory And Items

## 问题

- `ItemBar` / `Backpack` 用新 lambda 退订，解不掉原来的订阅，重复刷新且禁用后仍回调。
- `ItemUI.UpdateUI` 不处理 `ProjectileItem`，箭数量显示被清空。
- `UpdateProjectile` 减到 0 不清格子，留下幽灵箭。
- `ItemManager.GenerateItem` 反序列化失败后仍访问 `items.Count`；投射物 `amount` 不写入。
- `DroppedItem` 盲删 parent（与 03-Projectiles 约定同一修复）。

## 涉及文件

- `Assets/Scripts/UI/HUD/ItemBar.cs`
- `Assets/Scripts/UI/HUD/Backpack.cs`
- `Assets/Scripts/UI/HUD/ItemUI.cs`
- `Assets/Scripts/Managers/ItemManager.cs`
- `Assets/Scripts/Items/DroppedItem.cs`
- `Assets/Scripts/Characters/Player/PlayerInventory.cs`

## 根因

匿名 lambda 每次都是新委托，`-=` 对不上。UI 刷新路径按消耗品写死。配置加载与空引用未防护。

## 改法

1. 事件：成员方法订阅，例如 `OnItemsUpdated += HandleItemsUpdated`，`OnDisable` / `OnDestroy` 用同一方法 `-=`。
2. `ItemUI.UpdateUI`：`ProjectileItem` 走 `UpdateAmount(amount)`。
3. `ItemBar.UpdateProjectile`：`amount <= 0` 时清空格子（与 `UpdateAmount(0)` 一致），不要留幽灵箭。
4. `ItemManager.GenerateItem`：
   - `items == null` 则 return null。
   - `ProjectileItem` 也写入 `amount`。
   - 加载改走 09-ConfigLoading，不读 `Assets/Config` 绝对路径。
5. `DroppedItem`：Destroy `IItem` 根物体（见 03-Projectiles）。
6. `HandleInventoryFull`：至少调用 `GameEvents.AddItemWhenInventoryFull()`，避免物品静默丢失（地面掉回可第二期）。

## 验收

- 开关物品栏不重复刷 UI。
- 射箭数字减少；射光格子空。
- 生成失败只 log 不炸。
