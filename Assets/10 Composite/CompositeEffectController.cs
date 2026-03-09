using UnityEngine;
using UnityEngine.InputSystem;
using KinoGlitch;

public sealed class CompositeEffectController : MonoBehaviour
{
    [SerializeField] InputAction _glitch1Action = null;
    [SerializeField] InputAction _glitch2Action = null;
    [SerializeField] InputAction _glitch3Action = null;
    [SerializeField] InputAction _glitch4Action = null;

    AnalogGlitchController _glitch;

    void Start()
    {
        _glitch = GetComponent<AnalogGlitchController>();

        _glitch1Action.Enable();
        _glitch2Action.Enable();
        _glitch3Action.Enable();
        _glitch4Action.Enable();
    }

    void Update()
    {
        _glitch.ScanLineJitter   = _glitch1Action.ReadValue<float>();
        _glitch.VerticalJump     = _glitch2Action.ReadValue<float>();
        _glitch.HorizontalShake  = _glitch3Action.ReadValue<float>();
        _glitch.HorizontalRipple = _glitch4Action.ReadValue<float>();
    }
}
