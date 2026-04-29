using Oculus.Interaction;
using UnityEngine;

public class QuestInteractableEventBridge : MonoBehaviour
{
    [SerializeField] private QuestInteractionBridge interactionBridge;
    [SerializeField] private InteractableObject interactableObject;

    private void Awake()
    {
        if (interactionBridge == null || interactableObject == null)
        {
            Debug.LogError($"{nameof(QuestInteractableEventBridge)} needs a QuestInteractionBridge and InteractableObject assigned.", this);
            enabled = false;
        }
    }

    public void NotifyHoverEntered()
    {
        interactionBridge.NotifyHoverEntered(interactableObject);
    }

    public void NotifyHoverEntered(PointerEvent pointerEvent)
    {
        NotifyHoverEntered();
    }

    public void NotifyHoverExited()
    {
        interactionBridge.NotifyHoverExited(interactableObject);
    }

    public void NotifyHoverExited(PointerEvent pointerEvent)
    {
        NotifyHoverExited();
    }

    public void NotifySelectEntered()
    {
        // Meta wrappers can call this from hand grab, distance grab, or ray selection events.
        interactionBridge.NotifySelectEntered(interactableObject);
    }

    public void NotifySelectEntered(PointerEvent pointerEvent)
    {
        NotifySelectEntered();
    }

    public void NotifySelectExited()
    {
        interactionBridge.NotifySelectExited(interactableObject);
    }

    public void NotifySelectExited(PointerEvent pointerEvent)
    {
        NotifySelectExited();
    }
}
