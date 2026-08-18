# Boss00 功能脚本说明

本目录只提供**功能脚本**，不含预制体、动画、音效或特效资源。把脚本挂到 Boss 物体上，在 Inspector 里填引用和数值即可使用。

默认约定：

- 剧情信号名：`Boss00_DrawSword`
- 拔剑动画 Trigger：`DrawSword`
- 追逐动画 Bool：`IsRunning`
- 玩家 Tag：`Player`
- 弹球 Layer：若项目里存在 `Projectile` 层，发射时会自动切过去

---

## 目录

1. [文件一览](#1-文件一览)
2. [推荐物体结构](#2-推荐物体结构)
3. [整体流程](#3-整体流程)
4. [剧情信号](#4-剧情信号)
5. [拔剑演出](#5-拔剑演出)
6. [弹球](#6-弹球)
7. [行为树 AI](#7-行为树-ai)
8. [默认行为树解读](#8-默认行为树解读)
9. [Animator 约定](#9-animator-约定)
10. [对外接口](#10-对外接口)
11. [与现有系统的衔接](#11-与现有系统的衔接)
12. [常见问题](#12-常见问题)
13. [验收清单](#13-验收清单)

---

## 1. 文件一览

| 路径 | 类型 | 作用 |
| --- | --- | --- |
| `StorySignalRelay.cs` | 组件 | Timeline / 剧情节点用来广播字符串信号 |
| `BossDrawSword.cs` | 组件 | 收到信号后播拔剑：动画、音效、特效、镜头特写 |
| `BossBouncyBall.cs` | 组件 | 单颗弹球：碰墙反弹不掉速，碰玩家受伤并播受击特效 |
| `BossBouncyBallVolley.cs` | 组件 | 朝一个方向同时打出覆盖 120°~180° 的 3 向弹球 |
| `BossAIController.cs` | 组件 | 行为树运行器，负责开战时机、移动、转向 |
| `BehaviorTree/BossBTTypes.cs` | 数据 | 任务 / 条件 / 动作的可序列化结构 |
| `BehaviorTree/BossBehaviorTreeAsset.cs` | 资源 | 可复用的行为树 ScriptableObject |
| `BehaviorTree/BossBTExecutor.cs` | 运行时 | 优先选择 + 条件序列的执行器 |
| `Editor/BossBTDrawers.cs` | 编辑器 | Inspector 里按条件/动作类型只显示相关字段 |

本目录之外有两处衔接改动：

- `GameEvents` 增加 `StorySignal(string)` / `OnStorySignal`
- `PlayerAttacked` 增加 `GetAttacked(int damage, float force, Vector3 hitFrom)`，弹球不必再依赖 `PolygonCollider2D`

---

## 2. 推荐物体结构

```
Boss00                          ← 根节点，挂下面三个脚本
├── Animator                    ← Sprite + Animator（拔剑 / 奔跑）
├── AudioSource                 ← 可选，没有时拔剑脚本会临时加一个
├── CloseUpFocus                ← 可选，特写跟随点
├── Muzzle                      ← 可选，弹球出生点
└── Status                      ← 可选，NPCHealth / NPCStatusEffect（血量条件、死亡停 AI）
```

根节点建议同时挂：

1. `BossDrawSword`
2. `BossBouncyBallVolley`
3. `BossAIController`

移动二选一（有则优先用前者）：

- `NavMeshAgent`：2D 寻路，脚本会关掉 `updateRotation` / `updateUpAxis`
- `Rigidbody2D`：没有 Agent 时用速度位移
- 两者都没有：直接改 `transform.position`

Timeline 绑定物体（或任意剧情触发器）另挂：

- `StorySignalRelay`，`Signal Id` 填 `Boss00_DrawSword`

弹球预制体（建议单独做，没有也能运行时生成无贴图球）：

```
Boss00_BouncyBall
├── Sprite / 粒子
├── CircleCollider2D
├── Rigidbody2D          Gravity Scale = 0
└── BossBouncyBall       把受击特效拖到 Hit Vfx Prefab
```

引用留空时的自动查找：

| 组件 | 空引用时 |
| --- | --- |
| `BossDrawSword.animator` | `GetComponentInChildren<Animator>()` |
| `BossDrawSword.audioSource` | 自身 `AudioSource`，播音时没有会 `AddComponent` |
| `BossDrawSword.vfxPoint` / `closeUpTarget` | 自身 Transform |
| `BossBouncyBallVolley.muzzle` | 自身 Transform |
| `BossAIController` 的 DrawSword / Volley | 同物体 `GetComponent` |
| `BossAIController` 的 Health / Status | `GetComponentInChildren` |
| 玩家 | `PlayerController.Instance`，找不到再用 Tag `Player` |

---

## 3. 整体流程

```
剧情 Timeline 发出信号 Boss00_DrawSword
        │
        ▼
StorySignalRelay.Emit
        │
        ▼
GameEvents.StorySignal("Boss00_DrawSword")
        │
        ├─► BossDrawSword 播拔剑（动画 / 音 / 特效 / 特写）
        │         │
        │         └─ 播完 Completed
        │                   │
        └─► BossAIController 记下信号
                            │
                            ▼
              Start Mode = After Draw Sword（默认）
                            │
                            ▼
                      BeginCombat()
                            │
                            ▼
              行为树每帧选任务：弹球 / 靠近 / 待机
```

默认是「先看剧情拔剑，再开打」。若要跳过演出直接打，把 `BossAIController` 的 `Start Mode` 改成 `On Enable` 或 `On Story Signal`。

---

## 4. 剧情信号

### 4.1 为什么要信号

拔剑不是在 `Start` 里自动播，而是等剧情节点或 Timeline 通知。这样开场对白、镜头、拔剑可以对齐到同一帧。

信号是字符串，全场广播。默认 id 为 `Boss00_DrawSword`。`BossDrawSword` 只响应与 `Required Signal Id` 相同的信号；该字段留空则接受任意信号。

### 4.2 三种接法（任选一种）

**接法 A：Timeline Signal（推荐）**

1. 在 Timeline 里加 Signal Track
2. 在要拔剑的时间打 Signal Marker
3. 场景里找一个会被 Timeline 绑定的物体，挂 `StorySignalRelay`
4. 该物体加 `Signal Receiver`，把 Marker 映射到 `StorySignalRelay.Emit`（无参）
5. 确认 `Signal Id` 为 `Boss00_DrawSword`

**接法 B：直接调 Boss**

Signal Receiver 也可以直接绑 `BossDrawSword.OnStorySignal`（无参）。这样不走 `GameEvents`，只有这一只 Boss 会拔剑。

**接法 C：代码**

```csharp
GameEvents.Instance.StorySignal("Boss00_DrawSword");
```

对话结束、任务完成、Trigger 进房都可以这样发。

### 4.3 StorySignalRelay 字段

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Signal Id | `Boss00_DrawSword` | 无参 `Emit()` 发出的字符串 |

`Emit(string id)` 可从别的脚本传入不同 id，用来扩展第二段剧情（例如 `Boss00_Phase2`）。

---

## 5. 拔剑演出

组件：`BossDrawSword`。

收到匹配信号后，同一段协程里依次：

1. `Animator.SetTrigger(DrawSword)`
2. 若勾选开场播音：`AudioSource.PlayOneShot`
3. 若勾选开场播特效：播粒子 / 生成特效预制体
4. 拉高特写虚拟相机 Priority（或临时建一只）
5. 等待 `max(Play Duration, Close Up Duration)`
6. 恢复相机，置 `HasPlayed = true`，触发 `Completed`

`BossAIController` 默认订阅 `Completed`，拔剑结束后开战。

### 5.1 字段说明

**剧情信号**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Required Signal Id | `Boss00_DrawSword` | 只响应这个 id；留空 = 任意信号都播 |
| Listen Game Events | 开 | 是否订阅 `GameEvents.OnStorySignal` |
| Play Once | 开 | 播过一次后忽略后续信号，避免 Timeline 重发再拔一次 |
| Allow Replay | 关 | 即使 `Play Once`，也允许再次 `Play()` |

**动画**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Animator | 自动找子物体 | 没有 Animator 时只播音、特效、特写 |
| Draw Sword Trigger | `DrawSword` | 必须在 Animator Controller 里存在同名 Trigger |

**音效**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Audio Source | 自动找 | 没有且需要播音时会临时添加 |
| Draw Sword Sfx | 空 | 拔剑 `AudioClip`，空则静音 |
| Sfx Volume | 1 | `PlayOneShot` 音量 |
| Play Sfx On Start | 开 | 开场立刻播。若要卡在挥剑那一帧，关掉，改用动画事件 |

**特效**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Draw Sword Vfx Prefab | 空 | 在 `Vfx Point` 处 Instantiate，到时销毁 |
| Draw Sword Particles | 空 | 已挂在场景里的粒子，会 `Clear` 再 `Play` |
| Vfx Point | 自身 | 特效出生点，建议放在剑或手 |
| Vfx Lifetime | 3 | 预制体销毁延迟（秒） |
| Play Vfx On Start | 开 | 同音效，可改由动画事件触发 |

两种特效可以同时用：场景粒子负责刀光，预制体负责落地尘土。

**特写**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Close Up Camera | 空 | 预先做好的 Cinemachine 虚拟相机 |
| Close Up Target | 自身 | Follow / LookAt 目标 |
| Create Runtime Close Up | 开 | 没有指定相机时，临时建一只正交虚拟相机 |
| Close Up Ortho Size | 3 | 临时相机的正交尺寸，越小越近 |
| Close Up Priority | 100 | 须高于平时跟随相机（本项目跟随相机一般远低于 100） |
| Close Up Duration | 1.5 | 特写持续秒数，结束后恢复原 Priority / 销毁临时相机 |

主相机上需要有 `CinemachineBrain`（项目里 `MainCamera` 已挂）。物体禁用时会立刻恢复镜头，避免卡在特写。

**时序**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Play Duration | 1.5 | 整段演出最短等待。实际等待取它和特写时间的较大值 |

请把 `Play Duration` 调到不短于拔剑动画长度，否则动画没播完就开战。

### 5.2 动画事件（可选）

在拔剑动画指定帧添加事件：

| 方法 | 作用 |
| --- | --- |
| `AnimEvent_PlaySlashVfx` | 在当前 `Vfx Point` 再播一次特效 |
| `AnimEvent_PlaySlashSfx` | 再播一次音效 |

典型用法：关掉 `Play Vfx/Sfx On Start`，只在刀出鞘那一帧触发，避免和动画错位。

### 5.3 运行时状态

| 属性 | 含义 |
| --- | --- |
| `IsPlaying` | 正在播这段演出 |
| `HasPlayed` | 至少完整播完过一次 |
| `Completed` | 播完时的 C# 事件，AI 用它开战 |

正在播放时再次 `Play()` 会被忽略。`Play Once` 且已播完时也会忽略，除非 `Allow Replay`。

---

## 6. 弹球

两层：

- `BossBouncyBallVolley`：发射器，负责方向、扇形、持续秒数
- `BossBouncyBall`：单颗球，负责飞行、反弹、伤害

### 6.1 扇形怎么算

以「一个基准方向」为中线，按 `Spread Angle`（钳在 120°~180°）同时打出 3 颗：

| 球 | 相对中线 |
| --- | --- |
| 左 | −一半夹角 |
| 中 | 0° |
| 右 | +一半夹角 |

例子：

- 150°（默认）：−75°、0°、+75°
- 120°：−60°、0°、+60°
- 180°：−90°、0°、+90°（左右与中线垂直）

出生点 = `Muzzle` 位置 + 该方向 × `Spawn Offset`，避免一出来就撞上 Boss 自己的碰撞体。脚本还会对 Boss 身上所有 Collider 以及其它弹球做 `Physics2D.IgnoreCollision`。

### 6.2 发射器字段（BossBouncyBallVolley）

**弹球**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Ball Prefab | 空 | 弹球预制体。空则运行时生成只有碰撞、没有贴图的球（能打、看不见） |
| Muzzle | 自身 | 出生点 |
| Ball Speed | 8 | 初速，反弹后仍保持这个速率 |
| Ball Lifetime | 6 | 每颗存活秒数，到时 `Destroy` |
| Spawn Offset | 0.6 | 沿发射方向往外挪，减少卡在 Boss 体内 |

**扇形**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Spread Angle | 150 | 三向总张角，Inspector 限制 120~180，运行时再钳一次 |
| Direction Mode | Toward Player | 中线怎么取，见下表 |
| Custom Direction | (−1, 0) | 仅 `Custom` 模式使用 |

| Direction Mode | 中线 |
| --- | --- |
| Toward Player | 指向玩家；没有玩家则用朝向 |
| Facing | Boss 当前朝向（与狼 AI 相同：Y 旋转 180 视为朝右） |
| Custom | 用 `Custom Direction` |

**持续发射**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Volley Duration | 0 | 技能持续秒数。`0` = 只打一波，球仍按 `Ball Lifetime` 活着 |
| Spawn Interval | 0.8 | 持续发射时，两波之间的间隔 |
| Retarget Each Wave | 开 | 每一波重新算中线（玩家在走时扇形会跟着转） |
| Hold Until Balls Expire | 开 | `Volley Duration = 0` 时，`IsFiring` 仍保持到弹球寿命结束，方便行为树等这轮打完再选下一任务 |

行为树动作 `FireBouncyVolley` 的 `Duration > 0` 时，会覆盖这里的 `Volley Duration`，用来做「低血持续扫射 3 秒」这类招。

正在发射时再次 `Fire()` 会被忽略。需要强制打断时调 `StopVolley()`（已飞出的球不会回收）。

### 6.3 单颗弹球字段（BossBouncyBall）

预制体上调这些；发射器传入的速度和寿命会覆盖运动相关默认值。

**运动**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Speed | 8 | `Launch` 没传速度时的后备值 |
| Lifetime | 6 | `Launch` 没传寿命时的后备值 |
| Rotate To Velocity | 开 | 每帧把 Z 旋转对齐速度方向 |

刚体在 `Awake` 里会被改成：重力 0、连续碰撞、冻结旋转、摩擦 0 / 弹性 1 的运行时材质。真正保速不靠材质，而靠代码。

**伤害**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Damage | 1 | 传给 `PlayerAttacked.GetAttacked` |
| Knockback | 0.15 | 击退强度，算法与狼撕咬一致 |
| Hit Cooldown | 0.6 | 同一颗球两次结算的最小间隔，避免贴着玩家每帧扣血 |
| Hit Vfx Prefab | 空 | 在接触点生成的受击特效 |
| Hit Vfx Lifetime | 1.5 | 该特效销毁延迟 |

**碰撞**

| 字段 | 默认 | 说明 |
| --- | --- | --- |
| Bounce Mask | Everything | 哪些层会触发反弹 |
| Player Tag | `Player` | 用来判定受伤目标 |

`Ignore Raycast`、`Item` 层即使在 Mask 里也不会反弹。

### 6.4 反弹与伤害规则

反弹：

1. 每帧记下当前速度 `_lastVelocity`
2. `OnCollisionEnter2D` 取接触法线
3. `velocity = Reflect(入射, 法线).normalized * 原速率`
4. 之后每帧若速率漂了，再拉回原速率

因此撞墙、撞地、撞实体玩家都**不掉速度**。物理材质的弹性只是辅助，不以它为准。

对玩家：

- 玩家碰撞体是 Trigger（本项目 Animator 上常见）：走 `OnTriggerEnter2D`，受伤、**不挡路**
- 玩家碰撞体是实体：走 `OnCollisionEnter2D`，受伤并反弹
- 先查 `PlayerAttacked`（自身 / 父 / 子），再退回 `PlayerHealth.TakeDamage`
- `PlayerHealth.isInvincible` 为真时不结算、不播受击特效
- 有 `PlayerAttacked` 时会播玩家的 `GetAttacked` Trigger（受击动作 / 无敌动画事件仍由玩家自己的动画负责）

弹球会挂到 `WitchHunter.Environment.Instance.Projectiles` 下，避免跟 Boss 一起被挪走。

### 6.5 怎么手动试射

- 动画事件绑 `BossBouncyBallVolley.Fire`
- 行为树动作 `FireBouncyVolley`
- 调试代码：`GetComponent<BossBouncyBallVolley>().FireTowardPlayer()`

---

## 7. 行为树 AI

组件：`BossAIController`。

这里**没有节点图画布**。配置方式是一张任务表，对应行为树里最常见的结构：

```
Repeater（每帧）
 └── Selector（从上到下，选第一条能跑的）
      ├── Sequence 任务A：条件1 ∧ 条件2 → 动作1 → 动作2
      ├── Sequence 任务B：……
      └── Sequence 任务C：Always → Idle
```

改列表顺序、勾选条件、填秒数即可调招式，不用写代码。

### 7.1 树从哪里来

优先级：

1. `Tree Asset` 不为空且里面有任务 → 用资源
2. 否则用组件上的 `Inline Tasks`
3. 两者都空 → 运行时写入默认三任务（弹球 / 靠近 / 待机）

在 Inspector 里第一次挂上组件时，`Reset()` 也会填入同一套默认任务，方便直接改。

做资源：`Create → WitchHunter → Boss00 → Behavior Tree`。多只 Boss 共用同一套招、或要做难度变体时用资源；只打一次的 Boss 用 Inline 即可。

### 7.2 开战时机（Start Mode）

| 模式 | 行为 |
| --- | --- |
| After Draw Sword（默认） | 订阅拔剑 `Completed`；若进场时已经拔过，`Start` 里也会开战 |
| On Story Signal | 收到 `Start Signal Id`（默认同样是 `Boss00_DrawSword`）立刻开战，不必等拔剑播完 |
| On Enable | 物体一启用就开战 |
| Manual | 自己调 `BeginCombat()` / `StopCombat()` |

无论哪种模式，收到的剧情信号都会记进黑板，供条件 `HasStorySignal` 使用。

### 7.3 控制器字段

**行为树**

| 字段 | 说明 |
| --- | --- |
| Tree Asset | 外部行为树资源，优先于 Inline |
| Inline Tasks | 直接配在组件上的任务列表 |
| Start Mode | 见上表 |
| Start Signal Id | 仅 `On Story Signal` 使用 |

**引用**

| 字段 | 说明 |
| --- | --- |
| Animator | 追逐时拨 `Running Bool` |
| Draw Sword / Volley / Health / Status Effect | 空则自动找 |
| Running Bool | 默认 `IsRunning`，追逐开、停下关 |

**调试**

| 字段 | 说明 |
| --- | --- |
| Current Task Name | 运行时只读，显示当前任务名，用来确认选中了哪一条 |

### 7.4 一条任务怎么跑

每条 `BossBTTask`：

| 字段 | 说明 |
| --- | --- |
| Name | 显示名，也用作冷却的 key。不要两条任务同名，否则会抢同一条冷却 |
| Enabled | 关掉等于从树里拿掉，不用删 |
| Abort If Condition Fails | 执行中途条件不再成立则立刻停步、重新选任务。追逐建议开，出招建议关（避免扇形打到一半被打断） |
| Cooldown | **整段序列成功结束后**的冷却（秒）。失败或中断不加冷却 |
| Conditions | 全部成立才选中（AND）。一条都没有 = 永远可入选 |
| Actions | 按列表顺序执行。某个动作返回 Running 时，下一帧继续它，不会跳到下一条动作 |

选择规则（每帧，若当前没有 Running 的任务）：

1. 从上到下扫
2. 跳过未启用
3. 跳过冷却未好
4. 跳过条件未全过
5. 第一条剩下的成为当前任务

死亡（`NPCStatusEffect.Dead`）会停树并 Reset。眩晕（`Stunned`）时暂停 Tick，恢复后从当前动作继续。

### 7.5 条件

每条可勾「取反」。

| 类型 | 成立条件 | 要用的字段 |
| --- | --- | --- |
| Always | 总是成立 | 无 |
| PlayerInRange | 有玩家，且距离 ∈ [最小, 最大] | 最小距离、最大距离 |
| PlayerOutsideRange | 没有玩家，或距离 < 最小，或距离 > 最大 | 最小距离、最大距离 |
| HealthPercentBelow | 当前生命 / 上限 **小于** 比例 | 生命比例 0~1 |
| HealthPercentAbove | 当前生命 / 上限 **大于** 比例 | 生命比例 0~1 |
| HasStorySignal | 本场已收到过该 id | 信号 Id |
| DrawSwordCompleted | 拔剑组件存在，且已播完、当前没在播 | 无 |
| VolleyIdle | 没有发射器，或发射器 `IsFiring == false` | 无 |

没有 `NPCHealth` 时，生命比例视为 `1`（满血）。因此「低血加招」必须挂血量组件才会触发。

距离用 Boss 根节点与 `PlayerController` 根节点的平面距离。

`PlayerOutsideRange` 的读法：把「内侧区间」填成 min~max，条件表示「不在这个区间里」。默认树里 `min=0, max=3` 表示「距离大于 3 才靠近」。

### 7.6 动作

Inspector 会按类型隐藏无关字段。

| 类型 | 行为 | 字段 |
| --- | --- | --- |
| Wait | 原地空等，不改移动 | 持续秒 |
| Idle | 停步、关奔跑动画，再等 | 持续秒 |
| FacePlayer | 立刻转向玩家，当帧完成 | 无 |
| ChasePlayer | 面向玩家并追。有 NavMeshAgent 用寻路，否则用刚体/位移 | 最长秒（0 = 直到靠近）、移速、停止距离 |
| StopMove | 立刻停步，当帧完成 | 无 |
| PlayAnimation | 当帧 `SetTrigger`，停步并等待 | Trigger 名、持续秒（至少按 0.05 算） |
| DrawSword | 调 `BossDrawSword.Play`，等到播完 | 持续秒 > 0 时可提前结束等待 |
| FireBouncyVolley | 停步、转向、开火，等到 `IsFiring == false` | 持续秒 > 0 时覆盖发射器的 Volley Duration |

追逐结束条件（满足任一即可）：

- 距离 ≤ 停止距离
- `Duration > 0` 且时间到

转向规则与狼 AI 相同：看向右侧时根节点 `rotation.y = 180`，看向左侧为 `0`。弹球的 `Facing` 模式也按这个判断左右。

### 7.7 怎么加新招（不改代码）

在任务列表**上方**插入更高优先级的任务。例如低血持续扫射：

1. 名称：`低血连射`
2. 冷却：`8`
3. 中断：关掉（出招不要被靠近打断）
4. 条件：
   - `HealthPercentBelow`，生命比例 `0.4`
   - `VolleyIdle`
5. 动作：
   - `FacePlayer`
   - `FireBouncyVolley`，持续秒 `3`

这样血量掉到 40% 以下时，会优先于普通「弹球扇形」打出 3 秒连射。

再例如先播一记砍击再接弹球：

1. 动作 1：`PlayAnimation`，Trigger 填你的攻击名，持续秒填动画长度
2. 动作 2：`FireBouncyVolley`

---

## 8. 默认行为树解读

刚挂 `BossAIController` 时的三条任务：

**1. 弹球扇形**（最高优先）

- 冷却 4 秒，出招过程不中断
- 条件：拔剑已结束 ∧ 当前没在发射 ∧ 距离 ≤ 14
- 动作：转向 → 打一波（用发射器自己的 Duration，默认只打一波）
- 效果：开打后只要玩家还在 14 以内，大约每 4 秒扇形弹一次

**2. 靠近玩家**

- 会中断：玩家走进 3 以内就停追
- 条件：距离 > 3（`PlayerOutsideRange`，内侧 0~3）
- 动作：追逐，最多 1.5 秒，或进入 2.5 停止
- 效果：离远了就贴上来，好让弹球打得到

**3. 待机**

- 条件：Always（兜底，保证树不会空转）
- 动作：Idle 0.4 秒
- 效果：贴身又在弹球冷却中时，原地顿一下再重新选

运行时看 `Current Task Name`，应在这三个名字之间切换。

---

## 9. Animator 约定

| 参数 | 类型 | 谁在用 | 不配会怎样 |
| --- | --- | --- | --- |
| `DrawSword` | Trigger | `BossDrawSword` | 只出音/特效/特写，没有拔剑动作 |
| `IsRunning` | Bool | `BossAIController` 追逐 / 停下 | 滑步移动，动画仍站着 |
| 任意 Trigger | Trigger | 动作 `PlayAnimation` | 该动作空等持续秒 |

建议再给玩家保留已有的 `GetAttacked` Trigger：弹球命中时会打这个，用来播受击和无敌动画事件。

拔剑动画长度请与 `BossDrawSword.Play Duration` 对齐，或把 Duration 略设长一点。

---

## 10. 对外接口

其它脚本或 Timeline / 动画事件可调用：

```csharp
// 信号
StorySignalRelay.Emit();
StorySignalRelay.Emit("Boss00_Phase2");
GameEvents.Instance.StorySignal("Boss00_DrawSword");

// 拔剑
bossDrawSword.OnStorySignal();
bossDrawSword.OnStorySignal("Boss00_DrawSword");
bossDrawSword.Play();
bossDrawSword.AnimEvent_PlaySlashVfx();
bossDrawSword.AnimEvent_PlaySlashSfx();

// 弹球
volley.Fire();
volley.FireTowardPlayer();
volley.Fire(Vector2.left, 3f);   // 朝左持续 3 秒
volley.StopVolley();

// AI
ai.BeginCombat();
ai.StopCombat();
ai.NotifyStorySignal("Boss00_Phase2");
```

只读状态：`BossDrawSword.IsPlaying` / `HasPlayed`，`BossBouncyBallVolley.IsFiring`，`BossAIController.IsRunning`。

---

## 11. 与现有系统的衔接

| 系统 | 用法 |
| --- | --- |
| `GameEvents` | 新增全场剧情信号，拔剑和 AI 都听它 |
| `PlayerAttacked` / `PlayerHealth` | 弹球伤害、击退、无敌、受击 Trigger |
| `WitchHunter.Environment.Projectiles` | 弹球的父节点，避免跟 Boss 走 |
| Cinemachine | 特写靠提高 Virtual Camera Priority，与 `CMFollow` 同一套脑 |
| `NPCHealth` / `NPCStatusEffect` | 血量条件、死亡停树、眩晕暂停 |
| `NavMeshAgent` | 与狼相同的 2D 寻路设置 |
| Timeline / `TimeLineManager` | 用 Signal Receiver 调 `Emit` 或 `OnStorySignal`，不改 Timeline 管理器本身 |

本目录脚本**不**依赖狼的 `EnemyAIController`，两套 AI 不要挂在同一只怪上抢 Agent。

---

## 12. 常见问题

**发了信号但没有拔剑**

- 信号字符串是否完全一致（区分大小写）
- `Listen Game Events` 是否打开；若只用接法 B，Receiver 有没有绑到这只 Boss
- `Play Once` 是否已经播过
- 物体是否处于禁用

**有拔剑没有特写**

- 主相机是否有 `CinemachineBrain`
- 指定的虚拟相机 Priority 是否其实已经高于 100
- `Create Runtime Close Up` 是否被关掉，同时又没拖相机

**弹球看不见**

- 没做预制体时运行时球没有 Sprite，这是预期。做一只有贴图的预制体拖到 `Ball Prefab`

**弹球一出来就消失 / 卡在 Boss 里**

- 加大 `Spawn Offset`
- 确认 Boss 碰撞体不是异常大
- 脚本已 Ignore Boss 和其它弹球；若仍卡住，检查场景里是否有别的触发器在销毁投射物

**弹球撞墙后变慢或停下**

- 不应发生。若发生，看是不是别的脚本在改这颗球的 `Rigidbody2D.velocity`
- `Bounce Mask` 是否把墙那一层勾上了；没勾则不会走反弹逻辑，可能被物理卡住

**打到玩家不掉血**

- 玩家物体 Tag 是否为 `Player`
- 是否正处于 `isInvincible`
- 碰撞是否打在带 `PlayerAttacked` 或能向上找到它的物体上
- `Hit Vfx Prefab` 为空只影响特效，不影响伤害

**AI 一直待机、不打弹球**

- `Start Mode` 是否还在等拔剑，而拔剑其实没播完（`HasPlayed` 仍为假）
- 默认弹球任务要求 `DrawSwordCompleted`。没挂 `BossDrawSword` 时这条永远不成立，只会靠近 / 待机。可删掉该条件，或改 `Start Mode`
- 玩家是否远于 14（默认弹球最大距离）
- 看 `Current Task Name` 确认选中了哪一条

**两条任务冷却互相影响**

- 冷却按任务 `Name` 存。改成不同名字

**Boss 朝向反了**

- 与狼一致：美术默认应朝左，朝右靠 Y=180。若你的图默认朝右，需要改图或改 `FacePlayer` / `GetFacing`

---

## 13. 验收清单

**拔剑**

- [ ] Timeline 打出 `Boss00_DrawSword` 后，动画、音效、特效、特写同时出现
- [ ] 特写结束后镜头回到跟随玩家
- [ ] 同一信号再发一次，不会再拔剑（`Play Once`）
- [ ] 动画事件触发的音/特效落在挥剑那一帧（若你改成事件驱动）

**弹球**

- [ ] `Fire()` 打出 3 颗，张角肉眼约 120°~180°
- [ ] 改 `Spread Angle` 为 120 和 180，扇形明显变窄 / 变宽
- [ ] 撞墙、撞地后速率不变，方向按法线折返
- [ ] 碰到玩家掉血，出现受击动作和受击特效
- [ ] 玩家无敌时不掉血、不播受击特效
- [ ] 同一颗球不会连续每帧扣血
- [ ] 到达 `Ball Lifetime` 后消失
- [ ] `Volley Duration = 3`、`Spawn Interval = 0.8` 时会连打数波

**行为树**

- [ ] 拔剑结束后 `Current Task Name` 开始变化
- [ ] 玩家在 14 内会周期性打出扇形，间隔约 4 秒
- [ ] 玩家走远会切到「靠近玩家」
- [ ] 贴身且弹球冷却中会切到「待机」
- [ ] 只改 Inspector（冷却、距离、动作顺序）即可换招，不必改脚本
- [ ] 死亡后不再移动、不再发射
