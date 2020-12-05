using UnityEngine.UI;

namespace alexnown
{
    public interface ISlider
    {
        Slider.SliderEvent ValueChanged { get; set; }
        float Value { get; set; }
        float MinValue { get; set; }
        float MaxValue { get; set; }
    }
}
