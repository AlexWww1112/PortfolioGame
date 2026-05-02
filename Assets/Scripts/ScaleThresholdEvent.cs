using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ThresholdStateChangedEvent : UnityEvent<bool>
{
}

public class ScaleThresholdEvent : MonoBehaviour
{
    private enum ThresholdValueSource
    {
        ScaleMultiplier,
        LinkedFloatValue
    }

    [Header("Source")]
    [SerializeField] private ThresholdValueSource valueSource = ThresholdValueSource.ScaleMultiplier;
    [SerializeField] private InteractableObject interactableObject;
    [SerializeField] private ScaleLinkedFloatValue linkedFloatValue;

    [Header("Thresholds")]
    [SerializeField] private float enterThreshold = 1f;
    [SerializeField] private float exitThreshold = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent enteredThreshold = new UnityEvent();
    [SerializeField] private UnityEvent exitedThreshold = new UnityEvent();
    [SerializeField] private ThresholdStateChangedEvent thresholdStateChanged = new ThresholdStateChangedEvent();

    private bool initialized;

    public float CurrentValue { get; private set; }
    public bool IsInsideThreshold { get; private set; }

    private void Awake()
    {
        if (exitThreshold > enterThreshold)
        {
            Debug.LogError($"{nameof(ScaleThresholdEvent)} needs exitThreshold to be less than or equal to enterThreshold.", this);
            enabled = false;
            return;
        }

        switch (valueSource)
        {
            case ThresholdValueSource.ScaleMultiplier:
                if (interactableObject == null)
                {
                    Debug.LogError($"{nameof(ScaleThresholdEvent)} needs an {nameof(InteractableObject)} reference.", this);
                    enabled = false;
                    return;
                }
                break;

            case ThresholdValueSource.LinkedFloatValue:
                if (linkedFloatValue == null)
                {
                    Debug.LogError($"{nameof(ScaleThresholdEvent)} needs a {nameof(ScaleLinkedFloatValue)} reference.", this);
                    enabled = false;
                    return;
                }
                break;
        }

        CurrentValue = ReadCurrentValue();
        IsInsideThreshold = CurrentValue >= enterThreshold;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        float newValue = ReadCurrentValue();

        if (Mathf.Approximately(CurrentValue, newValue))
        {
            return;
        }

        CurrentValue = newValue;
        EvaluateThresholdState();
    }

    public void EvaluateNow()
    {
        if (!initialized)
        {
            return;
        }

        CurrentValue = ReadCurrentValue();
        EvaluateThresholdState();
    }

    private float ReadCurrentValue()
    {
        switch (valueSource)
        {
            case ThresholdValueSource.LinkedFloatValue:
                return linkedFloatValue.CurrentValue;

            default:
                return interactableObject.ScaleMultiplier;
        }
    }

    private void EvaluateThresholdState()
    {
        if (!IsInsideThreshold && CurrentValue >= enterThreshold)
        {
            IsInsideThreshold = true;
            enteredThreshold.Invoke();
            thresholdStateChanged.Invoke(true);
            return;
        }

        if (IsInsideThreshold && CurrentValue <= exitThreshold)
        {
            IsInsideThreshold = false;
            exitedThreshold.Invoke();
            thresholdStateChanged.Invoke(false);
        }
    }
}
