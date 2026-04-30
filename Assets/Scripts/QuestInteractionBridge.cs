using UnityEngine;

public class QuestInteractionBridge : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    [Header("Scale")]
    [SerializeField] private OVRInput.Controller scaleController = OVRInput.Controller.RTouch;
    [SerializeField] private OVRInput.Button enlargeButton = OVRInput.Button.One;
    [SerializeField] private OVRInput.Button shrinkButton = OVRInput.Button.Two;
    [SerializeField] private float scaleButtonStep = 0.25f;

    private InteractableObject hoveredObject;
    private InteractableObject selectedObject;

    public InteractableObject HoveredObject => hoveredObject;
    public InteractableObject SelectedObject => selectedObject;

    private void Update()
    {
        if (selectedObject == null || !selectedObject.IsHeld)
        {
            return;
        }

        float scaleDelta = 0f;

        if (OVRInput.GetDown(enlargeButton, scaleController))
        {
            scaleDelta += scaleButtonStep;
            LogDebug($"Scale up pressed for {selectedObject.name}.");
        }

        if (OVRInput.GetDown(shrinkButton, scaleController))
        {
            scaleDelta -= scaleButtonStep;
            LogDebug($"Scale down pressed for {selectedObject.name}.");
        }

        if (Mathf.Approximately(scaleDelta, 0f))
        {
            return;
        }

        // Quest button presses should change the scale multiplier directly instead of reusing mouse scroll tuning.
        ApplyScaleToSelection(scaleDelta);
    }

    public void NotifyHoverEntered(InteractableObject target)
    {
        if (target == null)
        {
            Debug.LogError($"{nameof(QuestInteractionBridge)} received a null hover target.", this);
            return;
        }

        hoveredObject = target;
    }

    public void NotifyHoverExited(InteractableObject target)
    {
        if (hoveredObject == target)
        {
            hoveredObject = null;
        }
    }

    public void NotifySelectEntered(InteractableObject target)
    {
        if (target == null)
        {
            Debug.LogError($"{nameof(QuestInteractionBridge)} received a null select target.", this);
            return;
        }

        if (selectedObject != null && selectedObject != target)
        {
            ReleaseSelection(selectedObject);
        }

        SetSelectedObject(target);

        if (target.CanBeHeld)
        {
            target.SetHeld(true);
        }
    }

    public void NotifySelectExited(InteractableObject target)
    {
        if (target == null)
        {
            return;
        }

        ReleaseSelection(target);
    }

    public void ApplyScaleToSelection(float scaleDelta)
    {
        if (selectedObject == null || !selectedObject.IsHeld)
        {
            return;
        }

        // Use the shared clamp path while letting Quest buttons express a direct multiplier step.
        selectedObject.SetScaleMultiplier(selectedObject.ScaleMultiplier + scaleDelta);
        LogDebug($"Applied scale step {scaleDelta} to {selectedObject.name}. New multiplier: {selectedObject.ScaleMultiplier}.");
    }

    public void ClearSelection()
    {
        if (selectedObject == null)
        {
            return;
        }

        ReleaseSelection(selectedObject);
    }

    private void ReleaseSelection(InteractableObject target)
    {
        target.SetHeld(false);

        if (selectedObject == target)
        {
            SetSelectedObject(null);
        }
    }

    private void SetSelectedObject(InteractableObject newSelection)
    {
        if (selectedObject == newSelection)
        {
            return;
        }

        if (selectedObject != null)
        {
            selectedObject.SetSelected(false);
        }

        selectedObject = newSelection;

        if (selectedObject != null)
        {
            selectedObject.SetSelected(true);
        }
    }

    private void LogDebug(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[{nameof(QuestInteractionBridge)}] {message}", this);
    }
}
