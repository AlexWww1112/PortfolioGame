using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform cameraPitchRoot;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private float pitch;

    private void Awake()
    {
        if (inputManager == null || cameraPitchRoot == null)
        {
            Debug.LogError($"{nameof(PlayerLook)} needs an InputManager and camera pitch root assigned.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Vector2 lookInput = inputManager.LookInput * lookSensitivity;

        // Yaw turns the player body; pitch only tilts the assigned camera root.
        transform.Rotate(Vector3.up, lookInput.x, Space.Self);

        pitch = Mathf.Clamp(pitch - lookInput.y, minPitch, maxPitch);
        cameraPitchRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
