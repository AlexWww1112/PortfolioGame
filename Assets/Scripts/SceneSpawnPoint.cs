using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private bool applyYaw = true;

    public bool ApplyYaw => applyYaw;
    public Vector3 SpawnPosition => transform.position;
    public Quaternion SpawnRotation => transform.rotation;
}
