using UnityEngine;

namespace Karbon {

public sealed class TransformJitter : MonoBehaviour
{
    [field:SerializeField] public Vector3 PositionJitter { get; set; }
    [field:SerializeField] public Vector3 RotationJitter { get; set; }
    [field:SerializeField, Min(0)] public float ScaleJitter { get; set; }

    Vector3 _basePosition;
    Quaternion _baseRotation;
    Vector3 _baseScale;

    void OnEnable()
    {
        _basePosition = transform.localPosition;
        _baseRotation = transform.localRotation;
        _baseScale = transform.localScale;
    }

    void OnDisable()
    {
        transform.localPosition = _basePosition;
        transform.localRotation = _baseRotation;
        transform.localScale = _baseScale;
    }

    void LateUpdate()
    {
        transform.localPosition = _basePosition + RandomOffset(PositionJitter);
        transform.localRotation = _baseRotation * Quaternion.Euler(RandomOffset(RotationJitter));
        transform.localScale = _baseScale * (1 + Random.Range(-ScaleJitter, ScaleJitter));
    }

    static Vector3 RandomOffset(Vector3 amount)
      => new(Random.Range(-amount.x, amount.x),
             Random.Range(-amount.y, amount.y),
             Random.Range(-amount.z, amount.z));
}

} // namespace Karbon
