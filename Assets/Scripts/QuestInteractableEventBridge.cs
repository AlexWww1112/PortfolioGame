using Oculus.Interaction;
using UnityEngine;

public class QuestInteractableEventBridge : MonoBehaviour
{
    [SerializeField] private QuestInteractionBridge interactionBridge;
    [SerializeField] private InteractableObject interactableObject;
    [SerializeField] private PointableElement pointableElement;

    private void Awake()
    {
        if (interactionBridge == null || interactableObject == null || pointableElement == null)
        {
            Debug.LogError($"{nameof(QuestInteractableEventBridge)} needs a QuestInteractionBridge, InteractableObject, and PointableElement assigned.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        pointableElement.WhenPointerEventRaised += HandlePointerEventRaised;
    }

    private void OnDisable()
    {
        if (pointableElement == null)
        {
            return;
        }

        pointableElement.WhenPointerEventRaised -= HandlePointerEventRaised;
    }

    private void HandlePointerEventRaised(PointerEvent pointerEvent)
    {
        switch (pointerEvent.Type)
        {
            case PointerEventType.Hover:
                interactionBridge.NotifyHoverEntered(interactableObject);
                break;
            case PointerEventType.Unhover:
                interactionBridge.NotifyHoverExited(interactableObject);
                break;
            case PointerEventType.Select:
                interactionBridge.NotifySelectEntered(interactableObject);
                break;
            case PointerEventType.Unselect:
            case PointerEventType.Cancel:
                interactionBridge.NotifySelectExited(interactableObject);
                interactionBridge.NotifyHoverExited(interactableObject);
                break;
        }
    }
}
