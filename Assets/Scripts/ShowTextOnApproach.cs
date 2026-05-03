using UnityEngine;

public class ShowTextOnApproach : MonoBehaviour
{
    public GameObject textUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("触发了: " + other.name); 
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