using Klak.Hap;
using UnityEngine;
using UnityEngine.InputSystem;
using StrobePages;

namespace Karbon {

public sealed class FlipbookController : MonoBehaviour
{
    [SerializeField] string[] _videoFiles = null;
    [SerializeField] MeshRenderer _cameraQuad = null;
    [SerializeField] MeshRenderer _videoQuad = null;
    [SerializeField] StrobePagesController _target = null;
    [SerializeField, Min(0.01f)] float _interval = 0.1f;

    HapPlayer _player;
    int _current;

    async void Start()
    {
        while (true)
        {
            _target.StartPageTurn();
            await Awaitable.WaitForSecondsAsync(_interval);
            if (_player != null) AdvancePlayer();
            await Awaitable.NextFrameAsync();
        }
    }

    void Update()
    {
        var num = GetInputNumber();
        if (num < 0 || num == _current) return;

        if (_player != null)
        {
            Destroy(_player);
            _player = null;
        }

        if (num == 0)
        {
            _cameraQuad.enabled = true;
            _videoQuad.enabled = false;
        }
        else
        {
            BuildPlayer(num - 1);
            _cameraQuad.enabled = false;
            _videoQuad.enabled = true;
        }

        _current = num;
    }

    void BuildPlayer(int videoIndex)
    {
        var filePath = _videoFiles[videoIndex % _videoFiles.Length];
        _player = gameObject.AddComponent<HapPlayer>();
        _player.targetRenderer = _videoQuad;
        _player.targetMaterialProperty = "_BaseMap";
        _player.Open(filePath, HapPlayer.PathMode.StreamingAssets);
    }

    void AdvancePlayer()
    {
        if (!_player.isValid || _player.frameCount <= 0) return;
        var length = (float)_player.streamDuration;
        var dt = length / _player.frameCount;
        _player.time = (_player.time + dt) % length;
    }

    static int GetInputNumber()
    {
        var keyboard = Keyboard.current;
        if (keyboard.digit1Key.wasPressedThisFrame) return 0;
        if (keyboard.digit2Key.wasPressedThisFrame) return 1;
        if (keyboard.digit3Key.wasPressedThisFrame) return 2;
        if (keyboard.digit4Key.wasPressedThisFrame) return 3;
        if (keyboard.digit5Key.wasPressedThisFrame) return 4;
        if (keyboard.digit6Key.wasPressedThisFrame) return 5;
        if (keyboard.digit7Key.wasPressedThisFrame) return 6;
        if (keyboard.digit8Key.wasPressedThisFrame) return 7;
        if (keyboard.digit9Key.wasPressedThisFrame) return 8;
        return -1;
    }
}

} // namespace Karbon
