using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        if (inputManager == null || characterController == null)
        {
            Debug.LogError($"{nameof(PlayerMovement)} needs an InputManager and CharacterController assigned.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Vector2 input = inputManager.MoveInput;

        // Move relative to the player's current yaw so WASD follows the view direction.
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}