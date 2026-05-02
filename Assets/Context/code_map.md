# Important Files

## `Assets/Scripts/GameManager.cs`

- Path: `Assets/Scripts/GameManager.cs`
- Purpose: 负责关卡级场景切换，提供可从 UnityEvent 直接调用的场景过渡入口。
- Main dependencies: `UnityEngine.SceneManagement`
- Important notes: 当前使用“加载目标场景（Additive）+ 设为 Active + 卸载当前场景”的方式切换关卡。可选 `DontDestroyOnLoad` 持续存在，并会销毁重复实例。

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
- Important notes: 这是当前最适合作为“平台无关核心玩法层”起点的脚本。Quest 3 的 grab/ray/building blocks 和未来非 VR 的鼠标/手柄交互，都应尽量通过调用它的接口来驱动对象状态，而不是把规则复制到不同平台脚本里。现在可通过 `Scale Target` 指定真正被缩放的 Transform；若未指定，则优先使用同物体上的 Meta `Grabbable.Transform`。若有其他系统在缩放后覆盖 `localScale`，该脚本会在 `LateUpdate` 中重新应用期望值。

## `Assets/Scripts/QuestInteractionBridge.cs`

- Path: `Assets/Scripts/QuestInteractionBridge.cs`
- Purpose: 作为 Quest 侧的全局交互桥，维护当前 hover 与 selected 对象，并把 Quest 的抓取状态与右手控制器 `A / B` 缩放输入转成对 `InteractableObject` 的状态调用。
- Main dependencies: `InteractableObject`, `OVRInput`
- Important notes: 该脚本现在直接读取右手控制器按钮，不再依赖摇杆或 `DPadUnityEventWrapper`。默认 `A` 放大、`B` 缩小，并且只在当前 `selectedObject.IsHeld` 为真时响应。按钮步长直接作用于 `ScaleMultiplier`，而不是复用鼠标滚轮的缩放步长参数。

## `Assets/Scripts/QuestInteractableEventBridge.cs`

- Path: `Assets/Scripts/QuestInteractableEventBridge.cs`
- Purpose: 挂在每个 Quest 可交互物体上，直接订阅 Meta `PointableElement` 的 `WhenPointerEventRaised`，再把 hover/select/unselect/cancel 事件转发给全局 `QuestInteractionBridge`。
- Main dependencies: `QuestInteractionBridge`, `InteractableObject`, `Oculus.Interaction.PointableElement`, `Oculus.Interaction.PointerEvent`
- Important notes: 该脚本不再依赖 Inspector 里的 UnityEvent wrapper 接线。当前更推荐把 Building Blocks 生成的 `Grabbable` 组件拖给它的 `Pointable Element` 字段。

## `Assets/Scripts/SizeGateInteractionTarget.cs`

- Path: `Assets/Scripts/SizeGateInteractionTarget.cs`
- Purpose: 挂在门、锁、机关等目标物上，通过 trigger 自动检测进入的 `InteractableObject`，并判定指定对象、尺寸范围和持有状态是否满足条件。
- Main dependencies: `InteractableObject`, `Collider`, `UnityEvent`
- Important notes: 该脚本返回明确的 `SizeGateInteractionResult`，并在成功时触发 `onSuccess`。适合实现“拿着合适尺寸的钥匙靠近门即可开门”这类交互。

## `Assets/Scripts/ScaleLinkedFloatValue.cs`

- Path: `Assets/Scripts/ScaleLinkedFloatValue.cs`
- Purpose: 把某个 `InteractableObject` 的 `ScaleMultiplier` 映射成连续 float 数值，供重量、伤害、音量、时间倍率、容量等系统读取或响应。
- Main dependencies: `InteractableObject`, `AnimationCurve`, `UnityEvent`
- Important notes: 使用输入范围、输出范围和 `AnimationCurve` 控制映射关系；这是平台无关的连续型属性联动层，不依赖 Quest 或非 VR 交互实现。

## `Assets/Scripts/ScaleThresholdEvent.cs`

- Path: `Assets/Scripts/ScaleThresholdEvent.cs`
- Purpose: 监听尺寸倍率或 `ScaleLinkedFloatValue` 输出，在跨过阈值时触发进入/退出事件。
- Main dependencies: `InteractableObject`, `ScaleLinkedFloatValue`, `UnityEvent`
- Important notes: 适合实现“尺寸达到阈值后开始说话，回落后停止”“超过某值才启用机关”等状态切换。通过 `enterThreshold` / `exitThreshold` 支持简单 hysteresis，避免边界抖动反复触发。

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
