# 03 Projectiles

## 问题

- `BoneArrow` / `FangArrow` 未实现 `IProjectile`，射击与碰撞管线接不上。
- 骨箭 / 牙箭命中不造成伤害（`TODO`）。
- `FangArrow` 建成 id 13 的 `ConsumableItem("iron arrow")`，会和铁箭叠堆。
- `PlayerAttack.generateProjectile` 对空接口直接 `.Shoot()`，装备骨箭 / 牙箭时 NRE。
- `PlayerHand.addToHand` 无空判断地取 `DroppedItem`；Addressables 未完成就 `SetProjectile`。
- 骨箭无 `Sprite` 子物体时，`DroppedItem` 加在根节点，捡起会 `Destroy(Environment)`。

## 涉及文件

- `Assets/Scripts/Interfaces/Interfaces.cs`
- `Assets/Scripts/Items/Consumable/Projectile/Arrow.cs`
- `Assets/Scripts/Items/Consumable/Projectile/BoneArrow.cs`
- `Assets/Scripts/Items/Consumable/Projectile/FangArrow.cs`
- `Assets/Scripts/Items/Consumable/Projectile/ArrowHitBoxTrigger.cs`
- `Assets/Scripts/Characters/Player/PlayerHand.cs`
- `Assets/Scripts/Characters/Player/PlayerAttack.cs`
- `Assets/Scripts/Items/DroppedItem.cs`

## 根因

铁箭已按 `IProjectile` 重写，骨箭 / 牙箭仍是旧实现。`DroppedItem.Interacted` 盲删 `transform.parent`。

## 改法

1. `BoneArrow` / `FangArrow` 实现 `IProjectile`，`Hit(Collider2D)` 对齐 `Arrow.Hit`（忽略 Player / Item / Ignore Raycast）。
2. `stickOnto` 对齐 `Arrow.stickInto`：
   - 命中 Enemy → `NPCAttacked.GetAttacked(damage, 0, collider)`。
   - `isShooting` 防重复结算。
   - 父节点用 `Environment.Instance.Projectiles`，不要挂 Environment 根。
3. `FangArrow.Awake`：改为 `new ProjectileItem(14, "fang arrow", ..., "Prefabs/Items/fang_arrow.prefab", 1)`（id 与 `PlayerHand.tryEquipProjectile` 的 13/14/15 一致，以物品表为准）。
4. 消耗判定统一：`if (Random.value < consumedChance) 延迟销毁; else 启用 DroppedItem`。
5. 删掉 `if (force == null)`（`Vector2` 是值类型，恒为假）。
6. `PlayerAttack.generateProjectile`：找不到 `IProjectile` 则 return，并打 log。
7. `PlayerHand.addToHand`：
   - `DroppedItem` 可空。
   - 换箭先 `Addressables.Release` 旧 handle。
   - `SetProjectile` 在 `projectileObj == null` 时跳过或排队到加载完成。
8. `DroppedItem.Interacted`：`Destroy` 带 `IItem` 的根（`GetComponentInParent<IItem>()` 的 gameObject），禁止 `Destroy(transform.parent)` 这种盲删。

## 验收

- 三种箭能射出、能伤敌、能插地 / 插怪。
- 牙箭不进铁箭堆。
- 捡骨箭不删场景。
- 没箭或未实现接口时射击不 NRE。
