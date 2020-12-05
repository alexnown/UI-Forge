using UnityEngine;
using UnityEngine.UI;
namespace alexnown
{
    public class Bottle : MonoBehaviour
    {
        private readonly static int _hOffsetId = Shader.PropertyToID("_HueOffset");
        private readonly static int _sOffsetId = Shader.PropertyToID("_SaturationOffset");
        private readonly static int _vOffsetId = Shader.PropertyToID("_BrightnessOffset");

        [SerializeField]
        private Color32 _startColor = Color.red;
        [SerializeField]
        private Image _glowImg = null;
        [SerializeField]
        private Image _substanceImg = null;
        [SerializeField]
        private HsvColorPicker _colorPicker = null;

        private void Start()
        {
            _substanceImg.material = Instantiate(_substanceImg.material);
            _glowImg.material = Instantiate(_glowImg.material);
            _colorPicker.SetColor(_startColor);
            UpdateMaterialValues(_colorPicker.H, _colorPicker.S, _colorPicker.V);
            _colorPicker.ColorChanged += (color) =>
            {
                _startColor = color;
                UpdateMaterialValues(_colorPicker.H, _colorPicker.S, _colorPicker.V);
            };
        }

        private void UpdateMaterialValues(float h, float s, float v)
        {
            var glowMat = _glowImg.material;
            glowMat.SetFloat(_hOffsetId, h);
            glowMat.SetFloat(_sOffsetId, s - 1);
            glowMat.SetFloat(_vOffsetId, v - 1);
            var substanceMat = _substanceImg.material;
            substanceMat.SetFloat(_hOffsetId, h);
            substanceMat.SetFloat(_sOffsetId, s - 1);
            substanceMat.SetFloat(_vOffsetId, v - 1);
        }
    }
}
