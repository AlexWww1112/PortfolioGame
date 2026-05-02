using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ScaleLinkedFloatEvent : UnityEvent<float>
{
}

public class ScaleLinkedFloatValue : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private InteractableObject interactableObject;

    [Header("Input Range")]
    [SerializeField] private float minInputScale = 0.5f;
    [SerializeField] private float maxInputScale = 2f;

    [Header("Output Range")]
    [SerializeField] private float minOutputValue = 0f;
    [SerializeField] private float maxOutputValue = 1f;

    [Header("Mapping")]
    [SerializeField] private AnimationCurve responseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Events")]
    [SerializeField] private ScaleLinkedFloatEvent valueChanged = new ScaleLinkedFloatEvent();

    private float lastScaleMultiplier = float.NaN;

    public float CurrentValue { get; private set; }
    public InteractableObject SourceObject => interactableObject;

    private void Awake()
    {
        if (interactableObject == null)
        {
            Debug.LogError($"{nameof(ScaleLinkedFloatValue)} needs an {nameof(InteractableObject)} reference.", this);
            enabled = false;
            return;
        }

        if (Mathf.Approximately(minInputScale, maxInputScale))
        {
            Debug.LogError($"{nameof(ScaleLinkedFloatValue)} needs different min/max input scale values.", this);
            enabled = false;
            return;
        }

        Recalculate(true);
    }

    private void Update()
    {
        if (interactableObject == null)
        {
            return;
        }

        if (Mathf.Approximately(lastScaleMultiplier, interactableObject.ScaleMultiplier))
        {
            return;
        }

        Recalculate(false);
    }

    public void Recalculate()
    {
        Recalculate(false);
    }

    private void Recalculate(bool forceInvoke)
    {
        lastScaleMultiplier = interactableObject.ScaleMultiplier;

        // Normalize scale first, then let the curve shape the response before mapping to the output range.
        float normalizedScale = Mathf.InverseLerp(minInputScale, maxInputScale, lastScaleMultiplier);
        float curvedValue = responseCurve.Evaluate(normalizedScale);
        float mappedValue = Mathf.Lerp(minOutputValue, maxOutputValue, curvedValue);

        if (!forceInvoke && Mathf.Approximately(CurrentValue, mappedValue))
        {
            return;
        }

        CurrentValue = mappedValue;
        valueChanged.Invoke(CurrentValue);
    }
}
