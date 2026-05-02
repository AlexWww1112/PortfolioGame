# Done

- 建立并持续维护 `Assets/Context/` 文档体系。
- `plan.md` 已重写为 Meta Quest 3 原生优先，并要求为非 VR 版本预留适配层。
- 本次已重新核对 `Assets/Scripts/` 当前真实结构，确认项目目前只保留一条较新的输入与交互原型路线。
- 已新增 Quest bridge 原型：
- `QuestInteractionBridge` 负责维护 Quest 侧当前 hover / selected / held / scale 驱动。
- `QuestInteractableEventBridge` 负责把 Meta Interaction SDK / Building Blocks 的 UnityEvent 转发到核心交互状态。
- Quest bridge 已改为更直接的接法：
- `QuestInteractableEventBridge` 现在直接订阅 Meta `PointableElement` 的 `WhenPointerEventRaised`。
- `QuestInteractionBridge` 现在直接读取右手控制器 `A / B` 按钮做离散缩放，而不再依赖摇杆或 `DPadUnityEventWrapper`。
- 已为 Quest 缩放保留可开关调试日志，便于确认 `A / B` 缩放输入是否真的到达脚本。
- Quest 按钮缩放现在直接修改 `ScaleMultiplier`，不再复用 `scaleChangePerScroll` 这类鼠标滚轮语义参数。
- `InteractableObject` 现在支持单独指定 `Scale Target`，用于处理 Meta 抓取壳对象与实际可见模型不在同一 Transform 的情况。
- 如果 `Scale Target` 未手动指定，`InteractableObject` 现在会优先使用同物体上的 Meta `Grabbable.Transform` 作为缩放目标。
- `InteractableObject` 现在可选输出缩放目标调试日志，用于确认实际被缩放的 `scaleTarget` 名称与 `localScale`。
- `InteractableObject` 现在会在帧末检测 `scaleTarget.localScale` 是否被其他系统覆盖；若被覆盖，则会重新应用期望缩放，并在调试开启时输出 warning。
- 已新增 `SizeGateInteractionTarget`，支持触发区自动判定指定物体、尺寸范围、是否被持有，并返回明确结果。
- `GameManager` 现在负责场景切换，提供 `TransitionToScene(string)`，内部执行“加载目标场景 + 卸载当前场景”。
- `Scene1` 现阶段按当前目标暂时视为完成，后续若有新的关卡需求再回补。
- 已新增 `ScaleLinkedFloatValue`，支持把 `ScaleMultiplier` 通过可配置范围和 `AnimationCurve` 映射为连续 float 数值，并通过事件对外输出。
- 已新增 `ScaleThresholdEvent`，支持监听尺寸倍率或映射后的 float 数值，在跨过阈值时触发进入/退出事件。
- `GameManager` 现在改回使用 `LoadSceneMode.Single` 切换关卡，避免 Meta Building Blocks 的两个场景 rig 在 additive 过渡阶段短暂并存。

# In Progress

- 项目仍处于 very early prototype 阶段。
- 当前脚本主线由 `InputManager`、`PlayerMovement`、`PlayerLook`、`ObjectSelector`、`InteractableObject`、`QuestInteractionBridge`、`QuestInteractableEventBridge` 组成。
- 当前实现已经有 Quest bridge 骨架，但还没有完成真实场景里的 Meta Quest 3 Building Blocks 接线与真机验证。
- 当前已开始建立“尺寸变化驱动其他属性变化”的通用联动层，下一步是把具体玩法对象接到这层上验证。
- VR 场景切换当前优先验证 `LoadSceneMode.Single` 是否解决 `Scene1 -> Scene2` 的掉落/卡住问题。

# Blockers

- `ObjectSelector` 当前仍直接承担中心射线、拿起放下、缩放和准星绘制，它仍属于非 VR 原型层。
- `PlayerLook` 当前仍是明显偏非 VR 的鼠标/摇杆视角实现，不适合作为 Quest 3 头显视角层直接复用。
- 当前仍缺少 Quest 真机验证，尚未确认当前 Building Blocks 上实际使用的 `PointableElement` 是哪一个组件最稳定。
- 当前还没有 Meta Quest 3 原生所需的 XR 反馈和 XR 移动层实现细节，当前 bridge 只处理交互状态转发。
- `SizeGateInteractionTarget` 当前只提供目标端判定与结果事件；具体开门、失败提示、一次性消耗钥匙等表现仍需用户在 Inspector 中接线或后续补脚本。
- 如果 `GameManager` 需要跨多个关卡持续存在，应避免在目标场景里重复放置多个实例；当前代码会销毁重复实例。
- `ScaleLinkedFloatValue` 和 `ScaleThresholdEvent` 目前只提供通用数值/阈值能力；重量、伤害、音量、说话、时间倍率等具体行为仍需用户把事件接到各自对象逻辑上。
- 如果后续仍需要精确控制 VR 出生点，应优先考虑“全局持久化单 rig + 场景出生点”这一条更完整的架构，而不是继续在“每个场景各自带 rig + additive 叠场景”上打补丁。

# Notes

- `Assets/Scripts/SelectionManager.cs` 当前不存在，旧文档中的相关描述已失效。
- `InputSystem_Actions.inputactions` 当前位于 `Assets/Scripts/` 下。
- `InteractableObject` 已具备较适合作为核心玩法层的状态：`IsSelected`、`IsHeld`、尺寸倍率、尺寸事件。
- `ObjectSelector` 更适合作为当前非 VR 原型层，而不是最终的 Quest 3 原生交互层。
- `QuestInteractableEventBridge` 现在不再要求 `InteractableUnityEventWrapper` 或 `PointableUnityEventWrapper`；它直接订阅 `PointableElement.WhenPointerEventRaised`。
- `QuestInteractionBridge` 现在直接使用 `OVRInput` 读取右手控制器 `A / B` 按钮，避免依赖额外输入包装组件。
- `SizeGateInteractionTarget` 的触发方式是 trigger 自动判定，不额外要求按钮输入；成功与失败结果都可通过事件对外暴露。
- `SizeGateInteractionTarget.onSuccess` 现在可以直接连接到 `GameManager.TransitionToScene(string)`，实现例如 `Scene1 -> Scene2` 的关卡切换。
- 以后继续实现新玩法时，应优先把玩法规则挂到 `InteractableObject` 或其他平台无关组件上，而不是继续把规则写进 `ObjectSelector` 或 Meta 组件回调里。
- 接下来如果实现属性联动，优先做平台无关、Inspector 可配置的通用数值联动层，而不是直接写死“重量”“伤害”等单一玩法逻辑。
- `ScaleLinkedFloatValue` 适合做连续型联动，例如重量、伤害、音量、时间倍率、容量等。
- `ScaleThresholdEvent` 适合做阈值型联动，例如“大于某值开始说话，小于某值停止”“进入某区间后启用机关”等。
- 对 Meta Building Blocks 而言，若每个场景都自带 rig，`LoadSceneMode.Single` 比 “Additive 加载再手动卸载旧场景” 更稳。
