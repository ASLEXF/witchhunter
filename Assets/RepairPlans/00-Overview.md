# 修复总览

按系统拆分，互不绑死。建议顺序：

1. BuildCompatibility（不修则无法出包）
2. HealthAndDeath（死亡/治疗主循环）
3. Projectiles（弓箭管线）
4. EnemyAI（追击/死亡状态）
5. PlayerControlAndInput（隐藏玩家、蓄力、输入）
6. InventoryAndItems（栏、掉落、生成）
7. Interaction（交互列表/尸体）
8. StoryAndDialog（Enermy tag、剧情树）
9. ConfigLoading（真机读表）
10. Timeline
11. Quests

## 原则

- 每个系统只改本系统文件，跨系统只约定接口，不顺手重构。
- 先修正确性，再清死代码。
- 改完用该方案末尾的测试清单验收。

## 依赖关系

```
01 Build ──────────────────────────────────────► 可出包
02 Health ──► 04 AI 死亡切换
03 Projectiles ──► 06 DroppedItem Destroy 约定
05 Player ──► 08/10 EndStoryMode
09 Config ──► 06 ItemManager / 07 NPCInteract / 10 Timeline
```
