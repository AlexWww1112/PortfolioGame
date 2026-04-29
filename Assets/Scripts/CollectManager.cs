using UnityEngine;

public class CollectManager : MonoBehaviour
{
    public static CollectManager Instance;
    private int collectedCount = 0;
    public ShowISpyMessage showISpyMessage;
    public string finalMessage = "Skidaddle to the area near creative coding!";
    public float displayTime = 3f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnItemCollected()
    {
        collectedCount++;
        if (collectedCount == 5)
        {
            ShowFinalMessage();
        }
    }

    private void ShowFinalMessage()
    {
        if (showISpyMessage != null)
        {
            showISpyMessage.ShowCustomMessage(finalMessage, displayTime);
        }
    }
}