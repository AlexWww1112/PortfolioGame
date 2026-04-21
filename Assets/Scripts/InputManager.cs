using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string lookActionName = "Look";
    [SerializeField] private string selectActionName = "Select";
    [SerializeField] private string scaleActionName = "Scale";

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;

    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction selectAction;
    private InputAction scaleAction;

    public Vector2 MoveInput => moveAction.ReadValue<Vector2>();
    public Vector2 LookInput => lookAction.ReadValue<Vector2>();
    public bool SelectPressed => selectAction.WasPressedThisFrame();

    // Mouse scroll can report large deltas while gamepad buttons report +/-1, so expose one normalized step.
    public float ScrollDelta => Mathf.Clamp(scaleAction.ReadValue<float>(), -1f, 1f);

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError($"{nameof(InputManager)} needs an InputActionAsset assigned.", this);
            enabled = false;
            return;
        }

        playerActionMap = inputActions.FindActionMap(actionMapName, true);
        moveAction = playerActionMap.FindAction(moveActionName, true);
        lookAction = playerActionMap.FindAction(lookActionName, true);
        selectAction = playerActionMap.FindAction(selectActionName, true);
        scaleAction = playerActionMap.FindAction(scaleActionName, true);
    }

    private void OnEnable()
    {
        if (playerActionMap != null)
        {
            playerActionMap.Enable();
        }
    }

    private void OnDisable()
    {
        if (playerActionMap != null)
        {
            playerActionMap.Disable();
        }
    }

    private void Start()
    {
        if (!lockCursorOnStart)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
