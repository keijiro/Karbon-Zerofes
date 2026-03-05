using UnityEngine;

namespace Karbon {

static class ShaderID
{
    public static readonly int BodyPixTex = Shader.PropertyToID("_BodyPixTex");
    public static readonly int MainTex = Shader.PropertyToID("_MainTex");
    public static readonly int BGPlane = Shader.PropertyToID("_BGPlane");
    public static readonly int FGPlane1 = Shader.PropertyToID("_FGPlane1");
    public static readonly int FGPlane2 = Shader.PropertyToID("_FGPlane2");
    public static readonly int OverlayPlane = Shader.PropertyToID("_OverlayPlane");
    public static readonly int BGColor = Shader.PropertyToID("_BGColor");
    public static readonly int FGColor = Shader.PropertyToID("_FGColor");
}

} // namespace Karbon
