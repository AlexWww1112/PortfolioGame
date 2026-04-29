using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShowISpyMessage : MonoBehaviour
{
    public TMP_Text messageText; // 关联到UI上的Text组件
    public TMP_Text restart; // 关联到UI上的Text组件
    public Button restartButton; // 关联到UI上的Restart按钮
    public float displayTime = 10f; // 显示时长（秒）

    private Coroutine showCoroutine;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }
        if (restart != null)
        {
            restart.gameObject.SetActive(false);
        }
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    // 只保留自定义消息显示方法
    public void ShowCustomMessage(string message, float customDisplayTime = 10f)
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }
        showCoroutine = StartCoroutine(ShowCustomMessageCoroutine(message, customDisplayTime));
    }

    IEnumerator ShowCustomMessageCoroutine(string message, float customDisplayTime)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(customDisplayTime);
        messageText.gameObject.SetActive(false);
        if (restart != null)
        {
            //restart.text = message;
            restart.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("messageText未赋值");
        }
        if (restartButton != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            restartButton.gameObject.SetActive(true);
            messageText.text = "I understand the world better now... Lets go research worlds through moving images before our last stop";
            messageText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("restartButton未赋值");
        }
        yield return new WaitForSeconds(customDisplayTime);
        if (restart != null)
        {
            restart.gameObject.SetActive(false);
        }
        // 按钮不自动隐藏，直到玩家点击
    }

    public void RestartGame()
    {
        // 隐藏按钮，防止多次点击
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
