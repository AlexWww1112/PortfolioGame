# Important Files

## `Assets/Scripts/GameManager.cs`

- Path: `Assets/Scripts/GameManager.cs`
- Purpose: 当前为空的占位入口脚本，尚未承载项目级流程控制。
- Main dependencies: `UnityEngine`
- Important notes: 目前没有实际逻辑，后续应避免把所有系统直接堆入该文件。

## `Assets/Scripts/InputManager.cs`

- Path: `Assets/Scripts/InputManager.cs`
- Purpose: 封装 Unity Input System 的 Player action map，对外提供移动、视角、选择和缩放输入。
- Main dependencies: `UnityEngine.InputSystem`, `InputActionAsset`
- Important notes: 这是更接近未来 Quest 3 原生主线的输入路线；但当前代码库中仍并存多份直接使用旧输入 API 的脚本。

## `Assets/Scripts/InputSystem_Actions.inputactions`

- Path: `Assets/Scripts/InputSystem_Actions.inputactions`
- Purpose: Unity Input System 动作资源，定义 `Player/Move`、`Player/Look`、`Player/Select`、`Player/Scale` 等输入动作。
- Main dependencies: Unity Input System package
- Important notes: 当前资源位于 `Assets/Scripts/` 下，不在 `Assets/` 根目录。

## `Assets/Scripts/PlayerMovement.cs`

- Path: `Assets/Scripts/PlayerMovement.cs`
- Purpose: 当前实际使用的第一人称角色移动与跳跃脚本，负责重力、地面检测和可解锁跳跃。
- Main dependencies: `CharacterController`, `Physics.CheckSphere`, old Input Manager axes/buttons
- Important notes: 仍直接使用 `Input.GetAxis` 和 `Input.GetButtonDown("Jump")`；`canJump` 会被 `JumpUnlocker` 解锁。

## `Assets/Scripts/MouseMovement.cs`

- Path: `Assets/Scripts/MouseMovement.cs`
- Purpose: 当前实际使用的鼠标视角脚本，负责本地旋转并在背包打开时暂停。
- Main dependencies: old Input Manager mouse axes, `InventorySystem`
- Important notes: 使用 `Input.GetAxis("Mouse X/Y")`；和 `PlayerLook.cs` 代表的 Input System 视角路线并存。

## `Assets/Scripts/PlayerLook.cs`

- Path: `Assets/Scripts/PlayerLook.cs`
- Purpose: 较新的 Input System 视角原型，根据 `InputManager.LookInput` 控制 yaw/pitch。
- Main dependencies: `InputManager`, `Transform`
- Important notes: 这条路线目前在代码层存在，但从现有脚本依赖关系看，尚未完全取代 `MouseMovement.cs`；后续若转向 Quest 3 原生，其职责应进一步从“鼠标视角”转向“平台相关视角适配”。

## `Assets/Scripts/SelectionManager.cs`

- Path: `Assets/Scripts/SelectionManager.cs`
- Purpose: 用屏幕中心射线检测当前是否瞄准可交互物，并控制交互提示 UI。
- Main dependencies: `Camera.main`, `InteractableObject`, `TextMeshProUGUI`
- Important notes: 通过 `SelectionManager.Instance.onTarget` 向其他脚本暴露是否瞄准目标的状态；这是明显偏向非 VR 的旧选择流程，未来若按 Quest 3 原生推进，应被更通用的选择来源层替代或包裹。

## `Assets/Scripts/InteractableObject.cs`

- Path: `Assets/Scripts/InteractableObject.cs`
- Purpose: 当前实际使用的可收集物体脚本，负责在玩家靠近并点击时将物体加入背包并销毁场景实例。
- Main dependencies: `SelectionManager`, `InventorySystem`, `CollectManager`, old input API
- Important notes: 公开 `ItemName` 和 `playerInRange`；目前是“收集进背包”流程，而不是计划中更适合 Quest 3 与非 VR 共用的“选中/拿起/放下/缩放”核心交互状态。

