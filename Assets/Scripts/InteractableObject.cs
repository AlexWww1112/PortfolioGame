using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

public class InteractableObject : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableScaleDebugLogs = false;

    [Header("Hold")]
    [SerializeField] private bool canBeHeld = true;
    [SerializeField] private Rigidbody attachedRigidbody;

    [Header("Scale")]
    [SerializeField] private Grabbable scaleSourceGrabbable;
    [SerializeField] private Transform scaleTarget;
    [SerializeField] private float minScaleMultiplier = 0.5f;
    [SerializeField] private float maxScaleMultiplier = 2f;
    [SerializeField] private float scaleChangePerScroll = 0.1f;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool> selectionChanged = new UnityEvent<bool>();
    [SerializeField] private UnityEvent<bool> heldChanged = new UnityEvent<bool>();
    [SerializeField] private UnityEvent<float> scaleChanged = new UnityEvent<float>();

    private Vector3 initialScale;
    private Vector3 expectedScale;
    private float scaleMultiplier = 1f;
    private bool wasKinematicBeforeHeld;

    public bool IsSelected { get; private set; }
    public bool IsHeld { get; private set; }
    public bool CanBeHeld => canBeHeld;
    public float ScaleMultiplier => scaleMultiplier;

    private void Awake()
    {
        if (scaleSourceGrabbable == null)
        {
            TryGetComponent(out scaleSourceGrabbable);
        }

        if (scaleTarget == null && scaleSourceGrabbable != null && scaleSourceGrabbable.Transform != null)
        {
            // Meta grab setups can move a target transform different from the bridge object itself.
            scaleTarget = scaleSourceGrabbable.Transform;
        }

        if (scaleTarget == null)
        {
            scaleTarget = transform;
        }

        // Store the authored scale so multiplier changes remain relative to the scene setup.
        initialScale = scaleTarget.localScale;
        scaleMultiplier = Mathf.Clamp(scaleMultiplier, minScaleMultiplier, maxScaleMultiplier);
        ApplyScale();
    }

    public void SetSelected(bool selected)
    {
        if (IsSelected == selected)
        {
            return;
        }

        IsSelected = selected;
        selectionChanged.Invoke(IsSelected);
    }

    public void SetHeld(bool held)
    {
        if (!canBeHeld || IsHeld == held)
        {
            return;
        }

        IsHeld = held;
        UpdateRigidbodyHeldState();
        heldChanged.Invoke(IsHeld);
    }

    public void MoveHeld(Vector3 targetPosition)
    {
        if (!IsHeld)
        {
            return;
        }

        // Move the authored object directly; interaction placement is controlled by ObjectSelector.
        if (attachedRigidbody != null)
        {
            attachedRigidbody.MovePosition(targetPosition);
            return;
        }

        transform.position = targetPosition;
    }

    public void ApplyScaleDelta(float scrollDelta)
    {
        SetScaleMultiplier(scaleMultiplier + scrollDelta * scaleChangePerScroll);
    }

    public void SetScaleMultiplier(float newScaleMultiplier)
    {
        // Clamp in the shared setter so direct calls and scroll changes obey the same limits.
        float clampedScale = Mathf.Clamp(newScaleMultiplier, minScaleMultiplier, maxScaleMultiplier);

        if (Mathf.Approximately(scaleMultiplier, clampedScale))
        {
            return;
        }

        scaleMultiplier = clampedScale;
        ApplyScale();
        scaleChanged.Invoke(scaleMultiplier);
    }

    private void ApplyScale()
    {
        expectedScale = initialScale * scaleMultiplier;
        scaleTarget.localScale = expectedScale;

        if (enableScaleDebugLogs)
        {
            Debug.Log(
                $"[{nameof(InteractableObject)}] Applied scale to {scaleTarget.name}. " +
                $"Multiplier: {scaleMultiplier}, LocalScale: {scaleTarget.localScale}",
                this);
        }
    }

    private void LateUpdate()
    {
        if (scaleTarget == null)
        {
            return;
        }

        if (scaleTarget.localScale != expectedScale)
        {
            Vector3 actualScale = scaleTarget.localScale;
            scaleTarget.localScale = expectedScale;

            if (enableScaleDebugLogs)
            {
                Debug.LogWarning(
                    $"[{nameof(InteractableObject)}] Scale target {scaleTarget.name} was overridden after ApplyScale. " +
                    $"Expected: {expectedScale}, Actual: {actualScale}. Reapplied expected scale.",
                    this);
            }
        }
    }

    private void UpdateRigidbodyHeldState()
    {
        if (attachedRigidbody == null)
        {
            return;
        }

        if (IsHeld)
        {
            wasKinematicBeforeHeld = attachedRigidbody.isKinematic;
            attachedRigidbody.isKinematic = true;
            return;
        }

        attachedRigidbody.isKinematic = wasKinematicBeforeHeld;
    }
}
