using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Karbon {

[CustomEditor(typeof(OverlayFXController)), CanEditMultipleObjects]
sealed class OverlayFXControllerEditor : Editor
{
    [SerializeField] VisualTreeAsset _uxml = null;

    public override VisualElement CreateInspectorGUI()
    {
        var ui = _uxml.CloneTree();
        ui.Q<Button>("start-wiper-button").clicked += OnStartWiperButton;
        return ui;
    }

    void OnStartWiperButton()
    {
        foreach (var targetObject in targets)
        {
            var controller = (OverlayFXController)targetObject;
            controller.StartWiper();
        }
    }
}

} // namespace Karbon
