using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool persistAcrossSceneTransitions = true;

    private static GameManager instance;
    private bool transitionInProgress;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"{nameof(GameManager)} already exists. Destroying duplicate instance.", this);
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (persistAcrossSceneTransitions)
        {
            // Keep one manager alive while scene transitions complete.
            DontDestroyOnLoad(gameObject);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (transitionInProgress)
        {
            Debug.LogWarning($"{nameof(GameManager)} is already transitioning scenes.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"{nameof(GameManager)} needs a target scene name.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not available. Add it to Build Settings first.", this);
            return;
        }

        StartCoroutine(TransitionToSceneRoutine(sceneName));
    }

    public void ReloadActiveScene()
    {
        TransitionToScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator TransitionToSceneRoutine(string targetSceneName)
    {
        transitionInProgress = true;

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == targetSceneName)
        {
            transitionInProgress = false;
            yield break;
        }

        // Meta Building Blocks camera rigs are safer when the previous scene rig is removed
        // before the next scene rig becomes active.
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);

        if (loadOperation == null)
        {
            Debug.LogError($"Failed to start loading scene '{targetSceneName}'.", this);
            transitionInProgress = false;
            yield break;
        }

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Scene targetScene = SceneManager.GetSceneByName(targetSceneName);

        if (!targetScene.IsValid() || !targetScene.isLoaded)
        {
            Debug.LogError($"Scene '{targetSceneName}' did not finish loading correctly.", this);
            transitionInProgress = false;
            yield break;
        }

        SceneManager.SetActiveScene(targetScene);
        transitionInProgress = false;
    }
}
