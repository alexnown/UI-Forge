using UnityEngine;
using UnityEngine.UI;

namespace alexnown
{
    public static class UiHelper
    {
        public static ISlider GetSlider(GameObject go)
        {
            if (go.TryGetComponent<ISlider>(out var slider))
                return slider;
            if (go.TryGetComponent<Slider>(out var uiSlider))
                return go.AddComponent<UiSliderAdapter>();
            return null;
        }
    }
}
