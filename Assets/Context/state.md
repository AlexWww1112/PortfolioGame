# Done

- 建立 `Assets/Context/` 文档体系的基础文件。
- 根据当前想法补充了首版项目规划，聚焦“尺寸改变带动多属性变化”的解谜原型方向。
- 按用户反馈调整 `plan.md`：计划文件现在只描述实现职责和功能 backlog，不再包含 Codex 主动设计的系统规则或关卡方案。
- 同步最新 `AGENTS.md` 约束：这是 Unity 项目，脚本由用户手动 attach，代码中不要生成 objects。
- 同步最新 `AGENTS.md` 约束：尽量少写兜底机制，必要引用缺失时明确报错并停止对应脚本。
- 实现第一批键鼠原型脚本：旧版 `InputManager` 输入封装、WASD 移动、鼠标视角、左键选择可交互物体、滚轮缩放选中物体。
- 为 `ObjectSelector` 增加中心准星 UI，准星会根据普通、瞄准可交互物、已有选中物切换颜色。
- 修复左键选择可能失效的问题：`InputManager` 改为读取时直接查询输入，避免 `Update` 执行顺序导致单帧点击信号丢失。
- 为输入、移动、视角、选择、缩放脚本补充了关键逻辑注释。
- 修复准星不变色的常见原因：`ObjectSelector` 现在会扫描射线路径上的所有命中，选择最近的 `InteractableObject`，避免 `EveryLayer` 下先命中非交互 Collider 后直接失败。

# In Progress

- 项目仍处于 very early prototype 阶段。
- 当前玩法代码已具备基础键鼠移动、视角、选择和缩放能力；脚本需要用户手动 attach 并在 Inspector 中配置引用。

# Blockers

- `Assets/Context/` 约定文件此前缺失，已补建基础版本，但后续需要随着实现持续维护。
- 具体系统规则由用户手动设计；实现前需要用户确认首批功能模块和最小参数。

# Notes

- 当前 `plan.md` 已明确职责边界：用户负责系统和关卡设计，Codex 负责按规格实现功能。
- 后续不要主动替用户补充关卡流程、数值平衡或系统设计结论。
- `code_map.md` 只能记录 `Assets/Context/` 和 `Assets/Scripts/` 下的文件。
- 当前输入读取使用旧版 Unity Input Manager；手柄支持和 Unity Input System 重构已列入后续计划。
- 中心准星由 `ObjectSelector.OnGUI` 绘制，不创建 Canvas、Image 或其他场景对象。
- `ObjectSelector` 默认使用 `QueryTriggerInteraction.Collide`，可识别 trigger collider 上或父级上的 `InteractableObject`。
- `dotnet build Assembly-CSharp.csproj --no-restore` 因缺少 `Temp/obj/Assembly-CSharp/project.assets.json` 失败；普通 `dotnet build` 卡在 restore 阶段，尚未得到编译结果。
