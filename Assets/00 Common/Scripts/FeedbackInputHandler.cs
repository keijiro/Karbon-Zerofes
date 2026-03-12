using UnityEngine;
using UnityEngine.InputSystem;
using Kino.Feedback.Universal;

namespace Karbon {

public sealed class FeedbackInputHandler : MonoBehaviour
{
    [field:SerializeField] public float BaseScale { get; set; } = 1.0f;
    [field:SerializeField] public float ScaleAmount { get; set; } = 0.03f;
    [field:SerializeField] public float RotationAmount { get; set; } = 15.0f;
    [field:SerializeField] public float OffsetAmount { get; set; } = 0.03f;
    [field:SerializeField] public float HueShiftAmount { get; set; } = 1.0f;

    [field:Space, SerializeField] public InputAction ExpandButton { get; set; } = null;
    [field:SerializeField] public InputAction ShrinkButton { get; set; } = null;
    [field:SerializeField] public InputAction HueShiftButton { get; set; } = null;
    [field:SerializeField] public InputAction RotateLeftButton { get; set; } = null;
    [field:SerializeField] public InputAction RotateRightButton { get; set; } = null;
    [field:SerializeField] public InputAction MoveUpButton { get; set; } = null;
    [field:SerializeField] public InputAction MoveDownButton { get; set; } = null;
    [field:SerializeField] public InputAction MoveLeftButton { get; set; } = null;
    [field:SerializeField] public InputAction MoveRightButton { get; set; } = null;

    [Space, SerializeField] FeedbackController _target = null;

    void OnEnable()
    {
        ExpandButton.Enable();
        ShrinkButton.Enable();
        HueShiftButton.Enable();
        RotateLeftButton.Enable();
        RotateRightButton.Enable();
        MoveUpButton.Enable();
        MoveDownButton.Enable();
        MoveLeftButton.Enable();
        MoveRightButton.Enable();
    }

    void OnDisable()
    {
        ExpandButton.Disable();
        ShrinkButton.Disable();
        HueShiftButton.Disable();
        RotateLeftButton.Disable();
        RotateRightButton.Disable();
        MoveUpButton.Disable();
        MoveDownButton.Disable();
        MoveLeftButton.Disable();
        MoveRightButton.Disable();
    }

    void Update()
    {
        if (_target == null) return;

        var scale = BaseScale;
        scale += ExpandButton.ReadValue<float>() * ScaleAmount;
        scale -= ShrinkButton.ReadValue<float>() * ScaleAmount;
        var hueShift = HueShiftButton.ReadValue<float>() * HueShiftAmount;

        var rotation = 0.0f;
        rotation += RotateLeftButton.ReadValue<float>() * RotationAmount;
        rotation -= RotateRightButton.ReadValue<float>() * RotationAmount;

        var offset = Vector2.zero;
        offset += Vector2.up * MoveUpButton.ReadValue<float>() * OffsetAmount;
        offset += Vector2.down * MoveDownButton.ReadValue<float>() * OffsetAmount;
        offset += Vector2.left * MoveLeftButton.ReadValue<float>() * OffsetAmount;
        offset += Vector2.right * MoveRightButton.ReadValue<float>() * OffsetAmount;

        _target.Scale = Mathf.Max(0.001f, scale);
        _target.HueShift = Mathf.Clamp(hueShift, -1, 1);
        _target.Rotation = rotation;
        _target.Offset = offset;
    }
}

} // namespace Karbon
