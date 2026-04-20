using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private float minScaleMultiplier = 0.5f;
    [SerializeField] private float maxScaleMultiplier = 2f;
    [SerializeField] private float scaleChangePerScroll = 0.1f;
    [SerializeField] private UnityEvent<bool> selectionChanged = new UnityEvent<bool>();
    [SerializeField] private UnityEvent<float> scaleChanged = new UnityEvent<float>();

    private Vector3 initialScale;
    private float scaleMultiplier = 1f;

    public bool IsSelected { get; private set; }
    public float ScaleMultiplier => scaleMultiplier;

    private void Awake()
    {
        // Store the authored scale so multiplier changes remain relative to the scene setup.
        initialScale = transform.localScale;
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
        transform.localScale = initialScale * scaleMultiplier;
    }
}
