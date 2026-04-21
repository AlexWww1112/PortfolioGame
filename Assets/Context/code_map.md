# Important Files

## `Assets/Scripts/GameManager.cs`

- Path: `Assets/Scripts/GameManager.cs`
- Purpose: 当前唯一可见的游戏脚本入口，占位性质，尚未承载具体玩法逻辑。
- Main dependencies: `UnityEngine`
- Important notes: 目前为空实现，后续应避免把全部系统直接堆入该文件。

## `Assets/Scripts/InputManager.cs`

- Path: `Assets/Scripts/InputManager.cs`
- Purpose: 封装 Unity Input System 的 Player action map，向 gameplay 脚本提供移动、视角、选择和缩放输入。
- Main dependencies: `UnityEngine.InputSystem`, `InputActionAsset`
- Important notes: 默认读取 `Move`、`Look`、`Select`、`Scale` 动作；键鼠和基础手柄共用同一套输出接口。

## `Assets/Scripts/InputSystem_Actions.inputactions`

- Path: `Assets/Scripts/InputSystem_Actions.inputactions`
- Purpose: Unity Input System 动作资源，定义 Player 输入动作与键鼠、手柄、基础 XR 绑定。
- Main dependencies: Unity Input System package
- Important notes: `InputManager` 默认读取其中的 `Player/Move`、`Player/Look`、`Player/Select`、`Player/Scale`。

## `Assets/Scripts/PlayerMovement.cs`

- Path: `Assets/Scripts/PlayerMovement.cs`
- Purpose: 根据 `InputManager.MoveInput` 驱动玩家通过 `CharacterController` 进行 WASD 水平移动。
- Main dependencies: `InputManager`, `UnityEngine.CharacterController`
- Important notes: 需要用户在 Inspector 中手动绑定 `InputManager` 和 `CharacterController`，不会自动查找或创建组件。

## `Assets/Scripts/PlayerLook.cs`

- Path: `Assets/Scripts/PlayerLook.cs`
- Purpose: 根据 `InputManager.LookInput` 旋转玩家 yaw，并旋转指定 camera pitch root 实现鼠标视角。
- Main dependencies: `InputManager`, `UnityEngine.Transform`
- Important notes: 需要用户手动绑定 camera pitch root；脚本不会自动查找 Camera。

## `Assets/Scripts/InteractableObject.cs`

- Path: `Assets/Scripts/InteractableObject.cs`
- Purpose: 标记可被选中、拿起和缩放的场景对象，保存缩放倍率范围、拿起状态并触发相关事件。
- Main dependencies: `UnityEngine`, `UnityEngine.Events`
- Important notes: 当前只改变自身 `transform.localScale` 和位置；如果手动绑定 Rigidbody，拿起时会临时切换为 kinematic，放下时恢复。

## `Assets/Scripts/ObjectSelector.cs`

- Path: `Assets/Scripts/ObjectSelector.cs`
- Purpose: 通过指定 ray origin 发射射线，选择并拿起 `InteractableObject`，再次选择时放下，滚轮缩放当前拿起对象，并绘制屏幕中心准星。
- Main dependencies: `InputManager`, `InteractableObject`, `UnityEngine.Physics`
- Important notes: 需要用户手动绑定 `InputManager` 和 ray origin，并设置可选对象 LayerMask；射线会扫描所有命中并选最近的 `InteractableObject`；拿起位置由 `holdDistance` 控制；准星使用 `OnGUI` 绘制，不生成 UI GameObject。

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
