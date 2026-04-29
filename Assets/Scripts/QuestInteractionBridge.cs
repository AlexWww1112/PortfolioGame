using UnityEngine;

public class QuestInteractionBridge : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] private float scaleInputStep = 1f;

    private InteractableObject hoveredObject;
    private InteractableObject selectedObject;

    public InteractableObject HoveredObject => hoveredObject;
    public InteractableObject SelectedObject => selectedObject;

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

    public void ScaleSelectedUp()
    {
        ApplyScaleToSelection(scaleInputStep);
    }

    public void ScaleSelectedDown()
    {
        ApplyScaleToSelection(-scaleInputStep);
    }

    public void ApplyScaleToSelection(float scaleDelta)
    {
        if (selectedObject == null)
        {
            return;
        }

        // Keep scaling behavior inside InteractableObject so Quest and non-VR obey the same limits.
        selectedObject.ApplyScaleDelta(scaleDelta);
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
}
