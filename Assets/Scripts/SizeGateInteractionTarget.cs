using System;
using UnityEngine;
using UnityEngine.Events;

public enum SizeGateInteractionResult
{
    Success,
    MissingObject,
    WrongObject,
    NotHeld,
    TooSmall,
    TooLarge
}

[Serializable]
public class SizeGateInteractionResultEvent : UnityEvent<SizeGateInteractionResult>
{
}

public class SizeGateInteractionTarget : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private InteractableObject requiredInteractable;
    [SerializeField] private bool requireHeld = true;
    [SerializeField] private float minAllowedScale = 1f;
    [SerializeField] private float maxAllowedScale = 1f;
    [SerializeField] private bool disableAfterSuccess = true;

    [Header("Scene Transition")]
    [SerializeField] private string successSceneName = string.Empty;

    [Header("Events")]
    [SerializeField] private UnityEvent onSuccess = new UnityEvent();
    [SerializeField] private SizeGateInteractionResultEvent interactionEvaluated = new SizeGateInteractionResultEvent();

    private Collider triggerCollider;
    private InteractableObject lastEvaluatedInteractable;
    private SizeGateInteractionResult? lastReportedResult;
    private bool interactionCompleted;

    public SizeGateInteractionResult? LastResult => lastReportedResult;
    public bool InteractionCompleted => interactionCompleted;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();

        if (triggerCollider == null || !triggerCollider.isTrigger)
        {
            Debug.LogError($"{nameof(SizeGateInteractionTarget)} needs a trigger collider on the same object.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EvaluateFromCollider(other);
    }

    private void OnTriggerStay(Collider other)
    {
        EvaluateFromCollider(other);
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableObject interactable = other.GetComponentInParent<InteractableObject>();

        if (interactable == null || interactable != lastEvaluatedInteractable)
        {
            return;
        }

        lastEvaluatedInteractable = null;
        lastReportedResult = null;
    }

    public SizeGateInteractionResult TryInteract(InteractableObject interactable)
    {
        if (interactionCompleted)
        {
            return SizeGateInteractionResult.Success;
        }

        if (interactable == null)
        {
            return SizeGateInteractionResult.MissingObject;
        }

        if (requiredInteractable != null && interactable != requiredInteractable)
        {
            return SizeGateInteractionResult.WrongObject;
        }

        if (requireHeld && !interactable.IsHeld)
        {
            return SizeGateInteractionResult.NotHeld;
        }

        if (interactable.ScaleMultiplier < minAllowedScale)
        {
            return SizeGateInteractionResult.TooSmall;
        }

        if (interactable.ScaleMultiplier > maxAllowedScale)
        {
            return SizeGateInteractionResult.TooLarge;
        }

        return SizeGateInteractionResult.Success;
    }

    public void ResetInteractionState()
    {
        interactionCompleted = false;
        lastEvaluatedInteractable = null;
        lastReportedResult = null;
    }

    private void EvaluateFromCollider(Collider other)
    {
        if (interactionCompleted)
        {
            return;
        }

        InteractableObject interactable = other.GetComponentInParent<InteractableObject>();

        if (interactable == null)
        {
            return;
        }

        SizeGateInteractionResult result = TryInteract(interactable);

        // Report only when either the object or the result changes so triggers do not spam feedback every physics step.
        if (interactable == lastEvaluatedInteractable &&
            lastReportedResult.HasValue &&
            lastReportedResult.Value == result)
        {
            return;
        }

        lastEvaluatedInteractable = interactable;
        lastReportedResult = result;
        interactionEvaluated.Invoke(result);

        if (result != SizeGateInteractionResult.Success)
        {
            return;
        }

        onSuccess.Invoke();
        TriggerSceneTransitionIfConfigured();

        if (disableAfterSuccess)
        {
            interactionCompleted = true;
        }
    }

    private void TriggerSceneTransitionIfConfigured()
    {
        if (string.IsNullOrWhiteSpace(successSceneName))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError($"{nameof(SizeGateInteractionTarget)} cannot transition to '{successSceneName}' because no {nameof(GameManager)} instance exists.", this);
            return;
        }

        GameManager.Instance.TransitionToScene(successSceneName);
    }
}
