using UnityEngine;
using UnityEngine.Events;

namespace Karbon {

public sealed class ValueTriggerCombiner : MonoBehaviour
{
    [field:SerializeField] public float ReleaseTime { get; set; } = 0.2f;

    [Space, SerializeField] UnityEvent<float> _valueTarget = null;

    public float Value { get; set; }

    public void Trigger(float strength)
      => _triggerValue = strength;

    float _triggerValue;

    void Update()
    {
        _valueTarget?.Invoke(Value + _triggerValue);
        _triggerValue = Mathf.Max(0, _triggerValue - Time.deltaTime / ReleaseTime);
    }
}

} // namespace Karbon
