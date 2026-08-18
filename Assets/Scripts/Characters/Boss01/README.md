# Boss01 功能脚本说明

本目录只提供**功能脚本**，不含预制体、动画、音效或特效资源。把脚本挂到 Boss / 场地物体上，在 Inspector 里填引用和数值即可使用。

本 Boss 只做两件事，互不依赖：

1. **激光**：从起点到终点铺一块矩形，在持续时间内按间隔反复结算伤害
2. **破坏场地**：改部分场地造型，并封锁该区域，使其不可达

默认约定：

- 玩家 Tag：`Player`（碰撞体在子物体上、子物体没打 Tag 也能打到，见 5.4）
- 伤害走 `PlayerAttacked.GetAttacked(int, float, Vector3)`（尊重无敌）；没有该组件时退回 `PlayerHealth.TakeDamage`
- 玩家用 `Rigidbody2D` 移动，破坏后靠非 Trigger 的 `Collider2D` 挡住
- 敌人若用 `NavMeshAgent`，破坏后会启用带 Carve 的 `NavMeshObstacle`
- 朝向与 Boss00 / 狼 AI 相同：根节点欧拉角 `Y > 90` 视为朝右
- 本目录**不改** Boss00、玩家、寻路或 `GameEvents` 的现有脚本

没有预制体也能跑：激光会现场生成白色矩形；场地块若没指定阻挡碰撞，会按造型范围现场加一个 `BoxCollider2D`。

---

## 目录

