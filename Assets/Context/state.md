# Done

- 建立并持续维护 `Assets/Context/` 文档体系。
- `plan.md` 已重写为 Meta Quest 3 原生优先，并要求为非 VR 版本预留适配层。
- 本次已重新核对 `Assets/Scripts/` 当前真实结构，确认项目目前只保留一条较新的输入与交互原型路线。
- 已新增 Quest bridge 原型：
- `QuestInteractionBridge` 负责维护 Quest 侧当前 hover / selected / held / scale 驱动。
- `QuestInteractableEventBridge` 负责把 Meta Interaction SDK / Building Blocks 的 UnityEvent 转发到核心交互状态。

# In Progress

- 项目仍处于 very early prototype 阶段。
- 当前脚本主线由 `InputManager`、`PlayerMovement`、`PlayerLook`、`ObjectSelector`、`InteractableObject`、`QuestInteractionBridge`、`QuestInteractableEventBridge` 组成。
- 当前实现已经有 Quest bridge 骨架，但还没有完成真实场景里的 Meta Quest 3 Building Blocks 接线与验证。

# Blockers

- `ObjectSelector` 当前仍直接承担中心射线、拿起放下、缩放和准星绘制，它仍属于非 VR 原型层。
- `PlayerLook` 当前仍是明显偏非 VR 的鼠标/摇杆视角实现，不适合作为 Quest 3 头显视角层直接复用。
- 当前还没有完整的 Quest 3 场景接线示例；Building Blocks 的 ray / grab / thumbstick 事件仍需手动挂接到 bridge。
- 当前还没有 Meta Quest 3 原生所需的 XR 反馈和 XR 移动层实现细节，当前 bridge 只处理交互状态转发。

# Notes

- `Assets/Scripts/SelectionManager.cs` 当前不存在，旧文档中的相关描述已失效。
- `InputSystem_Actions.inputactions` 当前位于 `Assets/Scripts/` 下。
- `InteractableObject` 已具备较适合作为核心玩法层的状态：`IsSelected`、`IsHeld`、尺寸倍率、尺寸事件。
- `ObjectSelector` 更适合作为当前非 VR 原型层，而不是最终的 Quest 3 原生交互层。
- `QuestInteractionBridge` 目前不直接依赖 Meta 具体 interactor 类型，而是通过 UnityEvent 桥接接收事件，便于与 Building Blocks 对接，也便于未来替换具体 XR 交互实现。
- 以后继续实现新玩法时，应优先把玩法规则挂到 `InteractableObject` 或其他平台无关组件上，而不是继续把规则写进 `ObjectSelector` 或 Meta 组件回调里。
