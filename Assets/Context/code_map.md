# Important Files

## `Assets/Scripts/GameManager.cs`

- Path: `Assets/Scripts/GameManager.cs`
- Purpose: 当前为空的占位入口脚本，尚未承载项目级流程控制。
- Main dependencies: `UnityEngine`
- Important notes: 目前没有实际逻辑；后续如果需要全局编排，应避免把输入、交互、玩法规则全部堆进该文件。

## `Assets/Scripts/InputManager.cs`

- Path: `Assets/Scripts/InputManager.cs`
- Purpose: 封装 Unity Input System 的 `Player` action map，对外提供移动、视角、选择和缩放输入。
- Main dependencies: `UnityEngine.InputSystem`, `InputActionAsset`
- Important notes: 这是当前最接近未来 Quest 3 主线的输入入口；后续若对接 Meta Building Blocks，应尽量让更外层 XR 适配继续调用统一输入动作或等价动作，而不是绕开该边界把玩法逻辑直接写进 Building Blocks 回调。

## `Assets/Scripts/PlayerMovement.cs`

- Path: `Assets/Scripts/PlayerMovement.cs`
- Purpose: 根据 `InputManager.MoveInput` 驱动 `CharacterController` 做平面移动。
- Main dependencies: `InputManager`, `CharacterController`
- Important notes: 当前更像非 VR 原型的移动层；未来 Quest 3 原生版本大概率会由 XR locomotion 或 Building Blocks 组件替代，而核心玩法不应依赖该脚本的具体实现。

## `Assets/Scripts/PlayerLook.cs`

- Path: `Assets/Scripts/PlayerLook.cs`
- Purpose: 根据 `InputManager.LookInput` 控制玩家 yaw 和相机 pitch root。
- Main dependencies: `InputManager`, `Transform`
- Important notes: 当前是明显偏非 VR 的视角适配层；Quest 3 原生版本不应直接复用其“控制头部视角”的职责，但可以保留“平台相关视角脚本与核心玩法分离”的边界思想。

## `Assets/Scripts/ObjectSelector.cs`

- Path: `Assets/Scripts/ObjectSelector.cs`
- Purpose: 当前非 VR 原型的中心射线交互层，负责检测可交互物、拿起放下、持续搬运已拿起物体、缩放已选中物体，并绘制中心准星。
- Main dependencies: `InputManager`, `InteractableObject`, `Physics.RaycastAll`, `OnGUI`
- Important notes: 这是当前最典型的“非 VR 交互适配层”。后续对接 Meta Quest 3 Building Blocks 时，应把其职责拆看为：射线来源、选择输入、拿起/放下驱动、缩放驱动、反馈显示。真正可复用的核心逻辑不应长期留在这个脚本里。

## `Assets/Scripts/InteractableObject.cs`

- Path: `Assets/Scripts/InteractableObject.cs`
- Purpose: 维护可交互物体的核心状态，包括是否被选中、是否被拿起、当前尺寸倍率，以及相关 UnityEvent。
- Main dependencies: `Rigidbody`, `UnityEvent`
- Important notes: 这是当前最适合作为“平台无关核心玩法层”起点的脚本。Quest 3 的 grab/ray/building blocks 和未来非 VR 的鼠标/手柄交互，都应尽量通过调用它的接口来驱动对象状态，而不是把规则复制到不同平台脚本里。

## `Assets/Scripts/QuestInteractionBridge.cs`

- Path: `Assets/Scripts/QuestInteractionBridge.cs`
- Purpose: 作为 Quest 侧的全局交互桥，维护当前 hover 与 selected 对象，并把 Quest 的选择/释放/缩放动作转成对 `InteractableObject` 的状态调用。
- Main dependencies: `InteractableObject`
- Important notes: 该脚本不直接依赖 Meta 的具体 Building Block 组件类型；它期望由 UnityEvent 或其他适配层把 XR 事件转发进来。这让 Quest 实现与核心玩法之间保持薄桥接关系。

## `Assets/Scripts/QuestInteractableEventBridge.cs`

- Path: `Assets/Scripts/QuestInteractableEventBridge.cs`
- Purpose: 挂在每个 Quest 可交互物体上，把 Meta Interaction SDK / Building Blocks 的 hover、select、unselect 事件转发给全局 `QuestInteractionBridge`。
- Main dependencies: `QuestInteractionBridge`, `InteractableObject`, `Oculus.Interaction.PointerEvent`
- Important notes: 该脚本提供无参和 `PointerEvent` 两套入口，便于同时对接 `InteractableUnityEventWrapper` 与 `PointableUnityEventWrapper`。

## `Assets/Scripts/InputSystem_Actions.inputactions`

- Path: `Assets/Scripts/InputSystem_Actions.inputactions`
- Purpose: Unity Input System 动作资源，定义 `Player/Move`、`Player/Look`、`Player/Select`、`Player/Scale` 等动作。
- Main dependencies: Unity Input System package
- Important notes: 当前资源位于 `Assets/Scripts/` 下；若未来同时支持 Quest 3 和非 VR，动作命名应继续保持平台无关，避免写成仅限键鼠语义的名字。

## `Assets/Context/plan.md`

- Path: `Assets/Context/plan.md`
- Purpose: 记录当前项目目标、优先级、Quest 3 原生优先的阶段规划，以及为非 VR 版本预留的适配边界。
- Main dependencies: 用户手动维护的玩法方向
- Important notes: 权威高于 `state.md` 和 `code_map.md`；当与实际代码不一致时，应先在 `state.md` 记录 mismatch。

## `Assets/Context/state.md`

- Path: `Assets/Context/state.md`
- Purpose: 记录当前实现进度、事实状态和计划/代码偏差。
- Main dependencies: 实际代码和场景状态
- Important notes: 应优先反映当前真实代码，而不是沿用过期实现描述。

## `Assets/Context/code_map.md`

- Path: `Assets/Context/code_map.md`
- Purpose: 说明当前脚本结构与职责边界，帮助后续判断哪些更接近核心玩法层，哪些是非 VR 原型层，哪些未来应与 Quest 3 Building Blocks 对接。
- Main dependencies: `Assets/Scripts/` 目录现状
- Important notes: 只记录 `Assets/Context/` 和 `Assets/Scripts/` 下的重要文件。

## `Assets/Context/memory.md`

- Path: `Assets/Context/memory.md`
- Purpose: 为后续代理执行保留简洁的操作记忆和阶段提醒。
- Main dependencies: 当前计划与状态
- Important notes: 仅作辅助记忆，不能覆盖 `plan.md` 与 `state.md`。