1. [文件一览](#1-文件一览)
2. [推荐物体结构](#2-推荐物体结构)
3. [整体流程](#3-整体流程)
4. [空引用时怎么找](#4-空引用时怎么找)
5. [激光攻击](#5-激光攻击)
6. [破坏场地](#6-破坏场地)
7. [两套功能怎么一起用](#7-两套功能怎么一起用)
8. [对外接口](#8-对外接口)
9. [与现有系统的衔接](#9-与现有系统的衔接)
10. [常见问题](#10-常见问题)
11. [验收清单](#11-验收清单)

---

## 1. 文件一览

| 路径 | 类型 | 作用 |
| --- | --- | --- |
| `Boss01LaserBeam.cs` | 组件 | 单条激光判定体：按起点–终点对齐矩形，按间隔持续伤害 |
| `Boss01LaserAttack.cs` | 组件 | 激光技能：算起点/终点、生成判定体、可选跟随瞄准 / 撞墙截断 |
| `Boss01ArenaPiece.cs` | 组件 | 一块可破坏场地：换造型、关可走碰撞、开阻挡、可选改 Tilemap |
| `Boss01ArenaBreaker.cs` | 组件 | 场地破坏技能：按配置打碎一块 / 一批 / 指定 id / 世界矩形内的块 |

枚举：

| 类型 | 用在 | 取值 |
| --- | --- | --- |
| `Boss01LaserAimMode` | `Boss01LaserAttack` | `StartToEnd` / `TowardPlayer` / `Facing` / `Custom` |
| `Boss01ArenaBreakMode` | `Boss01ArenaBreaker` | `AllConfigured` / `NextInOrder` / `RandomUnused` / `ByIds` |

本目录之外**没有**衔接改动。伤害用的是 Boss00 已经加过的 `PlayerAttacked.GetAttacked(..., Vector3)`。

---

## 2. 推荐物体结构

```
Boss01                          ← 根节点，挂激光 /（可选）破坏脚本
├── Muzzle                      ← 可选，激光起点；空则用根节点
├── LaserEnd                    ← 可选，固定终点（Aim Mode = StartToEnd）
└── Status                      ← 可选，NPCHealth 等，本目录不用它做决策

Arena                           ← 场地根节点，可挂 Boss01ArenaBreaker
├── Piece_Bridge                ← Boss01ArenaPiece，Piece Id = bridge
│   ├── Intact                  ← 完好造型（破坏后关掉）
│   └── Destroyed               ← 破坏后造型（进场时先禁用）
├── Piece_Pillar                ← Piece Id = pillar
└── ...
```

Boss 根节点建议挂：

1. `Boss01LaserAttack`
2. `Boss01ArenaBreaker`（若破坏由 Boss 技能触发；也可以挂在场地根上）

每块要被拆的场地**必须**另挂 `Boss01ArenaPiece`。`ArenaBreaker` 可勾 `Auto Find Pieces`，或自己把块拖进列表。

激光预制体（建议单独做，没有也能运行时生成无贴图矩形）：

```
Boss01_LaserBeam
├── BoxCollider2D        Is Trigger = 开
├── SpriteRenderer       可选，用来画梁；空则运行时生成 1×1 白图并拉伸
└── Boss01LaserBeam
```

场地块两种常见做法：

**做法 A：两个子物体换造型（推荐）**

- `Intact`：完好桥 / 地板 / 柱，进场启用
- `Destroyed`：断裂、塌陷、碎石，进场禁用
- `Walkable Collider` 挂在完好一侧（破坏后关掉）
- `Block Collider` 挂在破坏一侧，或留空让脚本按造型范围生成

**做法 B：同一张 Sprite 换图**

- 只挂一个 `SpriteRenderer`
- 把完好 / 破坏两张图拖到 `Intact Sprite` / `Destroyed Sprite`
- 阻挡碰撞仍按上面规则配

引用留空时的自动查找见第 4 节。

---

## 3. 整体流程

两套功能各自独立。典型战斗里可以先后触发，也可以只做其中一件。

```
动画事件 / Timeline / 代码
        │
        ├─► Boss01LaserAttack.Fire()
        │         │
        │         ├─ 按 Aim Mode 算出起点、终点
        │         ├─ 可选：射线截到第一面墙
        │         ├─ 生成或复用 Boss01LaserBeam
        │         ├─ 把 BoxCollider2D 对齐到线段（长=距离，宽=Width）
        │         ├─ 持续 Duration 秒：每隔 Tick Interval 对矩形内玩家结算
        │         └─ 勾了 Follow 则每帧重算端点（跟踪 / 扫射）
        │
        └─► Boss01ArenaBreaker.Break()
                  │
                  ├─ 按 Break Mode 选出未破坏的块
                  ├─ Stagger > 0 时按间隔一块一块拆
                  └─ 每块 Boss01ArenaPiece.Break()
                            │
                            ├─ 关完好造型，开破坏造型
                            ├─ 关可走碰撞，开阻挡碰撞
                            ├─ 启用 NavMeshObstacle（Carve）
                            ├─ 可选：改 Tilemap 指定区域
                            └─ 若玩家叠在新墙上，推到边缘外
```

激光**不会**自动拆地。要「梁扫过的桥塌掉」，在放激光的同时自己调 `BreakById` 或 `BreakInWorldRect`，见第 7 节。

---

## 4. 空引用时怎么找

| 组件 | 空引用时 |
| --- | --- |
| `Boss01LaserAttack.muzzle` | 自身 Transform |
| `Boss01LaserAttack.startPoint` | 先用 Muzzle，再退回自身 |
| `Boss01LaserAttack.endPoint` | `StartToEnd` 模式下改用朝向 × `Max Length` |
| `Boss01LaserAttack.beamPrefab` | 运行时 `new GameObject("Boss01_LaserBeam")` + Trigger `BoxCollider2D` |
| `Boss01LaserBeam.beamRenderer` | `GetComponentInChildren<SpriteRenderer>()`；再没有就现场建白矩形 |
| 玩家（瞄准 / 挤开） | `PlayerController.Instance`，找不到再用 Tag `Player` |
| `Boss01ArenaPiece.sharedRenderer` | 自身 `SpriteRenderer` |
| `Boss01ArenaPiece.intactVisual` | 三个造型引用都空时，退回自身 GameObject（不会把整块 `SetActive(false)`） |
| `Boss01ArenaPiece.blockCollider` | 勾了 `Create Blocker If Missing` 时按造型范围加 `BoxCollider2D` |
| `Boss01ArenaBreaker.pieces` | 勾了 `Auto Find Pieces`：有 `Search Root` 则在其子树找，否则 `FindObjectsOfType` |

`Boss01LaserAttack` 的 owner 取 `transform.root`，用来忽略 Boss 自己的碰撞，以及墙体截断时不要打到自己。

---

## 5. 激光攻击

两层：

- `Boss01LaserAttack`：发射器，负责瞄准、时长、生成判定体
- `Boss01LaserBeam`：判定体，负责矩形对齐、持续伤害、显示

调用 `Boss01LaserAttack.Fire()`（动画事件、Timeline、其它脚本都可以）。正在放时再次 `Fire()` **会打断当前梁**，立刻换成新的一条（和 Boss00 弹球「开火中忽略」不同）。

物体禁用时会 `StopLaser()`，避免梁留在场上。

### 5.1 矩形怎么铺

脚本先得到世界坐标的起点 `A`、终点 `B`，然后：

1. `delta = B - A`，`length = |delta|`（短于 0.01 时按 0.01 处理，避免零面积）
2. 判定体放到中点 `(A + B) / 2`
3. Z 旋转对齐 `atan2(delta.y, delta.x)`
4. `BoxCollider2D.size = (length, Width)`，`offset = 0`，`isTrigger = true`

因此判定是**盖住整条线段的矩形**，不是细线，也不是扇形。玩家只要身体与这块矩形重叠，就会进入持续结算。

选中 `Boss01LaserAttack` 或 `Boss01LaserBeam` 时，Scene 里会画半透明红盒，用来对长度和宽度。

### 5.2 瞄准（Aim Mode）

起点一律先取 `Start Point`（空则 Muzzle / 自身），再沿瞄准方向偏出 `Start Offset`，避免梁从 Boss 身体中心长出来、一帧就打到自己。

| Aim Mode | 起点方向（只影响 Offset） | 终点 |
| --- | --- | --- |
| `StartToEnd` | 指向 `End Point`；没有终点则用朝向 | `End Point` 的世界坐标；没有则朝向 × `Max Length` |
| `TowardPlayer`（默认） | 指向玩家；没有玩家则用朝向 | 起点 + 朝玩家方向 × `Max Length` |
| `Facing` | Boss 当前朝向 | 起点 + 朝向 × `Max Length` |
| `Custom` | `Custom Direction`（零向量则视为左） | 起点 + 该方向 × `Max Length` |

朝向规则：`transform.rotation.eulerAngles.y > 90` → 右，否则左。美术默认应朝左，朝右靠 Y=180，与 Boss00 一致。

`FireTowardPlayer()` **不管**当前 Aim Mode，强制按「朝玩家 + Max Length」算终点。

`Fire(Vector2 start, Vector2 end)` 直接用你给的世界坐标，**不再**走 Aim Mode，也**不再**做墙体截断。需要截断时自己先算好再传入，或走无参 `Fire()`。

### 5.3 发射器字段（Boss01LaserAttack）

**端点**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Start Point | 空 → Muzzle / 自身 | 激光起点 Transform |
| End Point | 空 | 仅 `StartToEnd` 使用 |
| Muzzle | 自身 | `Start Point` 为空时的后备起点 |
| Aim Mode | Toward Player | 见上表 |
| Custom Direction | (−1, 0) | 仅 `Custom` 使用 |
| Max Length | 12 | `TowardPlayer` / `Facing` / `Custom` 的梁长；`StartToEnd` 缺终点时也用它 |
| Start Offset | 0.4 | 沿瞄准方向把起点往外挪 |

**激光**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Beam Prefab | 空 | 判定体预制体。空则运行时生成只有碰撞的矩形（能打；没有预制体贴图时会画白色拉伸块） |
| Width | 0.6 | 矩形宽度，运行时再钳到至少 0.05 |
| Duration | 1.6 | 梁存在秒数，至少按 0.01 算 |
| Tick Interval | 0.35 | 两次结算的间隔。`0` = 每帧都打（会非常痛，只建议调试） |
| Damage | 1 | 传给 `GetAttacked` / `TakeDamage` |
| Knockback | 0.1 | 击退强度，算法与 Boss00 弹球 / 狼撕咬一致 |
| Beam Color | 红、透明度 0.7 | 传给判定体的显示色 |
| Follow Aim While Active | 关 | 开着时每帧按 Aim Mode 重算两端（跟踪玩家或扫射）。`StartToEnd` 下若终点 Transform 在动，梁也会跟着动 |

**墙体截断（可选）**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Stop At Walls | 关 | 从起点向终点做 `Physics2D.Raycast`，打到第一面墙就把终点改到命中点 |
| Wall Mask | Everything | 哪些层算墙 |

射线会忽略：Boss 自己（含子物体）、Tag 为 `Player` 的碰撞。这样梁不会在玩家身上截断，也不会打到 Boss 自己的身体就停。

未勾 `Stop At Walls` 时，梁可以穿过墙继续打墙后的玩家。要「看得见的梁停在墙上」，把墙所在层勾进 `Wall Mask` 并打开此选项。

### 5.4 判定体字段与结算（Boss01LaserBeam）

预制体上可以预调这些；`Activate(...)` 时发射器会覆盖宽度、伤害、击退、间隔、颜色和寿命。

**判定**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Width | 0.6 | `Activate` 没传时的后备值 |
| Damage | 1 | 同上 |
| Knockback | 0.1 | 同上 |
| Tick Interval | 0.35 | 同上 |
| Player Tag | `Player` | 优先用 Tag 认玩家 |
| Hit Mask | Everything | `OverlapBox` 查哪些层 |

**显示**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Beam Color | 红 | 运行时会改 `SpriteRenderer.color` |
| Beam Renderer | 空则找子物体 | 没有就现场建 `LaserVisual`：1×1 白 Sprite，按矩形长宽拉伸 |

认玩家的顺序：

1. 碰撞体自身 Tag 是 `Player`
2. 否则向上找 `PlayerAttacked` 或 `PlayerController`

因此本项目里常见的「Tag 在根节点、碰撞在 Animator 子物体」也能打到，不必给每个子碰撞再打 Tag。

结算规则：

1. 每到 `Tick Interval`，用 `Physics2D.OverlapBoxNonAlloc` 查当前矩形（中心、尺寸、Z 角与 Box 一致）
2. 跳过空碰撞、非玩家、Boss 自己
3. `PlayerHealth.isInvincible` 为真则本跳过，梁仍在
4. 先找 `PlayerAttacked`（自身 / 父 / 子），调用 `GetAttacked(damage, knockback, 最近点)`
5. 没有受击组件则 `PlayerHealth.TakeDamage(damage)`（仍受无敌保护，因为第 3 步已经 return）
6. 同一跳里多个玩家碰撞体重叠时，每个通过检测的碰撞都会尝试结算；玩家无敌会挡住后续重复扣血

激光是 Trigger，**不挡路、不推人**。击退只通过 `GetAttacked` 的位移实现。

有 `WitchHunter.Environment` 时，判定体会挂到 `Projectiles` 下，避免跟 Boss 一起被挪走。

寿命到点后判定体自己 `Destroy`。发射器协程结束时若梁还在，也会再 `Deactivate()` 一次。`Deactivate()` 会关碰撞并销毁物体。

### 5.5 跟踪与打断

| 情况 | 行为 |
| --- | --- |
| `Follow Aim While Active` 关 | 开火瞬间定死两端，玩家走开矩形就不再受伤 |
| `Follow Aim While Active` 开 | 每帧 `Retarget`，梁跟着 Aim Mode 转 |
| 开火中再 `Fire()` | 毁掉旧梁，立刻开新梁 |
| `StopLaser()` | 停协程、毁梁、`IsFiring = false` |
| 组件 `OnDisable` | 等同 `StopLaser()` |
| 梁自己寿命先到 | `IsActive` 变假，发射器循环退出 |

`IsFiring` 从生成梁开始为真，到梁被关掉为假。没有「等玩家走出再结束」的逻辑。

### 5.6 怎么手动试射

- 动画事件绑 `Boss01LaserAttack.Fire`
- Timeline Signal Receiver 绑 `Fire`（无参）
- 调试代码：

```csharp
var laser = GetComponent<Boss01LaserAttack>();
laser.Fire();
laser.FireTowardPlayer();
laser.Fire(new Vector2(-4f, 0f), new Vector2(4f, 0f), 2f);
laser.StopLaser();
```

---

## 6. 破坏场地

两层：

- `Boss01ArenaPiece`：一块地，负责自己的造型和通行
- `Boss01ArenaBreaker`：技能，负责选哪些块、按什么顺序拆

`Break()` 之后这块地变为**不可达**：玩家刚体走不进去，NavMesh Agent 也不会再寻路穿过（在 Carve 生效的前提下）。

`Restore()` 把造型、碰撞、障碍、Tilemap 全部还原，方便调试或二阶段把路重新打开。

### 6.1 一块地被拆时发生什么

`Boss01ArenaPiece.Break()` 若已经是破坏态则直接返回，否则：

1. **造型**
   - `Intact Visual` 存在且**不是**本物体自身 → `SetActive(false)`
   - `Destroyed Visual` 存在 → `SetActive(true)`
   - 有 `Shared Renderer`：破坏时换成 `Destroyed Sprite`，还原时换成 `Intact Sprite`（对应 Sprite 为空则不改）
2. **通行**
   - `Walkable Collider` 关掉（完好时可走的桥面、地板触发器等）
   - `Block Collider` 打开（实体墙，挡住 `Rigidbody2D`）
   - 没有阻挡碰撞且勾了 `Create Blocker If Missing`：按造型包围盒现场加一个非 Trigger 的 `BoxCollider2D`，最小边长 0.2
3. **寻路**
   - 勾了 `Add Nav Mesh Obstacle`：没有则 `AddComponent<NavMeshObstacle>`，Box + Carve，尺寸跟造型走，只在破坏态启用
4. **Tilemap（可选）**
   - 填了 `Tilemap` 且 `Tile Region` 的 X/Y 尺寸大于 0：把该矩形格子清掉，或写成 `Destroyed Tile`
   - `Awake` 时先缓存原格子，`Restore()` 写回
5. **挤开玩家**
   - 非初始化、且刚切到破坏：若玩家碰撞与新墙包围盒相交，把玩家挪到 `ClosestPoint` 再沿离开中心的方向多推 0.15
   - 避免人站在桥上时桥变成实心墙、卡进碰撞

`Awake` 会按 `Start Destroyed` 套用一次状态（初始化时**不**挤玩家）。进场就要缺一块地时勾它。

`Intact Visual` 若被自动设成自身 GameObject，脚本**不会**把整块 `SetActive(false)`，以免把自己和碰撞一起关掉。这时请用换 Sprite，或把完好造型放到子物体上再拖进 `Intact Visual`。

### 6.2 场地块字段（Boss01ArenaPiece）

**标识**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Piece Id | 空 | 给 `BreakById` / `ByIds` 用。空字符串无法被按 id 选中。同一 id 可以有多块，会一起拆 |

**造型**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Intact Visual | 可空 | 完好物体。不要填本物体自身，除非你只想靠换 Sprite |
| Destroyed Visual | 可空 | 破坏物体，进场应先禁用 |
| Shared Renderer | 自身 SpriteRenderer | 同一渲染器换图时用 |
| Intact Sprite / Destroyed Sprite | 空 | 配合 Shared Renderer；空则不改图 |

两种造型可以同时用：子物体负责碎石模型，Shared Renderer 负责地板换裂图。

**通行**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Walkable Collider | 空 | 完好时可走 / 可交互的碰撞，破坏后关闭 |
| Block Collider | 空 | 破坏后启用的实体碰撞。必须是非 Trigger，否则玩家会穿过去 |
| Create Blocker If Missing | 开 | 没拖 Block Collider 时按造型范围生成 |
| Add Nav Mesh Obstacle | 开 | 给 2D NavMesh Agent 挖洞。玩家不走 NavMesh，关了也不影响挡玩家 |

阻挡盒的尺寸来源（按顺序）：Shared Renderer → Intact 子渲染器 → 任意子渲染器 → Tilemap 区域 → 边长 1 的默认盒。

**Tilemap（可选）**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Tilemap | 空 | 要改的地图。空则整段跳过 |
| Tile Region | 尺寸 0 | 格子范围，用 `BoundsInt`（Position + Size）。Size.x 或 Size.y ≤ 0 视为未配置 |
| Destroyed Tile | 空 | 破坏后写入的 Tile；空 = 把区域内格子清成 null |

`Tile Region` 的坐标是 **Tilemap 格子坐标**，不是世界坐标。在 Tilemap 上对着要塌的那一块看 Cell 再填。

**状态**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Start Destroyed | 关 | 进场就是破坏态（缺桥、封死的侧路） |

只读：`PieceId`、`IsDestroyed`。

查询：

| 方法 | 作用 |
| --- | --- |
| `ContainsWorldPoint(point)` | 世界点是否在这块包围盒里 |
| `OverlapsWorldRect(center, size, angle)` | 是否与世界轴对齐盒相交（`angle` 目前未参与运算，按 AABB） |
| `GetWorldBounds()` | 优先已启用的阻挡盒，否则可走盒 / 渲染器 / Intact / 自身位置 |

选中物体时 Scene 里会画包围盒：完好偏蓝，破坏后偏红。

### 6.3 破坏技能字段（Boss01ArenaBreaker）

**目标**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Pieces | 空列表 | 要管的块。可手拖，也可自动收集后追加（已在列表里的不会重复加） |
| Auto Find Pieces | 开 | `Awake` 时收集；列表被清空后下次取列表还会再收集一次 |
| Search Root | 空 | 只在这棵子树里找。空 = 全场景 `FindObjectsOfType<Boss01ArenaPiece>` |
| Break Mode | All Configured | 无参 `Break()` 怎么选目标 |
| Target Ids | 空 | 仅 `ByIds` 使用，与各块的 `Piece Id` 精确匹配（区分大小写） |

**时序**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Stagger | 0 | 一次要拆多块时，块与块之间的间隔秒数。`0` 或只有一块 = 同一帧全拆 |

`Stagger > 0` 且目标多于一块时，`IsBreaking` 为真，直到最后一块拆完。中途 `RestoreAll()` / `StopBreaking()` / 物体禁用会停掉协程。`StopBreaking()` **不会**把已经拆掉的块还原。

正在按间隔拆时再调 `Break()`，会停掉旧协程、按新目标重来。

### 6.4 怎么选要拆的块

无参 `Break()` 只看 `Break Mode`：

| Break Mode | 行为 |
| --- | --- |
| `AllConfigured` | 列表里所有 `IsDestroyed == false` 的块，一次拆完 |
| `NextInOrder` | 从上次位置往后扫列表，拆**下一块**未破坏的，然后记下下一格。已破坏的会跳过。全部拆完后再调不会做事 |
| `RandomUnused` | 在未破坏的块里均匀随机一块 |
| `ByIds` | 未破坏且 `Piece Id` 落在 `Target Ids` 里的全部块 |

下面这些方法**不看** `Break Mode`：

| 方法 | 选谁 |
| --- | --- |
| `BreakAll()` | 当前已知列表里全部未破坏块 |
| `BreakById("bridge")` | `Piece Id == "bridge"` 且未破坏的全部块 |
| `BreakAt(2)` | 列表下标 2，不论它是否已破坏（已破坏则 `Piece.Break()` 自己 return） |
| `BreakInWorldRect(center, size)` | 包围盒与该世界矩形相交的未破坏块 |
| `RestoreAll()` | 列表里每一块都 `Restore()`，并把 `NextInOrder` 的游标归零 |

`BreakAt` 的下标是 **Pieces 列表**的下标，不是 Piece Id。自动收集的顺序取决于场景里 `FindObjectsOfType` / 子树遍历，不稳定时请手拖列表。

`Auto Find Pieces` 在已有条目时**只追加不在列表里的**，不会清掉你手拖的引用。不想扫到别的房间的块：指定 `Search Root`，或关掉自动查找并手拖。

### 6.5 通行与寻路分别挡谁

| 对象 | 靠什么不可达 |
| --- | --- |
| 玩家（`Rigidbody2D`） | 非 Trigger 的 `Block Collider`。Trigger 挡不住 |
| 狼 / Boss00 等 `NavMeshAgent` | `NavMeshObstacle.carving = true`。场景必须已经 Bake 过 NavMesh，Carve 才挖得动 |
| 纯位移、不走物理也不走 NavMesh 的物体 | 本脚本挡不住，需要对方自己查碰撞 |

本项目玩家不走 NavMesh，所以**挡玩家只看 Collider2D**。`Add Nav Mesh Obstacle` 主要给场上其它 Agent 用。

2D NavMesh（`updateUpAxis = false`）上，障碍尺寸按 XY 包围盒、`size.z = 1` 来写。若 Carve 挖偏了，用手做的 `Block Collider` 对玩家仍然有效；Agent 则需要检查 NavMesh 是否按 XY 烘焙。

### 6.6 怎么手动试拆

- 动画事件绑 `Boss01ArenaBreaker.Break` / `BreakAll`
- Timeline Signal Receiver 绑无参 `Break`
- 单块调试：选中该物体，在别的脚本里 `GetComponent<Boss01ArenaPiece>().Break()`
- 代码：

```csharp
var breaker = GetComponent<Boss01ArenaBreaker>();
breaker.Break();
breaker.BreakById("bridge");
breaker.BreakAt(0);
breaker.BreakInWorldRect(Vector2.zero, new Vector2(4f, 2f));
breaker.RestoreAll();
```

---

## 7. 两套功能怎么一起用

脚本之间**没有**互相调用。要组合，在同一时机调两次。

**固定梁打断一座桥**

1. Aim Mode = `StartToEnd`，起点、终点对准桥的两端
2. 桥挂 `Boss01ArenaPiece`，`Piece Id = bridge`
3. 动画同一帧（或激光中段的动画事件）：

```csharp
laser.Fire();
breaker.BreakById("bridge");
```

**跟踪激光，扫到哪塌到哪**

1. Aim Mode = `TowardPlayer`，勾 `Follow Aim While Active`
2. 场地块按区域拆好，各写 Id 或保证包围盒准
3. 开火后每帧或按节拍：

```csharp
laser.Fire();
// 稍后，用当前梁的中点与尺寸：
var beam = /* 你自己记下的判定体，或按名字找 Boss01_LaserBeam */;
breaker.BreakInWorldRect(beam.transform.position, beam.GetComponent<BoxCollider2D>().size);
```

更省事的做法：不要跟梁做几何相交，直接按阶段 `BreakById("left")` / `BreakById("right")`。

**先拆路再激光逼走位**

1. 开战或二阶段：`breaker.Break()`（`AllConfigured` 或 `ByIds`）
2. 路变窄之后再 `laser.FireTowardPlayer()`

---

## 8. 对外接口

其它脚本、Timeline、动画事件可调用：

```csharp
// 激光
laser.Fire();
laser.FireTowardPlayer();
laser.Fire(start, end);
laser.Fire(start, end, 2f);
laser.StopLaser();

// 判定体（一般不必直接调）
beam.Activate(start, end, width, damage, knockback, interval, duration, owner, color);
beam.Retarget(start, end);
beam.Deactivate();

// 单块场地
piece.Break();
piece.Restore();
bool inside = piece.ContainsWorldPoint(player.position);
bool hit = piece.OverlapsWorldRect(center, size, 0f);
Bounds b = piece.GetWorldBounds();

// 破坏技能
breaker.Break();
breaker.BreakAll();
breaker.BreakById("bridge");
breaker.BreakAt(0);
breaker.BreakInWorldRect(center, size);
breaker.RestoreAll();
breaker.StopBreaking();
```

只读状态：

| 属性 | 含义 |
| --- | --- |
| `Boss01LaserAttack.IsFiring` | 当前有一条由该发射器管着的梁 |
| `Boss01LaserBeam.IsActive` | 已 Activate 且未到寿命 |
| `Boss01LaserBeam.StartPoint` / `EndPoint` / `Width` | 当前矩形两端和宽度 |
| `Boss01ArenaPiece.IsDestroyed` | 是否已是破坏态 |
| `Boss01ArenaPiece.PieceId` | Inspector 里填的 id |
| `Boss01ArenaBreaker.IsBreaking` | 正在按 `Stagger` 一块一块拆 |

---

## 9. 与现有系统的衔接

| 系统 | 用法 |
| --- | --- |
| `PlayerAttacked` / `PlayerHealth` | 激光伤害、击退、无敌、受击 Trigger。与 Boss00 弹球同一条路 |
| `PlayerController` | 瞄准玩家、破坏时挤开位置 |
| `WitchHunter.Environment.Projectiles` | 激光判定体的父节点，避免跟 Boss 走 |
| `NavMeshAgent` / `NavMeshObstacle` | 与狼相同的 2D 寻路；破坏后 Carve，不改 Bake 数据本身 |
| `Tilemap` | 可选，只改 `Tile Region` 那一块，不碰其它格子 |
| Timeline | Signal Receiver 绑 `Fire` / `Break` 即可，不改 `TimeLineManager` |
| Boss00 行为树 | 本目录没有节点。要进 Boss00 的树，在自定义动作里调这里的 `Fire()` / `Break()`，或动画事件触发 |

本目录脚本**不**依赖 `EnemyAIController` 或 `BossAIController`。可以和 Boss00 的 AI 挂在同一只 Boss 上：AI 负责走位，这里负责出招。不要把两套「抢同一 Agent 的移动」叠在一起；本目录自己不移动 Boss。

---

## 10. 常见问题

**调了 `Fire()` 什么都没有**

- 组件是否启用、物体是否在场景里处于 Active
- `Duration` 是否被改成 0（仍会按 0.01 秒闪一下，几乎看不见）
- 运行时球/梁没有预制体贴图时，会有一块白色拉伸矩形。若连白块都没有，看 Console 是否有脚本编译错误
- 选中发射器看 Scene 红盒：编辑模式下 `StartToEnd` 没拖终点时，预览会画成「起点往左 × Max Length」，和运行时不一定一致

**梁的位置不对、不朝玩家**

- `Aim Mode` 是否仍是 `StartToEnd`，却没拖 `End Point`
- `Start Offset` 过大，起点已经越过玩家
- `Facing` 模式依赖 Y 旋转。图默认朝右时，左右会反，与 Boss00 相同
- 无参 `Fire()` 才走 Aim Mode；`Fire(start, end)` 走你传入的坐标

**梁穿过墙还打人**

- 这是默认。勾 `Stop At Walls`，并把墙的层加进 `Wall Mask`
- 墙必须有 Collider2D，射线才能打到
- Mask 勾了 Everything 时，注意不要让地面、装饰触发器把梁截得极短；必要时单独做一层 Wall

**打到玩家不掉血**

- 是否正处于 `isInvincible`（受击后常见 2 秒）
- 玩家碰撞所在层是否被 `Hit Mask` 排除
- 碰撞是否能向上找到 `PlayerAttacked` 或 `PlayerController`
- `Tick Interval` 很大时，走进矩形后要等到下一跳才掉血；开火当下就会跳一次
- `Damage = 0` 会结算但不扣血

**站在激光里只掉一次血**

- 看 `Tick Interval`。默认 0.35 秒一次，不是每帧
- 走出再走进，要等下一跳
- 无敌期间的跳会被跳过，无敌结束后要再等下一跳

**激光打到 Boss 自己**

- owner 是 `transform.root`。激光脚本不要挂在和 Boss 不同根的物体上，否则 Ignore 对不上
- 判定是 Trigger Overlap，理论上仍可能扫到 Boss；`IsOwner` 会丢掉这些碰撞。若 Boss 根下还有带 `PlayerAttacked` 的物体，会被当成玩家——不要这么挂

**拆了地，造型变了但还能走**

- `Block Collider` 是否是 Trigger（必须关掉 Is Trigger）
- 是否关掉了 `Create Blocker If Missing` 又没拖阻挡碰撞
- 玩家是否被挤开后又从缺口绕回去（包围盒比美术小）
- 选中该 Piece，看 Scene 红盒是否盖住要封的路

**拆了地，玩家能走、敌人还能寻路穿过**

- 玩家能走：先修 Collider，见上
- 敌人还能穿：NavMesh 没 Bake、或不是 XY 平面、或 `Add Nav Mesh Obstacle` 被关掉
- Carve 有一帧延迟，拆的当下 Agent 可能还走旧路，随后会停或绕开

**`Break()` 没反应**

- 列表是否为空，且 `Auto Find Pieces` 关掉了
- `Auto Find` 开着但 `Search Root` 指错了父节点
- `Break Mode = ByIds` 但 `Target Ids` / `Piece Id` 不一致（大小写、空格）
- `NextInOrder` 已经拆完一轮，未破坏块为 0
- 块在 `Awake` 前就被别的脚本标成 `Start Destroyed`

**自动查找拆到了别的房间的地**

- 指定 `Search Root` 为当前场地根
- 或关掉 `Auto Find Pieces`，只手拖本场的块

**`Restore()` 之后 Tile 没回来**

- `Tile Region` 的 Size 是否在 `Awake` 时就是 0（那时不会缓存）
- 运行中途才赋值的 Tilemap / Region，不会有 `_originalTiles`
- 把区域在进场前配好

**破坏瞬间玩家卡进墙里**

- 挤开依赖玩家身上（或子物体）的 Collider2D，以及新墙的包围盒相交
- 玩家碰撞比 Sprite 小很多时，可能不相交、不会推
- 推完仍重叠：加大阻挡盒，或把 `Destroyed Visual` 做成周围有空隙的碎石，而不是整块实心

**激光和拆地谁先谁后**

- 同一帧里先 `Fire` 再 `Break`，或反过来，没有引擎级约定。拆地会改碰撞，可能影响下一帧的 `Stop At Walls` 射线。要梁先打满再塌，用动画事件错开，或 `Stagger`

---

## 11. 验收清单

**激光**

- [ ] `Fire()` 后，起点到终点出现一条矩形（白块或预制体贴图）
- [ ] 矩形长边对准两端，宽度与 `Width` 一致（Scene 红盒可对）
- [ ] `Aim Mode = StartToEnd`：两端钉在两个 Transform 上；未勾 Follow 时玩家走动，梁不动
- [ ] `TowardPlayer`：梁从 Muzzle 指向玩家一侧，长度约 `Max Length`
- [ ] 玩家走进矩形后按 `Tick Interval` 反复掉血，出现受击（有 `PlayerAttacked` 时）
- [ ] 走出矩形或 `Duration` 结束后不再掉血
- [ ] 无敌期间不掉血，梁仍显示
- [ ] `Tick Interval = 0.35` 时不会每帧扣血
- [ ] 勾 `Follow Aim While Active` 后，梁会跟着玩家转
- [ ] 勾 `Stop At Walls` 后，终点停在第一面墙上，墙后不受伤
- [ ] `StopLaser()` 或禁用 Boss 后，场上的梁消失
- [ ] 没有 `Beam Prefab` 时仍能打（运行时白矩形）

**场地**

- [ ] `Break()` / `Piece.Break()` 后完好造型消失、破坏造型出现（或 Sprite 换成裂图）
- [ ] 玩家不能再走进该区域（被实体碰撞挡住）
- [ ] 场上有 NavMesh Agent 时，不会再把路径打穿该区域（Carve 生效后）
- [ ] 人站在块上时被拆，会被推出墙外，不永久卡死
- [ ] `NextInOrder` 每次只拆下一块，顺序与列表一致
- [ ] `BreakById("bridge")` 只拆 id 匹配的块
- [ ] `Stagger = 0.2` 且一次多块时，能看出一块一块塌
- [ ] 配了 Tilemap 区域时，对应格子被清空或换成 `Destroyed Tile`
- [ ] `Restore()` / `RestoreAll()` 后造型、通行、Tile 恢复
- [ ] `Start Destroyed` 的块进场就是封死的，且不会把玩家莫名弹开

**组合（若你接了）**

- [ ] 同一技能里先梁后塌或先塌后梁，两套都生效，且不会互相销毁对方的物体
- [ ] 只挂激光、不挂破坏时，场地保持原样
- [ ] 只挂破坏、不放激光时，仍能封路
