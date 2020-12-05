using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace alexnown
{
    public class HsvColorPicker : MonoBehaviour
    {
        public event Action<Color32> ColorChanged;

        [SerializeField]
        private GameObject _hSliderGo = null;
        [SerializeField]
        private GameObject _sSliderGo = null;
        [SerializeField]
        private GameObject _vSliderGo = null;

        [Header("Preview")]
        [SerializeField]
        private Image _colorPreview = null;
        [SerializeField]
        private InputField _colorCodeFierd = null;

        [SerializeField]
        private RawImage _sSliderBackground = null;
        [SerializeField]
        private RawImage _vSliderBackground = null;

        private float _hValue = 1;
        private float _sValue = 1;
        private float _vValue = 1;
        private ISlider _hSlider;
        private ISlider _sSlider;
        private ISlider _vSlider;

        private readonly int _hShaderId = Shader.PropertyToID("_hValue");
        private readonly int _sShaderId = Shader.PropertyToID("_sValue");
        private readonly int _vShaderId = Shader.PropertyToID("_vValue");

        public float H
        {
            get { return _hValue; }
            set
            {
                _hValue = Mathf.Clamp01(value);
                SetHValueToSliderShaders(_hValue);
                UpdateColorByHsvValues();
            }
        }

        public float S
        {
            get { return _sValue; }
            set
            {
                _sValue = Mathf.Clamp01(value);
                UpdateColorByHsvValues();
            }
        }

        public float V
        {
            get { return _vValue; }
            set
            {
                _vValue = Mathf.Clamp01(value);
                UpdateColorByHsvValues();
            }
        }

        public void SetColor(float h, float s, float v)
        {
            _hValue = h;
            _sValue = s;
            _vValue = v;
            CurrentColor = Color.HSVToRGB(h, s, v);
            UpdateViews(CurrentColor, _hValue, _sValue, _vValue);
        }

        public void SetColor(Color32 color)
        {
            color.a = 255;
            CurrentColor = color;
            Color.RGBToHSV(color, out _hValue, out _sValue, out _vValue);
            UpdateViews(CurrentColor, _hValue, _sValue, _vValue);
        }

        public Color32 CurrentColor { get; private set; }

        private void Awake()
        {
            if (_sSliderBackground != null)
                _sSliderBackground.material = Instantiate(_sSliderBackground.material);
            if (_vSliderBackground != null)
                _vSliderBackground.material = Instantiate(_vSliderBackground.material);
        }
        private void OnEnable()
        {
            if (_hSliderGo != null)
            {
                _hSlider = _hSliderGo.GetComponent<ISlider>();
                _hSlider.Value = H;
                _hSlider.ValueChanged.AddListener(SetH);
            }
            if (_sSliderGo != null)
            {
                _sSlider = _sSliderGo.GetComponent<ISlider>();
                _sSlider.Value = S;
                _sSlider.ValueChanged.AddListener(SetS);
            }
            if (_vSliderGo != null)
            {
                _vSlider = _vSliderGo.GetComponent<ISlider>();
                _vSlider.Value = V;
                _vSlider.ValueChanged.AddListener(SetV);
            }
            if (_colorCodeFierd != null)
            {
                _colorCodeFierd.onValidateInput += OnValidateInput;
                _colorCodeFierd.onEndEdit.AddListener(OnColorCodeEndEdit);
            }
        }

        private void OnDisable()
        {
            if (_hSlider != null) _hSlider.ValueChanged.RemoveListener(SetH);
            if (_sSlider != null) _hSlider.ValueChanged.RemoveListener(SetS);
            if (_vSlider != null) _hSlider.ValueChanged.RemoveListener(SetV);
            if (_colorCodeFierd != null)
            {
                _colorCodeFierd.onValidateInput -= OnValidateInput;
                _colorCodeFierd.onEndEdit.RemoveListener(OnColorCodeEndEdit);
            }
        }

        private void SetH(float h) => H = h;
        private void SetS(float s) => S = s;
        private void SetV(float v) => V = v;

        private void OnColorCodeEndEdit(string colorCode)
        {
            try
            {
                if (colorCode.Length != 6) throw new Exception("Color hex length must be equal 6.");
                SetColor(FromHex(colorCode));
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                var msg = string.Format("{0} color code wrong format! {1}", colorCode, e);
                Debug.LogWarning(msg);
#endif
                UpdateColorByHsvValues();
            }
        }

        private string ColorToHex(Color32 color)
        {
            return String.Format("{0}{1}{2}", color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        private Color32 FromHex(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        private char OnValidateInput(string text, int index, char addedChar)
        {
            int charCode = (int)addedChar;
            if (charCode > 0)
            {
                if (charCode < 48) return '0';
                else if (charCode >= 48 && charCode <= 57) return addedChar;
                else if (charCode >= 65 && charCode <= 70) return addedChar;
                else if (charCode >= 97 && charCode <= 102) return addedChar;
                else return 'F';
            }
            return addedChar;
        }

        private void UpdateViews(Color32 color, float h, float s, float v)
        {
            if (_hSlider != null) _hSlider.Value = h;
            if (_sSlider != null) _sSlider.Value = s;
            if (_vSlider != null) _vSlider.Value = v;
            SetHValueToSliderShaders(_hValue);
            if (_colorPreview != null) _colorPreview.color = color;
            if (_colorCodeFierd != null)
            {
                _colorCodeFierd.text = ColorToHex(color);
            }
        }

        private void UpdateColorByHsvValues()
        {
            var color = Color.HSVToRGB(_hValue, _sValue, _vValue);
            if (_colorPreview != null) _colorPreview.color = color;
            if (_colorCodeFierd != null) _colorCodeFierd.text = ColorToHex(color);
            if (_sSliderBackground != null)
                _sSliderBackground.material.SetFloat(_vShaderId, _vValue);
            if (_vSliderBackground != null)
                _vSliderBackground.material.SetFloat(_sShaderId, _sValue);
            CurrentColor = color;
            if (ColorChanged != null) ColorChanged.Invoke(color);
        }


        private void SetHValueToSliderShaders(float h)
        {
            if (_sSliderBackground != null) _sSliderBackground.material.SetFloat(_hShaderId, h);
            if (_vSliderBackground != null) _vSliderBackground.material.SetFloat(_hShaderId, h);
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_hSliderGo != null) ValidateSliderGo(ref _hSliderGo);
            if (_sSliderGo != null) ValidateSliderGo(ref _sSliderGo);
            if (_vSliderGo != null) ValidateSliderGo(ref _vSliderGo);
        }

        private void ValidateSliderGo(ref GameObject go)
        {
            if (UiHelper.GetSlider(go) == null)
            {
                Debug.LogError($"{go.name} requires UnityEngine.UI.Slider component or component that implements ISlider interface", this);
                go = null;
            }
        }
#endif
    }
}
