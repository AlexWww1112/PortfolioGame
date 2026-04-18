# Important Files

## `Assets/Scripts/GameManager.cs`

- Path: `Assets/Scripts/GameManager.cs`
- Purpose: 当前唯一可见的游戏脚本入口，占位性质，尚未承载具体玩法逻辑。
- Main dependencies: `UnityEngine`
- Important notes: 目前为空实现，后续应避免把全部系统直接堆入该文件。

## `Assets/Context/plan.md`

- Path: `Assets/Context/plan.md`
- Purpose: 记录当前项目目标、优先级、阶段规划和待确认问题。
- Main dependencies: 玩法设计决策、原型范围控制
- Important notes: 以人类编辑优先，后续实现应与此文件对齐。

## `Assets/Context/state.md`

- Path: `Assets/Context/state.md`
- Purpose: 记录当前实现进度、完成项、阻塞项和已确认事实。
- Main dependencies: 实际代码和场景状态
- Important notes: 每次非 trivial 实现后都需要更新，避免计划与现实脱节。

## `Assets/Context/code_map.md`

- Path: `Assets/Context/code_map.md`
- Purpose: 记录重要代码和文档文件的职责边界。
- Main dependencies: 代码结构与目录结构
- Important notes: 当新增、删除、重命名或明显重构文件时必须更新。

## `Assets/Context/memory.md`

- Path: `Assets/Context/memory.md`
- Purpose: 为后续代理执行保留简洁的操作记忆和阶段提醒。
- Main dependencies: 当前计划与状态
- Important notes: 仅作辅助记忆，不能覆盖 `plan.md` 与 `state.md`。
