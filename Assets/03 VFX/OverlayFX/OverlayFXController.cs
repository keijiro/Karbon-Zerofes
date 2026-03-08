using UnityEngine;
using UnityEngine.Rendering;

namespace Karbon {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("URP Camera Effect/Overlay FX")]
public sealed class OverlayFXController : MonoBehaviour
{
    [field:SerializeField] public bool SlitsEnabled { get; set; } = true;
    [field:SerializeField] public bool BlotsEnabled { get; set; } = false;
    [field:SerializeField] public bool WiperEnabled { get; set; } = false;

    [field:SerializeField, Range(0, 1)] public float Slits { get; set; } = 0.5f;
    [field:SerializeField, Range(0, 1)] public float Blots { get; set; } = 0.5f;
    [field:SerializeField] public float WiperSpeed { get; set; } = 1;

    [SerializeField, HideInInspector] Shader _shader = null;

    public bool IsActive => SlitsEnabled || BlotsEnabled || WiperEnabled;

    Material _material;
    float _wiperTime;
    float _wiperTargetTime;
    bool _wiperRunning;

    public Material UpdateMaterial()
    {
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        UpdateWiperState();

        var fxParam = new Vector3(Slits, Blots, Mathf.Repeat(_wiperTime, 2));

        _material.SetVector("_FxParam", fxParam);
        _material.SetFloat("_FxSeed", _wiperTime);

        CoreUtils.SetKeyword(_material, "OVERLAYFX_SLITS", SlitsEnabled);
        CoreUtils.SetKeyword(_material, "OVERLAYFX_BLOTS", BlotsEnabled);
        CoreUtils.SetKeyword(_material, "OVERLAYFX_WIPER", WiperEnabled);

        return _material;
    }

    public void StartWiper()
    {
        var basePhase = Mathf.Floor(_wiperTime);
        _wiperTime = basePhase;
        _wiperTargetTime = basePhase + 1;
        _wiperRunning = true;
    }

    void UpdateWiperState()
    {
        if (!_wiperRunning) return;

        _wiperTime += Mathf.Max(0, WiperSpeed) * Time.deltaTime;
        if (_wiperTime < _wiperTargetTime) return;

        _wiperTime = _wiperTargetTime;
        _wiperRunning = false;
    }

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material); _material = null;
    }

    void OnDestroy() => ReleaseResources();
    void OnDisable() => ReleaseResources();
}

} // namespace Karbon
