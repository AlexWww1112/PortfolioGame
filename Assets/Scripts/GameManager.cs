using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool persistAcrossSceneTransitions = true;
    [SerializeField] private OVRCameraRig ovrCameraRig;

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
            // Keep one manager alive while additive load + unload transitions complete.
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

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

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
        AlignRigToSceneSpawn(targetScene);

        if (currentScene.IsValid() && currentScene.isLoaded)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

            if (unloadOperation != null)
            {
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }
        }

        transitionInProgress = false;
    }

    private void AlignRigToSceneSpawn(Scene targetScene)
    {
        if (ovrCameraRig == null)
        {
            Debug.LogError($"{nameof(GameManager)} needs an {nameof(OVRCameraRig)} reference to align scene spawns.", this);
            return;
        }

        if (ovrCameraRig.centerEyeAnchor == null)
        {
            Debug.LogError($"{nameof(GameManager)} needs {nameof(OVRCameraRig)}.{nameof(OVRCameraRig.centerEyeAnchor)} to align scene spawns.", this);
            return;
        }

        SceneSpawnPoint spawnPoint = FindSceneSpawnPoint(targetScene);

        if (spawnPoint == null)
        {
            Debug.LogWarning($"Scene '{targetScene.name}' has no {nameof(SceneSpawnPoint)}. Keeping the current rig position.", this);
            return;
        }

        Transform rigTransform = ovrCameraRig.transform;
        Transform eyeAnchor = ovrCameraRig.centerEyeAnchor;

        if (spawnPoint.ApplyYaw)
        {
            float rigYaw = rigTransform.eulerAngles.y;
            float eyeYaw = eyeAnchor.eulerAngles.y;
            float eyeYawOffset = Mathf.DeltaAngle(rigYaw, eyeYaw);
            float targetRigYaw = spawnPoint.SpawnRotation.eulerAngles.y - eyeYawOffset;

            // Align the rig yaw so the player's actual head forward matches the authored spawn forward.
            rigTransform.rotation = Quaternion.Euler(0f, targetRigYaw, 0f);
        }

        Vector3 eyeWorldOffset = eyeAnchor.position - rigTransform.position;
        rigTransform.position = spawnPoint.SpawnPosition - eyeWorldOffset;
    }

    private SceneSpawnPoint FindSceneSpawnPoint(Scene targetScene)
    {
        GameObject[] rootObjects = targetScene.GetRootGameObjects();
        SceneSpawnPoint foundSpawnPoint = null;

        foreach (GameObject rootObject in rootObjects)
        {
            SceneSpawnPoint[] spawnPoints = rootObject.GetComponentsInChildren<SceneSpawnPoint>(true);

            foreach (SceneSpawnPoint spawnPoint in spawnPoints)
            {
                if (foundSpawnPoint == null)
                {
                    foundSpawnPoint = spawnPoint;
                    continue;
                }

                Debug.LogWarning(
                    $"Scene '{targetScene.name}' has multiple {nameof(SceneSpawnPoint)} components. Using '{foundSpawnPoint.name}' and ignoring '{spawnPoint.name}'.",
                    this);
                return foundSpawnPoint;
            }
        }

        return foundSpawnPoint;
    }
}
