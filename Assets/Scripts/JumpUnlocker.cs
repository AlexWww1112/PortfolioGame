using UnityEngine;

public class JumpUnlocker : MonoBehaviour
{
    public bool playerInRange = false;

    void Update()
    {
        // Debug.Log(SelectionManager.Instance.onTarget);
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange)
        {
            PlayerMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.canJump = true;
                Debug.Log("You can now jump by pressing Space!");
                // Destroy(gameObject); // 一旦解锁跳跃后销毁这个Cube
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
