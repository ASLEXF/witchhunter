# 02 Health And Death

## 问题

- 敌人血量刚好到 0 不会死，需要再补一刀。
- 玩家治疗永远加不满最后一点。
- 约 80% 的 NPC 死亡会空引用（`NPCInteract` 未赋值却调用 `GenerateItems`）。
- 死亡掉落在 `SetDead` 里生成后被丢掉。
- 血条按 `MaxHealth` 索引，格子不够会越界。
- 死亡不通知新 AI，死敌仍可能行动。

## 涉及文件

- `Assets/Scripts/Characters/NPC/NPCHealth.cs`
- `Assets/Scripts/Characters/Player/PlayerHealth.cs`
- `Assets/Scripts/Characters/NPC/NPCStatusEffect.cs`
- `Assets/Scripts/Characters/NPC/NPCAttacked.cs`
- `Assets/Scripts/UI/HUD/PlayerStatus/HealthBarUI.cs`
- `Assets/Scripts/Characters/NPC/Enemy/EnemyAIController.cs`（只加死亡查询 / 切状态接口）

## 根因

- `TakeDamage` 用 `resultHealth < 0` 而不是 `<= 0`。
- `Heal` 用 `health < maxHealth`，把加满排除。
- `NPCStatusEffect.NPCInteract` 只声明、从未赋值；`SetDead` 还负责生成物品。
- `NPCHealth.NPCInteract` 同样未赋值，`Die()` 里的 `UpdateIsInteractable()` 是空操作。
- `HealthBarUI` 假设心形数量永远 ≥ 当前上限。

## 改法

1. `NPCHealth.TakeDamage`
   - 无敌则直接 return，不扣血。
   - `currentHealth = max(0, currentHealth - damage)`。
   - `currentHealth <= 0` 时 `Die()`，只死一次（`_dead` 标志或看 `statusEffect.Dead`）。

2. `PlayerHealth.Heal`
   - 改为 `currentHealth = min(maxHealth, currentHealth + amount)`，满血也要能加上最后一点。
   - `AddMaxHealth` 后钳制 `currentHealth`，并发送 `PlayerHealthChanged()`。
   - `ReduceMaxHealth`：`maxHealth = max(1, maxHealth - 1)`，`currentHealth = min(currentHealth, maxHealth)`。
   - `die()` 加守卫，避免同帧重复；复活后给短暂无敌。

3. `NPCStatusEffect`
   - `Awake` 里 `NPCInteract = transform.parent.GetComponentInChildren<NPCInteract>()`。
   - `SetDead`：只改 `lifeStatus`，不要在这里 `GenerateItems`（掉落留给玩家搜刮，见 07-Interaction）。
   - `Start` 不要无条件 `lifeStatus = Alive`；只在未序列化 / 首次初始化时设。
   - `Stunned` setter：已有则不要重复 `Add`。

4. `NPCHealth.Die`
   - 赋值并调用 `NPCInteract.UpdateIsInteractable()`。
   - 通知 `EnemyAIController` 切 `DeadState`（`GetComponent`，没有则跳过）。

5. `HealthBarUI.showHealthBar`
   - 循环上界 `min(hearts.Length, MaxHealth)`。
   - 心形数量不够时 clamp，不要越界。

## 验收

- 4 血吃 4 点伤害必死。
- 4/5 吃生肉到 5/5。
- 杀狼不抛异常。
- 尸体可交互。
- 药水加上限不炸 UI。
