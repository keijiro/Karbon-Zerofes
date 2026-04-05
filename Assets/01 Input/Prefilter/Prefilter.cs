using UnityEngine;
using BodyPix;

namespace Karbon {

public sealed class Prefilter : MonoBehaviour
{
    [SerializeField] RenderTexture _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] CustomRenderTexture _target = null;

    BodyDetector _detector;

    void Start()
      => _detector = new BodyDetector(_resources, 512, 384);

    void OnDestroy()
      => _detector.Dispose();

    void LateUpdate()
    {
        _detector.ProcessImage(_source);
        Shader.SetGlobalTexture(ShaderID.MainTex, _source);
        Shader.SetGlobalTexture(ShaderID.BodyPixTex, _detector.MaskTexture);
        _target.Update();
    }
}

} // namespace Karbon
