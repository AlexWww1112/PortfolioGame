using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Legacy Input Manager Axes")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string mouseXAxis = "Mouse X";
    [SerializeField] private string mouseYAxis = "Mouse Y";

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;

    // Read input on demand so one-frame button presses are not lost because of script execution order.
    public Vector2 MoveInput => new Vector2(
        Input.GetAxisRaw(horizontalAxis),
        Input.GetAxisRaw(verticalAxis));

    public Vector2 LookInput => new Vector2(
        Input.GetAxis(mouseXAxis),
        Input.GetAxis(mouseYAxis));

    public bool SelectPressed => Input.GetMouseButtonDown(0);
    public float ScrollDelta => Input.mouseScrollDelta.y;

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
