using UnityEngine;
using UnityEngine.UI;

namespace alexnown
{
    [ExecuteInEditMode]
    public class FillImageRadialCustomization : MonoBehaviour
    {
        [SerializeField]
        private Image _fillImage;
        private IRadialElement _radialElement;

        public void UpdateRadialView(float normValue)
        {
            if (_fillImage.fillClockwise != _radialElement.ClockwiseDirection)
                _fillImage.fillClockwise = _radialElement.ClockwiseDirection;
            _fillImage.fillAmount = normValue;
        }

        private void OnEnable()
        {
            if (_radialElement == null) _radialElement = GetComponent<IRadialElement>();
            _radialElement.ChangedNormValue += UpdateRadialView;
            UpdateRadialView(_radialElement.NormalizedValue);
        }

        private void OnDisable()
        {
            _radialElement.ChangedNormValue -= UpdateRadialView;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || _fillImage != null) return;
            foreach (var image in GetComponentsInChildren<Image>())
            {
                if (image.type == Image.Type.Filled)
                {
                    _fillImage = image;
                    if (_radialElement != null)
                        UpdateRadialView(_radialElement.NormalizedValue);
                    return;
                }
            }
        }
#endif
    }
}
