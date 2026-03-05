using UnityEngine;
using UnityEngine.Rendering;

namespace Karbon {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Karbon/Composite")]
public sealed class CompositeController : MonoBehaviour
{
    [field:SerializeField] public Texture BGPlane { get; set; }
    [field:SerializeField] public Texture FGPlane1 { get; set; }
    [field:SerializeField] public Texture FGPlane2 { get; set; }
    [field:SerializeField] public Texture OverlayPlane { get; set; }
    [field:SerializeField] public Color BGColor { get; set; } = Color.white;
    [field:SerializeField] public Color FGColor { get; set; } = Color.white;

    [SerializeField, HideInInspector] Shader _shader;

    public bool IsActive
      => BGPlane != null || FGPlane1 != null || FGPlane2 != null || OverlayPlane != null;

    Material _material;

    public Material UpdateMaterial()
    {
        if (_shader == null) _shader = Shader.Find("Hidden/Karbon/Composite");
        if (_shader == null) return null;

        if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

        _material.SetTexture(ShaderID.BGPlane, BGPlane);
        _material.SetTexture(ShaderID.FGPlane1, FGPlane1);
        _material.SetTexture(ShaderID.FGPlane2, FGPlane2);
        _material.SetTexture(ShaderID.OverlayPlane, OverlayPlane);
        _material.SetColor(ShaderID.BGColor, BGColor);
        _material.SetColor(ShaderID.FGColor, FGColor);

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

} // namespace Karbon
