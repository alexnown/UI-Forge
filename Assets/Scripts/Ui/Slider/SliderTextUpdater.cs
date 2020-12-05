using System;
using UnityEngine;
using UnityEngine.UI;

namespace alexnown
{
    public enum TextFormat
    {
        Default,
        WholeNumber,
        F1,
        F2,
        F3,
        Percent,
        Fraction
    }

    [ExecuteInEditMode]
    public class SliderTextUpdater : MonoBehaviour
    {
        [SerializeField]
        private TextFormat _format;
        private ISlider _targetSlider;

        public TextFormat Format
        {
            get => _format;
            set
            {
                _format = value;
                UpdateSliderText(_targetSlider.Value);
            }
        }
        [SerializeField]
        private Text _text = null;
        [SerializeField]
        private GameObject _sliderGo = null;

        private void OnEnable()
        {
#if UNITY_EDITOR
            OnValidate();
#endif
            _targetSlider = _sliderGo.GetComponent<ISlider>();
            _targetSlider.ValueChanged.AddListener(UpdateSliderText);
            UpdateSliderText(_targetSlider.Value);
        }

        private void OnDisable()
        {
            _targetSlider.ValueChanged.RemoveListener(UpdateSliderText);
        }

        private void UpdateSliderText(float value) => _text.text = FormatSliderValue(value);

        private string FormatSliderValue(float value)
        {
            switch (Format)
            {
                case TextFormat.WholeNumber:
                    return ((int)value).ToString();
                case TextFormat.F1:
                    return value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
                case TextFormat.F2:
                    return value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                case TextFormat.F3:
                    return value.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
                case TextFormat.Percent:
                    return $"{Math.Round(100 * value / _targetSlider.MaxValue)}%";
                case TextFormat.Fraction:
                    return $"{(int)value}/{(int)_targetSlider.MaxValue}";
                default:
                    return value.ToString();
            }
        }

#if UNITY_EDITOR
        private void Awake()
        {
            if (Application.isPlaying) return;
            if (_text == null) _text = GetComponentInChildren<Text>();
            if (_sliderGo == null)
            {
                Component slider = GetComponentInChildren<ISlider>() as Component;
                if (slider != null) _sliderGo = slider.gameObject;
                else
                {
                    slider = GetComponentInChildren<Slider>();
                    if (slider != null) _sliderGo = slider.gameObject;
                }
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (_sliderGo != null)
            {
                _targetSlider = UiHelper.GetSlider(_sliderGo);
                if (_targetSlider == null)
                {
                    _sliderGo = null;
                    Debug.LogError("SliderGo requires UnityEngine.UI.Slider component or component that implements ISlider interface", this);
                }
            }
            if (_targetSlider != null && _text != null) UpdateSliderText(_targetSlider.Value);
        }
#endif
    }
}
