using UnityEngine;
using UnityEngine.Rendering;

namespace URPCameraEffect {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("URP Camera Effect/Overlay FX")]
public sealed class OverlayFXController : MonoBehaviour
{
    public enum FXMode { Slits, Blots, Wiper }

    [field:SerializeField, Range(0, 1)] public float Intensity { get; set; } = 0.5f;
    [field:SerializeField] public FXMode Mode { get; set; } = FXMode.Slits;

    [SerializeField, HideInInspector] Shader _shader = null;

    public bool IsActive => Intensity > 0;
    public int PassIndex => (int)Mode;

    Material _material;

    public Material UpdateMaterial()
    {
        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);
        _material.SetFloat("_FxParam", Intensity);
        return _material;
    }

    void ReleaseResources()
    {
        CoreUtils.Destroy(_material);
        _material = null;
    }

    void OnDestroy() => ReleaseResources();
    void OnDisable() => ReleaseResources();
}

} // namespace URPCameraEffect
