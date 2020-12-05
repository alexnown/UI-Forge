using UnityEngine;
using UnityEngine.UI;

namespace alexnown
{
    [RequireComponent(typeof(Slider))]
    public class UiSliderAdapter : MonoBehaviour, ISlider
    {
        [SerializeField]
        private Slider _slider;

        public Slider.SliderEvent ValueChanged { get => _slider.onValueChanged; set => _slider.onValueChanged = value; }
        public float Value { get => _slider.value; set => _slider.value = value; }
        public float MinValue { get => _slider.minValue; set => _slider.minValue = value; }
        public float MaxValue { get => _slider.maxValue; set => _slider.maxValue = value; }

        private void OnValidate()
        {
            if (Application.isPlaying || _slider != null) return;
            _slider = GetComponent<Slider>();
        }
    }
}
