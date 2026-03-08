using UnityEngine;
using UnityEngine.Rendering;
using Klak.Math;

namespace Karbon {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("URP Camera Effect/Overlay FX")]
public sealed class OverlayFXController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)] public float Slits { get; set; } = 0.5f;
    [field:SerializeField, Range(0, 1)] public float Blots { get; set; } = 0.5f;
    [field:SerializeField, Min(0.1f)] public float WiperSpeed { get; set; } = 1;

    [SerializeField, HideInInspector] Shader _shader = null;

    public bool IsActive => Slits > 0 || Blots > 0 || IsWiperRunning;

    Material _material;
    (int target, float current) _wiper;

    bool IsWiperRunning
      => _wiper.current < _wiper.target || (_wiper.target & 1) != 0;

    public Material UpdateMaterial()
    {
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        var fxParam = new Vector3(Slits, Blots, _wiper.current % 2);
        var seed = new XXHash((uint)_wiper.current).Int(1u);

        _material.SetVector("_FxParam", fxParam);
        _material.SetInteger("_FxSeed", seed);

        CoreUtils.SetKeyword(_material, "OVERLAYFX_SLITS", Slits > 0);
        CoreUtils.SetKeyword(_material, "OVERLAYFX_BLOTS", Blots > 0);
        CoreUtils.SetKeyword(_material, "OVERLAYFX_WIPER", IsWiperRunning);

        return _material;
    }

    public void StartWiper()
      => _wiper.target = Mathf.Min(_wiper.target + 1, (int)_wiper.current + 2);

    void UpdateWiperState()
    {
        _wiper.current += Time.deltaTime * WiperSpeed;
        _wiper.current = Mathf.Min(_wiper.current, _wiper.target);
    }

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        _material = null;
    }

    void Update() => UpdateWiperState();
    void OnDestroy() => ReleaseResources();
    void OnDisable() => ReleaseResources();
}

} // namespace Karbon
