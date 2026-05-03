using UnityEngine;

public class ShowTextOnApproach : MonoBehaviour
{
    public GameObject textUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textUI.SetActive(false);
        }
    }
}