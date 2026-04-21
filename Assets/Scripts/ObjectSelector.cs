using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float selectDistance = 5f;
    [SerializeField] private LayerMask selectableLayers = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    [Header("Hold")]
    [SerializeField] private float holdDistance = 2f;

    [Header("Crosshair")]
    [SerializeField] private bool showCrosshair = true;
    [SerializeField] private Color defaultCrosshairColor = Color.white;
    [SerializeField] private Color hoverCrosshairColor = Color.yellow;
    [SerializeField] private Color selectedCrosshairColor = Color.green;
    [SerializeField] private float crosshairLength = 8f;
    [SerializeField] private float crosshairGap = 4f;
    [SerializeField] private float crosshairThickness = 2f;

    private InteractableObject hoveredObject;
    private InteractableObject selectedObject;

    private void Awake()
    {
        if (inputManager == null || rayOrigin == null)
        {
            Debug.LogError($"{nameof(ObjectSelector)} needs an InputManager and ray origin assigned.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // The same ray drives hover color, left-click selection, and scroll scaling target choice.
        hoveredObject = selectedObject != null && selectedObject.IsHeld ? null : FindInteractableFromRay();

        if (inputManager.SelectPressed)
        {
            ToggleHeldSelection();
        }

        if (selectedObject != null && selectedObject.IsHeld)
        {
            MoveHeldSelection();
        }

        if (selectedObject != null && !Mathf.Approximately(inputManager.ScrollDelta, 0f))
        {
            selectedObject.ApplyScaleDelta(inputManager.ScrollDelta);
        }
    }

    private void ToggleHeldSelection()
    {
        if (selectedObject != null && selectedObject.IsHeld)
        {
            PlaceSelectedObject();
            return;
        }

        if (hoveredObject != null && hoveredObject.CanBeHeld)
        {
            PickUpObject(hoveredObject);
        }
    }

    private void PickUpObject(InteractableObject interactable)
    {
        SetSelectedObject(interactable);
        selectedObject.SetHeld(true);
    }

    private void PlaceSelectedObject()
    {
        selectedObject.SetHeld(false);
        SetSelectedObject(null);
    }

    private void MoveHeldSelection()
    {
        Vector3 holdPosition = rayOrigin.position + rayOrigin.forward * holdDistance;
        selectedObject.MoveHeld(holdPosition);
    }

    private void OnGUI()
    {
        if (!showCrosshair)
        {
            return;
        }

        Color previousColor = GUI.color;
        GUI.color = GetCrosshairColor();

        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;

        DrawCrosshairLine(centerX - crosshairGap - crosshairLength * 0.5f, centerY, crosshairLength, crosshairThickness);
        DrawCrosshairLine(centerX + crosshairGap + crosshairLength * 0.5f, centerY, crosshairLength, crosshairThickness);
        DrawCrosshairLine(centerX, centerY - crosshairGap - crosshairLength * 0.5f, crosshairThickness, crosshairLength);
        DrawCrosshairLine(centerX, centerY + crosshairGap + crosshairLength * 0.5f, crosshairThickness, crosshairLength);

        GUI.color = previousColor;
    }

    private InteractableObject FindInteractableFromRay()
    {
        // Selection is center-screen based: rayOrigin should usually be the player camera transform.
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin.position, rayOrigin.forward, selectDistance, selectableLayers, triggerInteraction);
        InteractableObject nearestInteractable = null;
        float nearestDistance = float.PositiveInfinity;

        // EveryLayer may hit non-interactable colliders first, so choose the closest interactable hit.
        for (int i = 0; i < hits.Length; i++)
        {
            InteractableObject interactable = hits[i].collider.GetComponentInParent<InteractableObject>();

            if (interactable == null || hits[i].distance >= nearestDistance)
            {
                continue;
            }

            nearestInteractable = interactable;
            nearestDistance = hits[i].distance;
        }

        return nearestInteractable;
    }

    private Color GetCrosshairColor()
    {
        if (selectedObject != null)
        {
            return selectedCrosshairColor;
        }

        return hoveredObject != null ? hoverCrosshairColor : defaultCrosshairColor;
    }

    private static void DrawCrosshairLine(float centerX, float centerY, float width, float height)
    {
        Rect lineRect = new Rect(centerX - width * 0.5f, centerY - height * 0.5f, width, height);
        GUI.DrawTexture(lineRect, Texture2D.whiteTexture);
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
