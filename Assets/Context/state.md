# Done

- 建立并持续维护 `Assets/Context/` 文档体系。
- `plan.md` 已明确职责边界：用户负责系统与关卡设计，Codex 负责按规格实现功能。
- 本次已重新扫描 `Assets/Scripts/` 并按实际代码更新文档，移除过期的 `ObjectSelector` 描述，补充当前真实使用的脚本结构。
- `plan.md` 已重写为 Meta Quest 3 原生优先，并要求为非 VR 版本预留适配层。

# In Progress

- 项目仍处于 very early prototype 阶段。
- 当前代码库同时存在两套并行思路：
- 一套是较旧的第一人称探索/收集/背包流程，核心脚本为 `PlayerMovement`、`MouseMovement`、`SelectionManager`、`InteractableObject`、`InventorySystem`、`DragDrop`、`ItemSlot`、`CollectManager`、`ShowISpyMessage`、`JumpUnlocker`。
- 另一套是较新的 Input System 原型，核心脚本为 `InputManager`、`PlayerLook`，并配有 `InputSystem_Actions.inputactions` 资源。
- 当前计划已明确 Quest 3 原生优先，但实际代码尚未形成“平台无关核心玩法层 + Quest 3 交互层 + 非 VR 适配层”的边界。

# Blockers

- 当前实际主流程仍偏向旧的非 VR 收集/背包原型，与最新 `plan.md` 的 Quest 3 原生主线存在明显偏差。
- 文档中此前记录的 `ObjectSelector.cs` 已不在 `Assets/Scripts/` 中，实际选择逻辑已切换为 `SelectionManager` + `InteractableObject`。
- 当前代码仍混用旧输入 API 与 Input System，后续如果继续实现新功能，容易继续扩大两套输入栈的分叉。
- 当前尚未建立 Quest 3 原生所需的 XR 射线来源、XR 反馈和 XR 移动层。
- 当前尚未建立未来非 VR 版本要复用的清晰适配接口。

# Notes

- 当前输入资源 `InputSystem_Actions.inputactions` 位于 `Assets/Scripts/` 下，而不是旧记录中的 `Assets/` 根目录。
- `PlayerMovement`、`MouseMovement`、`InteractableObject`、`JumpUnlocker` 仍直接使用 `Input.GetAxis` / `Input.GetKeyDown` / `Input.GetButtonDown`。
- `InputManager` 与 `PlayerLook` 代表一条较新的 Input System 路线，但从代码静态关系看，尚未统一替换旧输入栈。
- `Follow.cs` 仍会在运行时 `Instantiate` 特效对象，这反映的是现有旧代码现实，与当前 `AGENTS.md` 的新实现约束不一致。
- 后续如果继续实现新玩法，应优先围绕 Quest 3 主线整理架构，而不是继续扩张旧的 PC-only 交互流程。
