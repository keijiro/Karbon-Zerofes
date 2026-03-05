using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Karbon {

public sealed class CompositeRendererFeature : ScriptableRendererFeature
{
    [SerializeField]
    RenderPassEvent _passEvent = RenderPassEvent.AfterRenderingPostProcessing;

    CompositePass _pass;

    public override void Create()
      => _pass = new CompositePass { renderPassEvent = _passEvent };

    public override void AddRenderPasses
      (ScriptableRenderer renderer, ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);
}

} // namespace Karbon