## `Assets/Scripts/InventorySystem.cs`

- Path: `Assets/Scripts/InventorySystem.cs`
- Purpose: 管理背包 UI 开关、槽位列表、物品实例化到槽位以及满背包检查。
- Main dependencies: `Resources.Load`, `ItemSlot`, old input API
- Important notes: 使用 `I` 键开关背包；槽位来自 `inventoryScreenUI` 下带 `Slot` 标签的子物体。

## `Assets/Scripts/DragDrop.cs`

- Path: `Assets/Scripts/DragDrop.cs`
- Purpose: 处理背包物品 UI 的开始拖拽、拖拽中和结束拖拽行为。
- Main dependencies: `RectTransform`, `CanvasGroup`, Unity UI event interfaces
- Important notes: 拖拽失败时会回到原始父节点和位置。

## `Assets/Scripts/ItemSlot.cs`

- Path: `Assets/Scripts/ItemSlot.cs`
- Purpose: 作为背包槽位的 drop 目标，接收 `DragDrop` 拖拽过来的物品。
- Main dependencies: Unity UI event interfaces, `DragDrop`
- Important notes: 只有空槽位才会接收拖拽物品。

## `Assets/Scripts/CollectManager.cs`

- Path: `Assets/Scripts/CollectManager.cs`
- Purpose: 统计被收集物体数量，到达阈值后触发最终提示消息。
- Main dependencies: `ShowISpyMessage`
- Important notes: 当前阈值写死为收集 5 个物体。

## `Assets/Scripts/ShowISpyMessage.cs`

- Path: `Assets/Scripts/ShowISpyMessage.cs`
- Purpose: 显示限时消息，并在后续显示重开按钮与附加文本。
- Main dependencies: `TMP_Text`, `Button`, `SceneManager`
- Important notes: 会在显示重开按钮时解锁鼠标光标。

## `Assets/Scripts/JumpUnlocker.cs`

- Path: `Assets/Scripts/JumpUnlocker.cs`
- Purpose: 玩家进入触发范围并点击后，解锁 `PlayerMovement.canJump`。
- Main dependencies: `SelectionManager`, `PlayerMovement`, old input API
- Important notes: 通过 `FindGameObjectWithTag("Player")` 查找玩家；当前不会在解锁后销毁自己。

## `Assets/Scripts/CharacterSwitch.cs`

- Path: `Assets/Scripts/CharacterSwitch.cs`
- Purpose: 在触发器内切换两个第一人称控制器对象的激活状态。
- Main dependencies: `GameObject.SetActive`
- Important notes: 依赖玩家对象拥有 `Player` 标签。

## `Assets/Scripts/Control.cs`

- Path: `Assets/Scripts/Control.cs`
- Purpose: 提供公开的重开场景方法，通常供 UI 按钮调用。
- Main dependencies: `SceneManager`
- Important notes: `Update()` 为空，实际作用集中在 `ResetTheGame()`。

## `Assets/Scripts/Follow.cs`

- Path: `Assets/Scripts/Follow.cs`
- Purpose: 在目标移动时按随机间隔生成并附着爆炸特效。
- Main dependencies: `Instantiate`, coroutine, `Random.Range`
- Important notes: 这是当前少数会在代码中实例化对象的旧脚本，与 `AGENTS.md` 的新实现约束不一致，但它反映的是现有代码现实。

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
- Purpose: 说明当前脚本结构与职责边界，帮助后续判断哪些是旧栈、哪些更接近 Quest 3 主线，以及哪里需要预留非 VR 适配接口。
- Main dependencies: `Assets/Scripts/` 目录现状
- Important notes: 只记录 `Assets/Context/` 和 `Assets/Scripts/` 下的重要文件。

## `Assets/Context/memory.md`

- Path: `Assets/Context/memory.md`
- Purpose: 为后续代理执行保留简洁的操作记忆和阶段提醒。
- Main dependencies: 当前计划与状态
- Important notes: 仅作辅助记忆，不能覆盖 `plan.md` 与 `state.md`。
