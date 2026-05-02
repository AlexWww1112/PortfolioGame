using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool persistAcrossSceneTransitions = true;
    [SerializeField] private OVRCameraRig persistentCameraRig;

    private static GameManager instance;
    private bool transitionInProgress;

    public static GameManager Instance => instance;

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

            if (persistentCameraRig != null)
            {
                DontDestroyOnLoad(persistentCameraRig.transform.root.gameObject);
            }
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

        if (persistAcrossSceneTransitions && persistentCameraRig == null)
        {
            Debug.LogError($"{nameof(GameManager)} needs a persistent {nameof(OVRCameraRig)} reference for VR scene transitions.", this);
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

        // Use Single load so the outgoing scene does not keep its Meta rig alive during the transition.
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
        yield return null;
        CleanupSceneRigs(targetScene);
        AlignPersistentRigToSceneSpawn(targetScene);
        transitionInProgress = false;
    }

    private void CleanupSceneRigs(Scene targetScene)
    {
        if (!persistAcrossSceneTransitions || persistentCameraRig == null)
        {
            return;
        }

        GameObject[] rootObjects = targetScene.GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            OVRCameraRig[] sceneRigs = rootObject.GetComponentsInChildren<OVRCameraRig>(true);

            foreach (OVRCameraRig sceneRig in sceneRigs)
            {
                if (sceneRig == null || sceneRig == persistentCameraRig)
                {
                    continue;
                }

                sceneRig.gameObject.SetActive(false);
                Destroy(sceneRig.gameObject);
            }
        }
    }

    private void AlignPersistentRigToSceneSpawn(Scene targetScene)
    {
        if (!persistAcrossSceneTransitions || persistentCameraRig == null)
        {
            return;
        }

        if (persistentCameraRig.centerEyeAnchor == null)
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

        Transform rigTransform = persistentCameraRig.transform;
        Transform eyeAnchor = persistentCameraRig.centerEyeAnchor;

        if (spawnPoint.ApplyYaw)
        {
            float rigYaw = rigTransform.eulerAngles.y;
            float eyeYaw = eyeAnchor.eulerAngles.y;
            float eyeYawOffset = Mathf.DeltaAngle(rigYaw, eyeYaw);
            float targetRigYaw = spawnPoint.SpawnRotation.eulerAngles.y - eyeYawOffset;

            // Rotate the rig so the tracked head forward matches the authored spawn forward.
            rigTransform.rotation = Quaternion.Euler(0f, targetRigYaw, 0f);
        }

        Vector3 eyeWorldOffset = eyeAnchor.position - rigTransform.position;
        Vector3 horizontalEyeOffset = Vector3.ProjectOnPlane(eyeWorldOffset, Vector3.up);
        Vector3 targetRigPosition = spawnPoint.SpawnPosition - horizontalEyeOffset;

        // Treat the spawn point as the player's floor/root position, not the current head height.
        targetRigPosition.y = spawnPoint.SpawnPosition.y;
        rigTransform.position = targetRigPosition;
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
