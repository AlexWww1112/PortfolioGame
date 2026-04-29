# Done

- 建立并持续维护 `Assets/Context/` 文档体系。
- `plan.md` 已重写为 Meta Quest 3 原生优先，并要求为非 VR 版本预留适配层。
- 本次已重新核对 `Assets/Scripts/` 当前真实结构，确认项目目前只保留一条较新的输入与交互原型路线。

# In Progress

- 项目仍处于 very early prototype 阶段。
- 当前脚本主线由 `InputManager`、`PlayerMovement`、`PlayerLook`、`ObjectSelector`、`InteractableObject` 组成。
- 当前实现更接近“非 VR 原型 + 为未来 XR 收敛留口”的状态，还没有真正接入 Meta Quest 3 Building Blocks。

# Blockers

- `ObjectSelector` 当前仍直接承担中心射线、拿起放下、缩放和准星绘制，尚未拆出更适合 Quest 3 Building Blocks 对接的交互边界。
- `PlayerLook` 当前仍是明显偏非 VR 的鼠标/摇杆视角实现，不适合作为 Quest 3 头显视角层直接复用。
- 当前还没有“平台无关核心玩法层 + Quest 3 Building Blocks 对接层 + 非 VR 适配层”的实际代码分层。
- 当前还没有 Meta Quest 3 原生所需的 XR 射线来源、XR 抓取、XR 反馈和 XR 移动层。

# Notes

- `Assets/Scripts/SelectionManager.cs` 当前不存在，旧文档中的相关描述已失效。
- `InputSystem_Actions.inputactions` 当前位于 `Assets/Scripts/` 下。
- `InteractableObject` 已具备较适合作为核心玩法层的状态：`IsSelected`、`IsHeld`、尺寸倍率、尺寸事件。
- `ObjectSelector` 更适合作为当前非 VR 原型层，而不是最终的 Quest 3 原生交互层。
- 以后继续实现新玩法时，应优先把玩法规则挂到 `InteractableObject` 或其他平台无关组件上，而不是继续把规则写进 `ObjectSelector`。
